using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Characters.Enemies
{
    class Bat : Enemy, ICollidable
    {
        bool isLookingForRefuge;
        bool isSleeping;

        Rectangle rangeRect;

        public Bat(int x, int y)
        {
            collRectangle = new Rectangle(x, y, 32, 64);
            drawRectangle = new Rectangle(x, y, 32, 64);
            sourceRectangle = new Rectangle(0, 0, 16, 32);
            Texture = Main.DefaultTexture;
            maxVelocity = new Vector2(3, 3);
            health = 100;
            // texture = ContentHelper.LoadTexture("Bat/bat");

            Initialize();
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

        public override void Update(Player player, GameTime gameTime)
        {
            if (isDead) return;

            rangeRect = new Rectangle(collRectangle.X - 100, collRectangle.Y - 100, collRectangle.Width + 200,  collRectangle.Height + 200);

            drawRectangle = collRectangle;

            if (player.collRectangle.Intersects(rangeRect))
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
                if (collRectangle.Y < player.collRectangle.Y - buffer)
                {
                    velocity.Y = maxVelocity.Y;
                }
                else if (collRectangle.Y > player.collRectangle.Y + buffer)
                {
                    velocity.Y = -maxVelocity.Y;
                }
                else
                {
                    velocity.Y = 0;
                }

                if (collRectangle.X < player.collRectangle.X - buffer)
                {
                    velocity.X = maxVelocity.X;
                }
                else if (collRectangle.X > player.collRectangle.X + buffer)
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

            base.Update(player,gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.DefaultTexture, rangeRect, Color.Red * .5f);
            spriteBatch.Draw(Texture, drawRectangle, Color.White);
        }
    }
}
