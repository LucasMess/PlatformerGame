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
        public override int MaxHealth => 50;

        protected override SoundFx MeanSound => null;

        protected override SoundFx AttackSound => null;

        protected override SoundFx DeathSound => null;

        protected override Rectangle DrawRectangle => CollRectangle;
    }
}
