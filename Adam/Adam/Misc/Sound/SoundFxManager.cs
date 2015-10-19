using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Sound
{
    public class SoundFxManager
    {
        Entity source;
        Dictionary<string, SoundFx> sounds = new Dictionary<string, SoundFx>();


        public SoundFxManager(Entity source)
        {
            this.source = source;
        }

        public SoundFxManager()
        {
        }

        public void AddSoundRef(string name, string location)
        {
            if (source != null)
            {
                SoundFx sound = new SoundFx(location, source);
                sounds.Add(name, sound);
            }
            else
            {
                SoundFx sound = new SoundFx(location);
                sounds.Add(name, sound);
            }
        }

        public SoundFx Get(string name)
        {
            SoundFx sound;
            if (sounds.TryGetValue(name, out sound))
            {
                return sound;
            }
            else throw new Exception("Sound not found: " + name);
        }

    }
}
