using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadioEmisoraRD.Services;

internal sealed class JsonFileStore<T>
    where T : class
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string filePath;
    private readonly string backupPath;
    private readonly IAppLogger logger;
    private readonly object syncRoot = new();

    public JsonFileStore(string filePath, IAppLogger logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        this.filePath = filePath;
        backupPath = filePath + ".bak";
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public T LoadOrCreate(Func<T> factory, Action<T>? normalize = null)
    {
        ArgumentNullException.ThrowIfNull(factory);

        lock (syncRoot)
        {
            if (TryRead(filePath, out T? value) && value is not null)
            {
                normalize?.Invoke(value);
                return value;
            }

            if (File.Exists(filePath))
                PreserveCorruptedFile();

            if (TryRead(backupPath, out value) && value is not null)
            {
                normalize?.Invoke(value);
                TrySaveCore(value, out _);
                logger.Warning($"Se recuperó '{Path.GetFileName(filePath)}' desde su respaldo.");
                return value;
            }

            value = factory();
            normalize?.Invoke(value);
            TrySaveCore(value, out _);
            return value;
        }
    }

    public bool TrySave(T value, out string? errorMessage)
    {
        ArgumentNullException.ThrowIfNull(value);

        lock (syncRoot)
        {
            return TrySaveCore(value, out errorMessage);
        }
    }

    private bool TryRead(string path, out T? value)
    {
        value = null;

        if (!File.Exists(path))
            return false;

        try
        {
            using FileStream stream = new(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            value = JsonSerializer.Deserialize<T>(stream, SerializerOptions);
            return value is not null;
        }
        catch (Exception exception) when (
            exception is JsonException or IOException or UnauthorizedAccessException)
        {
            logger.Warning($"No se pudo leer '{path}'.", exception);
            return false;
        }
    }

    private bool TrySaveCore(T value, out string? errorMessage)
    {
        string temporaryPath = filePath + ".tmp";

        try
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(value, SerializerOptions);
            File.WriteAllText(temporaryPath, json);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Replace(temporaryPath, filePath, backupPath, true);
                }
                catch (Exception exception) when (
                    (exception is PlatformNotSupportedException or IOException) &&
                    File.Exists(temporaryPath))
                {
                    File.Copy(filePath, backupPath, true);
                    File.Move(temporaryPath, filePath, true);
                }
            }
            else
            {
                File.Move(temporaryPath, filePath);
            }

            errorMessage = null;
            return true;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.Error($"No se pudo guardar '{filePath}'.", exception);
            errorMessage = "No se pudieron guardar los cambios en el disco.";

            try
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
            catch (Exception cleanupException) when (
                cleanupException is IOException or UnauthorizedAccessException)
            {
                logger.Warning("No se pudo eliminar un archivo temporal.", cleanupException);
            }

            return false;
        }
    }

    private void PreserveCorruptedFile()
    {
        try
        {
            string extension = Path.GetExtension(filePath);
            string name = Path.GetFileNameWithoutExtension(filePath);
            string? directory = Path.GetDirectoryName(filePath);
            string corruptedName = $"{name}.corrupt-{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
            string corruptedPath = Path.Combine(directory ?? string.Empty, corruptedName);
            File.Move(filePath, corruptedPath, true);
            logger.Warning($"Se preservó el archivo corrupto como '{corruptedName}'.");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            logger.Warning("No se pudo preservar el archivo JSON corrupto.", exception);
        }
    }
}
