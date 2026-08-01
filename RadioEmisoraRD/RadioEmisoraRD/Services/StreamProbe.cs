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

        using var timeoutCancellation = new CancellationTokenSource(timeout);
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutCancellation.Token);
        using var request = new HttpRequestMessage(HttpMethod.Get, streamUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/*"));
        request.Headers.TryAddWithoutValidation("Icy-MetaData", "1");

        HttpResponseMessage response;

        try
        {
            response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
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

            Uri finalUri = response.RequestMessage?.RequestUri ?? streamUri;
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
}
