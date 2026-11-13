using System.Net.Http;
using System.Net.Http.Headers;

namespace RadioEmisoraRD.Services;

public sealed record StreamProbeResult(Uri FinalUri, string? ContentType);

public interface IStreamProbe
{
    Task<StreamProbeResult> ProbeAsync(
        Uri streamUri,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}

public sealed class StreamProbe : IStreamProbe, IDisposable
{
    private static readonly string[] SupportedApplicationContentTypes =
    [
        "application/octet-stream",
        "application/vnd.apple.mpegurl",
        "application/x-mpegurl"
    ];

    private readonly HttpClient httpClient;
    private readonly bool ownsHttpClient;
    private bool disposed;

    public StreamProbe(HttpClient? httpClient = null)
    {
        this.httpClient = httpClient ?? CreateHttpClient();
        ownsHttpClient = httpClient is null;
    }

    public async Task<StreamProbeResult> ProbeAsync(
        Uri streamUri,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(streamUri);

        if (!NetworkUriPolicy.IsPublicHttpsUri(streamUri))
            throw new InvalidDataException("El destino del stream no es público o no usa HTTPS.");

        using var timeoutCancellation = new CancellationTokenSource(timeout);
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutCancellation.Token);

        HttpResponseMessage response;
        Uri finalUri;

        try
        {
            (response, finalUri) = await NetworkUriPolicy.SendWithSafeRedirectsAsync(
                httpClient,
                streamUri,
                static request =>
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/*"));
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.1));
                    request.Headers.TryAddWithoutValidation("Icy-MetaData", "1");
                },
                linkedCancellation.Token);
        }
        catch (OperationCanceledException exception) when (
            !cancellationToken.IsCancellationRequested && timeoutCancellation.IsCancellationRequested)
        {
            throw new TimeoutException("El stream agotó el tiempo de conexión.", exception);
        }

        using (response)
        {
            response.EnsureSuccessStatusCode();

            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (!IsSupportedContentType(mediaType))
                throw new InvalidDataException("El servidor no devolvió un stream de audio válido.");

            return new StreamProbeResult(finalUri, mediaType);
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;

        if (ownsHttpClient)
            httpClient.Dispose();

        GC.SuppressFinalize(this);
    }

    private static bool IsSupportedContentType(string? mediaType)
    {
        if (string.IsNullOrWhiteSpace(mediaType))
            return true;

        return mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
            SupportedApplicationContentTypes.Contains(mediaType, StringComparer.OrdinalIgnoreCase);
    }

    private static HttpClient CreateHttpClient() => NetworkUriPolicy.CreatePublicHttpClient();
}
