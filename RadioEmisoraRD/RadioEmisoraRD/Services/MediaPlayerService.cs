using System;
using System.Windows.Media;

namespace RadioEmisoraRD.Services
{
    public class MediaPlayerService
    {
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();

        public double Volume
        {
            get => mediaPlayer.Volume;
            set => mediaPlayer.Volume = value;
        }

        public void Play(string url)
        {
            mediaPlayer.Open(new Uri(url, UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();
        }

       
        public void Resume()
        {
            mediaPlayer.Play();
        }

        public void Pause()
        {
            mediaPlayer.Pause();
        }

        public void Stop()
        {
            mediaPlayer.Stop();
        }
    }
}