using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;

namespace RadioEmisoraRD.Services;

/// <summary>
/// Política de destinos de red para datos no confiables (catálogo remoto y streams).
/// Solo se aceptan URLs HTTPS absolutas hacia hosts públicos, sin credenciales embebidas.
/// </summary>
public static class NetworkUriPolicy
{
    public const int MaxUriLength = 2048;
    public const int MaxRedirects = 5;

    private static readonly string[] BlockedHostSuffixes =
    [
        ".localhost",
        ".local",
        ".localdomain",
        ".internal",
        ".intranet",
        ".lan",
        ".home.arpa"
    ];

    public static string UserAgent { get; } =
        $"RadioEmisoraRD/{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "3.1"}";

    public static bool TryCreatePublicHttpsUri(
        string? value,
        [NotNullWhen(true)] out Uri? uri)
    {
        uri = null;

        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxUriLength ||
            value.Any(char.IsControl) ||
            !Uri.TryCreate(value, UriKind.Absolute, out Uri? candidate) ||
            candidate.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(candidate.UserInfo) ||
            !string.IsNullOrEmpty(candidate.Fragment) ||
            string.IsNullOrWhiteSpace(candidate.Host))
        {
            return false;
        }

        string host = candidate.DnsSafeHost.TrimEnd('.');
        if (host.Length is 0 or > 253 ||
            host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
            BlockedHostSuffixes.Any(suffix => host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        if (IPAddress.TryParse(host, out IPAddress? address))
        {
            if (!IsPublicAddress(address))
                return false;
        }
        else if (!host.Contains('.', StringComparison.Ordinal))
        {
            // Nombres de una sola etiqueta ("router", "nas") se resuelven normalmente en la red local.
            return false;
        }

        uri = candidate;
        return true;
    }

    public static bool IsPublicHttpsUri(Uri? uri) =>
        uri is not null && uri.IsAbsoluteUri && TryCreatePublicHttpsUri(uri.AbsoluteUri, out _);

    /// <summary>
    /// Handler sin redirecciones automáticas que solo abre sockets hacia direcciones públicas.
    /// La validación ocurre después de resolver DNS y sobre la misma IP que se conecta,
    /// por lo que un DNS que apunte a la red interna no permite eludirla.
    /// </summary>
    public static SocketsHttpHandler CreatePublicNetworkHandler() => new()
    {
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.All,
        ConnectTimeout = TimeSpan.FromSeconds(8),
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        UseProxy = true,
        ConnectCallback = ConnectPublicAsync
    };

    public static HttpClient CreatePublicHttpClient()
    {
        var client = new HttpClient(CreatePublicNetworkHandler())
        {
            Timeout = Timeout.InfiniteTimeSpan,
            MaxResponseContentBufferSize = 1024 * 1024
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        return client;
    }

    /// <summary>
    /// Envía una solicitud siguiendo redirecciones manualmente y validando cada destino.
    /// </summary>
    public static async Task<(HttpResponseMessage Response, Uri FinalUri)> SendWithSafeRedirectsAsync(
        HttpClient httpClient,
        Uri requestUri,
        Action<HttpRequestMessage>? configureRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(requestUri);

        Uri currentUri = requestUri;

        for (int redirect = 0; ; redirect++)
        {
            if (!IsPublicHttpsUri(currentUri))
                throw new InvalidDataException("El destino de red no es público o no usa HTTPS.");

            using var request = new HttpRequestMessage(HttpMethod.Get, currentUri);
            configureRequest?.Invoke(request);

            HttpResponseMessage response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!IsRedirect(response.StatusCode))
                return (response, currentUri);

            Uri? location = response.Headers.Location;
            response.Dispose();

            if (location is null)
                throw new InvalidDataException("La redirección no indica un destino.");

            if (redirect >= MaxRedirects)
                throw new InvalidDataException("Se superó el número máximo de redirecciones.");

            Uri nextUri = location.IsAbsoluteUri ? location : new Uri(currentUri, location);
            if (!IsPublicHttpsUri(nextUri))
                throw new InvalidDataException("Se bloqueó una redirección hacia un destino no permitido.");

            currentUri = nextUri;
        }
    }

    internal static bool IsPublicAddress(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (address.IsIPv4MappedToIPv6)
            address = address.MapToIPv4();

        if (IPAddress.IsLoopback(address) ||
            address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any) ||
            address.Equals(IPAddress.None) || address.Equals(IPAddress.IPv6None))
        {
            return false;
        }

        byte[] bytes = address.GetAddressBytes();

        if (address.AddressFamily == AddressFamily.InterNetwork)
            return IsPublicIPv4(bytes);

        if (address.AddressFamily != AddressFamily.InterNetworkV6 ||
            address.IsIPv6LinkLocal || address.IsIPv6SiteLocal ||
            address.IsIPv6Multicast || address.IsIPv6UniqueLocal)
        {
            return false;
        }

        // NAT64 (64:ff9b::/96): se evalúa la IPv4 embebida para no romper redes IPv6-only.
        if (bytes[0] == 0x00 && bytes[1] == 0x64 && bytes[2] == 0xFF && bytes[3] == 0x9B &&
            bytes.Skip(4).Take(8).All(static value => value == 0))
        {
            return IsPublicIPv4(bytes[12..16]);
        }

        // 6to4 (2002::/16): la IPv4 embebida también debe ser pública.
        if (bytes[0] == 0x20 && bytes[1] == 0x02)
            return IsPublicIPv4(bytes[2..6]);

        // Documentación (2001:db8::/32).
        if (bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x0D && bytes[3] == 0xB8)
            return false;

        // Solo unicast global (2000::/3).
        return (bytes[0] & 0xE0) == 0x20;
    }

