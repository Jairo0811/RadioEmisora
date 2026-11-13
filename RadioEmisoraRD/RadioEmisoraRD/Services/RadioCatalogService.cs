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
    internal const long MaxCatalogSizeBytes = 512 * 1024;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        MaxDepth = 16
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

        if (!NetworkUriPolicy.TryCreatePublicHttpsUri(remoteUrl, out Uri? remoteUri))
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
            (HttpResponseMessage response, _) = await NetworkUriPolicy.SendWithSafeRedirectsAsync(
                httpClient,
                remoteUri,
                static request => request.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")),
                linkedCancellation.Token);

            StationCatalog? remoteCatalog;
            using (response)
            {
                response.EnsureSuccessStatusCode();

                if (response.Content.Headers.ContentLength > MaxCatalogSizeBytes)
                    throw new InvalidDataException("El catálogo remoto supera el tamaño permitido.");

                await using Stream stream = await response.Content.ReadAsStreamAsync(linkedCancellation.Token);
                await using MemoryStream boundedContent = await ReadBoundedAsync(
                    stream,
                    MaxCatalogSizeBytes,
                    linkedCancellation.Token);
                remoteCatalog = await JsonSerializer.DeserializeAsync<StationCatalog>(
                    boundedContent,
                    SerializerOptions,
                    linkedCancellation.Token);
            }

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
        catch (Exception exception) when (exception is InvalidDataException or JsonException)
        {
            logger.Warning($"El catálogo remoto fue rechazado: {AppLogger.Summarize(exception)}");
            return CurrentWithWarning("El catálogo remoto no es válido; se mantiene la copia local.");
        }
        catch (Exception exception) when (exception is HttpRequestException or IOException)
        {
            logger.Warning($"No se pudo actualizar el catálogo remoto: {AppLogger.Summarize(exception)}");
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
            if (new FileInfo(path).Length > MaxCatalogSizeBytes)
            {
                logger.Warning($"Se ignoró el catálogo '{Path.GetFileName(path)}' porque supera el tamaño permitido.");
                return null;
            }

            using FileStream stream = File.OpenRead(path);
            StationCatalog? catalog = JsonSerializer.Deserialize<StationCatalog>(stream, SerializerOptions);

            if (StationCatalogValidator.TryValidate(catalog, out IReadOnlyList<string> errors))
                return catalog;

            logger.Warning($"Catálogo inválido en '{Path.GetFileName(path)}': {string.Join(" ", errors.Take(3))}");
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.Warning($"No se pudo leer el catálogo '{Path.GetFileName(path)}': {AppLogger.Summarize(exception)}");
        }

        return null;
    }

    private static HttpClient CreateHttpClient() => NetworkUriPolicy.CreatePublicHttpClient();

    internal static async Task<MemoryStream> ReadBoundedAsync(
        Stream source,
        long maxBytes,
        CancellationToken cancellationToken)
    {
        var destination = new MemoryStream();
        byte[] buffer = new byte[16 * 1024];

        try
        {
            while (true)
            {
                int read = await source.ReadAsync(buffer, cancellationToken);
                if (read == 0)
                    break;

                if (destination.Length + read > maxBytes)
                    throw new InvalidDataException("El contenido remoto supera el tamaño permitido.");

                await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }

            destination.Position = 0;
            return destination;
        }
        catch
        {
            await destination.DisposeAsync();
            throw;
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);
}
