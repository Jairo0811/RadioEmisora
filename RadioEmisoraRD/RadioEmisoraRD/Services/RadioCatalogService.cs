using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public interface IRadioCatalogService
{
    CatalogLoadResult LoadLocal();

    Task<CatalogLoadResult> TryUpdateAsync(
        string remoteUrl,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);
}

public sealed class RadioCatalogService : IRadioCatalogService, IDisposable
{
    private const long MaxCatalogSizeBytes = 512 * 1024;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient httpClient;
    private readonly bool ownsHttpClient;
    private readonly string localCatalogPath;
    private readonly string cachedCatalogPath;
    private readonly JsonFileStore<StationCatalog> cachedCatalogStore;
    private readonly IAppLogger logger;
    private readonly SemaphoreSlim updateGate = new(1, 1);
    private StationCatalog? currentCatalog;
    private bool disposed;

    public RadioCatalogService(
        HttpClient? httpClient = null,
        string? localCatalogPath = null,
        string? dataDirectory = null,
        IAppLogger? logger = null)
    {
        this.httpClient = httpClient ?? CreateHttpClient();
        ownsHttpClient = httpClient is null;
        this.localCatalogPath = localCatalogPath ?? AppPaths.LocalCatalogPath;
        string directory = dataDirectory ?? AppPaths.DataDirectory;
        cachedCatalogPath = Path.Combine(directory, "catalogo.json");
        this.logger = logger ?? AppLogger.Current;
        cachedCatalogStore = new JsonFileStore<StationCatalog>(cachedCatalogPath, this.logger);
    }

    public CatalogLoadResult LoadLocal()
    {
        ThrowIfDisposed();

        StationCatalog bundledCatalog = ReadAndValidate(localCatalogPath) ??
            throw new InvalidDataException(
                "El catálogo local de emisoras no existe o no es válido.");

        StationCatalog? cachedCatalog = ReadAndValidate(cachedCatalogPath);
        currentCatalog = cachedCatalog is not null && cachedCatalog.Version >= bundledCatalog.Version
            ? cachedCatalog
            : bundledCatalog;

        string source = ReferenceEquals(currentCatalog, cachedCatalog) ? "Caché local" : "Catálogo incluido";
        return new CatalogLoadResult(currentCatalog, false, source);
    }

    public async Task<CatalogLoadResult> TryUpdateAsync(
        string remoteUrl,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (!Uri.TryCreate(remoteUrl, UriKind.Absolute, out Uri? remoteUri) ||
            remoteUri.Scheme != Uri.UriSchemeHttps)
        {
            return CurrentWithWarning("La dirección remota del catálogo no es válida.");
        }

        await updateGate.WaitAsync(cancellationToken);

        try
        {
            currentCatalog ??= LoadLocal().Catalog;

            using var timeoutCancellation = new CancellationTokenSource(timeout);
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCancellation.Token);
            using var request = new HttpRequestMessage(HttpMethod.Get, remoteUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using HttpResponseMessage response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                linkedCancellation.Token);

            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength > MaxCatalogSizeBytes)
                throw new InvalidDataException("El catálogo remoto supera el tamaño permitido.");

            await using Stream stream = await response.Content.ReadAsStreamAsync(linkedCancellation.Token);
            StationCatalog? remoteCatalog = await JsonSerializer.DeserializeAsync<StationCatalog>(
                stream,
                SerializerOptions,
                linkedCancellation.Token);

            if (!StationCatalogValidator.TryValidate(remoteCatalog, out IReadOnlyList<string> errors))
            {
                string detail = string.Join(" ", errors.Take(3));
                logger.Warning($"El catálogo remoto fue rechazado. {detail}");
                return CurrentWithWarning("El catálogo remoto no superó la validación.");
            }

            if (remoteCatalog!.Version <= currentCatalog.Version)
                return new CatalogLoadResult(currentCatalog, false, "Catálogo local");

            if (!cachedCatalogStore.TrySave(remoteCatalog, out string? saveError))
                return CurrentWithWarning(saveError ?? "No se pudo guardar el catálogo actualizado.");

            currentCatalog = remoteCatalog;
            logger.Info($"Catálogo actualizado a la versión {remoteCatalog.Version}.");
            return new CatalogLoadResult(remoteCatalog, true, "GitHub");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.Warning("La actualización del catálogo agotó el tiempo de espera.");
            return CurrentWithWarning("No se pudo consultar el catálogo remoto a tiempo.");
        }
        catch (Exception exception) when (
            exception is HttpRequestException or IOException or JsonException)
        {
            logger.Warning("No se pudo actualizar el catálogo remoto.", exception);
            return CurrentWithWarning("Sin conexión al catálogo remoto; se mantiene la copia local.");
        }
        finally
        {
            updateGate.Release();
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        updateGate.Dispose();

        if (ownsHttpClient)
            httpClient.Dispose();

        GC.SuppressFinalize(this);
    }

    private CatalogLoadResult CurrentWithWarning(string warning)
    {
        currentCatalog ??= LoadLocal().Catalog;
        return new CatalogLoadResult(currentCatalog, false, "Catálogo local", warning);
    }

    private StationCatalog? ReadAndValidate(string path)
    {
        if (!File.Exists(path))
            return null;

        try
        {
            using FileStream stream = File.OpenRead(path);
            StationCatalog? catalog = JsonSerializer.Deserialize<StationCatalog>(stream, SerializerOptions);

            if (StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors))
                return catalog;

            logger.Warning($"Catálogo inválido en '{path}': {string.Join(" ", errors.Take(3))}");
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.Warning($"No se pudo leer el catálogo '{path}'.", exception);
        }

        return null;
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = System.Net.DecompressionMethods.All,
            ConnectTimeout = TimeSpan.FromSeconds(8),
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        };

        var client = new HttpClient(handler)
        {
            Timeout = Timeout.InfiniteTimeSpan
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("RadioEmisoraRD/3.1");
        return client;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);
}
