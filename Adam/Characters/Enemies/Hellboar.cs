using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using ThereMustBeAnotherWay.Characters.Scripts;
using ThereMustBeAnotherWay.Misc.Helpers;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    public class Hellboar : Enemy
    {
        public Hellboar(int x, int y)
        {
            Behavior = new HellboarBehavior();
            Weight = 10;
            Behavior.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/hellboar");
            SetPosition(new Vector2(x, y));
            CollRectangle = new Rectangle(0, 0, 88, 60);
            SourceRectangle = new Rectangle(0, 0, 68, 60);

            _complexAnimation.AddAnimationData("still", new ComplexAnimData()
            {
                Priority = 1,
                Texture = Texture,
                DeltaRectangle = new Rectangle(15 * 2, 30 * 2, CollRectangle.Width, CollRectangle.Height),
                StartingY = 0,
                Width = SourceRectangle.Width,
                Height = SourceRectangle.Height,
                Speed = 125,
                FrameCount = 4,
                IsRepeating = true,
            });

            _complexAnimation.AddAnimationData("walk", new ComplexAnimData()
            {
                Priority = 10,
                Texture = Texture,
                DeltaRectangle = new Rectangle(15 * 2, 30 * 2, CollRectangle.Width, CollRectangle.Height),
                StartingY = SourceRectangle.Height * 1,
                Width = SourceRectangle.Width,
                Height = SourceRectangle.Height,
                Speed = 125,
                FrameCount = 4,
                IsRepeating = true,
            });

            _complexAnimation.AddAnimationData("angry", new ComplexAnimData()
            {
                Priority = 100,
                Texture = Texture,
                DeltaRectangle = new Rectangle(15 * 2, 30 * 2, CollRectangle.Width, CollRectangle.Height),
                StartingY = SourceRectangle.Height * 2,
                Width = SourceRectangle.Width,
                Height = SourceRectangle.Height,
                Speed = 125,
                FrameCount = 4,
                IsRepeating = false,
            });

            _complexAnimation.AddAnimationData("charge", new ComplexAnimData()
            {
                Priority = 1000,
                Texture = Texture,
                DeltaRectangle = new Rectangle(15 * 2, 30 * 2, CollRectangle.Width, CollRectangle.Height),
                StartingY = SourceRectangle.Height * 3,
                Width = SourceRectangle.Width,
                Height = SourceRectangle.Height,
                Speed = 125,
                FrameCount = 5,
                IsRepeating = true,
            });


            Sounds.AddSoundRef("idle", "Sounds/Hellboar/hellboar_breath");
            Sounds.AddSoundRef("scream", "Sounds/Hellboar/hellboar_scream");
            Sounds.AddSoundRef("fire", "Sounds/Hellboar/hellboar_fire");
        }

        public override int MaxHealth => EnemyDb.HellboarMaxHealth;

        protected override SoundFx MeanSound => Sounds.GetSoundRef("scream");

        protected override SoundFx AttackSound => null;

        protected override SoundFx DeathSound => null;

        protected override Rectangle DrawRectangle => new Rectangle(0,0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);
    }
}
