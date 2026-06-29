using RadioEmisoraRD.Models;
using System.Collections.Generic;
using System.Windows.Media;

namespace RadioEmisoraRD.Services
{
    public static class RadioService
    {
        public static List<Emisora> ObtenerEmisoras()
        {
            return new List<Emisora>
            {
                new Emisora("Mortal", "104.9 FM", "Urbano - Hip Hop", "Santo Domingo", "Grupo Telemicro", "mortal.png", Color.FromRgb(220, 40, 60), "https://n07.radiojar.com/x84tcy2wm2zuv"),

                new Emisora("Cima 100", "100.5 FM", "Pop - Baladas", "Santo Domingo", "Cima 100", "cima.png", Color.FromRgb(40, 130, 255), "https://sonicpanel.streaming10.net:8146/stream"),

                new Emisora("Fuego 90", "90.1 FM", "Salsa - Merengue", "Santo Domingo", "Grupo López", "fuego.png", Color.FromRgb(255, 95, 30), "https://radiordomi.com/8390/stream"),

                new Emisora("Escándalo 102", "102.5 FM", "Variada", "Santo Domingo", "RCC Media", "escandalo.png", Color.FromRgb(190, 60, 255), "https://stream-284.surfernetwork.com/iwqlyzpyry1uv?zt=eyJhbGciOiJIUzI1NiJ9.eyJzdHJlYW0iOiJpd3FseXpweXJ5MXV2IiwiaG9zdCI6InN0cmVhbS0yODQuc3VyZmVybmV0d29yay5jb20iLCJydHRsIjo1LCJqdGkiOiJnV3FDWkFDOFJ6ZVBERjBib3ZMNE1nIiwiaWF0IjoxNzgyNDM0Mjc1LCJleHAiOjE3ODI0MzQzMzV9.JDTMGyC__RtjtfpKp8hkKWTNztdlxP5S9R9z1F7Ct58"),

                new Emisora("Disco 106", "106.1 FM", "Pop - Disco", "Santo Domingo", "Grupo López", "disco.png", Color.FromRgb(255, 190, 40), "https://radiordomi.com/8118/stream"),

                new Emisora("Radio Disney", "97.3 FM", "Pop - Juvenil", "Santo Domingo", "Radio Disney Latinoamérica", "disney.png", Color.FromRgb(230, 40, 60), "https://stream-284.surfernetwork.com/zky9fvlfkootv?zt=eyJhbGciOiJIUzI1NiJ9.eyJzdHJlYW0iOiJ6a3k5ZnZsZmtvb3R2IiwiaG9zdCI6InN0cmVhbS0yODQuc3VyZmVybmV0d29yay5jb20iLCJydHRsIjo1LCJqdGkiOiI3U1czSnFVVlRmZVhQWjcxdWpCcEFnIiwiaWF0IjoxNzgyNDMzNjU1LCJleHAiOjE3ODI0MzM3MTV9.15EdC4uBgaTKEntV92mQqfjRTcL4Lc7plDOka5wKHVk"),

                new Emisora("Radio Popular", "950 AM", "Noticias - Informativa", "Santo Domingo", "Radio Popular", "popular.png", Color.FromRgb(40, 110, 255), "https://stream-175.surfernetwork.com/v5y1wkvoilivv?zt=eyJhbGciOiJIUzI1NiJ9.eyJzdHJlYW0iOiJ2NXkxd2t2b2lsaXZ2IiwiaG9zdCI6InN0cmVhbS0xNzUuc3VyZmVybmV0d29yay5jb20iLCJydHRsIjo1LCJqdGkiOiJuQmFsVXRCRFROR0VxdWdienFnV0ZRIiwiaWF0IjoxNzgyNDM0NTgxLCJleHAiOjE3ODI0MzQ2NDF9.fPlsC4AYX8zdddlaTM2Tt5wkUYKbEfvpT6Z_Ln2tApo"),

                new Emisora("Z 101", "101.3 FM", "Noticias - Opinión", "Santo Domingo", "Z101 Digital", "z101.png", Color.FromRgb(120, 70, 255), "https://streaming.z101digital.com/z101?cb=1782411866478"),

                new Emisora("Primera FM", "88.1 FM", "Variada", "Santo Domingo", "Grupo Telemicro", "primera.png", Color.FromRgb(0, 170, 190), "https://n0f.radiojar.com/5705c71sn2zuv"),

                new Emisora("Alofoke FM", "99.3 FM", "Urbano - Entretenimiento", "Santo Domingo", "Alofoke Media Group", "alofoke.png", Color.FromRgb(255, 70, 70), "https://radiordomi.com:8566/stream"),

                new Emisora("Independencia FM", "93.3 FM", "Variada", "Santo Domingo", "Grupo Telemicro", "independencia.png", Color.FromRgb(0, 180, 120), "https://n06.radiojar.com/nc893hafc8zuv"),

                new Emisora("La Mega", "97.9 FM", "Urbano - Tropical", "New York", "Spanish Broadcasting System", "lamega.png", Color.FromRgb(255, 120, 30), "https://liveaudio.lamusica.com/NY_WSKQ_icy"),

                new Emisora("Radio de Prueba", "Online", "Prueba técnica", "Online", "BBC World Service", "default.png", Color.FromRgb(120, 70, 255), "https://stream.live.vc.bbcmedia.co.uk/bbc_world_service")
            };
        }
    }
}