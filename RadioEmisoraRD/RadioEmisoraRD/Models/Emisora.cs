using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace RadioEmisoraRD.Models
{
    public class Emisora : INotifyPropertyChanged
    {
        private bool estaReproduciendo;
        private bool estaPausada;
        private bool esFavorita;

        public string Nombre { get; set; }
        public string Frecuencia { get; set; }
        public string Categoria { get; set; }
        public string Provincia { get; set; }
        public string Grupo { get; set; }
        public string Logo { get; set; }
        public string StreamUrl { get; set; }
        public Color ColorTema { get; set; }

        public bool EsFavorita
        {
            get => esFavorita;
            set
            {
                esFavorita = value;
                OnPropertyChanged();
            }
        }

        public bool EstaReproduciendo
        {
            get => estaReproduciendo;
            set
            {
                estaReproduciendo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstadoCard));
            }
        }

        public bool EstaPausada
        {
            get => estaPausada;
            set
            {
                estaPausada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstadoCard));
            }
        }

        public string EstadoCard
        {
            get
            {
                if (EstaReproduciendo) return "▶ SONANDO";
                if (EstaPausada) return "⏸ PAUSADA";
                return Estado;
            }
        }

        public string Estado => string.IsNullOrWhiteSpace(StreamUrl) ? "● OFFLINE" : "● EN VIVO";

        public Emisora(
            string nombre,
            string frecuencia,
            string categoria,
            string provincia,
            string grupo,
            string logo,
            Color colorTema,
            string streamUrl = "")
        {
            Nombre = nombre;
            Frecuencia = frecuencia;
            Categoria = categoria;
            Provincia = provincia;
            Grupo = grupo;
            Logo = logo;
            ColorTema = colorTema;
            StreamUrl = streamUrl;
            EsFavorita = false;
            EstaReproduciendo = false;
            EstaPausada = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}