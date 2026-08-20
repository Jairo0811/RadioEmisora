namespace RadioEmisoraRD.Services;

public interface IFavoriteService
{
    IReadOnlyList<string> Load();

    bool TrySave(IEnumerable<string> favorites, out string? errorMessage);
}

public sealed class FavoriteService : IFavoriteService
{
    private readonly JsonFileStore<List<string>> store;

    public FavoriteService(string? dataDirectory = null, IAppLogger? logger = null)
    {
        string directory = dataDirectory ?? AppPaths.DataDirectory;
        store = new JsonFileStore<List<string>>(
            Path.Combine(directory, "favoritos.json"),
            logger ?? AppLogger.Current);
    }

    public IReadOnlyList<string> Load() =>
        store.LoadOrCreate(static () => [], Normalize);

    public bool TrySave(IEnumerable<string> favorites, out string? errorMessage)
    {
        ArgumentNullException.ThrowIfNull(favorites);
        List<string> normalized = favorites.ToList();
        Normalize(normalized);
        return store.TrySave(normalized, out errorMessage);
    }

    private static void Normalize(List<string> favorites)
    {
        List<string> normalized = favorites
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item.Trim())
            .Where(static item => item.Length <= 64 && !item.Any(char.IsControl))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(500)
            .OrderBy(static item => item, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        favorites.Clear();
        favorites.AddRange(normalized);
    }
}
