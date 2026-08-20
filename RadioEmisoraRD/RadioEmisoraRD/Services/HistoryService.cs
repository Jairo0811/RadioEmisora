using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public static class HistoryService
{
    public static void Register(AppConfig config, string stationName, int capacity = 10)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentException.ThrowIfNullOrWhiteSpace(stationName);
        if (stationName.Length > 100 || stationName.Any(char.IsControl))
            throw new ArgumentException("El nombre de la emisora no es válido.", nameof(stationName));

        capacity = Math.Max(1, capacity);
        config.Historial ??= [];
        config.Historial.RemoveAll(
            item => string.Equals(item, stationName, StringComparison.OrdinalIgnoreCase));
        config.Historial.Insert(0, stationName.Trim());

        if (config.Historial.Count > capacity)
            config.Historial.RemoveRange(capacity, config.Historial.Count - capacity);

        config.UltimaEmisora = stationName.Trim();
    }
}
