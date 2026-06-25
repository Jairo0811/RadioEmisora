using System.Windows.Media;

namespace RadioEmisoraRD.Models
{
    public class Emisora
    {
        public string Nombre { get; set; }
        public string Frecuencia { get; set; }
        public string Categoria { get; set; }
        public string Provincia { get; set; }
        public string Logo { get; set; }
        public string StreamUrl { get; set; }
        public Color ColorTema { get; set; }
        public bool EsFavorita { get; set; }

        public string Estado
        {
            get
            {
                return string.IsNullOrWhiteSpace(StreamUrl) ? "● OFFLINE" : "● EN VIVO";
            }
        }

        public Emisora(
            string nombre,
            string frecuencia,
            string categoria,
            string provincia,
            string logo,
            Color colorTema,
            string streamUrl = "")
        {
            Nombre = nombre;
            Frecuencia = frecuencia;
            Categoria = categoria;
            Provincia = provincia;
            Logo = logo;
            ColorTema = colorTema;
            StreamUrl = streamUrl;
            EsFavorita = false;
        }
    }
}