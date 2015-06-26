using Adam;
using Adam.Misc.Errors;
using Adam.Misc.Interfaces;
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

        protected Texture2D texture = Game1.DefaultTexture;
        public Vector2 position;
        protected Vector2 origin;
        public Rectangle drawRectangle;
        public Rectangle collRectangle;
        protected Rectangle sourceRectangle;
        protected Animation animation;
        protected GameWorld gameWorld;
        public bool toDelete;
        public Vector2 velocity;

        public Rectangle yRect, xRect;

        public int TileIndex
        {
            get { return GetTileIndex(); }
        }

        protected ContentManager Content;

        protected float opacity = 1f;

        /// <summary>
        /// All things that move or can collide with other things inherit the Entity class.
        /// </summary>
        public Entity()
        {
            Content = Game1.Content;
        }

        /// <summary>
        /// The opacity of the object.
        /// </summary>
        public float Opacity
        {
            get { return opacity; }
            set { value = opacity; }
        }

        /// <summary>
        /// Base update that provides basic logic.
        /// </summary>
        public virtual void Update()
        {
            if (this is ICollidable)
            {
                CheckTerrainCollision();
            }
            if (this is INewtonian)
            {
                ApplyGravity();
            }
        }

        /// <summary>
        /// Basic draw logic for simple objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, Color.White * opacity);
        }

        /// <summary>
        /// Special draw method that will draw the object with its center as the main coordinate.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawFromCenter(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, null, Color.White, 0, origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Convert a Vector2 into Rectangle position to avoid having to write "(int)" every time.
        /// </summary>
        /// <param name="position"></param>
        public void SetRectPos(Vector2 position)
        {
            drawRectangle.X = (int)position.X;
            drawRectangle.Y = (int)position.Y;
        }

        /// <summary>
        /// Put the Rectangle coordinates at the origin so that the sprite is drawn as if it had no origin.
        /// </summary>
        /// <param name="position"></param>
        public void SetRectPosAtOrigin(Vector2 position)
        {
            drawRectangle.X = (int)(position.X + origin.X);
            drawRectangle.Y = (int)(position.Y + origin.Y);
        }

        /// <summary>
        /// If the entity is colliding with the terrain, it will return the collision location and the tile that it is colliding with.
        /// </summary>
        /// <param name="map">The map the entity is in.</param>
        /// <param name="tile">Tile that entity collided with.</param>
        /// <returns>The location of the collision.</returns>
        public CollisionLocation CheckTerrainCollision(GameWorld map, out Tile tile)
        {
            Texture2D mapTexture = map.worldData.mainMap;

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
            //If no collision was detected return null collision.
            return CollisionLocation.Null;
        }

        /// <summary>
        /// Checks for collision with other entity and returns the location of said collision.
        /// </summary>
        /// <param name="entity">The entity that collision will be checked on.</param>
        /// <returns>The location of the collision.</returns>
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

        /// <summary>
        /// Whether the entity is simply intersecting terrain.
        /// </summary>
        /// <param name="map">Map the entity is in.</param>
        /// <returns></returns>
        public bool IsTouchingTerrain(GameWorld map)
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
        /// Simple collision detection for entities.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsTouchingEntity(Entity entity)
        {
            return entity.collRectangle.Intersects(this.collRectangle);
        }

        /// <summary>
        /// Gets the entity' tile index. The tile index is used to determine an entity's position in the map.
        /// </summary>
        /// 
        /// <returns></returns>
        public int GetTileIndex()
        {
            if (gameWorld != null)
                return (int)(collRectangle.Center.Y / Game1.Tilesize * gameWorld.worldData.mainMap.Width) + (int)(collRectangle.Center.X / Game1.Tilesize);
            else return 0;
        }

        /// <summary>
        /// Gets tile index at specified coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public int GetTileIndex(Vector2 coord)
        {
            if (gameWorld != null)
                return (int)((int)coord.Y / Game1.Tilesize * gameWorld.worldData.mainMap.Width) + (int)((int)coord.X / Game1.Tilesize);
            else throw new Exception("Map is null");
        }

        /// <summary>
        /// Returns all of the tile indexes of the tiles surrounding the entity.
        /// </summary>
        /// <param name="map">The map the entity is in.</param>
        /// <returns></returns>
        public int[] GetNearbyTileIndexes(GameWorld map)
        {
            int width = map.worldData.mainMap.Width;
            int startingIndex = GetTileIndex(new Vector2(collRectangle.X, collRectangle.Y)) - width - 1;
            int heightInTiles = (collRectangle.Height / Game1.Tilesize) + 2;
            int widthInTiles = (collRectangle.Width / Game1.Tilesize) + 2;

            List<int> indexes = new List<int>();
            for (int h = 0; h < heightInTiles; h++)
            {
                for (int w = 0; w < widthInTiles; w++)
                {
                    int i = startingIndex + (h * width) + w;
                    indexes.Add(i);
                }
            }
            return indexes.ToArray();
        }

        /// <summary>
        /// This will check for terrain collision if entity implements ICollidable.
        /// </summary>
        private void CheckTerrainCollision()
        {
            if (this is ICollidable) { } else throw new Exception("The object: " + this.GetType().ToString() + " checked for collisions with terrain but it does not implement ICollidable.");

            ICollidable ent = (ICollidable)this;

            int[] q = GetNearbyTileIndexes(gameWorld);

            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < gameWorld.tileArray.Length)
                {
                    Tile tile = gameWorld.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant < gameWorld.tileArray.Length && tile.isSolid == true)
                    {
                        if (collRectangle.Intersects(tile.rectangle))
                        {
                            if (yRect.Intersects(gameWorld.tileArray[quadrant].rectangle))
                            {
                                if (position.Y < gameWorld.tileArray[quadrant].rectangle.Y) //hits bot
                                {
                                    ent.OnCollisionWithTerrainBelow(new TerrainCollisionEventArgs(tile));
                                }
                                else  //hits top
                                {
                                    ent.OnCollisionWithTerrainAbove(new TerrainCollisionEventArgs(tile));
                                }
                            }
                            else if (xRect.Intersects(tile.rectangle))
                            {
                                if (position.X < gameWorld.tileArray[quadrant].rectangle.X) //hits right
                                {
                                    ent.OnCollisionWithTerrainRight(new TerrainCollisionEventArgs(tile));
                                }
                                else //hits left
                                {
                                    ent.OnCollisionWithTerrainLeft(new TerrainCollisionEventArgs(tile));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the entity is simply colliding with terrain anywhere, it will raise an event.
        /// </summary>
        /// <param name="map">The map the entity is in.</param>
        public void CheckSimpleTerrainCollision(GameWorld map)
        {
            int[] q = GetNearbyTileIndexes(map);

            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= map.tileArray.Length - 1 && map.tileArray[quadrant].isSolid == true)
                {
                    if (collRectangle.Intersects(map.tileArray[quadrant].rectangle)) { }
                    //CollidedWithTerrainAnywhere(new TerrainCollisionEventArgs(map.tileArray[quadrant]));
                }
            }
        }


        /// <summary>
        /// This will return the volume that the listener should be hearing from the source.
        /// </summary>
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

        public void DrawSurroundIndexes(SpriteBatch spriteBatch)
        {
            if (gameWorld == null) return;
            foreach (int i in GetNearbyTileIndexes(gameWorld))
            {
                if (i < gameWorld.tileArray.Length && i >= 0)
                {
                    Tile t = gameWorld.tileArray[i];
                    spriteBatch.Draw(Game1.DefaultTexture, t.rectangle, t.sourceRectangle, Color.Red);
                }
            }
        }


        /// <summary>
        /// Makes objects fall.
        /// </summary>
        private void ApplyGravity()
        {
            if (this is INewtonian) { } else throw new Exception("This object is not affected by gravity because it does not implement INewtonian.");
            INewtonian newt = (INewtonian)this;

            //Checks to see if there is a block below the player, if there is, no gravity is applied to prevent the jittery bug.
            float gravity = newt.GravityStrength;
            int indexBelowEntity = GetTileIndex(new Vector2(collRectangle.Center.X, collRectangle.Y)) + (gameWorld.worldData.mainMap.Width * (collRectangle.Height / Game1.Tilesize));
            if (indexBelowEntity >= 0 && indexBelowEntity < gameWorld.tileArray.Length)
                if (!gameWorld.tileArray[indexBelowEntity].isSolid)
                {
                    newt.IsAboveTile = false;
                }
                else newt.IsAboveTile = true;

            if (!newt.IsAboveTile || newt.IsJumping)
            {
                velocity.Y += gravity;
            }
        }
    }
}
