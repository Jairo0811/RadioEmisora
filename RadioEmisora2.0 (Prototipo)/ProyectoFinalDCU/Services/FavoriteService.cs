using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProyectoFinalDCU.Services
{
    public static class FavoriteService
    {
        private static readonly string CarpetaDatos =
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "RadioEmisoraRD");

        private static readonly string ArchivoFavoritos =
            Path.Combine(CarpetaDatos, "favoritos.json");

        public static List<string> CargarFavoritos()
        {
            if (!Directory.Exists(CarpetaDatos))
                Directory.CreateDirectory(CarpetaDatos);

            if (!File.Exists(ArchivoFavoritos))
                File.WriteAllText(ArchivoFavoritos, "[]");

            string contenido = File.ReadAllText(ArchivoFavoritos)
                .Replace("[", "")
                .Replace("]", "")
                .Replace("\"", "");

            if (string.IsNullOrWhiteSpace(contenido))
                return new List<string>();

            return contenido
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        public static void GuardarFavoritos(List<string> favoritos)
        {
            if (!Directory.Exists(CarpetaDatos))
                Directory.CreateDirectory(CarpetaDatos);

            string json = "[\"" + string.Join("\",\"", favoritos) + "\"]";
            File.WriteAllText(ArchivoFavoritos, json);
        }
    }
}