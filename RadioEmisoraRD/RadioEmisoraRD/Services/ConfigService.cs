using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public interface IConfigService
{
    AppConfig Load();

    bool TrySave(AppConfig config, out string? errorMessage);
}

public sealed class ConfigService : IConfigService
{
    private readonly JsonFileStore<AppConfig> store;

    public ConfigService(string? dataDirectory = null, IAppLogger? logger = null)
    {
        string directory = dataDirectory ?? AppPaths.DataDirectory;
        store = new JsonFileStore<AppConfig>(
            Path.Combine(directory, "config.json"),
            logger ?? AppLogger.Current);
    }

    public AppConfig Load() => store.LoadOrCreate(static () => new AppConfig(), Normalize);

    public bool TrySave(AppConfig config, out string? errorMessage)
    {
        ArgumentNullException.ThrowIfNull(config);
        Normalize(config);
        return store.TrySave(config, out errorMessage);
    }

    internal static void Normalize(AppConfig config)
    {
        config.SchemaVersion = 2;
        config.UltimaEmisora = config.UltimaEmisora?.Trim() ?? string.Empty;
        config.Historial = (config.Historial ?? [])
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();
        config.Volumen = double.IsFinite(config.Volumen)
            ? Math.Clamp(config.Volumen, 0, 1)
            : 0.80;
        config.StreamTimeoutSeconds = Math.Clamp(config.StreamTimeoutSeconds, 3, 60);
        config.MaxReconnectAttempts = Math.Clamp(config.MaxReconnectAttempts, 0, 10);
        config.ReconnectDelaySeconds = Math.Clamp(config.ReconnectDelaySeconds, 1, 30);

        if (!Uri.TryCreate(config.CatalogUrl, UriKind.Absolute, out Uri? catalogUri) ||
            catalogUri.Scheme != Uri.UriSchemeHttps)
        {
            config.CatalogUrl = AppConfig.DefaultCatalogUrl;
        }

        config.CatalogVersion = Math.Max(1, config.CatalogVersion);
    }
}
