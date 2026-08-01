using System.Text.RegularExpressions;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public static class StationCatalogValidator
{
    private const int MaxStations = 200;
    private static readonly Regex StationIdPattern = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ThemeColorPattern = new(
        "^#[0-9A-Fa-f]{6}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryValidate(StationCatalog? catalog, out IReadOnlyList<string> errors)
    {
        var validationErrors = new List<string>();

        if (catalog is null)
        {
            errors = ["El catálogo está vacío."];
            return false;
        }

        if (catalog.Version < 1)
            validationErrors.Add("La versión del catálogo debe ser mayor que cero.");

        if (catalog.Stations is null || catalog.Stations.Count == 0)
        {
            validationErrors.Add("El catálogo no contiene emisoras.");
            errors = validationErrors;
            return false;
        }

        if (catalog.Stations.Count > MaxStations)
            validationErrors.Add($"El catálogo supera el límite de {MaxStations} emisoras.");

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < catalog.Stations.Count; index++)
        {
            StationCatalogItem station = catalog.Stations[index];
            string prefix = $"Emisora #{index + 1}";

            if (string.IsNullOrWhiteSpace(station.Id) || !StationIdPattern.IsMatch(station.Id))
                validationErrors.Add($"{prefix}: identificador inválido.");
            else if (!ids.Add(station.Id))
                validationErrors.Add($"{prefix}: identificador duplicado '{station.Id}'.");

            if (string.IsNullOrWhiteSpace(station.Nombre))
                validationErrors.Add($"{prefix}: nombre requerido.");
            else if (!names.Add(station.Nombre))
                validationErrors.Add($"{prefix}: nombre duplicado '{station.Nombre}'.");

            ValidateRequired(station.Frecuencia, "frecuencia", prefix, validationErrors);
            ValidateRequired(station.Categoria, "categoría", prefix, validationErrors);
            ValidateRequired(station.Provincia, "provincia", prefix, validationErrors);
            ValidateRequired(station.Grupo, "grupo", prefix, validationErrors);

            if (string.IsNullOrWhiteSpace(station.Logo) ||
                !string.Equals(Path.GetFileName(station.Logo), station.Logo, StringComparison.Ordinal) ||
                !station.Logo.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                validationErrors.Add($"{prefix}: logo PNG inválido.");
            }

            if (!ThemeColorPattern.IsMatch(station.ColorTema ?? string.Empty))
                validationErrors.Add($"{prefix}: color de tema inválido.");

            if (!Uri.TryCreate(station.StreamUrl, UriKind.Absolute, out Uri? streamUri) ||
                streamUri.Scheme != Uri.UriSchemeHttps)
            {
                validationErrors.Add($"{prefix}: el stream debe usar una URL HTTPS absoluta.");
            }
        }

        errors = validationErrors;
        return validationErrors.Count == 0;
    }

    private static void ValidateRequired(
        string? value,
        string fieldName,
        string prefix,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"{prefix}: {fieldName} requerida.");
    }

}
