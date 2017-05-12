using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ThereMustBeAnotherWay.Projectiles
{
    static class CollisionRay
    {

        /// <summary>
        /// Returns true if the other entity is in the field of view of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsEntityInSight(Entity entity, Entity other)
        {
            Rectangle rect = new Rectangle(entity.GetCollRectangle().Center.X, entity.GetCollRectangle().Center.Y, 1, 1);
            List<Rectangle> rects = new List<Rectangle>();

            double xVector = (double)(other.GetCollRectangle().Center.X - rect.Center.X);
            double yVector = (double)(other.GetCollRectangle().Center.Y - rect.Center.Y);

            Vector2 maxVelocity = new Vector2(30, 30);
            double magnitude = Math.Sqrt((Math.Pow(xVector, 2.0)) + (Math.Pow(yVector, 2.0)));
            Vector2 newVelocity = new Vector2(maxVelocity.X * (float)(xVector / magnitude), maxVelocity.Y * (float)(yVector / magnitude));

            for (int i = 0; i < 10; i++)
            {
                rects.Add(rect);

                int index = (int)(rect.Y / TMBAW_Game.Tilesize * GameWorld.WorldData.LevelWidth) + (int)(rect.X / TMBAW_Game.Tilesize);

                if (rect.Intersects(other.GetCollRectangle()))
                    return true;
                if (index > GameWorld.TileArray.Length - 1 || index < 0)
                    return false;
                if (GameWorld.TileArray[index].IsSolid)
                    return false;

                rect.X += (int)newVelocity.X;
                rect.Y += (int)newVelocity.Y;
            }
            return false;
        }

    }
}
