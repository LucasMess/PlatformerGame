using System;
using System.Collections.Generic;

namespace Adam.Misc.Sound
{
    public class SoundFxManager
    {
        Entity _source;
        Dictionary<string, SoundFx> _sounds = new Dictionary<string, SoundFx>();


        public SoundFxManager(Entity source)
        {
            this._source = source;
        }

        public SoundFxManager()
        {
        }

        public void AddSoundRef(string name, string location)
        {
            if (_source != null)
            {
                SoundFx sound = new SoundFx(location, _source);
                _sounds.Add(name, sound);
            }
            else
            {
                SoundFx sound = new SoundFx(location);
                _sounds.Add(name, sound);
            }
        }

        public SoundFx GetSoundRef(string name)
        {
            SoundFx sound;
            if (_sounds.TryGetValue(name, out sound))
            {
                return sound;
            }
            else throw new Exception("Sound not found: " + name);
        }

    }
}
