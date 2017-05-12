using ThereMustBeAnotherWay.Characters.Scripts;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    class FallingBoulder : Enemy
    {
        public FallingBoulder(int x, int y)
        {
            Weight = 10;
            Texture = GameWorld.SpriteSheet;
            SetPosition(new Vector2(x, y));
            SourceRectangle = new Rectangle(192, 416, 16 * 2, 16 * 2);
            CollRectangle = new Rectangle(0, 0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);

            Behavior = new FallingBoulderScript();
            Behavior.Initialize(this);
        }

        public override int MaxHealth
        {
            get
            {
                return int.MaxValue;
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
                return CollRectangle;
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                return null;
            }
        }
    }
}
