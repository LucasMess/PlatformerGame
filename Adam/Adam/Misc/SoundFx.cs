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
        bool isGlobal;
        Entity source;

        public SoundFx(string file)
        {
            soundEffect = ContentHelper.LoadSound(file);
            instance = soundEffect.CreateInstance();
        }

        public SoundFx(string file, Entity entity)
        {
            soundEffect = ContentHelper.LoadSound(file);
            instance = soundEffect.CreateInstance();
            source = entity;
            isGlobal = true;
        }

        public void Play()
        {
            if (isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
            instance.Play();
        }

        public void PlayNewInstanceOnce()
        {
            if (!isPlaying)
            {
                if (isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
                soundEffect.Play();
                isPlaying = true;
            }
        }

        public void PlayOnce()
        {
            if (!isPlaying)
            {
                if (isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
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
            {
                if (isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
                instance.Play();
            }
        }
    }
}
