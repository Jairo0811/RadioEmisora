namespace RadioEmisoraRD.Models;

public sealed record PlaybackOptions(
    TimeSpan Timeout,
    int MaxReconnectAttempts,
    TimeSpan ReconnectDelay)
{
    public static PlaybackOptions FromConfig(AppConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        return new PlaybackOptions(
            TimeSpan.FromSeconds(config.StreamTimeoutSeconds),
            config.MaxReconnectAttempts,
            TimeSpan.FromSeconds(config.ReconnectDelaySeconds));
    }
}
