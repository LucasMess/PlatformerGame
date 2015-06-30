using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    class Food : PowerUp, ICollidable, INewtonian
    {
        int healAmount;
        bool hasHealed;
        SoundFx hitGround;

        SoundEffect pickUpSound;

        /// <summary>
        /// Creates new food based on enemy killed.
        /// </summary>
        /// <param name="enemy"></param>
        public Food(Enemy enemy)
        {
            drawRectangle = new Rectangle(enemy.collRectangle.X, enemy.collRectangle.Y, 32, 32);
            collRectangle = drawRectangle;
            velocity.Y = -10f;

            hitGround = new SoundFx("Sounds/Items/item_pop");
            pickUpSound = ContentHelper.LoadSound("Sounds/eat");

            switch (enemy.CurrentEnemyType)
            {
                case EnemyType.Snake:
                    healAmount = 10;
                    texture = ContentHelper.LoadTexture("Objects/Food/snake_chest_v1");
                    break;
                case EnemyType.Potato:
                    break;
                case EnemyType.Shade:
                    break;
                case EnemyType.Drone:
                    break;
                case EnemyType.Bloodless:
                    break;
                case EnemyType.Frog:
                    healAmount = 5;
                    texture = ContentHelper.LoadTexture("Objects/Food/snake_chest_v1");
                    break;
                default:
                    break;
            }


        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Player player, GameWorld map)
        {
            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;
            drawRectangle = collRectangle;

            if (!hasHealed && player.collRectangle.Intersects(collRectangle) && player.health < player.maxHealth)
            {
                hasHealed = true;
                player.Heal(healAmount);
                pickUpSound.Play();
            }
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!hasHealed)
                spriteBatch.Draw(texture, drawRectangle, Color.White);
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            OnCollisionAbove(e);
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.drawRectangle.Y - collRectangle.Height;
            if (velocity.Y < 5)
                velocity.Y = 5;

            velocity.Y *= - .9f;
            hitGround.PlayIfStopped();
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            OnCollisionRight(e);
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            OnCollisionLeft(e);
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
        }

        public float GravityStrength
        {
            get { return Game1.Gravity; }
        }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public bool IsAboveTile { get; set; }
    }
}
