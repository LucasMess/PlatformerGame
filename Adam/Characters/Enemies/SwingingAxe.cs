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
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.PlayerCharacter;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    class SwingingAxe : Enemy
    {
        static Vector2 PivotPoint = new Vector2(24, 8);
        static Vector2 BladePivotPoint = new Vector2(24, 0);
        public float Rotation { get; set; }

        public Rectangle BladeRectangle { get; set; }

        public SwingingAxe(int x, int y)
        {
            Weight = 0;
            ObeysGravity = false;
            Texture = GameWorld.SpriteSheet;
            SetPosition(new Vector2(x, y) + PivotPoint * 2);
            SourceRectangle = new Rectangle(592, 80, 16 * 3, 16 * 7);
            CollRectangle = new Rectangle(0, 0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);
            // The collision boundary of the swinging axe is only at the blade.
            BladeRectangle = new Rectangle(0, 0, 30 * 2, 15 * 2);
            IsCollidableWithEnemies = false;
            Behavior = new SwingingAxeBehavior();
            Behavior.Initialize(this);
        }

        public override void Update()
        {
            base.Update();

            // Updates after to get the latest CollRect.
            int radius = 84 * 2;
            int x = (int)(radius * Math.Sin(Rotation));
            int y = (int)(radius * Math.Cos(Rotation));
            BladeRectangle = new Rectangle(CollRectangle.X - x, CollRectangle.Y + y, BladeRectangle.Width, BladeRectangle.Height);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White, Rotation, PivotPoint, SpriteEffects.None, 0);
            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), BladeRectangle, SourceRectangle, Color.Red * .5f, Rotation, BladePivotPoint, SpriteEffects.None, 0);
        }

        protected override bool IsIntersectingPlayer()
        {
            Player player = GameWorld.Player;
            return (player.GetCollRectangle().Intersects(BladeRectangle));
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
                //return new Rectangle(CollRectangle.X - 9 * 2, CollRectangle.Y - 92 * 2, SourceRectangle.Width * 2, SourceRectangle.Height *2);
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
