namespace RadioEmisoraRD.Models;

public sealed class StationCatalog
{
    public int Version { get; set; } = 1;

    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public List<StationCatalogItem> Stations { get; set; } = [];
}

public sealed class StationCatalogItem
{
    public string Id { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Frecuencia { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string Provincia { get; set; } = string.Empty;

    public string Grupo { get; set; } = string.Empty;

    public string Logo { get; set; } = string.Empty;

    public string ColorTema { get; set; } = "#6A35FF";

    public string StreamUrl { get; set; } = string.Empty;
}

public sealed record CatalogLoadResult(
    StationCatalog Catalog,
    bool UpdatedFromRemote,
    string Source,
    string? Warning = null);
