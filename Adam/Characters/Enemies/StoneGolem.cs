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
    public class StoneGolem : Enemy
    {
        public StoneGolem(int x, int y)
        {
            Weight = 90;
            Behavior = new StoneGolemBehavior();
            Behavior.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/stone_golem");
            SetPosition(new Vector2(x, y));
            CollRectangle = new Rectangle(0, 0, 18 * 2, 25 * 2);
            SourceRectangle = new Rectangle(0, 0, 32, 32);

            _complexAnimation.AddAnimationData("idle", new ComplexAnimData(1, Texture, new Rectangle(5, 7, 18, 25), 0, 32, 32, 200, 4, true));
            _complexAnimation.AddAnimationData("walk", new ComplexAnimData(100, Texture, new Rectangle(5, 7, 18, 25), 32, 32, 32, 125, 4, true));

            Sounds.AddSoundRef("jump", "Sounds/Frog/frog_jump");
            Sounds.AddSoundRef("idle", "Sounds/Frog/frog_croak");
        }

        public override int MaxHealth => 200;

        protected override SoundFx MeanSound => null;

        protected override SoundFx AttackSound => null;

        protected override SoundFx DeathSound => null;

        protected override Rectangle DrawRectangle => ComplexAnimation.GetDrawRectangle();
    }
}
