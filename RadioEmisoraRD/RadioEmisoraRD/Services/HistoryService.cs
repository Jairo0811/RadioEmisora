using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public static class HistoryService
{
    public const int DefaultCapacity = 10;

    public static void Register(AppConfig config, string stationName, int capacity = DefaultCapacity)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentException.ThrowIfNullOrWhiteSpace(stationName);
        if (!ConfigService.IsSafeStationReference(stationName.Trim()))
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
