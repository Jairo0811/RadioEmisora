using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.Tests;

[TestClass]
public sealed class PersistenceTests
{
    [TestMethod]
    public void ConfigService_Load_CreatesSafeDefaults()
    {
        using var directory = new TemporaryDirectory();
        var service = new ConfigService(directory.Path, new TestLogger());

        AppConfig config = service.Load();

        Assert.AreEqual(0.80, config.Volumen, 0.001);
        Assert.AreEqual(12, config.StreamTimeoutSeconds);
        Assert.AreEqual(3, config.MaxReconnectAttempts);
        Assert.IsTrue(File.Exists(directory.GetPath("config.json")));
    }

    [TestMethod]
    public void ConfigService_Save_NormalizesInvalidValues()
    {
        using var directory = new TemporaryDirectory();
        var service = new ConfigService(directory.Path, new TestLogger());
        var config = new AppConfig
        {
            Volumen = 4,
            StreamTimeoutSeconds = 1,
            MaxReconnectAttempts = 50,
            ReconnectDelaySeconds = 0,
            CatalogUrl = "http://insecure.test/catalog.json",
            Historial = [" Z 101 ", "z 101", string.Empty]
        };

        bool saved = service.TrySave(config, out string? error);
        AppConfig restored = service.Load();

        Assert.IsTrue(saved, error);
        Assert.AreEqual(1, restored.Volumen, 0.001);
        Assert.AreEqual(3, restored.StreamTimeoutSeconds);
        Assert.AreEqual(10, restored.MaxReconnectAttempts);
        Assert.AreEqual(1, restored.ReconnectDelaySeconds);
        Assert.AreEqual(AppConfig.DefaultCatalogUrl, restored.CatalogUrl);
        CollectionAssert.AreEqual(new[] { "Z 101" }, restored.Historial);
    }

    [TestMethod]
    public void ConfigService_Load_RecoversFromCorruptedJson()
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(directory.GetPath("config.json"), "{not-json");
        var service = new ConfigService(directory.Path, new TestLogger());

        AppConfig restored = service.Load();

        Assert.AreEqual(0.80, restored.Volumen, 0.001);
        Assert.AreEqual(1, Directory.GetFiles(directory.Path, "config.corrupt-*.json").Length);
    }

    [TestMethod]
    public void FavoriteService_Save_DeduplicatesAndPersistsFavorites()
    {
        using var directory = new TemporaryDirectory();
        var service = new FavoriteService(directory.Path, new TestLogger());

        bool saved = service.TrySave(
            [" z101-1013 ", "Z101-1013", "mortal-1049", string.Empty],
            out string? error);
        IReadOnlyList<string> restored = service.Load();

        Assert.IsTrue(saved, error);
        CollectionAssert.AreEqual(
            new[] { "mortal-1049", "z101-1013" },
            restored.ToArray());
    }

    [TestMethod]
    public void FavoriteService_Load_PreservesCorruptedFileAndContinues()
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(directory.GetPath("favoritos.json"), "invalid");
        var service = new FavoriteService(directory.Path, new TestLogger());

        IReadOnlyList<string> restored = service.Load();

        Assert.AreEqual(0, restored.Count);
        Assert.AreEqual(1, Directory.GetFiles(directory.Path, "favoritos.corrupt-*.json").Length);
    }

    [TestMethod]
    public void HistoryService_Register_MovesDuplicatesToFrontAndLimitsCapacity()
    {
        var config = new AppConfig
        {
            Historial = ["Mortal", "Z 101", "Disco 106"]
        };

        HistoryService.Register(config, "z 101", 3);
        HistoryService.Register(config, "Cima 100", 3);

        CollectionAssert.AreEqual(
            new[] { "Cima 100", "z 101", "Mortal" },
            config.Historial);
        Assert.AreEqual("Cima 100", config.UltimaEmisora);
    }
}
