using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ProyectoFinalDCU.Models;

namespace ProyectoFinalDCU.Services
{
    public static class ConfigService
    {
        private static readonly string CarpetaDatos =
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "RadioEmisoraRD");

        private static readonly string ArchivoConfig =
            Path.Combine(CarpetaDatos, "config.json");

        public static AppConfig CargarConfig()
        {
            if (!Directory.Exists(CarpetaDatos))
                Directory.CreateDirectory(CarpetaDatos);

            if (!File.Exists(ArchivoConfig))
            {
                AppConfig nueva = new AppConfig();
                GuardarConfig(nueva);
                return nueva;
            }

            string contenido = File.ReadAllText(ArchivoConfig);

            AppConfig config = new AppConfig();
            config.UltimaEmisora = ExtraerValor(contenido, "UltimaEmisora");
            config.Historial = ExtraerLista(contenido, "Historial");

            return config;
        }

        public static void GuardarConfig(AppConfig config)
        {
            if (!Directory.Exists(CarpetaDatos))
                Directory.CreateDirectory(CarpetaDatos);

            string historialJson = "";

            for (int i = 0; i < config.Historial.Count; i++)
            {
                if (i > 0)
                    historialJson += ",";

                historialJson += "\"" + Limpiar(config.Historial[i]) + "\"";
            }

            string json =
                "{\n" +
                "  \"UltimaEmisora\":\"" + Limpiar(config.UltimaEmisora) + "\",\n" +
                "  \"Historial\":[" + historialJson + "]\n" +
                "}";

            File.WriteAllText(ArchivoConfig, json);
        }

        private static string ExtraerValor(string json, string clave)
        {
            Match match = Regex.Match(json, "\"" + clave + "\"\\s*:\\s*\"([^\"]*)\"");

            if (match.Success)
                return match.Groups[1].Value;

            return "";
        }

        private static List<string> ExtraerLista(string json, string clave)
        {
            List<string> lista = new List<string>();

            Match match = Regex.Match(json, "\"" + clave + "\"\\s*:\\s*\\[(.*?)\\]");

            if (!match.Success)
                return lista;

            MatchCollection elementos = Regex.Matches(match.Groups[1].Value, "\"([^\"]*)\"");

            foreach (Match item in elementos)
            {
                if (!string.IsNullOrWhiteSpace(item.Groups[1].Value))
                    lista.Add(item.Groups[1].Value);
            }

            return lista;
        }

        private static string Limpiar(string texto)
        {
            if (texto == null)
                return "";

            return texto.Replace("\"", "").Replace("\\", "");
        }
    }
}