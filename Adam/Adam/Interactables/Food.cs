using Microsoft.Xna.Framework;
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

        /// <summary>
        /// Creates new food based on enemy killed.
        /// </summary>
        /// <param name="enemy"></param>
        public Food(Enemy enemy)
        {
            drawRectangle = new Rectangle(enemy.collRectangle.X, enemy.collRectangle.Y, 32, 32);
            collRectangle = drawRectangle;

            switch (enemy.CurrentEnemyType)
            {
                case EnemyType.Snake:
                    healAmount = 10;
                    texture = ContentHelper.LoadTexture("Objects/Gold Ore Chunk");                    
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

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Player player, Map map)
        {
            base.Update(gameTime, player, map);

            if (!hasHealed && wasPickedUp)
            {
                hasHealed = true;
                player.Heal(healAmount);
            }
        }
    }
}
