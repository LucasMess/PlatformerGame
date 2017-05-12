using ThereMustBeAnotherWay.Characters.Scripts;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;
using System;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    class Snake : Enemy
    {

        public Snake(int x, int y)
        {
            Weight = 10;
            Behavior = new SnakeBehavior();
            Behavior.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/snake");
            SetPosition(new Vector2(x, y));
            CollRectangle = new Rectangle(0, 0, 32 * 2, 48 * 2);
            SourceRectangle = new Rectangle(0, 0, 32, 48);

            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 32, 48), 0, 32, 48, 125, 8, true));

            _complexAnimation.AddAnimationData("attack", new ComplexAnimData()
            {
                Priority = 1000,
                Texture = Texture,
                DeltaRectangle = new Rectangle(0, 0, 32, 48),
                StartingY = SourceRectangle.Height * 1,
                Width = SourceRectangle.Width,
                Height = SourceRectangle.Height,
                Speed = 50,
                FrameCount = 8,
                IsRepeating = false,
            });

            Sounds.AddSoundRef("idle", "Sounds/Snake/snake_idle");
            Sounds.AddSoundRef("spit", "Sounds/Snake/snake_spit");
            Sounds.AddSoundRef("death", "Sounds/Snake/snake_death");
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDb.SnakeMaxHealth;
            }
        }

        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                return Sounds.GetSoundRef("death");
            }
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X, CollRectangle.Y, 32 * 2, 48 * 2);
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                return Sounds.GetSoundRef("idle");
            }
        }
    }
}
