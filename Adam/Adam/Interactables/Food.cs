using Adam.Characters.Enemies;
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
    public class Food : Item, INewtonian
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
            collRectangle = new Rectangle(enemy.GetCollRectangle().X, enemy.GetCollRectangle().Y, 32, 32);
            velocity.Y = -10f;

            hitGround = new SoundFx("Sounds/Items/item_pop", this);
            pickUpSound = ContentHelper.LoadSound("Sounds/eat");

            switch (enemy.ID)
            {
                case 201: // Snake
                    healAmount = 10;
                    Texture = ContentHelper.LoadTexture("Objects/Food/snake_chest_v1");
                    break;
                case 202: // Frog
                    healAmount = 5;
                    Texture = ContentHelper.LoadTexture("Objects/Food/frog_leg_v2");
                    break;
                case 204: // Lost
                    healAmount = 10;
                    break;
                case 205: // Hellboar
                    healAmount = 40;
                    break;
                case 207: // Bat
                    healAmount = 10;
                    break;
                case 208: // Duck                    
                    healAmount = 5;
                    break;
                case 209: // Being of Sight
                    healAmount = 30;
                    break;
                default:
                    break;
            }

            OnPlayerPickUp += Food_OnPlayerPickUp;
            CollidedWithTileBelow += OnCollisionWithTerrainBelow;
        }

        private void Food_OnPlayerPickUp(PickedUpArgs e)
        {
            e.Player.Heal(healAmount);
            pickUpSound.Play();
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.player;
            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!hasHealed)
                spriteBatch.Draw(Texture, DrawRectangle, Color.White);
        }

        public void OnCollisionWithTerrainBelow(Entity entity, Tile tile)
        {
            collRectangle.Y = tile.drawRectangle.Y - collRectangle.Height;
            if (velocity.Y < 3)
                velocity.Y = 0;
            else
            {
                velocity.Y *= -.9f;

                hitGround.PlayIfStopped();
            }
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

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }
    }
}
