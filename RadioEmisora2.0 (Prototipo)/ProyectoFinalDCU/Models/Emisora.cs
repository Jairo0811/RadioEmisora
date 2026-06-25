using System.Drawing;

namespace ProyectoFinalDCU.Models
{
    public class Emisora
    {
        public string Nombre { get; set; }
        public string Frecuencia { get; set; }
        public string Categoria { get; set; }
        public string Logo { get; set; }
        public string StreamUrl { get; set; }
        public string Provincia { get; set; }
        public string Descripcion { get; set; }
        public Color ColorTema { get; set; }
        public bool Favorita { get; set; }

        public Emisora(
            string nombre,
            string frecuencia,
            string categoria,
            string logo,
            Color colorTema,
            string provincia = "Santo Domingo",
            string descripcion = "",
            string streamUrl = "",
            bool favorita = false)
        {
            Nombre = nombre;
            Frecuencia = frecuencia;
            Categoria = categoria;
            Logo = logo;
            ColorTema = colorTema;
            Provincia = provincia;
            Descripcion = descripcion;
            StreamUrl = streamUrl;
            Favorita = favorita;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}