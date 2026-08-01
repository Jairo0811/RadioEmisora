using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.ViewModels;

namespace RadioEmisoraRD.Services;

internal static class PortfolioCaptureService
{
    public static async Task CaptureAsync(string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        Directory.CreateDirectory(outputDirectory);

        using var catalogService = new RadioCatalogService();
        StationCatalog catalog = catalogService.LoadLocal().Catalog;
        var playerService = new CaptureMediaPlayerService();
        var configService = new CaptureConfigService();
        var favoriteService = new CaptureFavoriteService();
        var staticCatalogService = new CaptureCatalogService(catalog);
        using var viewModel = new MainViewModel(
            playerService,
            configService,
            favoriteService,
            staticCatalogService,
            AppLogger.Current);
        var window = new MainWindow(viewModel)
        {
            Width = 1200,
            Height = 720,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = 20,
            Top = 20,
            ShowInTaskbar = false
        };

        window.Show();
        await WaitForRenderAsync();

        var animationFrames = new List<BitmapSource>();
        BitmapSource dashboard = Render(window);
        SavePng(dashboard, Path.Combine(outputDirectory, "dashboard.png"));
        animationFrames.Add(dashboard);

        SavePng(
            Render(window.GetHistoryCaptureTarget()),
            Path.Combine(outputDirectory, "historial.png"));

        viewModel.EntrarReproductorCommand.Execute(null);
        viewModel.EmisoraSeleccionada = viewModel.TodasLasEmisoras.First();
        viewModel.ReproducirCommand.Execute(null);
        await WaitForRenderAsync(250);
        BitmapSource player = Render(window);
        SavePng(player, Path.Combine(outputDirectory, "reproductor.png"));
        animationFrames.Add(player);

        viewModel.FiltroActual = "Favoritas";
        await WaitForRenderAsync();
        BitmapSource favorites = Render(window);
        SavePng(favorites, Path.Combine(outputDirectory, "favoritos.png"));
        animationFrames.Add(favorites);

        viewModel.FiltroActual = "Todas";
        viewModel.TextoBusqueda = "noticias";
        await WaitForRenderAsync();
        BitmapSource search = Render(window);
        SavePng(search, Path.Combine(outputDirectory, "busqueda.png"));
        animationFrames.Add(search);

        var aboutWindow = new AboutWindow
        {
            Owner = window
        };
        aboutWindow.Show();
        await WaitForRenderAsync();
        SavePng(Render(aboutWindow), Path.Combine(outputDirectory, "acerca-de.png"));
        aboutWindow.Close();

        SaveAnimatedGif(animationFrames, Path.Combine(outputDirectory, "demo.gif"));
        window.Close();
    }

    private static async Task WaitForRenderAsync(int delayMilliseconds = 80)
    {
        if (delayMilliseconds > 0)
            await Task.Delay(delayMilliseconds);

        await Application.Current.Dispatcher.InvokeAsync(
            static () => { },
            DispatcherPriority.ApplicationIdle);
        await Application.Current.Dispatcher.InvokeAsync(
            static () => { },
            DispatcherPriority.Render);
    }

    private static RenderTargetBitmap Render(FrameworkElement element)
    {
        element.UpdateLayout();
        int width = Math.Max(1, (int)Math.Ceiling(element.ActualWidth));
        int height = Math.Max(1, (int)Math.Ceiling(element.ActualHeight));
        var bitmap = new RenderTargetBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Pbgra32);
        bitmap.Render(element);
        bitmap.Freeze();
        return bitmap;
    }

    private static void SavePng(BitmapSource bitmap, string path)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        using FileStream stream = File.Create(path);
        encoder.Save(stream);
    }

    private static void SaveAnimatedGif(IEnumerable<BitmapSource> frames, string path)
    {
        var encoder = new GifBitmapEncoder();

        foreach (BitmapSource source in frames)
        {
            var metadata = new BitmapMetadata("gif");
            metadata.SetQuery("/grctlext/Delay", (ushort)120);
            metadata.SetQuery("/grctlext/Disposal", (byte)2);
            encoder.Frames.Add(BitmapFrame.Create(source, null, metadata, null));
        }

        using FileStream stream = File.Create(path);
        encoder.Save(stream);
    }

    private sealed class CaptureMediaPlayerService : IMediaPlayerService
    {
        public event EventHandler<PlayerStateChangedEventArgs>? StateChanged;

        public event EventHandler<PlaybackErrorEventArgs>? PlaybackError
        {
            add { }
            remove { }
        }

        public PlayerState State { get; private set; } = PlayerState.Detenido;

        public double Volume { get; set; } = 0.80;

        public async Task PlayAsync(
            string url,
            PlaybackOptions options,
            CancellationToken cancellationToken = default)
        {
            TransitionTo(PlayerState.Conectando);
            await Task.Delay(30, cancellationToken);
            TransitionTo(PlayerState.Buffering);
            await Task.Delay(30, cancellationToken);
            TransitionTo(PlayerState.Reproduciendo);
        }

        public void Pause() => TransitionTo(PlayerState.Pausado);

        public void ResumePlayback() => TransitionTo(PlayerState.Reproduciendo);

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            TransitionTo(PlayerState.Detenido);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void TransitionTo(PlayerState nextState)
        {
            PlayerState previousState = State;
            State = nextState;
            StateChanged?.Invoke(
                this,
                new PlayerStateChangedEventArgs(previousState, nextState, null));
        }
    }

    private sealed class CaptureConfigService : IConfigService
    {
        private readonly AppConfig config = new()
        {
            Historial = ["Z 101", "Mortal", "Radio Disney", "Disco 106"],
            UltimaEmisora = "z101-1013",
            Volumen = 0.72
        };

        public AppConfig Load() => config;

        public bool TrySave(AppConfig value, out string? errorMessage)
        {
            errorMessage = null;
            return true;
        }
    }

    private sealed class CaptureFavoriteService : IFavoriteService
    {
        private readonly List<string> favorites = ["mortal-1049", "z101-1013"];

        public IReadOnlyList<string> Load() => favorites;

        public bool TrySave(IEnumerable<string> value, out string? errorMessage)
        {
            favorites.Clear();
            favorites.AddRange(value);
            errorMessage = null;
            return true;
        }
    }

    private sealed class CaptureCatalogService : IRadioCatalogService
    {
        private readonly StationCatalog catalog;

        public CaptureCatalogService(StationCatalog catalog)
        {
            this.catalog = catalog;
        }

        public CatalogLoadResult LoadLocal() => new(catalog, false, "Captura");

        public Task<CatalogLoadResult> TryUpdateAsync(
            string remoteUrl,
            TimeSpan timeout,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new CatalogLoadResult(catalog, false, "Captura"));
    }
}
