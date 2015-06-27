using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    class SoundFx
    {
        SoundEffect soundEffect;
        SoundEffectInstance instance;
        bool isPlaying;

        public SoundFx(string file)
        {
            soundEffect = ContentHelper.LoadSound(file);
            instance = soundEffect.CreateInstance();
        }

        public void Play()
        {
            soundEffect.Play();
        }

        public void PlayOnce()
        {
            if (!isPlaying)
            {
                instance.Play();
                isPlaying = true;
            }
        }

        public void Reset()
        {
            isPlaying = false;
        }

        public void PlayIfStopped()
        {
            if (instance.State == SoundState.Stopped)
                instance.Play();
        }
    }
}
