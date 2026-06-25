using System.Collections.Generic;

namespace ProyectoFinalDCU.Models
{
    public class AppConfig
    {
        public string UltimaEmisora { get; set; }
        public List<string> Historial { get; set; }

        public AppConfig()
        {
            UltimaEmisora = "";
            Historial = new List<string>();
        }
    }
}