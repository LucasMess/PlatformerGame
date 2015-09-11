using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    public class SoundFx
    {
        SoundEffect soundEffect;
        SoundEffectInstance instance;
        bool isPlaying;
        bool isGlobal = true;
        Entity source;

        public float Volume { get; set; } = Main.MaxVolume;

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
            isGlobal = false;
        }

        public void Play()
        {
            if (!isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
            if (instance.Volume > Volume)
            {
                instance.Volume = Volume;
            }
            instance.Play();
        }

        public void PlayNewInstanceOnce()
        {
            if (!isPlaying)
            {
                SoundEffectInstance newInstance = soundEffect.CreateInstance();
                if (!isGlobal) newInstance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
                if (newInstance.Volume > Volume)
                {
                    newInstance.Volume = Volume;
                }
                newInstance.Play();
                isPlaying = true;
            }
        }

        public void PlayOnce()
        {
            if (!isPlaying)
            {
                if (!isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
                if (instance.Volume > Volume)
                {
                    instance.Volume = Volume;
                }
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
                if (!isGlobal) instance.Volume = source.GetSoundVolume(GameWorld.Instance.player);
                if (instance.Volume > Volume)
                {
                    instance.Volume = Volume;
                }
                instance.Play();
            }
        }

        public void Stop()
        {
            instance.Stop();
        }
    }
}
