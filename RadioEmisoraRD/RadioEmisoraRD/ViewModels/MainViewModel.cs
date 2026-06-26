using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RadioEmisoraRD.Helpers;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MediaPlayerService playerService = new MediaPlayerService();
        private readonly DispatcherTimer equalizerTimer = new DispatcherTimer();
        private readonly DispatcherTimer toastTimer = new DispatcherTimer();
        private readonly Random random = new Random();

        private string textoBusqueda = "";
        private string filtroActual = "Todas";
        private Emisora? emisoraSeleccionada;
        private string estadoReproductor = "EN ESPERA";
        private string ahoraSuena = "🎵 Ahora suena: ninguna emisora";
        private double volumen = 0.80;
        private bool mostrarDashboard = true;
        private bool reproduciendo = false;
        private bool pausado = false;
        private Visibility toastVisibility = Visibility.Collapsed;
        private string toastTitulo = "";
        private string toastMensaje = "";
        private AppConfig config;

        private double bar1 = 18, bar2 = 28, bar3 = 42, bar4 = 58, bar5 = 42, bar6 = 28, bar7 = 18;

        public ObservableCollection<Emisora> TodasLasEmisoras { get; set; }
        public ObservableCollection<Emisora> EmisorasFiltradas { get; set; }

        public ICommand ReproducirCommand { get; }
        public ICommand DetenerCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand FavoritoCommand { get; }
        public ICommand FiltroCommand { get; }
        public ICommand AcercaDeCommand { get; }
        public ICommand SalirCommand { get; }
        public ICommand EntrarReproductorCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestAbout;
        public event Action? RequestExit;

        public MainViewModel()
        {
            config = ConfigService.Cargar();

            TodasLasEmisoras = new ObservableCollection<Emisora>(RadioService.ObtenerEmisoras());
            EmisorasFiltradas = new ObservableCollection<Emisora>();

            foreach (Emisora emisora in TodasLasEmisoras)
                emisora.Logo = ObtenerRutaLogo(emisora.Logo);

            volumen = config.Volumen;
            playerService.Volume = volumen;

            ReproducirCommand = new RelayCommand(_ => Reproducir());
            DetenerCommand = new RelayCommand(_ => Detener());
            ActualizarCommand = new RelayCommand(_ => Actualizar());
            FavoritoCommand = new RelayCommand(_ => AlternarFavorito());
            FiltroCommand = new RelayCommand(valor => CambiarFiltro(valor?.ToString() ?? "Todas"));
            AcercaDeCommand = new RelayCommand(_ => RequestAbout?.Invoke());
            SalirCommand = new RelayCommand(_ => RequestExit?.Invoke());
            EntrarReproductorCommand = new RelayCommand(_ => EntrarAlReproductor());

            equalizerTimer.Interval = TimeSpan.FromMilliseconds(180);
            equalizerTimer.Tick += (s, e) => AnimarEcualizador();

            toastTimer.Interval = TimeSpan.FromSeconds(3);
            toastTimer.Tick += (s, e) =>
            {
                ToastVisibility = Visibility.Collapsed;
                toastTimer.Stop();
            };

            AplicarFavoritosGuardados();
            FiltrarEmisoras();
            RestaurarUltimaEmisora();
        }

        public Visibility DashboardVisibility => mostrarDashboard ? Visibility.Visible : Visibility.Collapsed;
        public Visibility MainLayoutVisibility => mostrarDashboard ? Visibility.Collapsed : Visibility.Visible;

        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                textoBusqueda = value;
                OnPropertyChanged();
                FiltrarEmisoras();
            }
        }

        public string FiltroActual
        {
            get => filtroActual;
            set
            {
                filtroActual = value;
                OnPropertyChanged();
                FiltrarEmisoras();
            }
        }

        public Emisora? EmisoraSeleccionada
        {
            get => emisoraSeleccionada;
            set
            {
                emisoraSeleccionada = value;

                if (emisoraSeleccionada != null)
                {
                    config.UltimaEmisora = emisoraSeleccionada.Nombre;
                    ConfigService.Guardar(config);
                }

                OnPropertyChanged();
                RefrescarVistaActual();
                RefrescarDashboard();
            }
        }

        public double Volumen
        {
            get => volumen;
            set
            {
                volumen = value;
                playerService.Volume = volumen;
                config.Volumen = volumen;
                ConfigService.Guardar(config);

                OnPropertyChanged();
                OnPropertyChanged(nameof(VolumenTexto));
            }
        }

        public string VolumenTexto => $"{(int)(Volumen * 100)}%";

        public string TextoBotonReproducir
        {
            get
            {
                if (reproduciendo)
                    return "⏸ Pausar";

                if (pausado)
                    return "▶ Continuar";

                return "▶ Reproducir";
            }
        }

        public string NombreActual => EmisoraSeleccionada?.Nombre ?? "Selecciona FM";

        public string CategoriaActual =>
            EmisoraSeleccionada == null
                ? "Radio dominicana"
                : $"{EmisoraSeleccionada.Categoria} • {EmisoraSeleccionada.Frecuencia}";

        public string ProvinciaActual => EmisoraSeleccionada?.Provincia ?? "República Dominicana";

        public string GrupoActual => EmisoraSeleccionada?.Grupo ?? "Grupo no especificado";

        public string StreamEstadoActual =>
            EmisoraSeleccionada == null
                ? "Sin stream seleccionado"
                : string.IsNullOrWhiteSpace(EmisoraSeleccionada.StreamUrl)
                    ? "Stream no disponible"
                    : "Streaming online";

        public string EstadoActual
        {
            get
            {
                if (EmisoraSeleccionada == null)
                    return EstadoReproductor;

                if (reproduciendo)
                    return "▶ REPRODUCIENDO";

                if (pausado)
                    return "⏸ PAUSADO";

                if (EstadoReproductor == "DETENIDO")
                    return "■ DETENIDO";

                return EmisoraSeleccionada.Estado;
            }
        }

        public string EstadoReproductor
        {
            get => estadoReproductor;
            set
            {
                estadoReproductor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstadoActual));
            }
        }

        public string AhoraSuena
        {
            get => ahoraSuena;
            set
            {
                ahoraSuena = value;
                OnPropertyChanged();
            }
        }

        public string TextoFavorito
        {
            get
            {
                if (EmisoraSeleccionada == null)
                    return "☆ Favoritos";

                return EmisoraSeleccionada.EsFavorita ? "★ Favorita" : "☆ Favoritos";
            }
        }

        public SolidColorBrush ColorTemaBrush
        {
            get
            {
                Color color = EmisoraSeleccionada?.ColorTema ?? Color.FromRgb(95, 45, 230);
                return new SolidColorBrush(color);
            }
        }

        public BitmapImage? LogoActual
        {
            get
            {
                if (EmisoraSeleccionada == null || string.IsNullOrWhiteSpace(EmisoraSeleccionada.Logo))
                    return null;

                return new BitmapImage(new Uri(EmisoraSeleccionada.Logo, UriKind.RelativeOrAbsolute));
            }
        }

        public int TotalEmisoras => TodasLasEmisoras.Count;
        public int TotalFavoritas => TodasLasEmisoras.Count(e => e.EsFavorita);
        public int TotalFiltradas => EmisorasFiltradas.Count;
        public int TotalFM => TodasLasEmisoras.Count(e => e.Frecuencia.Contains("FM"));
        public int TotalAM => TodasLasEmisoras.Count(e => e.Frecuencia.Contains("AM"));
        public int TotalOnline => TodasLasEmisoras.Count(e => e.Frecuencia.Contains("Online"));
        public string FmAmTexto => $"{TotalFM} / {TotalAM}";

        public string UltimaEmisoraTexto =>
            string.IsNullOrWhiteSpace(config.UltimaEmisora)
                ? "Ninguna todavía"
                : config.UltimaEmisora;

        public ObservableCollection<string> HistorialDashboard
        {
            get
            {
                if (config.Historial == null || config.Historial.Count == 0)
                    return new ObservableCollection<string> { "Sin historial por ahora." };

                return new ObservableCollection<string>(
                    config.Historial.Take(5).Select(nombre =>
                    {
                        Emisora? emisora = TodasLasEmisoras.FirstOrDefault(e => e.Nombre == nombre);

                        if (emisora == null)
                            return "📻 " + nombre;

                        return $"📻 {emisora.Nombre}  •  {emisora.Frecuencia}  •  {emisora.Grupo}";
                    })
                );
            }
        }

        public Visibility ToastVisibility
        {
            get => toastVisibility;
            set
            {
                toastVisibility = value;
                OnPropertyChanged();
            }
        }

        public string ToastTitulo
        {
            get => toastTitulo;
            set
            {
                toastTitulo = value;
                OnPropertyChanged();
            }
        }

        public string ToastMensaje
        {
            get => toastMensaje;
            set
            {
                toastMensaje = value;
                OnPropertyChanged();
            }
        }

        public double Bar1 { get => bar1; set { bar1 = value; OnPropertyChanged(); } }
        public double Bar2 { get => bar2; set { bar2 = value; OnPropertyChanged(); } }
        public double Bar3 { get => bar3; set { bar3 = value; OnPropertyChanged(); } }
        public double Bar4 { get => bar4; set { bar4 = value; OnPropertyChanged(); } }
        public double Bar5 { get => bar5; set { bar5 = value; OnPropertyChanged(); } }
        public double Bar6 { get => bar6; set { bar6 = value; OnPropertyChanged(); } }
        public double Bar7 { get => bar7; set { bar7 = value; OnPropertyChanged(); } }

        private void EntrarAlReproductor()
        {
            mostrarDashboard = false;
            OnPropertyChanged(nameof(DashboardVisibility));
            OnPropertyChanged(nameof(MainLayoutVisibility));
        }

        public void Reproducir()
        {
            if (reproduciendo)
            {
                playerService.Pause();

                reproduciendo = false;
                pausado = true;
                EstadoReproductor = "PAUSADO";

                equalizerTimer.Stop();
                ActualizarEstadoTarjetas();

                OnPropertyChanged(nameof(TextoBotonReproducir));
                OnPropertyChanged(nameof(EstadoActual));

                MostrarToast("⏸ Pausado", EmisoraSeleccionada?.Nombre ?? "Reproducción pausada");
                return;
            }

            if (pausado)
            {
                playerService.Resume();

                reproduciendo = true;
                pausado = false;
                EstadoReproductor = "REPRODUCIENDO";

                equalizerTimer.Start();
                ActualizarEstadoTarjetas();

                OnPropertyChanged(nameof(TextoBotonReproducir));
                OnPropertyChanged(nameof(EstadoActual));

                MostrarToast("▶ Continuando", EmisoraSeleccionada?.Nombre ?? "Reproducción reanudada");
                return;
            }

            if (EmisoraSeleccionada == null)
            {
                EstadoReproductor = "SELECCIONA UNA EMISORA";
                MostrarToast("Aviso", "Selecciona una emisora primero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmisoraSeleccionada.StreamUrl))
            {
                EstadoReproductor = "STREAM NO DISPONIBLE";
                MostrarToast("Stream no disponible", EmisoraSeleccionada.Nombre);
                return;
            }

            playerService.Play(EmisoraSeleccionada.StreamUrl);

            reproduciendo = true;
            pausado = false;
            EstadoReproductor = "REPRODUCIENDO";
            AhoraSuena = "🎵 Ahora suena: " + EmisoraSeleccionada.Nombre;

            AgregarAlHistorial(EmisoraSeleccionada.Nombre);

            equalizerTimer.Start();
            ActualizarEstadoTarjetas();

            OnPropertyChanged(nameof(TextoBotonReproducir));
            OnPropertyChanged(nameof(EstadoActual));

            MostrarToast("▶ Reproduciendo", EmisoraSeleccionada.Nombre);
        }

        public void Detener()
        {
            playerService.Stop();

            reproduciendo = false;
            pausado = false;
            EstadoReproductor = "DETENIDO";
            AhoraSuena = "🎵 Ahora suena: ninguna emisora";

            equalizerTimer.Stop();
            ResetearEcualizador();
            ActualizarEstadoTarjetas();

            OnPropertyChanged(nameof(TextoBotonReproducir));
            OnPropertyChanged(nameof(EstadoActual));

            MostrarToast("⏹ Detenido", "Reproducción detenida.");
        }

        public void Actualizar()
        {
            TextoBusqueda = "";
            FiltroActual = "Todas";
            FiltrarEmisoras();

            EmisoraSeleccionada = null;
            Detener();

            EstadoReproductor = "LISTA ACTUALIZADA";
            MostrarToast("↻ Lista actualizada", "Catálogo restaurado.");
        }

        public void AlternarFavorito()
        {
            if (EmisoraSeleccionada == null)
            {
                AhoraSuena = "☆ Selecciona una emisora para agregarla a favoritos";
                MostrarToast("Favoritos", "Selecciona una emisora primero.");
                return;
            }

            EmisoraSeleccionada.EsFavorita = !EmisoraSeleccionada.EsFavorita;
            GuardarFavoritos();

            OnPropertyChanged(nameof(TextoFavorito));
            OnPropertyChanged(nameof(TotalFavoritas));

            AhoraSuena = EmisoraSeleccionada.EsFavorita
                ? "★ Agregada a favoritos: " + EmisoraSeleccionada.Nombre
                : "☆ Eliminada de favoritos: " + EmisoraSeleccionada.Nombre;

            FiltrarEmisoras();
            RefrescarDashboard();

            MostrarToast(
                EmisoraSeleccionada.EsFavorita ? "★ Favorita agregada" : "☆ Favorita eliminada",
                EmisoraSeleccionada.Nombre
            );
        }

        private void CambiarFiltro(string filtro)
        {
            FiltroActual = filtro;
        }

        private void AplicarFavoritosGuardados()
        {
            var favoritos = FavoriteService.Cargar();

            foreach (Emisora emisora in TodasLasEmisoras)
                emisora.EsFavorita = favoritos.Contains(emisora.Nombre);
        }

        private void GuardarFavoritos()
        {
            var favoritos = TodasLasEmisoras
                .Where(e => e.EsFavorita)
                .Select(e => e.Nombre)
                .ToList();

            FavoriteService.Guardar(favoritos);
        }

        private void AgregarAlHistorial(string nombreEmisora)
        {
            config.Historial.Remove(nombreEmisora);
            config.Historial.Insert(0, nombreEmisora);

            if (config.Historial.Count > 10)
                config.Historial = config.Historial.Take(10).ToList();

            config.UltimaEmisora = nombreEmisora;

            ConfigService.Guardar(config);
            RefrescarDashboard();
        }

        private void RestaurarUltimaEmisora()
        {
            if (string.IsNullOrWhiteSpace(config.UltimaEmisora))
                return;

            Emisora? ultima = TodasLasEmisoras.FirstOrDefault(e => e.Nombre == config.UltimaEmisora);

            if (ultima == null)
                return;

            EmisoraSeleccionada = ultima;
            AhoraSuena = "🎵 Última emisora: " + ultima.Nombre;
        }

        private void FiltrarEmisoras()
        {
            string filtroTexto = textoBusqueda.Trim().ToLower();

            EmisorasFiltradas.Clear();

            var resultado = TodasLasEmisoras.Where(e =>
                string.IsNullOrWhiteSpace(filtroTexto) ||
                e.Nombre.ToLower().Contains(filtroTexto) ||
                e.Categoria.ToLower().Contains(filtroTexto) ||
                e.Frecuencia.ToLower().Contains(filtroTexto) ||
                e.Provincia.ToLower().Contains(filtroTexto) ||
                e.Grupo.ToLower().Contains(filtroTexto));

            if (FiltroActual == "Favoritas")
                resultado = resultado.Where(e => e.EsFavorita);

            if (FiltroActual == "FM")
                resultado = resultado.Where(e => e.Frecuencia.Contains("FM"));

            if (FiltroActual == "AM")
                resultado = resultado.Where(e => e.Frecuencia.Contains("AM"));

            if (FiltroActual == "Online")
                resultado = resultado.Where(e => e.Frecuencia.Contains("Online"));

            foreach (Emisora emisora in resultado)
                EmisorasFiltradas.Add(emisora);

            OnPropertyChanged(nameof(TotalFiltradas));
        }

        private string ObtenerRutaLogo(string archivo)
        {
            return "/Assets/logos/" + archivo;
        }

        private void ActualizarEstadoTarjetas()
        {
            foreach (Emisora emisora in TodasLasEmisoras)
            {
                emisora.EstaReproduciendo = false;
                emisora.EstaPausada = false;
            }

            if (EmisoraSeleccionada != null)
            {
                EmisoraSeleccionada.EstaReproduciendo = reproduciendo;
                EmisoraSeleccionada.EstaPausada = pausado;
            }
        }

        private void MostrarToast(string titulo, string mensaje)
        {
            ToastTitulo = titulo;
            ToastMensaje = mensaje;
            ToastVisibility = Visibility.Visible;

            toastTimer.Stop();
            toastTimer.Start();
        }

        private void AnimarEcualizador()
        {
            Bar1 = random.Next(14, 42);
            Bar2 = random.Next(18, 58);
            Bar3 = random.Next(22, 70);
            Bar4 = random.Next(28, 78);
            Bar5 = random.Next(22, 70);
            Bar6 = random.Next(18, 58);
            Bar7 = random.Next(14, 42);
        }

        private void ResetearEcualizador()
        {
            Bar1 = 18;
            Bar2 = 28;
            Bar3 = 42;
            Bar4 = 58;
            Bar5 = 42;
            Bar6 = 28;
            Bar7 = 18;
        }

        private void RefrescarVistaActual()
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
        }

        private void RefrescarDashboard()
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

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}