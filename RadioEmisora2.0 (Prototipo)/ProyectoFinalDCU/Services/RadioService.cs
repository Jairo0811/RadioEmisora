using System.Collections.Generic;
using System.Drawing;
using ProyectoFinalDCU.Models;

namespace ProyectoFinalDCU.Services
{
    public static class RadioService
    {
        public static List<Emisora> ObtenerEmisoras()
        {
            return new List<Emisora>
            {
                new Emisora(
                    "Mortal 104.9 FM",
                    "104.9 FM",
                    "Urbano - Hip Hop",
                    "mortal.png",
                    Color.FromArgb(220, 40, 60),
                    descripcion: "La Estacion oficial de la Musica Urbana."
                ),

                new Emisora(
                    "Cima 100.5 FM",
                    "100.5 FM",
                    "Pop - Baladas",
                    "cima.png",
                    Color.FromArgb(40, 130, 255),
                    descripcion: "Estación musical enfocada en pop, baladas y programación variada."
                ),

                new Emisora(
                    "Fuego 90.1 FM",
                    "90.1 FM",
                    "Urbano - Reggaetón",
                    "fuego.png",
                    Color.FromArgb(255, 95, 30),
                    descripcion: "Radio de música urbana con identidad energética y juvenil."
                ),

                new Emisora(
                    "Escándalo 102.5 FM",
                    "102.5 FM",
                    "Variada",
                    "escandalo.png",
                    Color.FromArgb(190, 60, 255),
                    descripcion: "Emisora dominicana de programación variada y entretenimiento."
                ),

                new Emisora(
                    "Disco 106.1 FM",
                    "106.1 FM",
                    "Pop - Disco",
                    "disco.png",
                    Color.FromArgb(255, 190, 40),
                    descripcion: "Estación musical orientada al pop, música disco y éxitos clásicos."
                ),

                new Emisora(
                    "Radio Disney 97.3 FM",
                    "97.3 FM",
                    "Pop - Juvenil",
                    "disney.png",
                    Color.FromArgb(230, 40, 60),
                    descripcion: "Radio juvenil con programación musical pop y familiar."
                ),

                new Emisora(
                    "Radio Popular 950 AM",
                    "950 AM",
                    "Noticias - Informativa",
                    "popular.png",
                    Color.FromArgb(40, 110, 255),
                    descripcion: "Emisora AM dominicana con enfoque informativo y contenido de actualidad."
                ),

                new Emisora(
                    "Z 101.3 FM",
                    "101.3 FM",
                    "Noticias - Opinión",
                    "z101.png",
                    Color.FromArgb(120, 70, 255),
                    descripcion: "Estación de noticias, opinión pública y análisis de actualidad nacional."
                ),

                new Emisora(
                    "Primera 88.1 FM",
                    "88.1 FM",
                    "Variada",
                    "primera.png",
                    Color.FromArgb(0, 170, 190),
                    descripcion: "Emisora dominicana con programación musical e informativa variada."
                ),

                new Emisora(
                    "Alofoke 99.3 FM",
                    "99.3 FM",
                    "Urbano - Entretenimiento",
                    "alofoke.png",
                    Color.FromArgb(255, 70, 70),
                    descripcion: "Plataforma radial urbana enfocada en entretenimiento, entrevistas y cultura popular."
                ),

                new Emisora(
                    "Independencia 93.3 FM",
                    "93.3 FM",
                    "Variada",
                    "independencia.png",
                    Color.FromArgb(0, 180, 120),
                    descripcion: "Emisora dominicana de contenido variado."
                ),

                new Emisora(
                    "La Mega 97.9 FM",
                    "97.9 FM",
                    "Urbano - Tropical",
                    "mega.png",
                    Color.FromArgb(255, 120, 30),
                    descripcion: "Estación tropical y urbana con música popular dominicana."
                ),

                new Emisora(
                    "Radio de Prueba",
                    "Online",
                    "Prueba técnica",
                    "default.png",
                    Color.FromArgb(95, 45, 230),
                    descripcion: "Stream de prueba utilizado para validar el reproductor.",
                    streamUrl: "https://stream.live.vc.bbcmedia.co.uk/bbc_world_service"
                )
            };
        }
    }
}