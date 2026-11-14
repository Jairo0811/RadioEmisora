using System.Text.RegularExpressions;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public static class StationCatalogValidator
{
    internal const int MaxStations = 200;
    internal const int MaxIdLength = 64;
    internal const int MaxNameLength = 100;

    // \z (no $): en .NET "$" también acepta un salto de línea final, lo que dejaría pasar "id\n".
    private static readonly Regex StationIdPattern = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*\\z",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex LogoFileNamePattern = new(
        "^[a-z0-9][a-z0-9-]{0,62}\\.png\\z",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ThemeColorPattern = new(
        "^#[0-9A-Fa-f]{6}\\z",
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
        {
            validationErrors.Add($"El catálogo supera el límite de {MaxStations} emisoras.");
            errors = validationErrors;
            return false;
        }

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < catalog.Stations.Count; index++)
        {
            string prefix = $"Emisora #{index + 1}";
            StationCatalogItem? station = catalog.Stations[index];
            if (station is null)
            {
                validationErrors.Add($"{prefix}: entrada vacía.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(station.Id) || station.Id.Length > MaxIdLength ||
                !StationIdPattern.IsMatch(station.Id))
            {
                validationErrors.Add($"{prefix}: identificador inválido.");
            }
            else if (!ids.Add(station.Id))
            {
                validationErrors.Add($"{prefix}: identificador duplicado.");
            }

            if (ValidateText(station.Nombre, "nombre", prefix, MaxNameLength, validationErrors) &&
                !names.Add(station.Nombre.Trim()))
            {
                validationErrors.Add($"{prefix}: nombre duplicado.");
            }

            ValidateText(station.Frecuencia, "frecuencia", prefix, 32, validationErrors);
            ValidateText(station.Categoria, "categoría", prefix, 64, validationErrors);
            ValidateText(station.Provincia, "provincia", prefix, 64, validationErrors);
            ValidateText(station.Grupo, "grupo", prefix, MaxNameLength, validationErrors);

            // Solo nombres simples: impide rutas relativas, absolutas o recursos fuera de Assets/logos.
            if (string.IsNullOrWhiteSpace(station.Logo) || !LogoFileNamePattern.IsMatch(station.Logo))
                validationErrors.Add($"{prefix}: logo PNG inválido.");

            if (!ThemeColorPattern.IsMatch(station.ColorTema ?? string.Empty))
                validationErrors.Add($"{prefix}: color de tema inválido.");

            if (!NetworkUriPolicy.TryCreatePublicHttpsUri(station.StreamUrl, out _))
                validationErrors.Add($"{prefix}: el stream debe usar una URL HTTPS pública.");
        }

        errors = validationErrors;
        return validationErrors.Count == 0;
    }

    private static bool ValidateText(
        string? value,
        string fieldName,
        string prefix,
        int maximumLength,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{prefix}: {fieldName} requerida.");
            return false;
        }

        if (value.Length > maximumLength || value.Any(char.IsControl))
        {
            errors.Add($"{prefix}: {fieldName} inválida.");
            return false;
        }

        return true;
    }
}
