using System.Net.Http;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public sealed class PlaybackErrorEventArgs : EventArgs
{
    public PlaybackErrorEventArgs(string friendlyMessage, Exception exception)
    {
        FriendlyMessage = friendlyMessage;
        Exception = exception;
    }

    public string FriendlyMessage { get; }

    public Exception Exception { get; }
}

public interface IMediaPlayerService : IDisposable
{
    event EventHandler<PlayerStateChangedEventArgs>? StateChanged;

    event EventHandler<PlaybackErrorEventArgs>? PlaybackError;

    PlayerState State { get; }

    double Volume { get; set; }

    Task PlayAsync(
        string url,
        PlaybackOptions options,
        CancellationToken cancellationToken = default);

    void Pause();

    void Resume();

    Task StopAsync(CancellationToken cancellationToken = default);
}

public sealed class MediaPlayerService : IMediaPlayerService
{
    private readonly IMediaPlaybackEngine engine;
    private readonly IStreamProbe streamProbe;
    private readonly bool ownsStreamProbe;
    private readonly IAppLogger logger;
    private readonly PlayerStateMachine stateMachine = new();
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private readonly object syncRoot = new();
    private CancellationTokenSource? sessionCancellation;
    private Uri? currentStreamUri;
    private PlaybackOptions? currentOptions;
    private int sessionGeneration;
    private bool reconnectScheduled;
    private bool disposed;

    public MediaPlayerService(
        IMediaPlaybackEngine? engine = null,
        IStreamProbe? streamProbe = null,
        IAppLogger? logger = null)
    {
        this.engine = engine ?? new WpfMediaPlaybackEngine();
        this.streamProbe = streamProbe ?? new StreamProbe();
        ownsStreamProbe = streamProbe is null;
        this.logger = logger ?? AppLogger.Current;

        this.engine.Opened += OnEngineOpened;
        this.engine.BufferingStarted += OnBufferingStarted;
        this.engine.BufferingEnded += OnBufferingEnded;
        this.engine.Failed += OnEngineFailed;
        stateMachine.StateChanged += OnStateMachineChanged;
    }

    public event EventHandler<PlayerStateChangedEventArgs>? StateChanged;

    public event EventHandler<PlaybackErrorEventArgs>? PlaybackError;

    public PlayerState State => stateMachine.CurrentState;

    public double Volume
    {
        get => engine.Volume;
        set => engine.Volume = Math.Clamp(value, 0, 1);
    }

