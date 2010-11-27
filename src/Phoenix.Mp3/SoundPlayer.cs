using System;
using Microsoft.DirectX.AudioVideoPlayback;

namespace Phoenix
{
    public static class SoundPlayer
    {
        public static void Play(string soundLocation)
        {
            Audio audio = Audio.FromFile(soundLocation);
            audio.Play();
            System.Threading.Thread.Sleep((int)(audio.Duration * 1000) + 1);
        }
    }
}
