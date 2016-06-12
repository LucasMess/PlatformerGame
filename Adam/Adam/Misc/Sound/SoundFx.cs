using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.Misc.Helpers;

namespace Adam.Misc
{
    public class SoundFx
    {
        SoundEffect _soundEffect;
        SoundEffectInstance _instance;
        bool _isPlaying;
        bool _isGlobal = true;
        Entity _source;

        public float MaxVolume { get; set; } = Main.MaxVolume;

        public SoundFx(string file)
        {
            _soundEffect = ContentHelper.LoadSound(file);
            _instance = _soundEffect.CreateInstance();
        }

        public SoundFx(string file, Entity entity)
        {
            _soundEffect = ContentHelper.LoadSound(file);
            _instance = _soundEffect.CreateInstance();
            _source = entity;
            _isGlobal = false;
        }

        /// <summary>
        /// Plays a new instance of the sound and automatically resets it.
        /// </summary>
        public void Play()
        {
            PlayNewInstanceOnce();
            Reset();

            //if (!_isGlobal) _instance.Volume = _source.GetSoundVolume(GameWorld.Player, MaxVolume);
            //if (_instance.Volume > MaxVolume)
            //{
            //    _instance.Volume = MaxVolume;
            //}
            //_instance.Play();
        }

        public void PlayNewInstanceOnce()
        {
            if (!_isPlaying)
            {
                SoundEffectInstance newInstance = _soundEffect.CreateInstance();
                if (!_isGlobal) newInstance.Volume = _source.GetSoundVolume(GameWorld.Player, MaxVolume);
                if (newInstance.Volume > MaxVolume)
                {
                    newInstance.Volume = MaxVolume;
                }
                newInstance.Play();
                _isPlaying = true;
            }
        }

        public void PlayOnce()
        {
            if (!_isPlaying)
            {
                if (!_isGlobal) _instance.Volume = _source.GetSoundVolume(GameWorld.Player, MaxVolume);
                if (_instance.Volume > MaxVolume)
                {
                    _instance.Volume = MaxVolume;
                }
                _instance.Play();
                _isPlaying = true;
            }
        }

        public void Reset()
        {
            _isPlaying = false;
        }

        public void PlayIfStopped()
        {
            if (_instance.State == SoundState.Stopped)
            {
                if (!_isGlobal) _instance.Volume = _source.GetSoundVolume(GameWorld.Player, MaxVolume);
                if (_instance.Volume > MaxVolume)
                {
                    _instance.Volume = MaxVolume;
                }
                _instance.Play();
            }
        }

        public void Stop()
        {
            _instance.Stop();
        }
    }
}
