using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.Tests;

[TestClass]
public sealed class SecurityTests
{
    [TestMethod]
    [DataRow("https://127.0.0.1/stream")]
    [DataRow("https://192.168.1.20/stream")]
    [DataRow("https://[::1]/stream")]
    [DataRow("https://localhost/stream")]
    [DataRow("http://example.com/stream")]
    [DataRow("https://user:password@example.com/stream")]
    public void NetworkPolicyRejectsUnsafeDestinations(string value)
    {
        Assert.IsFalse(NetworkUriPolicy.TryCreatePublicHttpsUri(value, out _));
    }

    [TestMethod]
    public async Task StreamProbeRejectsPrivateAddressBeforeSendingRequest()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            throw new AssertFailedException("No debe realizarse una solicitud privada."));
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => probe.ProbeAsync(
            new Uri("https://127.0.0.1/audio"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None));
    }

    [TestMethod]
    public async Task CatalogUpdateRejectsChunkedBodyAboveLimit()
    {
        using var directory = new TemporaryDirectory();
        string localPath = directory.GetPath("local.json");
        var localCatalog = CreateCatalog(1);
        await File.WriteAllTextAsync(localPath, JsonSerializer.Serialize(localCatalog));

        string oversizedJson = JsonSerializer.Serialize(CreateCatalog(2)) +
            new string(' ', 600 * 1024);
        using var client = new HttpClient(new StubHttpMessageHandler((_, _) =>
        {
            var content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(oversizedJson)));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentLength = null;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content,
                RequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, "https://example.test/catalog.json")
            });
        }));
        using var service = new RadioCatalogService(
            client, localPath, directory.Path, new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://example.test/catalog.json", TimeSpan.FromSeconds(2));

        Assert.IsFalse(result.UpdatedFromRemote);
        Assert.AreEqual(1, result.Catalog.Version);
        Assert.IsNotNull(result.Warning);
    }

    [TestMethod]
    public void CatalogValidatorRejectsNullAndOversizedFields()
    {
        StationCatalog catalog = CreateCatalog(1);
        catalog.Stations.Add(null!);
        catalog.Stations[0].Nombre = new string('A', 101);

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsFalse(valid);
        Assert.IsTrue(errors.Any(error => error.Contains("nombre", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(errors.Any(error => error.Contains("vacía", StringComparison.OrdinalIgnoreCase)));
    }

    private static StationCatalog CreateCatalog(int version) => new()
    {
        Version = version,
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
