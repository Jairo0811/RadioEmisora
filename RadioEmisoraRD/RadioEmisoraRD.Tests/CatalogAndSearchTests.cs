using System.Net.Http;
using System.Text.Json;
using System.Windows.Media;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.Tests;

[TestClass]
public sealed class CatalogAndSearchTests
{
    [TestMethod]
    public void RadioServiceFilterSearchesIgnoringCaseAndAccents()
    {
        List<Emisora> stations = CreateStations();

        Emisora[] result = RadioService.Filter(stations, "escandalo", "Todas").ToArray();

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("Escándalo 102", result[0].Nombre);
    }

    [TestMethod]
    public void RadioServiceFilterAppliesFavoriteAndBandFilters()
    {
        List<Emisora> stations = CreateStations();
        stations[0].EsFavorita = true;

        Assert.AreEqual(1, RadioService.Filter(stations, null, "Favoritas").Count());
        Assert.AreEqual(2, RadioService.Filter(stations, null, "FM").Count());
        Assert.AreEqual(1, RadioService.Filter(stations, null, "AM").Count());
    }

    [TestMethod]
    public void StationCatalogValidatorRejectsDuplicateAndInsecureStations()
    {
        StationCatalog catalog = CreateCatalog(2);
        catalog.Stations.Add(new StationCatalogItem
        {
            Id = catalog.Stations[0].Id,
            Nombre = "Duplicada",
            Frecuencia = "100.0 FM",
            Categoria = "Variada",
            Provincia = "Santo Domingo",
            Grupo = "Grupo",
            Logo = "logo.png",
            ColorTema = "#123456",
            StreamUrl = "http://insecure.test/stream"
        });

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsFalse(valid);
        Assert.IsTrue(errors.Any(error => error.Contains("duplicado", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(errors.Any(error => error.Contains("HTTPS", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task RadioCatalogServiceTryUpdateStoresNewerValidCatalog()
    {
        using var directory = new TemporaryDirectory();
        string localPath = directory.GetPath("local.json");
        await File.WriteAllTextAsync(localPath, JsonSerializer.Serialize(CreateCatalog(1)));
        string remoteJson = JsonSerializer.Serialize(CreateCatalog(2));
        using var client = new HttpClient(new StubHttpMessageHandler(
            (_, _) => Task.FromResult(StubHttpMessageHandler.Json(remoteJson))));
        using var service = new RadioCatalogService(
            client,
            localPath,
            directory.Path,
            new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://example.test/catalog.json",
            TimeSpan.FromSeconds(2));

        Assert.IsTrue(result.UpdatedFromRemote);
        Assert.AreEqual(2, result.Catalog.Version);
        Assert.IsTrue(File.Exists(directory.GetPath("catalogo.json")));
    }

    [TestMethod]
    public async Task RadioCatalogServiceTryUpdateUsesLocalCatalogWhenNetworkFails()
    {
        using var directory = new TemporaryDirectory();
        string localPath = directory.GetPath("local.json");
        await File.WriteAllTextAsync(localPath, JsonSerializer.Serialize(CreateCatalog(1)));
        using var client = new HttpClient(new StubHttpMessageHandler(
            (_, _) => throw new HttpRequestException("offline")));
        using var service = new RadioCatalogService(
            client,
            localPath,
            directory.Path,
            new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://example.test/catalog.json",
            TimeSpan.FromSeconds(2));

        Assert.IsFalse(result.UpdatedFromRemote);
        Assert.AreEqual(1, result.Catalog.Version);
        Assert.IsNotNull(result.Warning);
    }

    private static List<Emisora> CreateStations() =>
    [
        new Emisora(
            "escandalo-1025",
            "Escándalo 102",
            "102.5 FM",
            "Tropical",
            "Santo Domingo",
            "RCC Media",
            "/logo.png",
            Colors.Purple,
            "https://example.test/1"),
        new Emisora(
            "mortal-1049",
            "Mortal",
            "104.9 FM",
            "Urbano",
            "Santo Domingo",
            "Telemicro",
            "/logo.png",
            Colors.Red,
            "https://example.test/2"),
        new Emisora(
            "popular-950",
            "Radio Popular",
            "950 AM",
            "Noticias",
            "Santo Domingo",
            "RCC Media",
            "/logo.png",
            Colors.Blue,
            "https://example.test/3")
    ];

    private static StationCatalog CreateCatalog(int version) => new()
    {
        Version = version,
        UpdatedAtUtc = DateTimeOffset.UtcNow,
        Stations =
        [
            new StationCatalogItem
            {
                Id = "mortal-1049",
                Nombre = "Mortal",
                Frecuencia = "104.9 FM",
                Categoria = "Urbano",
                Provincia = "Santo Domingo",
                Grupo = "Telemicro",
                Logo = "mortal.png",
                ColorTema = "#DC283C",
                StreamUrl = "https://example.test/stream"
            }
        ]
    };
}
