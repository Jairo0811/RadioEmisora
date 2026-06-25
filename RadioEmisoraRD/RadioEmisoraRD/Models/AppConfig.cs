using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace RadioEmisoraRD.Models
{
    public class AppConfig
    {
        public string UltimaEmisora { get; set; } = "";
        public List<string> Historial { get; set; } = new List<string>();
    }
}
