using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    class Bat : Enemy, ICollidable
    {
        bool isLookingForRefuge;
        bool isSleeping;

        Rectangle rangeRect;
        Vector2 maxVelocity;

        public override byte ID
        {
            get
            {
                return 207;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDB.Bat_MaxHealth;
            }
        }

        SoundFx meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                return null;
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
                return collRectangle;
            }
        }

        public Bat(int x, int y)
        {
            collRectangle = new Rectangle(x, y, 32, 64);
            sourceRectangle = new Rectangle(0, 0, 16, 32);
            Texture = Main.DefaultTexture;
            maxVelocity = new Vector2(3, 3);
            // texture = ContentHelper.LoadTexture("Bat/bat");
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            if (isLookingForRefuge)
            {
                isSleeping = true;
            }
            else
            {
                velocity.Y = 0;
            }
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = 0;
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            velocity.X = 0;
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.GetPlayer();

            rangeRect = new Rectangle(collRectangle.X - 100, collRectangle.Y - 100, collRectangle.Width + 200, collRectangle.Height + 200);

            if (player.GetCollRectangle().Intersects(rangeRect))
            {
                isSleeping = false;
                isLookingForRefuge = false;
            }
            else
            {
                isLookingForRefuge = true;
            }

            if (!isLookingForRefuge)
            {
                int buffer = 5;
                if (collRectangle.Y < player.GetCollRectangle().Y - buffer)
                {
                    velocity.Y = maxVelocity.Y;
                }
                else if (collRectangle.Y > player.GetCollRectangle().Y + buffer)
                {
                    velocity.Y = -maxVelocity.Y;
                }
                else
                {
                    velocity.Y = 0;
                }

                if (collRectangle.X < player.GetCollRectangle().X - buffer)
                {
                    velocity.X = maxVelocity.X;
                }
                else if (collRectangle.X > player.GetCollRectangle().X + buffer)
                {
                    velocity.X = -maxVelocity.X;
                }
                else
                {
                    velocity.X = 0;
                }
            }
            else
            {
                velocity.X = 0;
                velocity.Y = -maxVelocity.Y;
            }

            if (isSleeping)
            {
                velocity = Vector2.Zero;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.DefaultTexture, rangeRect, Color.Red * .5f);
            spriteBatch.Draw(Texture, DrawRectangle, Color.White);
        }
    }
}
