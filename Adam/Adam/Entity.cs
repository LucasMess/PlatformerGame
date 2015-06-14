using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    enum CollisionLocation
    {
        Bottom,
        Right,
        Left,
        Top,
        Null,
    }

    class Entity
    {
        public delegate void TerrainCollisionHandler(TerrainCollisionEventArgs e);

        public event TerrainCollisionHandler CollidedWithTerrainAbove;
        public event TerrainCollisionHandler CollidedWithTerrainBelow;
        public event TerrainCollisionHandler CollidedWithTerrainRight;
        public event TerrainCollisionHandler CollidedWithTerrainLeft;
        public event TerrainCollisionHandler CollidedWithTerrainAnywhere;

        protected Texture2D texture;
        public Vector2 position;
        protected Vector2 origin;
        public Rectangle drawRectangle;
        public Rectangle collRectangle;
        protected Rectangle sourceRectangle;
        protected Animation animation;

        public Rectangle yRect, xRect;

        public int TileIndex { get; set; }

        protected ContentManager Content;

        protected float opacity = 1f;

        public Entity()
        {
            Content = Game1.Content;
        }

        public float Opacity
        {
            get { return opacity; }
            set { value = opacity; }
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, Color.White * opacity);
        }

        public virtual void DrawFromCenter(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, null, Color.White, 0, origin, SpriteEffects.None, 0);
        }

        public void SetRectPos(Vector2 position)
        {
            drawRectangle.X = (int)position.X;
            drawRectangle.Y = (int)position.Y;
        }

        public void SetRectPosAtOrigin(Vector2 position)
        {
            drawRectangle.X = (int)(position.X + origin.X);
            drawRectangle.Y = (int)(position.Y + origin.Y);
        }

        public CollisionLocation CheckTerrainCollision(Map map, out Tile tile)
        {
            Texture2D mapTexture = map.mapTexture;


            //Gets all the tile indexes of the tiles surrounding the entity.
            int[] q = new int[12];
            q[0] = TileIndex - mapTexture.Width - 1;
            q[1] = TileIndex - mapTexture.Width;
            q[2] = TileIndex - mapTexture.Width + 1;
            q[3] = TileIndex - 1;
            q[4] = TileIndex;
            q[5] = TileIndex + 1;
            q[6] = TileIndex + mapTexture.Width - 1;
            q[7] = TileIndex + mapTexture.Width;
            q[8] = TileIndex + mapTexture.Width + 1;
            q[9] = TileIndex + mapTexture.Width + mapTexture.Width - 1;
            q[10] = TileIndex + mapTexture.Width + mapTexture.Width;
            q[11] = TileIndex + mapTexture.Width + mapTexture.Width + 1;

            //check the tiles around the entity for collision
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < map.tileArray.Length)
                {
                    tile = map.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant <= map.tileArray.Length - 1 && map.tileArray[quadrant].isSolid == true)
                    {
                        if (yRect.Intersects(map.tileArray[quadrant].rectangle))
                        {
                            if (position.Y < map.tileArray[quadrant].rectangle.Y) //hits bot
                            {
                                return CollisionLocation.Bottom;
                            }
                            else  //hits top
                            {
                                return CollisionLocation.Top;
                            }
                        }
                        else if (xRect.Intersects(map.tileArray[quadrant].rectangle))
                        {
                            if (position.X < map.tileArray[quadrant].rectangle.X) //hits right
                            {
                                return CollisionLocation.Right;
                            }
                            else //hits left
                            {
                                return CollisionLocation.Left;
                            }
                        }
                    }
                }
            }
            tile = new Tile();
            return CollisionLocation.Null;
        }

        public CollisionLocation CheckCollisionWithOtherEntity(Entity entity)
        {
            if (yRect.Intersects(entity.collRectangle))
            {
                if (collRectangle.Y < entity.collRectangle.Y) //hits bot
                {
                    return CollisionLocation.Bottom;
                }
                else  //hits top
                {
                    return CollisionLocation.Top;
                }
            }
            else if (xRect.Intersects(entity.collRectangle))
            {
                if (collRectangle.X < entity.collRectangle.X) //hits right
                {
                    return CollisionLocation.Right;
                }
                else  //hits left
                {
                    return CollisionLocation.Left;
                }
            }
            else return CollisionLocation.Null;
        }

        public bool IsTouchingTerrain(Map map)
        {
            int[] q = GetNearbyTileIndexes(map);

            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= map.tileArray.Length - 1 && map.tileArray[quadrant].isSolid == true)
                {
                    if (collRectangle.Intersects(map.tileArray[quadrant].rectangle))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the tile index and returns it.
        /// </summary>
        /// 
        /// <param name="map"> The map so that the size of the world can be retrieved.</param>
        /// <returns></returns>
        public int GetTileIndex(Map map)
        {
            TileIndex = (int)(collRectangle.Y / Game1.Tilesize * map.mapTexture.Width) + (int)(collRectangle.X / Game1.Tilesize);
            return TileIndex;
        }

        /// <summary>
        /// Returns all of the tile indexes of the tiles surrounding the entity.
        /// </summary>
        /// 
        /// <param name="map">The map the entity is in.</param>
        /// <returns></returns>
        public int[] GetNearbyTileIndexes(Map map)
        {
            TileIndex = GetTileIndex(map);
            Size size = Size.GetSize(collRectangle);
            int width = map.mapTexture.Width;
            int[] q = new int[0];

            if (size == new Size(1, 1) || size == new Size(0, 0))
            {
                q = new int[]
                {
                    TileIndex - width - 1,
                    TileIndex - width,
                    TileIndex - width + 1,
                    TileIndex - 1,
                    TileIndex,
                    TileIndex + 1,
                    TileIndex + width - 1,
                    TileIndex + width,
                    TileIndex + width + 1,
                };
            }
            else
            {
                q = new int[12];
                q[0] = TileIndex - width - 1;
                q[1] = TileIndex - width;
                q[2] = TileIndex - width + 1;
                q[3] = TileIndex - 1;
                q[4] = TileIndex;
                q[5] = TileIndex + 1;
                q[6] = TileIndex + width - 1;
                q[7] = TileIndex + width;
                q[8] = TileIndex + width + 1;
                q[9] = TileIndex + width + width - 1;
                q[10] = TileIndex + width + width;
                q[11] = TileIndex + width + width + 1;
            }

            return q;
        }

        /// <summary>
        /// Call this method if the entity needs collision check with terrain. The method will check collisions and raise events.
        /// </summary>
        /// <param name="map">The map the entity is in.</param>
        public void CheckTerrainCollision(Map map)
        {
            int[] q = GetNearbyTileIndexes(map);

            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < map.tileArray.Length)
                {
                    Tile tile = map.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant <= map.tileArray.Length - 1 && tile.isSolid == true)
                    {
                        if (yRect.Intersects(map.tileArray[quadrant].rectangle))
                        {
                            if (position.Y < map.tileArray[quadrant].rectangle.Y) //hits bot
                            {
                                CollidedWithTerrainBelow(new TerrainCollisionEventArgs(tile));
                            }
                            else  //hits top
                            {
                                CollidedWithTerrainAbove(new TerrainCollisionEventArgs(tile));
                            }
                        }
                        else if (xRect.Intersects(tile.rectangle))
                        {
                            if (position.X < map.tileArray[quadrant].rectangle.X) //hits right
                            {
                                CollidedWithTerrainRight(new TerrainCollisionEventArgs(tile));
                            }
                            else //hits left
                            {
                                CollidedWithTerrainLeft(new TerrainCollisionEventArgs(tile));
                            }
                        }
                    }
                }
            }
        }

        public void CheckSimpleTerrainCollision(Map map)
        {
            int[] q = GetNearbyTileIndexes(map);

            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= map.tileArray.Length - 1 && map.tileArray[quadrant].isSolid == true)
                {
                    if (collRectangle.Intersects(map.tileArray[quadrant].rectangle))
                        CollidedWithTerrainAnywhere(new TerrainCollisionEventArgs(map.tileArray[quadrant]));
                }
            }
        }


        /// <summary>
        /// This will return the volume that the listener should be hearing from the source.
        /// </summary>
        /// 
        /// <param name="listener">Who the listener is.</param>
        /// <returns>Volume of sound.</returns>
        public float GetSoundVolume(Entity listener)
        {
            float xDist = listener.collRectangle.Center.X - drawRectangle.Center.X;
            float yDist = listener.collRectangle.Center.Y - drawRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);

            if (distanceTo > 1000)
                return 0;
            else return .5f - (distanceTo / 1000) / 2;
        }

    }



    class TerrainCollisionEventArgs : EventArgs
    {
        Tile tile;
        public TerrainCollisionEventArgs(Tile tile)
        {
            this.tile = tile;
        }

        public Tile Tile { get { return tile; } }
    }

    class EntityCollisionEventArgs : EventArgs
    {

    }

    struct Size
    {
        byte X { get; set; }
        byte Y { get; set; }

        /// <summary>
        /// Variable that stores the dimensions of a rectangle in terms of tiles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Size(byte x, byte y)
        {
            this = new Size();
            X = x;
            Y = y;
        }

        /// <summary>
        /// Takes a rectangle and calculates how many tiles wide and tall it is.
        /// </summary>
        /// <param name="rectangle">Rectangle to be examined.</param>
        /// <returns>Size variable with X and Y components.</returns>
        public static Size GetSize(Rectangle rectangle)
        {
            byte x = (byte)(rectangle.X / Game1.Tilesize);
            byte y = (byte)(rectangle.Y / Game1.Tilesize);

            return new Size(x, y);
        }

        public static bool operator ==(Size size1, Size size2)
        {
            if (size1.X == size2.X && size1.Y == size2.Y)
                return true;
            else return false;
        }
        public static bool operator !=(Size size1, Size size2)
        {
            if (size1.X != size2.X || size1.Y != size2.Y)
                return true;
            else return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if ((Size)obj == this)
                return base.Equals(obj);
            else return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new NotImplementedException();
            //return base.GetHashCode();
        }
    }
}
