using Adam.Characters.Scripts;
using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.Characters.Enemies
{
    class FallingBoulder : Enemy
    {

        public FallingBoulder(int x, int y)
        {
            Weight = 10;
            Texture = GameWorld.SpriteSheet;
            SetPosition(new Vector2(x, y));
            SourceRectangle = new Rectangle(193, 417, 16 * 2, 16 * 2);
            CollRectangle = new Rectangle(0, 0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);

            Script = new FallingBoulderScript();
            Script.Initialize(this);
        }

        public override byte Id
        {
            get
            {
                return (byte)AdamGame.TileType.FallingBoulder;
            }
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
