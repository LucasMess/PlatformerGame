using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Characters.Behavior;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    class SwingingAxe : Enemy
    {
        public Vector2 PivotPoint { get; set; }
        public float Rotation { get; set; }

        public SwingingAxe(int x, int y)
        {
            Weight = 0;
            ObeysGravity = false;
            Texture = GameWorld.SpriteSheet;
            SetPosition(new Vector2(x, y));
            SourceRectangle = new Rectangle(592, 80, 16 * 3, 16 * 7);
            CollRectangle = new Rectangle(0, 0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);

            Behavior = new SwingingAxeBehavior();
            Behavior.Initialize(this);
            PivotPoint = new Vector2(24, 8);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White, Rotation, PivotPoint, SpriteEffects.None, 0);
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
