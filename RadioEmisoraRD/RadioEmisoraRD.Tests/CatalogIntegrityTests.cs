using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Text.Json;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.Tests;

[TestClass]
public sealed class CatalogIntegrityTests
{
    private const long MaxLogoBytes = 1024 * 1024;
    private const int MaxLogoDimension = 1024;
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    [TestMethod]
    public void BundledCatalogPassesValidation()
    {
        StationCatalog catalog = LoadRepositoryCatalog();

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsTrue(valid, string.Join(Environment.NewLine, errors));
    }

    [TestMethod]
    public void BundledCatalogUsesHttpsForEveryStream()
    {
        StationCatalog catalog = LoadRepositoryCatalog();

        foreach (StationCatalogItem station in catalog.Stations)
        {
            Assert.IsTrue(
                NetworkUriPolicy.TryCreatePublicHttpsUri(station.StreamUrl, out _),
                $"{station.Id} no usa una URL HTTPS pública.");
        }
    }

    [TestMethod]
    public void EveryCatalogLogoExistsIsValidPngAndReasonablySized()
    {
        StationCatalog catalog = LoadRepositoryCatalog();
        string logosDirectory = GetLogosDirectory();

        foreach (StationCatalogItem station in catalog.Stations)
        {
            string path = Path.Combine(logosDirectory, station.Logo);
            Assert.IsTrue(File.Exists(path), $"Falta el logo '{station.Logo}' de {station.Id}.");

            byte[] header = new byte[24];
            using (FileStream stream = File.OpenRead(path))
            {
                stream.ReadExactly(header);
                Assert.IsTrue(stream.Length <= MaxLogoBytes, $"{station.Logo} supera 1 MB.");
            }

            CollectionAssert.AreEqual(PngSignature, header[..8], $"{station.Logo} no es un PNG.");
            int width = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(16, 4));
            int height = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(20, 4));
            Assert.IsTrue(width is > 0 and <= MaxLogoDimension, $"{station.Logo}: ancho {width}.");
            Assert.IsTrue(height is > 0 and <= MaxLogoDimension, $"{station.Logo}: alto {height}.");
        }
    }

    [TestMethod]
    public void EveryBundledLogoIsReferencedByTheCatalog()
    {
        HashSet<string> referenced = LoadRepositoryCatalog().Stations
            .Select(static station => station.Logo)
            .ToHashSet(StringComparer.Ordinal);

        string[] unused = Directory.GetFiles(GetLogosDirectory(), "*.png")
            .Select(static path => Path.GetFileName(path))
            .Where(name => !referenced.Contains(name))
            .ToArray();

        Assert.AreEqual(0, unused.Length, "Logos sin uso: " + string.Join(", ", unused));
    }

    /// <summary>
    /// Smoke test opcional contra los endpoints reales. Se omite salvo que
    /// RADIOEMISORARD_LIVE_STREAMS=1, porque depende de servicios externos.
    /// CI lo ejecuta en un paso informativo que no bloquea la build.
    /// </summary>
    [TestMethod]
    [TestCategory("LiveStreams")]
    public async Task LiveCatalogStreamsRespondWithAudio()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("RADIOEMISORARD_LIVE_STREAMS"),
                "1",
                StringComparison.Ordinal))
        {
            Assert.Inconclusive("Define RADIOEMISORARD_LIVE_STREAMS=1 para probar los streams reales.");
        }

        StationCatalog catalog = LoadRepositoryCatalog();
        using var probe = new StreamProbe();

        (StationCatalogItem Station, string Result, bool Ok)[] results = await Task.WhenAll(
            catalog.Stations.Select(async station =>
            {
                try
                {
                    StreamProbeResult result = await probe.ProbeAsync(
                        new Uri(station.StreamUrl),
                        TimeSpan.FromSeconds(20),
                        CancellationToken.None);
                    string redirect = result.FinalUri == new Uri(station.StreamUrl)
                        ? string.Empty
                        : $" → {result.FinalUri.Host}";
                    return (station, $"✅ {result.ContentType ?? "sin Content-Type"}{redirect}", true);
                }
                catch (Exception exception) when (exception is HttpRequestException or IOException or
                    InvalidDataException or TimeoutException or OperationCanceledException)
                {
                    return (station, "❌ " + AppLogger.Summarize(exception), false);
                }
            }));

        var report = new StringBuilder();
        report.AppendLine("### Smoke test de streams del catálogo");
        report.AppendLine();
        report.AppendLine("| Emisora | Host | Resultado |");
        report.AppendLine("|---|---|---|");
        foreach ((StationCatalogItem station, string result, _) in results)
        {
            report.AppendLine(CultureInfo.InvariantCulture,
                $"| {station.Nombre} | {new Uri(station.StreamUrl).Authority} | {result.Replace('|', '/')} |");
        }

        string markdown = report.ToString();
        Console.WriteLine(markdown);

        string? summaryPath = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY");
        if (!string.IsNullOrWhiteSpace(summaryPath))
            await File.AppendAllTextAsync(summaryPath, markdown);

        string[] failed = results.Where(static item => !item.Ok).Select(static item => item.Station.Id).ToArray();
        Assert.AreEqual(0, failed.Length, "Streams sin respuesta válida: " + string.Join(", ", failed));
    }

    private static StationCatalog LoadRepositoryCatalog()
    {
        string path = Path.Combine(FindRepositoryRoot(), "catalog", "emisoras.json");
        StationCatalog? catalog = JsonSerializer.Deserialize<StationCatalog>(
            File.ReadAllText(path),
            SerializerOptions);
        Assert.IsNotNull(catalog);
        return catalog;
    }

    private static string GetLogosDirectory() =>
        Path.Combine(FindRepositoryRoot(), "RadioEmisoraRD", "RadioEmisoraRD", "Assets", "logos");

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "catalog", "emisoras.json")))
                return directory.FullName;

            directory = directory.Parent;
        }

        Assert.Fail("No se encontró la raíz del repositorio (catalog/emisoras.json).");
        return string.Empty;
    }
}
