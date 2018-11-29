using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace TopDownShooter.Utils
{
    public static class SoundManager
    {
        // play the sound once
        public static void PlaySound(SoundEffectInstance instance)
        {
            // basically, to play it once we have to check if the instance isn't playing
            // if not, we play it so once we check back it will be played and then we won't restart it.
            if (instance.State != SoundState.Playing)
            {
                instance.Play();
            }
        }

        // used to loop the background music
        public static void LoopSound(Song song)
        {
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
        }

        public static void StopSound()
        {
            MediaPlayer.Stop();
        }
    }
}
