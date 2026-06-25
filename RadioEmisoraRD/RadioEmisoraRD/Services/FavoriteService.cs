using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RadioEmisoraRD.Services
{
    public static class FavoriteService
    {
        private static readonly string CarpetaDatos =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RadioEmisoraRD");

        private static readonly string ArchivoFavoritos =
            Path.Combine(CarpetaDatos, "favoritos.json");

        public static List<string> Cargar()
        {
            try
            {
                if (!Directory.Exists(CarpetaDatos))
                    Directory.CreateDirectory(CarpetaDatos);

                if (!File.Exists(ArchivoFavoritos))
                {
                    Guardar(new List<string>());
                    return new List<string>();
                }

                string json = File.ReadAllText(ArchivoFavoritos);

                List<string>? favoritos = JsonSerializer.Deserialize<List<string>>(json);

                return favoritos ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static void Guardar(List<string> favoritos)
        {
            if (!Directory.Exists(CarpetaDatos))
                Directory.CreateDirectory(CarpetaDatos);

            JsonSerializerOptions opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(favoritos, opciones);
            File.WriteAllText(ArchivoFavoritos, json);
        }
    }
}