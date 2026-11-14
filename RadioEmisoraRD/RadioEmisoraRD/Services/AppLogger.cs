using System.Globalization;

namespace RadioEmisoraRD.Services;

public interface IAppLogger
{
    string LogFilePath { get; }

    void Info(string message);

    void Warning(string message, Exception? exception = null);

    void LogError(string message, Exception exception);
}

/// <summary>
/// Registro local sin telemetría. Un archivo por día, rotación por tamaño y retención limitada.
/// </summary>
public sealed class AppLogger : IAppLogger
{
    internal const long MaxLogFileSizeBytes = 2 * 1024 * 1024;
    internal const int MaxRetainedLogFiles = 10;
    internal const int MaxMessageLength = 2000;
    private const string LogFilePrefix = "radioemisorard-";

    private readonly object syncRoot = new();
    private readonly string logsDirectory;
    private bool retentionApplied;

    public AppLogger(string? logsDirectory = null)
        : this(logsDirectory, DateTime.UtcNow)
    {
    }

    internal AppLogger(string? logsDirectory, DateTime utcNow)
    {
        this.logsDirectory = logsDirectory ?? AppPaths.LogsDirectory;
        LogFilePath = Path.Combine(
            this.logsDirectory,
            string.Create(CultureInfo.InvariantCulture, $"{LogFilePrefix}{utcNow:yyyyMMdd}.log"));
    }

    public static AppLogger Current { get; } = new();

    public string LogFilePath { get; }

    public void Info(string message) => Write("INFO", message, null);

    public void Warning(string message, Exception? exception = null) =>
        Write("WARN", message, exception);

    public void LogError(string message, Exception exception) => Write("ERROR", message, exception);

    /// <summary>
    /// Resume una excepción esperada (red, stream, archivo) en una sola línea, sin stack trace.
    /// </summary>
    public static string Summarize(Exception? exception)
    {
        if (exception is null)
            return string.Empty;

        string summary = $"{exception.GetType().Name}: {exception.Message}";
        if (exception.InnerException is { } inner)
            summary += $" ({inner.GetType().Name}: {inner.Message})";

        return Sanitize(summary);
    }

    internal static string Sanitize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // Una línea por evento: evita inyectar entradas falsas en el registro.
        var builder = new System.Text.StringBuilder(Math.Min(value.Length, MaxMessageLength));
        foreach (char character in value)
        {
            if (builder.Length >= MaxMessageLength)
            {
                builder.Append('…');
                break;
            }

            builder.Append(char.IsControl(character) ? ' ' : character);
        }

        return builder.ToString();
    }

    private void Write(string level, string message, Exception? exception)
    {
        try
        {
            Directory.CreateDirectory(logsDirectory);

            string line = string.Create(
                CultureInfo.InvariantCulture,
                $"{DateTimeOffset.Now:O} [{level}] {Sanitize(message)}");

            if (exception is not null)
                line += Environment.NewLine + exception;

            lock (syncRoot)
            {
                ApplyRetentionOnce();
                RotateIfNeeded();
                File.AppendAllText(LogFilePath, line + Environment.NewLine);
            }
        }
        catch
        {
            // El logging nunca debe provocar una segunda excepción en la aplicación.
        }
    }

    private void RotateIfNeeded()
    {
        var file = new FileInfo(LogFilePath);
        if (file.Exists && file.Length >= MaxLogFileSizeBytes)
            File.Move(LogFilePath, LogFilePath + ".1", true);
    }

    private void ApplyRetentionOnce()
    {
        if (retentionApplied)
            return;

        retentionApplied = true;

        IEnumerable<FileInfo> staleFiles = new DirectoryInfo(logsDirectory)
            .EnumerateFiles(LogFilePrefix + "*.log*")
            .Where(file => !string.Equals(file.FullName, LogFilePath, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(static file => file.Name, StringComparer.Ordinal)
            .Skip(MaxRetainedLogFiles - 1);

        foreach (FileInfo staleFile in staleFiles)
        {
            try
            {
                staleFile.Delete();
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // Un archivo bloqueado se intentará eliminar en el próximo inicio.
            }
        }
    }
}
