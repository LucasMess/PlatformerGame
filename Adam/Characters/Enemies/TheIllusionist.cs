using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Characters.Behavior;
using ThereMustBeAnotherWay.Misc.Helpers;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    class TheIllusionist : Enemy
    {
        public TheIllusionist(int x, int y)
        {
            Weight = 100;
            Behavior = new TheIllusionistBehavior();
            Behavior.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/illusionist");
            SetPosition(new Vector2(x, y));
            CollRectangle = new Rectangle(0, 0, 21 * 2, 41 * 2);
            SourceRectangle = new Rectangle(0, 0, 56, 60);

            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(23, 15, 21, 41), 56,60,56,125, 4, true));
            _complexAnimation.AddAnimationData("stopTime", new ComplexAnimData(100, Texture, new Rectangle(24, 13, 21, 41), 0, 60, 56, 125, 4, false));
            _complexAnimation.AddAnimationData("castSpell", new ComplexAnimData(200, Texture, new Rectangle(23, 13, 21, 41), 56*2, 60, 56, 60, 4, true));

            AddAnimationToQueue("still");

            Sounds.AddSoundRef("laugh", "Sounds/Illusionist/evil_laugh");
            Sounds.AddSoundRef("spawn_enemies", "Sounds/Illusionist/spawn_enemies");
        }
        public override int MaxHealth => 1000;

        protected override SoundFx MeanSound => null;

        protected override SoundFx AttackSound => null;

        protected override SoundFx DeathSound => null;

        protected override Rectangle DrawRectangle => ComplexAnimation.GetDrawRectangle();
    }
}
