using System.Globalization;
using System.Windows.Media;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public static class RadioService
{
    private static readonly CompareInfo SearchComparer =
        CultureInfo.GetCultureInfo("es-DO").CompareInfo;

    private const CompareOptions SearchOptions =
        CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    public static IReadOnlyList<Emisora> CreateStations(StationCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        return catalog.Stations
            .Select(static item => new Emisora(
                item.Id,
                item.Nombre,
                item.Frecuencia,
                item.Categoria,
                item.Provincia,
                item.Grupo,
                $"/Assets/logos/{item.Logo}",
                ParseColor(item.ColorTema),
                item.StreamUrl))
            .ToList();
    }

    public static IEnumerable<Emisora> Filter(
        IEnumerable<Emisora> stations,
        string? searchText,
        string? categoryFilter)
    {
        ArgumentNullException.ThrowIfNull(stations);

        string term = searchText?.Trim() ?? string.Empty;
        IEnumerable<Emisora> result = stations;

        if (term.Length > 0)
        {
            result = result.Where(station =>
                Contains(station.Nombre, term) ||
                Contains(station.Categoria, term) ||
                Contains(station.Frecuencia, term) ||
                Contains(station.Provincia, term) ||
                Contains(station.Grupo, term));
        }

        return categoryFilter switch
        {
            "Favoritas" => result.Where(static station => station.EsFavorita),
            "FM" => result.Where(static station =>
                station.Frecuencia.Contains("FM", StringComparison.OrdinalIgnoreCase)),
            "AM" => result.Where(static station =>
                station.Frecuencia.Contains("AM", StringComparison.OrdinalIgnoreCase)),
            "Online" => result.Where(static station =>
                station.Frecuencia.Contains("Online", StringComparison.OrdinalIgnoreCase)),
            _ => result
        };
    }

    private static bool Contains(string source, string term) =>
        SearchComparer.IndexOf(source, term, SearchOptions) >= 0;

    private static Color ParseColor(string value) =>
        (Color)ColorConverter.ConvertFromString(value)!;
}
