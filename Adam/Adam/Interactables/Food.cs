using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    class Food : PowerUp
    {
        int healAmount;
        bool hasHealed;

        SoundEffect pickUpSound;

        /// <summary>
        /// Creates new food based on enemy killed.
        /// </summary>
        /// <param name="enemy"></param>
        public Food(Enemy enemy)
        {
            IsCollidable = true;
            drawRectangle = new Rectangle(enemy.collRectangle.X, enemy.collRectangle.Y, 32, 32);
            collRectangle = drawRectangle;

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
                default:
                    break;
            }


        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Player player, GameWorld map)
        {
            base.Update(gameTime, player, map);

            if (!hasHealed && wasPickedUp)
            {
                hasHealed = true;
                player.Heal(healAmount);
                pickUpSound.Play();
            }

            velocity.Y += .3f;
        }
    }
}
