using Adam.Characters.Scripts;
using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;

namespace Adam.Characters.Enemies
{
    public class Frog : Enemy
    {
        public Frog(int x, int y)
        {
            Weight = 10;
            Behavior = new FrogBehavior();
            Behavior.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/frog");
            SetPosition(new Vector2(x, y));
            CollRectangle = new Rectangle(0, 0, 32, 32);
            SourceRectangle = new Rectangle(0, 0, 24, 32);

            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(4, 16, 24, 32), 0, 24, 32, 125, 4, true));
            _complexAnimation.AddAnimationData("jump", new ComplexAnimData(100, Texture, new Rectangle(4, 16, 24, 32), 32, 24, 32, 50, 4, false));

            Sounds.AddSoundRef("jump", "Sounds/Frog/frog_jump");
            Sounds.AddSoundRef("idle", "Sounds/Frog/frog_croak");
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDb.FrogMaxHealth;
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                return Sounds.GetSoundRef("idle");
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
                return null;
            }
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X - 8, CollRectangle.Y - 32, 48, 64);
            }
        }
    }
}