    public async Task PlayAsync(
        string url,
        PlaybackOptions options,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(options);

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? streamUri) ||
            (streamUri.Scheme != Uri.UriSchemeHttps && streamUri.Scheme != Uri.UriSchemeHttp) ||
            !string.IsNullOrEmpty(streamUri.UserInfo))
        {
            var exception = new ArgumentException("La URL del stream no es válida.", nameof(url));
            ReportFinalError("La dirección de esta emisora no es válida.", exception);
            return;
        }

        (CancellationTokenSource session, int generation) = StartNewSession(cancellationToken);

        await operationGate.WaitAsync(session.Token);

        try
        {
            currentStreamUri = streamUri;
            currentOptions = options;
            reconnectScheduled = false;
            CloseEngine();
            await ConnectWithRetryAsync(streamUri, options, 0, generation, session.Token);
        }
        catch (OperationCanceledException) when (session.IsCancellationRequested)
        {
            logger.Info("Conexión de stream cancelada.");
        }
        finally
        {
            operationGate.Release();
        }
    }

    public void Pause()
    {
        ThrowIfDisposed();

        if (State is not (PlayerState.Reproduciendo or PlayerState.Buffering))
            return;

        engine.Pause();
        stateMachine.TransitionTo(PlayerState.Pausado);
    }

    public void Resume()
    {
        ThrowIfDisposed();

        if (State != PlayerState.Pausado)
            return;

        engine.Play();
        stateMachine.TransitionTo(PlayerState.Reproduciendo);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        CancelCurrentSession();

        await operationGate.WaitAsync(cancellationToken);

        try
        {
            CloseEngine();
            currentStreamUri = null;
            currentOptions = null;
            reconnectScheduled = false;
            stateMachine.TransitionTo(PlayerState.Detenido);
        }
        finally
        {
            operationGate.Release();
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        CancelCurrentSession();

        engine.Opened -= OnEngineOpened;
        engine.BufferingStarted -= OnBufferingStarted;
        engine.BufferingEnded -= OnBufferingEnded;
        engine.Failed -= OnEngineFailed;
        stateMachine.StateChanged -= OnStateMachineChanged;
        CloseEngine();
        engine.Dispose();

        if (ownsStreamProbe && streamProbe is IDisposable disposableProbe)
            disposableProbe.Dispose();

        operationGate.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task ConnectWithRetryAsync(
        Uri streamUri,
        PlaybackOptions options,
        int firstAttempt,
        int generation,
        CancellationToken cancellationToken)
    {
        Exception? lastException = null;

        for (int attempt = firstAttempt; attempt <= options.MaxReconnectAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureCurrentGeneration(generation, cancellationToken);
            try
            {
                if (attempt == 0)
                {
                    stateMachine.TransitionTo(PlayerState.Conectando);
                }
                else
                {
                    stateMachine.TransitionTo(
                        PlayerState.Reconectando,
                        $"Intento {attempt} de {options.MaxReconnectAttempts}");
                    await Task.Delay(options.ReconnectDelay, cancellationToken);
                }

                StreamProbeResult probeResult = await streamProbe.ProbeAsync(
                    streamUri,
                    options.Timeout,
                    cancellationToken);
                EnsureCurrentGeneration(generation, cancellationToken);

                CloseEngine();
                stateMachine.TransitionTo(PlayerState.Buffering);
                engine.Open(probeResult.FinalUri);
                engine.Play();
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception) when (
                exception is HttpRequestException or IOException or InvalidOperationException or
                    NotSupportedException or TimeoutException)
            {
                lastException = exception;
                logger.Warning($"Falló la conexión al stream (intento {attempt + 1}).", exception);
            }
        }

        ReportFinalError(
            "No fue posible conectar con esta emisora. Verifica Internet e inténtalo de nuevo.",
            lastException ?? new IOException("El stream no respondió."));
    }

    private async Task ReconnectAfterFailureAsync(
        Uri streamUri,
        PlaybackOptions options,
        int generation,
        CancellationToken cancellationToken)
    {
        try
        {
            await operationGate.WaitAsync(cancellationToken);

            try
            {
                CloseEngine();
                await ConnectWithRetryAsync(streamUri, options, 1, generation, cancellationToken);
            }
            finally
            {
                operationGate.Release();
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.Info("Reconexión cancelada.");
        }
        finally
        {
            lock (syncRoot)
            {
                if (generation == sessionGeneration)
                    reconnectScheduled = false;
            }
        }
    }

    private (CancellationTokenSource Session, int Generation) StartNewSession(
        CancellationToken cancellationToken)
    {
        lock (syncRoot)
        {
            sessionCancellation?.Cancel();
            sessionCancellation?.Dispose();
            sessionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            sessionGeneration++;
            return (sessionCancellation, sessionGeneration);
        }
    }

    private void CancelCurrentSession()
    {
        lock (syncRoot)
        {
            sessionGeneration++;
            sessionCancellation?.Cancel();
            sessionCancellation?.Dispose();
            sessionCancellation = null;
        }
    }

    private void OnEngineOpened(object? sender, EventArgs e)
    {
        stateMachine.TransitionTo(PlayerState.Reproduciendo);
    }

    private void OnBufferingStarted(object? sender, EventArgs e)
    {
        if (State != PlayerState.Pausado)
            stateMachine.TransitionTo(PlayerState.Buffering);
    }

    private void OnBufferingEnded(object? sender, EventArgs e)
    {
        if (State != PlayerState.Pausado)
            stateMachine.TransitionTo(PlayerState.Reproduciendo);
    }

    private void OnEngineFailed(object? sender, PlaybackEngineFailedEventArgs e)
    {
        logger.Warning("MediaPlayer informó un fallo de reproducción.", e.Exception);

        lock (syncRoot)
        {
            if (disposed || reconnectScheduled || currentStreamUri is null ||
                currentOptions is null || sessionCancellation is null)
            {
                return;
            }

            reconnectScheduled = true;
            _ = ReconnectAfterFailureAsync(
                currentStreamUri,
                currentOptions,
                sessionGeneration,
                sessionCancellation.Token);
        }
    }

    private void OnStateMachineChanged(object? sender, PlayerStateChangedEventArgs e) =>
        StateChanged?.Invoke(this, e);

    private void ReportFinalError(string friendlyMessage, Exception exception)
    {
        stateMachine.TransitionTo(PlayerState.Error, friendlyMessage);
        logger.Error(friendlyMessage, exception);
        PlaybackError?.Invoke(this, new PlaybackErrorEventArgs(friendlyMessage, exception));
    }

    private void EnsureCurrentGeneration(int generation, CancellationToken cancellationToken)
    {
        if (generation != sessionGeneration)
            throw new OperationCanceledException(cancellationToken);
    }

    private void CloseEngine()
    {
        try
        {
            engine.Stop();
            engine.Close();
        }
        catch (Exception exception) when (exception is InvalidOperationException or ObjectDisposedException)
        {
            logger.Warning("No se pudo cerrar limpiamente el reproductor.", exception);
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);
}
