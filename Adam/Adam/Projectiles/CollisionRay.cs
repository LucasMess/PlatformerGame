﻿using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Projectiles
{
    static class CollisionRay
    {
        public static bool IsPlayerInSight(Enemy enemy, Player player, GameWorld map, out List<Rectangle> rects)
        {
            Rectangle rect = new Rectangle(enemy.collRectangle.Center.X, enemy.collRectangle.Center.Y, 1, 1);
            rects = new List<Rectangle>();

            double xVector = (double)(player.collRectangle.Center.X - rect.Center.X);
            double yVector = (double)(player.collRectangle.Center.Y - rect.Center.Y);

            Vector2 maxVelocity = new Vector2(30, 30);
            double magnitude = Math.Sqrt((Math.Pow(xVector, 2.0)) + (Math.Pow(yVector, 2.0)));
            Vector2 newVelocity = new Vector2(maxVelocity.X * (float)(xVector / magnitude), maxVelocity.Y * (float)(yVector / magnitude));

            for (int i = 0; i < 10; i++)
            {
                rects.Add(rect);

                int index = (int)(rect.Y / Game1.Tilesize * map.mapTexture.Width) + (int)(rect.X / Game1.Tilesize);

                if (rect.Intersects(player.collRectangle))
                    return true;
                if (map.tileArray[index].isSolid)
                    return false;

                rect.X += (int)newVelocity.X;
                rect.Y += (int)newVelocity.Y;
            }
            return false;
        }

    }
}
