using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.Tests;

[TestClass]
public sealed class PlayerTests
{
    [TestMethod]
    public void PlayerStateMachine_TracksEveryRequiredStateWithoutDuplicateEvents()
    {
        var machine = new PlayerStateMachine();
        var observed = new List<PlayerState>();
        machine.StateChanged += (_, args) => observed.Add(args.Current);

        machine.TransitionTo(PlayerState.Conectando);
        machine.TransitionTo(PlayerState.Buffering);
        machine.TransitionTo(PlayerState.Reproduciendo);
        machine.TransitionTo(PlayerState.Pausado);
        machine.TransitionTo(PlayerState.Reconectando);
        machine.TransitionTo(PlayerState.Error);
        machine.TransitionTo(PlayerState.Detenido);
        machine.TransitionTo(PlayerState.Detenido);

        CollectionAssert.AreEqual(
            new[]
            {
                PlayerState.Conectando,
                PlayerState.Buffering,
                PlayerState.Reproduciendo,
                PlayerState.Pausado,
                PlayerState.Reconectando,
                PlayerState.Error,
                PlayerState.Detenido
            },
            observed);
    }

    [TestMethod]
    public async Task MediaPlayerService_Play_TransitionsFromConnectingToPlaying()
    {
        var engine = new FakePlaybackEngine();
        var probe = new FakeStreamProbe();
        using var service = new MediaPlayerService(engine, probe, new TestLogger());
        var observed = new List<PlayerState>();
        service.StateChanged += (_, args) => observed.Add(args.Current);

        await service.PlayAsync(
            "https://example.test/stream",
            FastOptions());
        engine.RaiseOpened();

        CollectionAssert.AreEqual(
            new[]
            {
                PlayerState.Conectando,
                PlayerState.Buffering,
                PlayerState.Reproduciendo
            },
            observed);
        Assert.AreEqual(1, engine.PlayCalls);
    }

    [TestMethod]
    public async Task MediaPlayerService_Play_RetriesAnUnavailableStream()
    {
        var engine = new FakePlaybackEngine();
        var probe = new FakeStreamProbe { FailuresBeforeSuccess = 1 };
        using var service = new MediaPlayerService(engine, probe, new TestLogger());
        var observed = new List<PlayerState>();
        service.StateChanged += (_, args) => observed.Add(args.Current);

        await service.PlayAsync(
            "https://example.test/stream",
            new PlaybackOptions(TimeSpan.FromSeconds(1), 2, TimeSpan.FromMilliseconds(1)));

        Assert.AreEqual(2, probe.Calls);
        Assert.IsTrue(observed.Contains(PlayerState.Reconectando));
        Assert.AreEqual(PlayerState.Buffering, service.State);
    }

    [TestMethod]
    public async Task MediaPlayerService_Play_ReportsErrorWithoutThrowingAfterRetries()
    {
        var engine = new FakePlaybackEngine();
        var probe = new FakeStreamProbe { FailuresBeforeSuccess = int.MaxValue };
        using var service = new MediaPlayerService(engine, probe, new TestLogger());
        PlaybackErrorEventArgs? reportedError = null;
        service.PlaybackError += (_, args) => reportedError = args;

        await service.PlayAsync(
            "https://example.test/stream",
            new PlaybackOptions(TimeSpan.FromSeconds(1), 1, TimeSpan.FromMilliseconds(1)));

        Assert.AreEqual(PlayerState.Error, service.State);
        Assert.IsNotNull(reportedError);
        Assert.AreEqual(2, probe.Calls);
    }

    [TestMethod]
    public async Task MediaPlayerService_PauseResumeAndStop_KeepSingleEngineSession()
    {
        var engine = new FakePlaybackEngine();
        using var service = new MediaPlayerService(engine, new FakeStreamProbe(), new TestLogger());
        await service.PlayAsync("https://example.test/stream", FastOptions());
        engine.RaiseOpened();

        service.Pause();
        Assert.AreEqual(PlayerState.Pausado, service.State);
        service.Resume();
        Assert.AreEqual(PlayerState.Reproduciendo, service.State);
        await service.StopAsync();

        Assert.AreEqual(PlayerState.Detenido, service.State);
        Assert.AreEqual(1, engine.PauseCalls);
        Assert.IsTrue(engine.CloseCalls >= 2);
    }

    private static PlaybackOptions FastOptions() =>
        new(TimeSpan.FromSeconds(1), 0, TimeSpan.FromMilliseconds(1));

    private sealed class FakeStreamProbe : IStreamProbe
    {
        public int Calls { get; private set; }

        public int FailuresBeforeSuccess { get; init; }

        public Task<StreamProbeResult> ProbeAsync(
            Uri streamUri,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Calls++;

            if (Calls <= FailuresBeforeSuccess)
                throw new IOException("offline");

            return Task.FromResult(new StreamProbeResult(streamUri, "audio/mpeg"));
        }
    }

    private sealed class FakePlaybackEngine : IMediaPlaybackEngine
    {
        public event EventHandler? Opened;

        public event EventHandler? BufferingStarted;

        public event EventHandler? BufferingEnded;

        public event EventHandler<PlaybackEngineFailedEventArgs>? Failed;

        public double Volume { get; set; }

        public int PlayCalls { get; private set; }

        public int PauseCalls { get; private set; }

        public int CloseCalls { get; private set; }

        public void Open(Uri source)
        {
        }

        public void Play() => PlayCalls++;

        public void Pause() => PauseCalls++;

        public void Stop()
        {
        }

        public void Close() => CloseCalls++;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void RaiseOpened() => Opened?.Invoke(this, EventArgs.Empty);

        public void RaiseBufferingStarted() => BufferingStarted?.Invoke(this, EventArgs.Empty);

        public void RaiseBufferingEnded() => BufferingEnded?.Invoke(this, EventArgs.Empty);

        public void RaiseFailure(Exception exception) =>
            Failed?.Invoke(this, new PlaybackEngineFailedEventArgs(exception));
    }
}
