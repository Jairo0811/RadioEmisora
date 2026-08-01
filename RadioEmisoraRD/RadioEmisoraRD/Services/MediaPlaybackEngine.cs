using System.Windows;
using System.Windows.Media;

namespace RadioEmisoraRD.Services;

public sealed class PlaybackEngineFailedEventArgs : EventArgs
{
    public PlaybackEngineFailedEventArgs(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}

public interface IMediaPlaybackEngine : IDisposable
{
    event EventHandler? Opened;

    event EventHandler? BufferingStarted;

    event EventHandler? BufferingEnded;

    event EventHandler<PlaybackEngineFailedEventArgs>? Failed;

    double Volume { get; set; }

    void Open(Uri source);

    void Play();

    void Pause();

    void StopPlayback();

    void Close();
}

public sealed class WpfMediaPlaybackEngine : IMediaPlaybackEngine
{
    private readonly MediaPlayer mediaPlayer = new();
    private bool disposed;

    public WpfMediaPlaybackEngine()
    {
        mediaPlayer.MediaOpened += OnMediaOpened;
        mediaPlayer.BufferingStarted += OnBufferingStarted;
        mediaPlayer.BufferingEnded += OnBufferingEnded;
        mediaPlayer.MediaFailed += OnMediaFailed;
    }

    public event EventHandler? Opened;

    public event EventHandler? BufferingStarted;

    public event EventHandler? BufferingEnded;

    public event EventHandler<PlaybackEngineFailedEventArgs>? Failed;

    public double Volume
    {
        get => mediaPlayer.Volume;
        set => mediaPlayer.Volume = Math.Clamp(value, 0, 1);
    }

    public void Open(Uri source)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        mediaPlayer.Open(source);
    }

    public void Play()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        mediaPlayer.Play();
    }

    public void Pause()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        mediaPlayer.Pause();
    }

    public void StopPlayback()
    {
        if (!disposed)
            mediaPlayer.Stop();
    }

    public void Close()
    {
        if (!disposed)
            mediaPlayer.Close();
    }

    public void Dispose()
    {
        if (disposed)
            return;

        mediaPlayer.MediaOpened -= OnMediaOpened;
        mediaPlayer.BufferingStarted -= OnBufferingStarted;
        mediaPlayer.BufferingEnded -= OnBufferingEnded;
        mediaPlayer.MediaFailed -= OnMediaFailed;
        mediaPlayer.Stop();
        mediaPlayer.Close();
        disposed = true;
        GC.SuppressFinalize(this);
    }

    private void OnMediaOpened(object? sender, EventArgs e) => Opened?.Invoke(this, EventArgs.Empty);

    private void OnBufferingStarted(object? sender, EventArgs e) =>
        BufferingStarted?.Invoke(this, EventArgs.Empty);

    private void OnBufferingEnded(object? sender, EventArgs e) =>
        BufferingEnded?.Invoke(this, EventArgs.Empty);

    private void OnMediaFailed(object? sender, ExceptionEventArgs e) =>
        Failed?.Invoke(this, new PlaybackEngineFailedEventArgs(e.ErrorException));
}
