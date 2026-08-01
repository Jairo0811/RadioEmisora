using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RadioEmisoraRD.Helpers;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IMediaPlayerService playerService;
    private readonly IConfigService configService;
    private readonly IFavoriteService favoriteService;
    private readonly IRadioCatalogService catalogService;
    private readonly IAppLogger logger;
    private readonly Dispatcher dispatcher;
    private readonly DispatcherTimer equalizerTimer = new();
    private readonly DispatcherTimer toastTimer = new();
    private readonly DispatcherTimer configSaveTimer = new();
    private readonly Random random = new();
    private readonly Dictionary<string, BitmapImage> logoCache = new(StringComparer.Ordinal);
    private readonly Dictionary<Color, SolidColorBrush> brushCache = [];
    private readonly bool ownsServices;
    private readonly AsyncRelayCommand playCommand;
    private readonly AsyncRelayCommand stopCommand;
    private readonly AsyncRelayCommand updateCommand;
    private readonly RelayCommand favoriteCommand;
    private string textoBusqueda = string.Empty;
    private string filtroActual = "Todas";
    private Emisora? emisoraSeleccionada;
    private Emisora? emisoraEnReproduccion;
    private PlayerState playerState = PlayerState.Detenido;
    private string? playerStateDetail;
    private string ahoraSuena = "🎵 Ahora suena: ninguna emisora";
    private double volumen = 0.80;
    private bool mostrarDashboard = true;
    private bool historyRecordedForSession;
    private Visibility toastVisibility = Visibility.Collapsed;
    private string toastTitulo = string.Empty;
    private string toastMensaje = string.Empty;
    private AppConfig config;
    private bool disposed;
    private double bar1 = 18;
    private double bar2 = 28;
    private double bar3 = 42;
    private double bar4 = 58;
    private double bar5 = 42;
    private double bar6 = 28;
    private double bar7 = 18;

    public MainViewModel()
        : this(
            new MediaPlayerService(),
            new ConfigService(),
            new FavoriteService(),
            new RadioCatalogService(),
            AppLogger.Current,
            true)
    {
    }

    public MainViewModel(
        IMediaPlayerService playerService,
        IConfigService configService,
        IFavoriteService favoriteService,
        IRadioCatalogService catalogService,
        IAppLogger logger)
        : this(playerService, configService, favoriteService, catalogService, logger, false)
    {
    }

    private MainViewModel(
        IMediaPlayerService playerService,
        IConfigService configService,
        IFavoriteService favoriteService,
        IRadioCatalogService catalogService,
        IAppLogger logger,
        bool ownsServices)
    {
        this.playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
        this.favoriteService = favoriteService ?? throw new ArgumentNullException(nameof(favoriteService));
        this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ownsServices = ownsServices;
        dispatcher = Dispatcher.CurrentDispatcher;

        config = configService.Load();
        CatalogLoadResult localCatalog = catalogService.LoadLocal();
        TodasLasEmisoras = new ObservableCollection<Emisora>(
            RadioService.CreateStations(localCatalog.Catalog));
        EmisorasFiltradas = [];

        volumen = config.Volumen;
        playerService.Volume = volumen;
        playerState = playerService.State;

        playCommand = new AsyncRelayCommand(
            (_, cancellationToken) => PlayAsync(cancellationToken),
            onError: HandleCommandError,
            cancelPrevious: true);
        stopCommand = new AsyncRelayCommand(
            (_, cancellationToken) => StopAsync(cancellationToken),
            onError: HandleCommandError);
        updateCommand = new AsyncRelayCommand(
            (_, cancellationToken) => UpdateCatalogAsync(true, cancellationToken),
            onError: HandleCommandError);
        favoriteCommand = new RelayCommand(_ => ToggleFavorite());

        ReproducirCommand = playCommand;
        DetenerCommand = stopCommand;
        ActualizarCommand = updateCommand;
        FavoritoCommand = favoriteCommand;
        FiltroCommand = new RelayCommand(value => FiltroActual = value?.ToString() ?? "Todas");
        AcercaDeCommand = new RelayCommand(_ => RequestAbout?.Invoke());
        SalirCommand = new RelayCommand(_ => RequestExit?.Invoke());
        EntrarReproductorCommand = new RelayCommand(_ => EnterPlayer());

        equalizerTimer.Interval = TimeSpan.FromMilliseconds(180);
        equalizerTimer.Tick += OnEqualizerTimerTick;
        toastTimer.Interval = TimeSpan.FromSeconds(4);
        toastTimer.Tick += OnToastTimerTick;
        configSaveTimer.Interval = TimeSpan.FromMilliseconds(450);
        configSaveTimer.Tick += OnConfigSaveTimerTick;

        playerService.StateChanged += OnPlayerStateChanged;
        playerService.PlaybackError += OnPlaybackError;

        ApplyStoredFavorites();
        FilterStations();
        RestoreLastStation();

        _ = InitializeRemoteCatalogAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action? RequestAbout;

    public event Action? RequestExit;

    public ObservableCollection<Emisora> TodasLasEmisoras { get; }

    public ObservableCollection<Emisora> EmisorasFiltradas { get; }

    public ICommand ReproducirCommand { get; }

    public ICommand DetenerCommand { get; }

    public ICommand ActualizarCommand { get; }

    public ICommand FavoritoCommand { get; }

    public ICommand FiltroCommand { get; }

    public ICommand AcercaDeCommand { get; }

    public ICommand SalirCommand { get; }

    public ICommand EntrarReproductorCommand { get; }

    public Visibility DashboardVisibility => mostrarDashboard ? Visibility.Visible : Visibility.Collapsed;

    public Visibility MainLayoutVisibility => mostrarDashboard ? Visibility.Collapsed : Visibility.Visible;

    public string TextoBusqueda
    {
        get => textoBusqueda;
        set
        {
            if (!SetField(ref textoBusqueda, value ?? string.Empty))
                return;

            FilterStations();
        }
    }

    public string FiltroActual
    {
        get => filtroActual;
        set
        {
            if (!SetField(ref filtroActual, value ?? "Todas"))
                return;

            FilterStations();
        }
    }

    public Emisora? EmisoraSeleccionada
    {
        get => emisoraSeleccionada;
        set
        {
            if (ReferenceEquals(emisoraSeleccionada, value))
                return;

            emisoraSeleccionada = value;

            if (value is not null)
            {
                config.UltimaEmisora = value.Id;
                ScheduleConfigSave();
            }

            OnPropertyChanged();
            RefreshCurrentView();
            RefreshDashboard();
        }
    }

    public double Volumen
    {
        get => volumen;
        set
        {
            double normalized = double.IsFinite(value) ? Math.Clamp(value, 0, 1) : 0.80;
            if (!SetField(ref volumen, normalized))
                return;

            playerService.Volume = normalized;
            config.Volumen = normalized;
            ScheduleConfigSave();
            OnPropertyChanged(nameof(VolumenTexto));
        }
    }

    public string VolumenTexto => $"{(int)Math.Round(Volumen * 100)}%";

    public string TextoBotonReproducir
    {
        get
        {
            bool sameStation = ReferenceEquals(EmisoraSeleccionada, emisoraEnReproduccion);

            if (!sameStation)
                return "▶ Reproducir";

            return playerState switch
            {
                PlayerState.Reproduciendo or PlayerState.Buffering => "⏸ Pausar",
                PlayerState.Pausado => "▶ Continuar",
                PlayerState.Conectando or PlayerState.Reconectando => "↻ Reconectar",
                PlayerState.Error => "↻ Reintentar",
                _ => "▶ Reproducir"
            };
        }
    }

    public string NombreActual => EmisoraSeleccionada?.Nombre ?? "Selecciona FM";

    public string CategoriaActual => EmisoraSeleccionada is null
        ? "Radio dominicana"
        : $"{EmisoraSeleccionada.Categoria} • {EmisoraSeleccionada.Frecuencia}";

    public string ProvinciaActual => EmisoraSeleccionada?.Provincia ?? "República Dominicana";

    public string GrupoActual => EmisoraSeleccionada?.Grupo ?? "Grupo no especificado";

    public string StreamEstadoActual
    {
        get
        {
            if (EmisoraSeleccionada is null)
                return "Sin stream seleccionado";

            if (string.IsNullOrWhiteSpace(EmisoraSeleccionada.StreamUrl))
                return "Stream no disponible";

            if (ReferenceEquals(EmisoraSeleccionada, emisoraEnReproduccion))
                return playerStateDetail ?? playerState.ToDisplayText();

            return "Streaming online";
        }
    }

    public string EstadoActual
    {
        get
        {
            if (EmisoraSeleccionada is null)
                return EstadoReproductor;

            if (!ReferenceEquals(EmisoraSeleccionada, emisoraEnReproduccion))
                return EmisoraSeleccionada.Estado;

            return playerState switch
            {
                PlayerState.Reproduciendo => "▶ REPRODUCIENDO",
                PlayerState.Pausado => "⏸ PAUSADO",
                PlayerState.Detenido => "■ DETENIDO",
                PlayerState.Error => "⚠ ERROR",
                _ => playerState.ToDisplayText()
            };
        }
    }

    public string EstadoReproductor => playerStateDetail is null
        ? playerState.ToDisplayText()
        : $"{playerState.ToDisplayText()} • {playerStateDetail}";

    public string AhoraSuena
    {
        get => ahoraSuena;
        private set => SetField(ref ahoraSuena, value);
    }

    public string TextoFavorito => EmisoraSeleccionada switch
    {
        null => "☆ Favoritos",
        { EsFavorita: true } => "★ Favorita",
        _ => "☆ Favoritos"
    };

    public SolidColorBrush ColorTemaBrush
    {
        get
        {
            Color color = EmisoraSeleccionada?.ColorTema ?? Color.FromRgb(95, 45, 230);
            if (brushCache.TryGetValue(color, out SolidColorBrush? brush))
                return brush;

            brush = new SolidColorBrush(color);
            brush.Freeze();
            brushCache[color] = brush;
            return brush;
        }
    }

    public BitmapImage? LogoActual => EmisoraSeleccionada is null
        ? null
        : GetLogo(EmisoraSeleccionada.Logo);

    public int TotalEmisoras => TodasLasEmisoras.Count;

    public int TotalFavoritas => TodasLasEmisoras.Count(static station => station.EsFavorita);

    public int TotalFiltradas => EmisorasFiltradas.Count;

    public int TotalFM => TodasLasEmisoras.Count(static station =>
        station.Frecuencia.Contains("FM", StringComparison.OrdinalIgnoreCase));

    public int TotalAM => TodasLasEmisoras.Count(static station =>
        station.Frecuencia.Contains("AM", StringComparison.OrdinalIgnoreCase));

    public int TotalOnline => TodasLasEmisoras.Count(static station =>
        station.Frecuencia.Contains("Online", StringComparison.OrdinalIgnoreCase));

    public string FmAmTexto => $"{TotalFM} / {TotalAM}";

    public string UltimaEmisoraTexto
    {
        get
        {
            if (string.IsNullOrWhiteSpace(config.UltimaEmisora))
                return "Ninguna todavía";

            Emisora? station = FindStation(config.UltimaEmisora);
            return station?.Nombre ?? config.UltimaEmisora;
        }
    }

    public ObservableCollection<string> HistorialDashboard
    {
        get
        {
            if (config.Historial is null || config.Historial.Count == 0)
                return ["Sin historial por ahora."];

            return new ObservableCollection<string>(
                config.Historial.Take(5).Select(name =>
                {
                    Emisora? station = FindStation(name);
                    return station is null
                        ? "📻 " + name
                        : $"📻 {station.Nombre}  •  {station.Frecuencia}  •  {station.Grupo}";
                }));
        }
    }

    public Visibility ToastVisibility
    {
        get => toastVisibility;
        private set => SetField(ref toastVisibility, value);
    }

    public string ToastTitulo
    {
        get => toastTitulo;
        private set => SetField(ref toastTitulo, value);
    }

    public string ToastMensaje
    {
        get => toastMensaje;
        private set => SetField(ref toastMensaje, value);
    }

    public double Bar1 { get => bar1; private set => SetField(ref bar1, value); }

    public double Bar2 { get => bar2; private set => SetField(ref bar2, value); }

    public double Bar3 { get => bar3; private set => SetField(ref bar3, value); }

    public double Bar4 { get => bar4; private set => SetField(ref bar4, value); }

    public double Bar5 { get => bar5; private set => SetField(ref bar5, value); }

    public double Bar6 { get => bar6; private set => SetField(ref bar6, value); }

    public double Bar7 { get => bar7; private set => SetField(ref bar7, value); }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        equalizerTimer.Stop();
        toastTimer.Stop();
        configSaveTimer.Stop();
        equalizerTimer.Tick -= OnEqualizerTimerTick;
        toastTimer.Tick -= OnToastTimerTick;
        configSaveTimer.Tick -= OnConfigSaveTimerTick;
        playerService.StateChanged -= OnPlayerStateChanged;
        playerService.PlaybackError -= OnPlaybackError;
        playCommand.Dispose();
        stopCommand.Dispose();
        updateCommand.Dispose();
        SaveConfig(false);

        if (ownsServices)
        {
            playerService.Dispose();

            if (catalogService is IDisposable disposableCatalogService)
                disposableCatalogService.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    private void EnterPlayer()
    {
        mostrarDashboard = false;
        OnPropertyChanged(nameof(DashboardVisibility));
        OnPropertyChanged(nameof(MainLayoutVisibility));
    }

    private async Task PlayAsync(CancellationToken cancellationToken)
    {
        Emisora? selectedStation = EmisoraSeleccionada;
        if (selectedStation is null)
        {
            ShowToast("Aviso", "Selecciona una emisora primero.");
            return;
        }

        bool sameStation = ReferenceEquals(selectedStation, emisoraEnReproduccion);

        if (sameStation && playerState is PlayerState.Reproduciendo or PlayerState.Buffering)
        {
            playerService.Pause();
            ShowToast("⏸ Pausado", selectedStation.Nombre);
            return;
        }

        if (sameStation && playerState == PlayerState.Pausado)
        {
            playerService.Resume();
            ShowToast("▶ Continuando", selectedStation.Nombre);
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedStation.StreamUrl))
        {
            ShowToast("Stream no disponible", selectedStation.Nombre);
            return;
        }

        emisoraEnReproduccion = selectedStation;
        historyRecordedForSession = false;
        AhoraSuena = "🎵 Conectando con: " + selectedStation.Nombre;
        RefreshPlayerView();

        await playerService.PlayAsync(
            selectedStation.StreamUrl,
            PlaybackOptions.FromConfig(config),
            cancellationToken);
    }

    private async Task StopAsync(CancellationToken cancellationToken)
    {
        await playerService.StopAsync(cancellationToken);
        emisoraEnReproduccion = null;
        historyRecordedForSession = false;
        AhoraSuena = "🎵 Ahora suena: ninguna emisora";
        ResetEqualizer();
        RefreshPlayerView();
        ShowToast("⏹ Detenido", "Reproducción detenida.");
    }

    private async Task UpdateCatalogAsync(bool userInitiated, CancellationToken cancellationToken)
    {
        if (!userInitiated)
            await Task.Yield();

        CatalogLoadResult result = await catalogService.TryUpdateAsync(
            config.CatalogUrl,
            TimeSpan.FromSeconds(config.StreamTimeoutSeconds),
            cancellationToken);

        if (disposed)
            return;

        if (result.UpdatedFromRemote)
        {
            ReplaceCatalog(result.Catalog);
            config.CatalogVersion = result.Catalog.Version;
            config.LastCatalogUpdateUtc = DateTimeOffset.UtcNow;
            SaveConfig(userInitiated);

            if (userInitiated)
                ShowToast("↻ Catálogo actualizado", $"Versión {result.Catalog.Version} instalada.");
        }
        else if (userInitiated)
        {
            ShowToast(
                result.Warning is null ? "Catálogo al día" : "Actualización no disponible",
                result.Warning ?? $"Ya utilizas la versión {result.Catalog.Version}.");
        }

        if (userInitiated)
        {
            TextoBusqueda = string.Empty;
            FiltroActual = "Todas";
        }
    }

    private void ToggleFavorite()
    {
        if (EmisoraSeleccionada is null)
        {
            AhoraSuena = "☆ Selecciona una emisora para agregarla a favoritos";
            ShowToast("Favoritos", "Selecciona una emisora primero.");
            return;
        }

        EmisoraSeleccionada.EsFavorita = !EmisoraSeleccionada.EsFavorita;
        IReadOnlyList<string> favorites = TodasLasEmisoras
            .Where(static station => station.EsFavorita)
            .Select(static station => station.Id)
            .ToList();

        if (!favoriteService.TrySave(favorites, out string? errorMessage))
        {
            EmisoraSeleccionada.EsFavorita = !EmisoraSeleccionada.EsFavorita;
            ShowToast("No se guardó el favorito", errorMessage ?? "Inténtalo nuevamente.");
            return;
        }

        OnPropertyChanged(nameof(TextoFavorito));
        OnPropertyChanged(nameof(TotalFavoritas));

        AhoraSuena = EmisoraSeleccionada.EsFavorita
            ? "★ Agregada a favoritos: " + EmisoraSeleccionada.Nombre
            : "☆ Eliminada de favoritos: " + EmisoraSeleccionada.Nombre;

        FilterStations();
        RefreshDashboard();
        ShowToast(
            EmisoraSeleccionada.EsFavorita ? "★ Favorita agregada" : "☆ Favorita eliminada",
            EmisoraSeleccionada.Nombre);
    }

    private void ApplyStoredFavorites()
    {
        var favorites = new HashSet<string>(favoriteService.Load(), StringComparer.OrdinalIgnoreCase);

        foreach (Emisora station in TodasLasEmisoras)
        {
            station.EsFavorita = favorites.Contains(station.Id) || favorites.Contains(station.Nombre);
        }
    }

    private void ReplaceCatalog(StationCatalog catalog)
    {
        string? selectedId = EmisoraSeleccionada?.Id;
        string? playingId = emisoraEnReproduccion?.Id;
        var favorites = new HashSet<string>(
            TodasLasEmisoras.Where(static station => station.EsFavorita).Select(static station => station.Id),
            StringComparer.OrdinalIgnoreCase);
        IReadOnlyList<Emisora> replacement = RadioService.CreateStations(catalog);

        TodasLasEmisoras.Clear();
        foreach (Emisora station in replacement)
        {
            station.EsFavorita = favorites.Contains(station.Id);
            TodasLasEmisoras.Add(station);
        }

        emisoraEnReproduccion = playingId is null
            ? null
            : TodasLasEmisoras.FirstOrDefault(station =>
                string.Equals(station.Id, playingId, StringComparison.OrdinalIgnoreCase));
        EmisoraSeleccionada = selectedId is null
            ? null
            : TodasLasEmisoras.FirstOrDefault(station =>
                string.Equals(station.Id, selectedId, StringComparison.OrdinalIgnoreCase));

        FilterStations();
        UpdateStationStates();
        RefreshDashboard();
    }

    private void RestoreLastStation()
    {
        if (string.IsNullOrWhiteSpace(config.UltimaEmisora))
            return;

        Emisora? lastStation = FindStation(config.UltimaEmisora);
        if (lastStation is null)
            return;

        emisoraSeleccionada = lastStation;
        AhoraSuena = "🎵 Última emisora: " + lastStation.Nombre;
        OnPropertyChanged(nameof(EmisoraSeleccionada));
        RefreshCurrentView();
    }

    private void RegisterHistory()
    {
        if (historyRecordedForSession || emisoraEnReproduccion is null)
            return;

        HistoryService.Register(config, emisoraEnReproduccion.Nombre);
        config.UltimaEmisora = emisoraEnReproduccion.Id;
        historyRecordedForSession = true;
        SaveConfig(false);
        RefreshDashboard();
    }

    private void FilterStations()
    {
        IReadOnlyList<Emisora> result = RadioService
            .Filter(TodasLasEmisoras, textoBusqueda, filtroActual)
            .ToList();

        EmisorasFiltradas.Clear();
        foreach (Emisora station in result)
            EmisorasFiltradas.Add(station);

        OnPropertyChanged(nameof(TotalFiltradas));
    }

    private Emisora? FindStation(string idOrName) => TodasLasEmisoras.FirstOrDefault(station =>
        string.Equals(station.Id, idOrName, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(station.Nombre, idOrName, StringComparison.OrdinalIgnoreCase));

    private BitmapImage? GetLogo(string path)
    {
        if (logoCache.TryGetValue(path, out BitmapImage? cached))
            return cached;

        try
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            image.EndInit();
            image.Freeze();
            logoCache[path] = image;
            return image;
        }
        catch (Exception exception) when (
            exception is IOException or NotSupportedException or UriFormatException)
        {
            logger.Warning($"No se pudo cargar el logo '{path}'.", exception);
            return null;
        }
    }

    private void OnPlayerStateChanged(object? sender, PlayerStateChangedEventArgs e) =>
        RunOnUiThread(() =>
        {
            playerState = e.Current;
            playerStateDetail = e.Detail;

            switch (e.Current)
            {
                case PlayerState.Reproduciendo:
                    equalizerTimer.Start();
                    if (emisoraEnReproduccion is not null)
                    {
                        AhoraSuena = "🎵 Ahora suena: " + emisoraEnReproduccion.Nombre;
                        RegisterHistory();
                    }

                    break;
                case PlayerState.Pausado:
                    equalizerTimer.Stop();
                    break;
                case PlayerState.Conectando:
                case PlayerState.Buffering:
                case PlayerState.Reconectando:
                    equalizerTimer.Stop();
                    break;
                case PlayerState.Detenido:
                    equalizerTimer.Stop();
                    ResetEqualizer();
                    break;
                case PlayerState.Error:
                    equalizerTimer.Stop();
                    ResetEqualizer();
                    break;
            }

            RefreshPlayerView();
        });

    private void OnPlaybackError(object? sender, PlaybackErrorEventArgs e) =>
        RunOnUiThread(() =>
        {
            AhoraSuena = "⚠ " + e.FriendlyMessage;
            ShowToast("No se pudo reproducir", e.FriendlyMessage);
        });

    private async Task InitializeRemoteCatalogAsync()
    {
        try
        {
            await UpdateCatalogAsync(false, CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.Warning("La actualización inicial del catálogo no pudo completarse.", exception);
        }
    }

    private void OnEqualizerTimerTick(object? sender, EventArgs e)
    {
        Bar1 = random.Next(14, 42);
        Bar2 = random.Next(18, 58);
        Bar3 = random.Next(22, 70);
        Bar4 = random.Next(28, 78);
        Bar5 = random.Next(22, 70);
        Bar6 = random.Next(18, 58);
        Bar7 = random.Next(14, 42);
    }

    private void OnToastTimerTick(object? sender, EventArgs e)
    {
        ToastVisibility = Visibility.Collapsed;
        toastTimer.Stop();
    }

    private void OnConfigSaveTimerTick(object? sender, EventArgs e)
    {
        configSaveTimer.Stop();
        SaveConfig(true);
    }

    private void ScheduleConfigSave()
    {
        configSaveTimer.Stop();
        configSaveTimer.Start();
    }

    private void SaveConfig(bool showError)
    {
        if (!configService.TrySave(config, out string? errorMessage) && showError && !disposed)
            ShowToast("No se guardó la configuración", errorMessage ?? "Inténtalo nuevamente.");
    }

    private void UpdateStationStates()
    {
        bool isPlaying = playerState is PlayerState.Conectando or PlayerState.Buffering or
            PlayerState.Reproduciendo or PlayerState.Reconectando;
        bool isPaused = playerState == PlayerState.Pausado;

        foreach (Emisora station in TodasLasEmisoras)
        {
            bool isCurrent = ReferenceEquals(station, emisoraEnReproduccion);
            station.EstaReproduciendo = isCurrent && isPlaying;
            station.EstaPausada = isCurrent && isPaused;
        }
    }

    private void ShowToast(string title, string message)
    {
        ToastTitulo = title;
        ToastMensaje = message;
        ToastVisibility = Visibility.Visible;
        toastTimer.Stop();
        toastTimer.Start();
    }

    private void ResetEqualizer()
    {
        Bar1 = 18;
        Bar2 = 28;
        Bar3 = 42;
        Bar4 = 58;
        Bar5 = 42;
        Bar6 = 28;
        Bar7 = 18;
    }

    private void RefreshPlayerView()
    {
        UpdateStationStates();
        OnPropertyChanged(nameof(TextoBotonReproducir));
        OnPropertyChanged(nameof(EstadoReproductor));
        OnPropertyChanged(nameof(EstadoActual));
        OnPropertyChanged(nameof(StreamEstadoActual));
    }

    private void RefreshCurrentView()
    {
        OnPropertyChanged(nameof(NombreActual));
        OnPropertyChanged(nameof(CategoriaActual));
        OnPropertyChanged(nameof(ProvinciaActual));
        OnPropertyChanged(nameof(GrupoActual));
        OnPropertyChanged(nameof(StreamEstadoActual));
        OnPropertyChanged(nameof(EstadoActual));
        OnPropertyChanged(nameof(ColorTemaBrush));
        OnPropertyChanged(nameof(LogoActual));
        OnPropertyChanged(nameof(TextoFavorito));
        OnPropertyChanged(nameof(TextoBotonReproducir));
    }

    private void RefreshDashboard()
    {
        OnPropertyChanged(nameof(TotalEmisoras));
        OnPropertyChanged(nameof(TotalFavoritas));
        OnPropertyChanged(nameof(TotalFM));
        OnPropertyChanged(nameof(TotalAM));
        OnPropertyChanged(nameof(TotalOnline));
        OnPropertyChanged(nameof(FmAmTexto));
        OnPropertyChanged(nameof(UltimaEmisoraTexto));
        OnPropertyChanged(nameof(HistorialDashboard));
    }

    private void HandleCommandError(Exception exception)
    {
        logger.Error("Falló una acción de la interfaz.", exception);
        RunOnUiThread(() => ShowToast(
            "No se pudo completar la acción",
            "La aplicación se recuperó. Consulta el registro si el problema continúa."));
    }

    private void RunOnUiThread(Action action)
    {
        if (dispatcher.CheckAccess())
            action();
        else
            _ = dispatcher.BeginInvoke(action);
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
