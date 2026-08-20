using System.Globalization;

namespace RadioEmisoraRD.Services;

public interface IAppLogger
{
    string LogFilePath { get; }

    void Info(string message);

    void Warning(string message, Exception? exception = null);

    void LogError(string message, Exception exception);
}

public sealed class AppLogger : IAppLogger
{
    private const long MaxLogSizeBytes = 5 * 1024 * 1024;
    private readonly object syncRoot = new();

    public AppLogger(string? logsDirectory = null)
    {
        string directory = logsDirectory ?? AppPaths.LogsDirectory;
        LogFilePath = Path.Combine(
            directory,
            $"radioemisorard-{DateTime.UtcNow:yyyyMMdd}.log");
    }

    public static AppLogger Current { get; } = new();

    public string LogFilePath { get; }

    public void Info(string message) => Write("INFO", message, null);

    public void Warning(string message, Exception? exception = null) =>
        Write("WARN", message, exception);

    public void LogError(string message, Exception exception) => Write("ERROR", message, exception);

    private void Write(string level, string message, Exception? exception)
    {
        try
        {
            string? directory = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            string line = string.Create(
                CultureInfo.InvariantCulture,
                $"{DateTimeOffset.Now:O} [{level}] {message}");

            if (exception is not null)
                line += Environment.NewLine + exception;

            lock (syncRoot)
            {
                if (File.Exists(LogFilePath) && new FileInfo(LogFilePath).Length >= MaxLogSizeBytes)
                    File.Move(LogFilePath, LogFilePath + ".1", true);

                File.AppendAllText(LogFilePath, line + Environment.NewLine);
            }
        }
        catch
        {
            // El logging nunca debe provocar una segunda excepción en la aplicación.
        }
    }
}
