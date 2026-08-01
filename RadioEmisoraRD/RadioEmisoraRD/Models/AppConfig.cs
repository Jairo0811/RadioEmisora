namespace RadioEmisoraRD.Models;

public sealed class AppConfig
{
    public const string DefaultCatalogUrl =
        "https://raw.githubusercontent.com/Jairo0811/RadioEmisora/master/catalog/emisoras.json";

    public int SchemaVersion { get; set; } = 2;

    public string UltimaEmisora { get; set; } = string.Empty;

    public List<string> Historial { get; set; } = [];

    public double Volumen { get; set; } = 0.80;

    public int StreamTimeoutSeconds { get; set; } = 12;

    public int MaxReconnectAttempts { get; set; } = 3;

    public int ReconnectDelaySeconds { get; set; } = 3;

    public string CatalogUrl { get; set; } = DefaultCatalogUrl;

    public int CatalogVersion { get; set; } = 1;

    public DateTimeOffset? LastCatalogUpdateUtc { get; set; }
}
