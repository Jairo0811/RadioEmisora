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
            mediaPlayer.Open(new Uri(url));
            mediaPlayer.Play();
        }

        public void Stop()
        {
            mediaPlayer.Stop();
        }
    }
}