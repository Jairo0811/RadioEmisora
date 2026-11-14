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
    private static readonly string[] SafeHistory = ["Mortal", "Z 101"];
    private static readonly byte[] AudioBytes = [1, 2, 3];

    [TestMethod]
    [DataRow("https://127.0.0.1/stream")]
    [DataRow("https://10.0.0.5/stream")]
    [DataRow("https://172.16.4.1/stream")]
    [DataRow("https://192.168.1.20/stream")]
    [DataRow("https://169.254.169.254/latest/meta-data")]
    [DataRow("https://100.64.0.1/stream")]
    [DataRow("https://0.0.0.0/stream")]
    [DataRow("https://[::1]/stream")]
    [DataRow("https://[fe80::1]/stream")]
    [DataRow("https://[fd00::1]/stream")]
    [DataRow("https://[::ffff:127.0.0.1]/stream")]
    [DataRow("https://[64:ff9b::a00:1]/stream")]
    [DataRow("https://[2002:c0a8:0101::1]/stream")]
    [DataRow("https://localhost/stream")]
    [DataRow("https://LOCALHOST./stream")]
    [DataRow("https://radio.localhost/stream")]
    [DataRow("https://equipo.local/stream")]
    [DataRow("https://servicio.internal/stream")]
    [DataRow("https://router.home.arpa/stream")]
    [DataRow("https://router/stream")]
    [DataRow("http://example.com/stream")]
    [DataRow("ftp://example.com/stream")]
    [DataRow("file:///C:/Windows/win.ini")]
    [DataRow("https://user:password@example.com/stream")]
    [DataRow("https://example.com/stream#fragment")]
    [DataRow("https://example.com/st\nream")]
    [DataRow("/relative/stream")]
    [DataRow("")]
    public void NetworkPolicyRejectsUnsafeDestinations(string value)
    {
        Assert.IsFalse(NetworkUriPolicy.TryCreatePublicHttpsUri(value, out _));
    }

    [TestMethod]
    [DataRow("https://n07.radiojar.com/x84tcy2wm2zuv")]
    [DataRow("https://sonicpanel.streaming10.net:8146/stream")]
    [DataRow("https://raw.githubusercontent.com/Jairo0811/RadioEmisora/master/catalog/emisoras.json")]
    [DataRow("https://8.8.8.8/stream")]
    [DataRow("https://[2606:4700:4700::1111]/stream")]
    [DataRow("https://[64:ff9b::808:808]/stream")]
    public void NetworkPolicyAcceptsPublicHttpsDestinations(string value)
    {
        Assert.IsTrue(NetworkUriPolicy.TryCreatePublicHttpsUri(value, out Uri? uri));
        Assert.IsNotNull(uri);
    }

    [TestMethod]
    public void NetworkPolicyRejectsExcessivelyLongUrls()
    {
        string url = "https://example.com/" + new string('a', NetworkUriPolicy.MaxUriLength);

        Assert.IsFalse(NetworkUriPolicy.TryCreatePublicHttpsUri(url, out _));
    }

    [TestMethod]
    [DataRow("127.0.0.1")]
    [DataRow("10.1.2.3")]
    [DataRow("192.168.0.1")]
    [DataRow("169.254.10.10")]
    [DataRow("224.0.0.1")]
    [DataRow("::1")]
    [DataRow("fe80::1")]
    [DataRow("fc00::1")]
    [DataRow("ff02::1")]
    [DataRow("2001:db8::1")]
    public void NetworkPolicyRejectsResolvedInternalAddresses(string address)
    {
        Assert.IsFalse(NetworkUriPolicy.IsPublicAddress(IPAddress.Parse(address)));
    }

    [TestMethod]
    public async Task StreamProbeRejectsPrivateAddressBeforeSendingRequest()
    {
        var handler = new RecordingHandler(_ =>
            throw new AssertFailedException("No debe enviarse una solicitud a la red privada."));
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => probe.ProbeAsync(
            new Uri("https://127.0.0.1/audio"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None));
        Assert.AreEqual(0, handler.Calls);
    }

    [TestMethod]
    [DataRow("https://192.168.0.10/audio")]
    [DataRow("http://192.168.0.10/audio")]
    [DataRow("http://localhost/audio")]
    [DataRow("https://localhost/audio")]
    [DataRow("http://user:secret@cdn.example.test/audio")]
    [DataRow("ftp://cdn.example.test/audio")]
    public async Task StreamProbeBlocksUnsafeRedirectWithoutFollowingIt(string location)
    {
        var handler = new RecordingHandler(_ => Redirect(location));
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => probe.ProbeAsync(
            new Uri("https://radio.example.test/live"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None));
        Assert.AreEqual(1, handler.Calls);
    }

    [TestMethod]
    public async Task StreamProbeFollowsSafeRedirectsAndReportsFinalUri()
    {
        var handler = new RecordingHandler(request =>
            request.RequestUri!.Host == "radio.example.test"
                ? Redirect("https://cdn.example.test/live?token=abc")
                : Audio());
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        StreamProbeResult result = await probe.ProbeAsync(
            new Uri("https://radio.example.test/live"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None);

        Assert.AreEqual(new Uri("https://cdn.example.test/live?token=abc"), result.FinalUri);
        Assert.AreEqual(2, handler.Calls);
    }

    [TestMethod]
    [DataRow("http://n0a.radio.example.test/live?rj-tok=abc", "https://n0a.radio.example.test/live?rj-tok=abc")]
    [DataRow("http://n0a.radio.example.test:80/live", "https://n0a.radio.example.test/live")]
    [DataRow("http://cdn.example.test:8000/live", "https://cdn.example.test:8000/live")]
    public async Task StreamProbeUpgradesHttpRedirectsToHttpsAndNeverRequestsHttp(
        string location,
        string expectedFinalUri)
    {
        var requested = new List<Uri>();
        var handler = new RecordingHandler(request =>
        {
            requested.Add(request.RequestUri!);
            return request.RequestUri!.Host == "radio.example.test" ? Redirect(location) : Audio();
        });
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        StreamProbeResult result = await probe.ProbeAsync(
            new Uri("https://radio.example.test/live"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None);

        Assert.AreEqual(new Uri(expectedFinalUri), result.FinalUri);
        Assert.IsTrue(requested.TrueForAll(static uri => uri.Scheme == Uri.UriSchemeHttps));
        Assert.AreEqual(2, handler.Calls);
    }

    [TestMethod]
    public async Task StreamProbeStopsAfterTooManyRedirects()
    {
        var handler = new RecordingHandler(_ => Redirect("https://radio.example.test/again"));
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => probe.ProbeAsync(
            new Uri("https://radio.example.test/live"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None));
        Assert.AreEqual(NetworkUriPolicy.MaxRedirects + 1, handler.Calls);
    }

    [TestMethod]
    public async Task StreamProbeRejectsNonAudioContent()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html></html>", Encoding.UTF8, "text/html")
        });
        using var client = new HttpClient(handler);
        using var probe = new StreamProbe(client);

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => probe.ProbeAsync(
            new Uri("https://radio.example.test/live"),
            TimeSpan.FromSeconds(1),
            CancellationToken.None));
    }

    [TestMethod]
    public async Task MediaPlayerServiceRejectsInsecureStreamWithoutProbing()
    {
        var probe = new CountingProbe(_ => throw new AssertFailedException("No debe sondearse."));
        using var service = new MediaPlayerService(new NullEngine(), probe, new TestLogger());

        await service.PlayAsync("http://radio.example.test/live", FastOptions());

        Assert.AreEqual(PlayerState.Error, service.State);
        Assert.AreEqual(0, probe.Calls);
    }

    [TestMethod]
    public async Task MediaPlayerServiceDoesNotRetryBlockedOrInvalidStreams()
    {
        var probe = new CountingProbe(_ => throw new InvalidDataException("bloqueado"));
        using var service = new MediaPlayerService(new NullEngine(), probe, new TestLogger());
        PlaybackErrorEventArgs? reported = null;
        service.PlaybackError += (_, args) => reported = args;

        await service.PlayAsync("https://radio.example.test/live", FastOptions());

        Assert.AreEqual(PlayerState.Error, service.State);
        Assert.AreEqual(1, probe.Calls);
        Assert.IsNotNull(reported);
    }

    [TestMethod]
    public async Task CatalogUpdateRejectsChunkedBodyAboveLimit()
    {
        using var directory = new TemporaryDirectory();
        string localPath = await WriteLocalCatalogAsync(directory);
        string oversizedJson = JsonSerializer.Serialize(CreateCatalog(2)) +
            new string(' ', (int)RadioCatalogService.MaxCatalogSizeBytes + 1024);
        using var client = new HttpClient(new RecordingHandler(_ =>
        {
            var content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(oversizedJson)));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentLength = null;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        }));
        using var service = new RadioCatalogService(client, localPath, directory.Path, new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://example.test/catalog.json",
            TimeSpan.FromSeconds(2));

        Assert.IsFalse(result.UpdatedFromRemote);
        Assert.AreEqual(1, result.Catalog.Version);
        Assert.IsNotNull(result.Warning);
        Assert.IsFalse(File.Exists(directory.GetPath("catalogo.json")));
    }

    [TestMethod]
    public async Task CatalogUpdateRejectsDeeplyNestedJson()
    {
        using var directory = new TemporaryDirectory();
        string localPath = await WriteLocalCatalogAsync(directory);
        string catalogJson = JsonSerializer.Serialize(CreateCatalog(2));
        string nested = new string('[', 40) + new string(']', 40);
        string maliciousJson = "{\"extra\":" + nested + "," + catalogJson[1..];
        using var client = new HttpClient(new RecordingHandler(_ =>
            StubHttpMessageHandler.Json(maliciousJson)));
        using var service = new RadioCatalogService(client, localPath, directory.Path, new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://example.test/catalog.json",
            TimeSpan.FromSeconds(2));

        Assert.IsFalse(result.UpdatedFromRemote);
        Assert.AreEqual(1, result.Catalog.Version);
        Assert.IsNotNull(result.Warning);
    }

    [TestMethod]
    public async Task CatalogUpdateBlocksRedirectToLoopback()
    {
        using var directory = new TemporaryDirectory();
        string localPath = await WriteLocalCatalogAsync(directory);
        var handler = new RecordingHandler(_ => Redirect("http://127.0.0.1/catalog.json"));
        using var client = new HttpClient(handler);
        using var service = new RadioCatalogService(client, localPath, directory.Path, new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://example.test/catalog.json",
            TimeSpan.FromSeconds(2));

        Assert.IsFalse(result.UpdatedFromRemote);
        Assert.IsNotNull(result.Warning);
        Assert.AreEqual(1, handler.Calls);
    }

    [TestMethod]
    public async Task CatalogUpdateRejectsPrivateConfiguredUrlWithoutNetworkAccess()
    {
        using var directory = new TemporaryDirectory();
        string localPath = await WriteLocalCatalogAsync(directory);
        var handler = new RecordingHandler(_ =>
            throw new AssertFailedException("No debe consultarse una URL privada."));
        using var client = new HttpClient(handler);
        using var service = new RadioCatalogService(client, localPath, directory.Path, new TestLogger());

        CatalogLoadResult result = await service.TryUpdateAsync(
            "https://192.168.1.1/catalog.json",
            TimeSpan.FromSeconds(2));

        Assert.IsFalse(result.UpdatedFromRemote);
        Assert.AreEqual(0, handler.Calls);
    }

    [TestMethod]
    public async Task LocalCatalogLargerThanLimitFallsBackToBundledCatalog()
    {
        using var directory = new TemporaryDirectory();
        string localPath = await WriteLocalCatalogAsync(directory);
        string cachedJson = JsonSerializer.Serialize(CreateCatalog(5)) +
            new string(' ', (int)RadioCatalogService.MaxCatalogSizeBytes + 1);
        await File.WriteAllTextAsync(directory.GetPath("catalogo.json"), cachedJson);
        using var client = new HttpClient(new RecordingHandler(_ => Audio()));
        using var service = new RadioCatalogService(client, localPath, directory.Path, new TestLogger());

        CatalogLoadResult result = service.LoadLocal();

        Assert.AreEqual(1, result.Catalog.Version);
    }

    [TestMethod]
    public void CatalogValidatorRejectsNullAndOversizedFields()
    {
        StationCatalog catalog = CreateCatalog(1);
        catalog.Stations.Add(null!);
        catalog.Stations[0].Nombre = new string('A', 101);
        catalog.Stations[0].Grupo = "Grupo\u0007";

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsFalse(valid);
        Assert.IsTrue(errors.Any(error => error.Contains("nombre", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(errors.Any(error => error.Contains("grupo", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(errors.Any(error => error.Contains("vacía", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    [DataRow("../mortal.png")]
    [DataRow("..\\mortal.png")]
    [DataRow("C:\\logos\\mortal.png")]
    [DataRow("/Assets/logos/mortal.png")]
    [DataRow("pack://application:,,,/x.png")]
    [DataRow("mortal.svg")]
    [DataRow("Mortal.PNG")]
    [DataRow(".png")]
    [DataRow("mortal.png ")]
    [DataRow("mortal.png\n")]
    public void CatalogValidatorRejectsUnsafeLogoNames(string logo)
    {
        StationCatalog catalog = CreateCatalog(1);
        catalog.Stations[0].Logo = logo;

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsFalse(valid);
        Assert.IsTrue(errors.Any(error => error.Contains("logo", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    [DataRow("Mortal")]
    [DataRow("mortal_1049")]
    [DataRow("mortal--1049")]
    [DataRow("-mortal")]
    [DataRow("mortal/1049")]
    [DataRow("mortal-1049\n")]
    public void CatalogValidatorRejectsInvalidIdentifiers(string id)
    {
        StationCatalog catalog = CreateCatalog(1);
        catalog.Stations[0].Id = id;

        Assert.IsFalse(StationCatalogValidator.TryValidate(catalog, out _));
    }

    [TestMethod]
    [DataRow("#DC283C\n")]
    [DataRow("#DC283")]
    [DataRow("red")]
    [DataRow("#DC283C00")]
    public void CatalogValidatorRejectsInvalidThemeColors(string color)
    {
        StationCatalog catalog = CreateCatalog(1);
        catalog.Stations[0].ColorTema = color;

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsFalse(valid);
        Assert.IsTrue(errors.Any(error => error.Contains("color", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void CatalogValidatorRejectsPrivateStreamDestinations()
    {
        StationCatalog catalog = CreateCatalog(1);
        catalog.Stations[0].StreamUrl = "https://10.0.0.8:8000/live";

        bool valid = StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors);

        Assert.IsFalse(valid);
        Assert.IsTrue(errors.Any(error => error.Contains("HTTPS", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void CatalogValidatorRejectsCatalogsAboveStationLimit()
    {
        StationCatalog catalog = CreateCatalog(1);
        StationCatalogItem template = catalog.Stations[0];
        catalog.Stations.Clear();

        for (int index = 0; index <= StationCatalogValidator.MaxStations; index++)
        {
            catalog.Stations.Add(new StationCatalogItem
            {
                Id = $"emisora-{index}",
                Nombre = $"Emisora {index}",
                Frecuencia = template.Frecuencia,
                Categoria = template.Categoria,
                Provincia = template.Provincia,
                Grupo = template.Grupo,
                Logo = template.Logo,
                ColorTema = template.ColorTema,
                StreamUrl = template.StreamUrl
            });
        }

        Assert.IsFalse(StationCatalogValidator.TryValidate(catalog, out _));
    }

    [TestMethod]
    public void ConfigNormalizationDropsMalformedHistoryAndUnsafeCatalogUrl()
    {
        var config = new AppConfig
        {
            UltimaEmisora = "mortal\r\n[ERROR] falso",
            Historial = ["Mortal", new string('x', 500), "Z 101", "linea\nnueva", null!, "  "],
            CatalogUrl = "https://127.0.0.1/catalog.json"
        };

        ConfigService.Normalize(config);

        Assert.AreEqual(string.Empty, config.UltimaEmisora);
        CollectionAssert.AreEqual(SafeHistory, config.Historial);
        Assert.AreEqual(AppConfig.DefaultCatalogUrl, config.CatalogUrl);
    }

    [TestMethod]
    public void ConfigLoadIgnoresOversizedFileAndPreservesIt()
    {
        using var directory = new TemporaryDirectory();
        string oversized = "{\"volumen\":0.25,\"relleno\":\"" +
            new string('x', (int)JsonFileStore<AppConfig>.MaxFileSizeBytes) + "\"}";
        File.WriteAllText(directory.GetPath("config.json"), oversized);
        var service = new ConfigService(directory.Path, new TestLogger());

        AppConfig config = service.Load();

        Assert.AreEqual(0.80, config.Volumen, 0.001);
        Assert.AreEqual(1, Directory.GetFiles(directory.Path, "config.corrupt-*.json").Length);
    }

    [TestMethod]
    public void ConfigLoadRejectsDeeplyNestedJson()
    {
        using var directory = new TemporaryDirectory();
        string nested = new string('[', 40) + new string(']', 40);
        File.WriteAllText(directory.GetPath("config.json"), "{\"extra\":" + nested + ",\"volumen\":0.3}");
        var service = new ConfigService(directory.Path, new TestLogger());

        AppConfig config = service.Load();

        Assert.AreEqual(0.80, config.Volumen, 0.001);
    }

    [TestMethod]
    public void FavoriteServiceBoundsAndSanitizesEntries()
    {
        using var directory = new TemporaryDirectory();
        var service = new FavoriteService(directory.Path, new TestLogger());
        List<string> favorites = Enumerable.Range(0, 600).Select(static index => $"emisora-{index}").ToList();
        favorites.Add("mala\u0000entrada");
        favorites.Add(new string('a', 300));

        bool saved = service.TrySave(favorites, out string? error);
        IReadOnlyList<string> restored = service.Load();

        Assert.IsTrue(saved, error);
        Assert.AreEqual(FavoriteService.MaxFavorites, restored.Count);
        Assert.IsFalse(restored.Any(static item => item.Any(char.IsControl) || item.Length > 100));
    }

    [TestMethod]
    public void HistoryRegisterRejectsMalformedStationNames()
    {
        var config = new AppConfig();

        Assert.ThrowsExactly<ArgumentException>(() =>
            HistoryService.Register(config, "Mortal\r\nFalso"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            HistoryService.Register(config, new string('x', 101)));
        Assert.AreEqual(0, config.Historial.Count);
    }

    [TestMethod]
    public void LoggerRotatesFileWhenSizeLimitIsReached()
    {
        using var directory = new TemporaryDirectory();
        var logger = new AppLogger(directory.Path, new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));
        File.WriteAllBytes(logger.LogFilePath, new byte[AppLogger.MaxLogFileSizeBytes]);

        logger.Info("después de rotar");

        Assert.IsTrue(File.Exists(logger.LogFilePath + ".1"));
        Assert.IsTrue(new FileInfo(logger.LogFilePath).Length < 1024);
    }

    [TestMethod]
    public void LoggerKeepsALimitedNumberOfFiles()
    {
        using var directory = new TemporaryDirectory();
        for (int day = 1; day <= 20; day++)
            File.WriteAllText(directory.GetPath($"radioemisorard-202610{day:00}.log"), "antiguo");
        File.WriteAllText(directory.GetPath("otro-archivo.txt"), "no es un log");
        var logger = new AppLogger(directory.Path, new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        logger.Info("nuevo");

        string[] logs = Directory.GetFiles(directory.Path, "radioemisorard-*.log*");
        Assert.AreEqual(AppLogger.MaxRetainedLogFiles, logs.Length);
        Assert.IsTrue(File.Exists(logger.LogFilePath));
        Assert.IsTrue(File.Exists(directory.GetPath("radioemisorard-20261020.log")));
        Assert.IsFalse(File.Exists(directory.GetPath("radioemisorard-20261001.log")));
        Assert.IsTrue(File.Exists(directory.GetPath("otro-archivo.txt")));
    }

    [TestMethod]
    public void LoggerWritesOneLinePerMessage()
    {
        using var directory = new TemporaryDirectory();
        var logger = new AppLogger(directory.Path, new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc));

        logger.Warning("primera\r\n2026-01-01 [ERROR] entrada falsa");

        string[] lines = File.ReadAllLines(logger.LogFilePath);
        Assert.AreEqual(1, lines.Length);
        Assert.IsTrue(lines[0].Contains("[WARN]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void LoggerSummaryOmitsStackTrace()
    {
        Exception exception;
        try
        {
            throw new HttpRequestException("sin conexión", new IOException("socket"));
        }
        catch (HttpRequestException caught)
        {
            exception = caught;
        }

        string summary = AppLogger.Summarize(exception);

        Assert.AreEqual("HttpRequestException: sin conexión (IOException: socket)", summary);
    }

    private static PlaybackOptions FastOptions() =>
        new(TimeSpan.FromSeconds(1), 2, TimeSpan.FromMilliseconds(1));

    private static HttpResponseMessage Redirect(string location)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Found);
        response.Headers.Location = new Uri(location, UriKind.RelativeOrAbsolute);
        return response;
    }

    private static HttpResponseMessage Audio()
    {
        var content = new ByteArrayContent(AudioBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
    }

    private static async Task<string> WriteLocalCatalogAsync(TemporaryDirectory directory)
    {
        string localPath = directory.GetPath("local.json");
        await File.WriteAllTextAsync(localPath, JsonSerializer.Serialize(CreateCatalog(1)));
        return localPath;
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

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> respond;

        public RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            this.respond = respond;
        }

        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(respond(request));
        }
    }

    private sealed class CountingProbe : IStreamProbe
    {
        private readonly Func<Uri, StreamProbeResult> probe;

        public CountingProbe(Func<Uri, StreamProbeResult> probe)
        {
            this.probe = probe;
        }

        public int Calls { get; private set; }

        public Task<StreamProbeResult> ProbeAsync(
            Uri streamUri,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(probe(streamUri));
        }
    }

    private sealed class NullEngine : IMediaPlaybackEngine
    {
        public event EventHandler? Opened
        {
            add { }
            remove { }
        }

        public event EventHandler? BufferingStarted
        {
            add { }
            remove { }
        }

        public event EventHandler? BufferingEnded
        {
            add { }
            remove { }
        }

        public event EventHandler<PlaybackEngineFailedEventArgs>? Failed
        {
            add { }
            remove { }
        }

        public double Volume { get; set; }

        public void Open(Uri source)
        {
        }

        public void Play()
        {
        }

        public void Pause()
        {
        }

        public void StopPlayback()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}