    private static bool IsPublicIPv4(byte[] bytes) => bytes[0] switch
    {
        0 or 10 or 127 => false,
        100 when bytes[1] is >= 64 and <= 127 => false,
        169 when bytes[1] == 254 => false,
        172 when bytes[1] is >= 16 and <= 31 => false,
        192 when bytes[1] == 0 && bytes[2] is 0 or 2 => false,
        192 when bytes[1] == 168 => false,
        198 when bytes[1] is 18 or 19 => false,
        198 when bytes[1] == 51 && bytes[2] == 100 => false,
        203 when bytes[1] == 0 && bytes[2] == 113 => false,
        >= 224 => false,
        _ => true
    };

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.MovedPermanently or
        HttpStatusCode.Found or
        HttpStatusCode.SeeOther or
        HttpStatusCode.TemporaryRedirect or
        HttpStatusCode.PermanentRedirect;

    private static async ValueTask<Stream> ConnectPublicAsync(
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken)
    {
        IPAddress[] addresses = await Dns.GetHostAddressesAsync(
            context.DnsEndPoint.Host,
            cancellationToken);

        // Cuando Windows tiene configurado un proxy, el socket se abre hacia ese proxy
        // (elegido por el usuario o su organización) y no hacia el destino solicitado.
        // En ese caso el destino ya fue validado por nombre en TryCreatePublicHttpsUri.
        bool connectingToConfiguredProxy = !string.Equals(
            context.DnsEndPoint.Host,
            context.InitialRequestMessage.RequestUri?.IdnHost,
            StringComparison.OrdinalIgnoreCase);

        if (addresses.Length == 0 ||
            (!connectingToConfiguredProxy && addresses.Any(static address => !IsPublicAddress(address))))
        {
            throw new HttpRequestException("El destino de red no es público.");
        }

        Exception? lastException = null;

        foreach (IPAddress address in addresses)
        {
            var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            try
            {
                await socket.ConnectAsync(
                    new IPEndPoint(address, context.DnsEndPoint.Port),
                    cancellationToken);
                return new NetworkStream(socket, ownsSocket: true);
            }
            catch (SocketException exception)
            {
                socket.Dispose();
                lastException = exception;
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        throw new HttpRequestException("No fue posible conectar con el destino público.", lastException);
    }
}
