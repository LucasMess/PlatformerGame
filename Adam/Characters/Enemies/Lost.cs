using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ThereMustBeAnotherWay.Misc;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    public class Lost : Enemy
    {
        public Lost(int x, int y) { }
        public override int MaxHealth => throw new NotImplementedException();

        protected override SoundFx MeanSound => throw new NotImplementedException();

        protected override SoundFx AttackSound => throw new NotImplementedException();

        protected override SoundFx DeathSound => throw new NotImplementedException();

        protected override Rectangle DrawRectangle => throw new NotImplementedException();
    }
}
