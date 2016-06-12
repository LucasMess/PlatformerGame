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
using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.PlayerCharacter;

namespace Adam.Interactables
{
    public class Food : Item, INewtonian
    {
        int _healAmount;
        bool _hasHealed;
        SoundFx _hitGround;

        SoundEffect _pickUpSound;

        /// <summary>
        /// Creates new food based on enemy killed.
        /// </summary>
        /// <param name="enemy"></param>
        public Food(Enemy enemy)
        {
            CollRectangle = new Rectangle(enemy.GetCollRectangle().X, enemy.GetCollRectangle().Y, 32, 32);
            Velocity.Y = -10f;

            _hitGround = new SoundFx("Sounds/Items/item_pop", this);
            _pickUpSound = ContentHelper.LoadSound("Player/eatSound");

            switch (enemy.Id)
            {
                case 201: // Snake
                    _healAmount = 10;
                    Texture = ContentHelper.LoadTexture("Objects/Food/snake_chest_v1");
                    break;
                case 202: // Frog
                    _healAmount = 5;
                    Texture = ContentHelper.LoadTexture("Objects/Food/frog_leg_v2");
                    break;
                case 204: // Lost
                    _healAmount = 10;
                    break;
                case 205: // Hellboar
                    _healAmount = 40;
                    break;
                case 207: // Bat
                    _healAmount = 10;
                    break;
                case 208: // Duck                    
                    _healAmount = 5;
                    break;
                case 209: // Being of Sight
                    _healAmount = 30;
                    break;
                default:
                    break;
            }

            OnPlayerPickUp += Food_OnPlayerPickUp;
            CollidedWithTileBelow += OnCollisionWithTerrainBelow;
        }

        private void Food_OnPlayerPickUp(PickedUpArgs e)
        {
            e.Player.Heal(_healAmount);
            _pickUpSound.Play();
        }

        public override void Update()
        {
            Player player = GameWorld.Player;
            CollRectangle.X += (int)Velocity.X;
            CollRectangle.Y += (int)Velocity.Y;

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_hasHealed)
                spriteBatch.Draw(Texture, DrawRectangle, Color.White);
        }

        public void OnCollisionWithTerrainBelow(Entity entity, Tile tile)
        {
            CollRectangle.Y = tile.DrawRectangle.Y - CollRectangle.Height;
            if (Velocity.Y < 3)
                Velocity.Y = 0;
            else
            {
                Velocity.Y *= -.9f;

                _hitGround.PlayIfStopped();
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
                return CollRectangle;
            }
        }
    }
}
