using CodenameAdam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    class WidePlatformUp : Obstacle
    {
        bool hasStartedMoving;
        bool hasReachedDestination;
        Vector2 velocity;
        Rectangle original;

        Vector2 endPoint;

        public WidePlatformUp(Player player)
        {            
            player.PlayerRespawned += player_PlayerRespawned;
            texture = ContentHelper.LoadTexture("Tiles/Obstacles/goldbrick_platform_wide");

            IsCollidable = true;
        }

        /// <summary>
        /// For when the platform location is found before the endpoint of the platform.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetStartPoint(int x, int y)
        {
            collRectangle = new Rectangle(x, y, Game1.Tilesize * 12, Game1.Tilesize);
            drawRectangle = collRectangle;
            original = drawRectangle;
        }

        /// <summary>
        /// For when the end of the platform is found before the platform.
        /// </summary>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        public void SetEndPoint(int endX, int endY)
        {
            endPoint = new Vector2(endX, endY);
        }

        void player_PlayerRespawned()
        {
            hasStartedMoving = false;
            hasReachedDestination = false;
            drawRectangle = original;
            collRectangle = original;
        }

        public override void Update(GameTime gameTime, Player player, Map map)
        {
            base.Update(gameTime, player, map);

            collRectangle.Y += (int)velocity.Y;
            drawRectangle = collRectangle;

            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 15, collRectangle.Width, collRectangle.Height - 20);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, collRectangle.Width - 20, collRectangle.Height);

            if (!hasStartedMoving)
            {
                if (collRectangle.Intersects(player.collRectangle))
                    hasStartedMoving = true;
            }

            if (hasStartedMoving)
            {
                if (hasReachedDestination)
                {
                    velocity.Y = 0;
                    collRectangle.Y = (int)endPoint.Y;
                }
                else
                {
                    velocity.Y = -1f;

                    if (collRectangle.Y <= (int)endPoint.Y)
                    {
                        hasReachedDestination = true;
                    }
                }
            }
        }

    }
}
