using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    public class TestEnemy : NewEnemy
    {
        protected override SoundFx AttackSound
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override int MaxHealth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }
}
