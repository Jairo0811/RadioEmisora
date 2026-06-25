using System.Collections.Generic;
using System.Windows.Media;
using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services
{
    public static class RadioService
    {
        public static List<Emisora> ObtenerEmisoras()
        {
            return new List<Emisora>
            {
                new Emisora("Mortal 104.9 FM", "104.9 FM", "Urbano - Hip Hop", "Santo Domingo", "mortal.png", Color.FromRgb(220, 40, 60)),
                new Emisora("Cima 100.5 FM", "100.5 FM", "Pop - Baladas", "Santo Domingo", "cima.png", Color.FromRgb(40, 130, 255)),
                new Emisora("Fuego 90.1 FM", "90.1 FM", "Urbano - Reggaetón", "Santo Domingo", "fuego.png", Color.FromRgb(255, 95, 30)),
                new Emisora("Escándalo 102.5 FM", "102.5 FM", "Variada", "Santo Domingo", "escandalo.png", Color.FromRgb(190, 60, 255)),
                new Emisora("Disco 106.1 FM", "106.1 FM", "Pop - Disco", "Santo Domingo", "disco.png", Color.FromRgb(255, 190, 40)),
                new Emisora("Radio Disney 97.3 FM", "97.3 FM", "Pop - Juvenil", "Santo Domingo", "disney.png", Color.FromRgb(230, 40, 60)),
                new Emisora("Radio Popular 950 AM", "950 AM", "Noticias - Informativa", "Santo Domingo", "popular.png", Color.FromRgb(40, 110, 255)),
                new Emisora("Z 101.3 FM", "101.3 FM", "Noticias - Opinión", "Santo Domingo", "z101.png", Color.FromRgb(120, 70, 255)),
                new Emisora("Primera 88.1 FM", "88.1 FM", "Variada", "Santo Domingo", "primera.png", Color.FromRgb(0, 170, 190)),
                new Emisora("Alofoke 99.3 FM", "99.3 FM", "Urbano - Entretenimiento", "Santo Domingo", "alofoke.png", Color.FromRgb(255, 70, 70)),
                new Emisora("Independencia 93.3 FM", "93.3 FM", "Variada", "Santo Domingo", "independencia.png", Color.FromRgb(0, 180, 120)),
                new Emisora("La Mega 97.9 FM", "97.9 FM", "Urbano - Tropical", "Santo Domingo", "mega.png", Color.FromRgb(255, 120, 30)),

                new Emisora(
                    "Radio de Prueba",
                    "Online",
                    "Prueba técnica",
                    "Online",
                    "default.png",
                    Color.FromRgb(120, 70, 255),
                    "https://stream.live.vc.bbcmedia.co.uk/bbc_world_service"
                )
            };
        }
    }
}