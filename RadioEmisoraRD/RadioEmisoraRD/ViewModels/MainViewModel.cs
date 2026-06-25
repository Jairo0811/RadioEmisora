using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RadioEmisoraRD.Helpers;
using RadioEmisoraRD.Models;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MediaPlayerService playerService = new MediaPlayerService();

        private string textoBusqueda = "";
        private string filtroActual = "Todas";
        private Emisora? emisoraSeleccionada;
        private string estadoReproductor = "EN ESPERA";
        private string ahoraSuena = "🎵 Ahora suena: ninguna emisora";
        private double volumen = 0.80;
        private bool mostrarDashboard = true;
        private AppConfig config;

        public string FmAmTexto => $"{TotalFM} / {TotalAM}";
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

            ReproducirCommand = new RelayCommand(_ => Reproducir());
            DetenerCommand = new RelayCommand(_ => Detener());
            ActualizarCommand = new RelayCommand(_ => Actualizar());
            FavoritoCommand = new RelayCommand(_ => AlternarFavorito());
            FiltroCommand = new RelayCommand(valor => CambiarFiltro(valor?.ToString() ?? "Todas"));
            AcercaDeCommand = new RelayCommand(_ => RequestAbout?.Invoke());
            SalirCommand = new RelayCommand(_ => RequestExit?.Invoke());
            EntrarReproductorCommand = new RelayCommand(_ => EntrarAlReproductor());

            playerService.Volume = volumen;

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
                OnPropertyChanged();
                OnPropertyChanged(nameof(VolumenTexto));
            }
        }

        public string VolumenTexto => $"{(int)(Volumen * 100)}%";

        public string NombreActual => EmisoraSeleccionada?.Nombre ?? "Selecciona FM";

        public string CategoriaActual =>
            EmisoraSeleccionada == null
                ? "Radio dominicana"
                : $"{EmisoraSeleccionada.Categoria} • {EmisoraSeleccionada.Frecuencia}";

        public string ProvinciaActual => EmisoraSeleccionada?.Provincia ?? "República Dominicana";

        public string EstadoActual
        {
            get
            {
                if (EmisoraSeleccionada == null)
                    return EstadoReproductor;

                if (EstadoReproductor == "REPRODUCIENDO")
                    return "REPRODUCIENDO";

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

        public string UltimaEmisoraTexto =>
            string.IsNullOrWhiteSpace(config.UltimaEmisora)
                ? "Ninguna todavía"
                : config.UltimaEmisora;

        public string HistorialTexto
        {
            get
            {
                if (config.Historial == null || config.Historial.Count == 0)
                    return "Sin historial por ahora.";

                return string.Join("\n", config.Historial.Take(6).Select(x => "• " + x));
            }
        }

        private void EntrarAlReproductor()
        {
            mostrarDashboard = false;
            OnPropertyChanged(nameof(DashboardVisibility));
            OnPropertyChanged(nameof(MainLayoutVisibility));
        }

        public void Reproducir()
        {
            if (EmisoraSeleccionada == null)
            {
                EstadoReproductor = "SELECCIONA UNA EMISORA";
                return;
            }

            if (string.IsNullOrWhiteSpace(EmisoraSeleccionada.StreamUrl))
            {
                EstadoReproductor = "STREAM NO DISPONIBLE";
                return;
            }

            playerService.Play(EmisoraSeleccionada.StreamUrl);

            EstadoReproductor = "REPRODUCIENDO";
            AhoraSuena = "🎵 Ahora suena: " + EmisoraSeleccionada.Nombre;

            AgregarAlHistorial(EmisoraSeleccionada.Nombre);
        }

        public void Detener()
        {
            playerService.Stop();
            EstadoReproductor = "DETENIDO";
            AhoraSuena = "🎵 Ahora suena: ninguna emisora";
        }

        public void Actualizar()
        {
            TextoBusqueda = "";
            FiltroActual = "Todas";
            FiltrarEmisoras();

            EmisoraSeleccionada = null;
            Detener();

            EstadoReproductor = "LISTA ACTUALIZADA";
        }

        public void AlternarFavorito()
        {
            if (EmisoraSeleccionada == null)
            {
                AhoraSuena = "☆ Selecciona una emisora para agregarla a favoritos";
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
                e.Provincia.ToLower().Contains(filtroTexto));

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

        private void RefrescarVistaActual()
        {
            OnPropertyChanged(nameof(NombreActual));
            OnPropertyChanged(nameof(CategoriaActual));
            OnPropertyChanged(nameof(ProvinciaActual));
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
            OnPropertyChanged(nameof(UltimaEmisoraTexto));
            OnPropertyChanged(nameof(HistorialTexto));
            OnPropertyChanged(nameof(HistorialDashboard));
            OnPropertyChanged(nameof(FmAmTexto));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public int TotalFM => TodasLasEmisoras.Count(e => e.Frecuencia.Contains("FM"));
        public int TotalAM => TodasLasEmisoras.Count(e => e.Frecuencia.Contains("AM"));
        public int TotalOnline => TodasLasEmisoras.Count(e => e.Frecuencia.Contains("Online"));

        public ObservableCollection<string> HistorialDashboard
        {
            get
            {
                if (config.Historial == null || config.Historial.Count == 0)
                    return new ObservableCollection<string> { "Sin historial por ahora." };

                return new ObservableCollection<string>(
                    config.Historial.Take(5).Select(x => "📻 " + x)
                );
            }
        }
    }
}