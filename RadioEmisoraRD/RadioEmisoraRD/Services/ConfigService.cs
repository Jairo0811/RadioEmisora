using System;
using System.IO;
using System.Text.Json;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services
{
    public static class ConfigService
    {
        private static readonly string CarpetaDatos =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RadioEmisoraRD");

        private static readonly string ArchivoConfig =
            Path.Combine(CarpetaDatos, "config.json");

        public static AppConfig Cargar()
        {
            try
            {
                if (!Directory.Exists(CarpetaDatos))
                    Directory.CreateDirectory(CarpetaDatos);

                if (!File.Exists(ArchivoConfig))
                {
                    AppConfig configNueva = new AppConfig();
                    Guardar(configNueva);
                    return configNueva;
                }

                string json = File.ReadAllText(ArchivoConfig);

                AppConfig? config = JsonSerializer.Deserialize<AppConfig>(json);

                return config ?? new AppConfig();
            }
            catch
            {
                return new AppConfig();
            }
        }

        public static void Guardar(AppConfig config)
        {
            if (!Directory.Exists(CarpetaDatos))
                Directory.CreateDirectory(CarpetaDatos);

            JsonSerializerOptions opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(config, opciones);
            File.WriteAllText(ArchivoConfig, json);
        }
    }
}