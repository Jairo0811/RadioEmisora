using System.Collections.Generic;

namespace RadioEmisoraRD.Models
{
    public class AppConfig
    {
        public string UltimaEmisora { get; set; } = "";
        public List<string> Historial { get; set; } = new List<string>();
        public double Volumen { get; set; } = 0.80;
    }
}