using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.Misc.Interfaces;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.Interactables
{
    public class Food : Item, INewtonian
    {
        int _healAmount;
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

            _healAmount = 0;

            OnPlayerPickUp += Food_OnPlayerPickUp;
            CollidedWithTileBelow += OnCollisionWithTerrainBelow;
        }

        private void Food_OnPlayerPickUp(PickedUpArgs e)
        {
            e.Player.Heal(_healAmount);
            _pickUpSound.Play();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, Color.White);
        }

        public void OnCollisionWithTerrainBelow(Entity entity, Tile tile)
        {
            SetY(tile.DrawRectangle.Y - CollRectangle.Height);
            if (Velocity.Y < 3)
                Velocity.Y = 0;
            else
            {
                Velocity.Y *= -.9f;

                _hitGround.PlayIfStopped();
            }
        }

        public bool IsFlying { get; set; }

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
