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
    public class Food : Item, ICollidable, INewtonian
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

            hitGround = new SoundFx("Sounds/Items/item_pop", this);
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
                    texture = ContentHelper.LoadTexture("Objects/Food/frog_leg_v2");
                    break;
                default:
                    break;
            }


        }

        public override void Update()
        {
            Player player = GameWorld.Instance.player;
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
            if (velocity.Y < 3)
                velocity.Y = 0;
            else
            {
                velocity.Y *= -.9f;

                hitGround.PlayIfStopped();
            }
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
            get { return Main.Gravity; }
            set
            {
                GravityStrength = value;
            }
        }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public bool IsAboveTile { get; set; }
    }
}
