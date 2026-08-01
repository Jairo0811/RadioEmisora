namespace RadioEmisoraRD.Services;

public static class AppPaths
{
    private const string AppFolderName = "RadioEmisoraRD";
    private const string PortableEnvironmentVariable = "RADIOEMISORARD_PORTABLE";

    public static bool IsPortable =>
        File.Exists(Path.Combine(AppContext.BaseDirectory, "portable.flag")) ||
        string.Equals(
            Environment.GetEnvironmentVariable(PortableEnvironmentVariable),
            "1",
            StringComparison.Ordinal);

    public static string DataDirectory => IsPortable
        ? Path.Combine(AppContext.BaseDirectory, "Data")
        : Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppFolderName);

    public static string LogsDirectory => Path.Combine(DataDirectory, "Logs");

    public static string LocalCatalogPath =>
        Path.Combine(AppContext.BaseDirectory, "Assets", "Data", "emisoras.json");
}
