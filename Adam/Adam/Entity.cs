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
    public enum CollisionLocation
    {
        Bottom,
        Right,
        Left,
        Top,
        Null,
    }

    public class Entity
    {

        public Texture2D texture = Main.DefaultTexture;
        protected Vector2 origin;
        public Rectangle drawRectangle;
        public Rectangle collRectangle;
        public Rectangle sourceRectangle;
        protected Animation animation;
        protected GameWorld gameWorld;
        public bool toDelete;
        public Vector2 velocity;
        public bool isFacingRight;
        public bool isDead;

        public Rectangle yRect, xRect;

        public int TileIndex
        {
            get { return GetTileIndex(); }
        }

        protected ContentManager Content;

        protected float opacity = 1f;

        /// <summary>
        /// All things that move or can collide with other things inherit the Entity public class.
        /// </summary>
        public Entity()
        {
            Content = Main.Content;
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
            gameWorld = GameWorld.Instance;

            if (this is INewtonian)
            {
                ApplyGravity();
            }


            if (this is ICollidable)
            {
                UpdateXYRects();
                CheckTerrainCollision();
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


        protected void UpdateXYRects()
        {
            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 10, collRectangle.Width, collRectangle.Height - 20);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, collRectangle.Width - 20, collRectangle.Height);
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
                    if (collRectangle.Intersects(map.tileArray[quadrant].drawRectangle))
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
                return (int)(collRectangle.Center.Y / Main.Tilesize * gameWorld.worldData.LevelWidth) + (int)(collRectangle.Center.X / Main.Tilesize);
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
                return (int)((int)coord.Y / Main.Tilesize * gameWorld.worldData.LevelWidth) + (int)((int)coord.X / Main.Tilesize);
            else throw new Exception("Map is null");
        }

        /// <summary>
        /// Returns all of the tile indexes of the tiles surrounding the entity.
        /// </summary>
        /// <param name="map">The map the entity is in.</param>
        /// <returns></returns>
        public int[] GetNearbyTileIndexes(GameWorld map)
        {
            int width = map.worldData.LevelWidth;
            int startingIndex = GetTileIndex(new Vector2(collRectangle.Center.X, collRectangle.Y)) - width - 1;
            int heightInTiles = (int)(Math.Ceiling((double)collRectangle.Height / Main.Tilesize) + 2);
            int widthInTiles = (int)(Math.Ceiling((double)collRectangle.Width / Main.Tilesize) + 2);

            List<int> indexes = new List<int>();
            for (int h = 0; h < heightInTiles; h++)
            {
                for (int w = 0; w < widthInTiles; w++)
                {
                    int i = startingIndex + (h * width) + w;
                    indexes.Add(i);
                }
            }

            ////Remove corner indexes
            //int lastItem = indexes.Count - 1;
            //int[] copy = indexes.ToArray();
            //indexes.Remove(copy[lastItem]);
            //indexes.Remove(copy[lastItem - widthInTiles + 1]);
            //indexes.Remove(copy[widthInTiles - 1]);
            //indexes.Remove(copy[0]);

            return indexes.ToArray();
        }

        /// <summary>
        /// This will check for terrain collision if entity implements ICollidable.
        /// </summary>
        private void CheckTerrainCollision()
        {
            if (isDead) return;
            if (this is ICollidable) { } else throw new InvalidOperationException("The object: " + this.GetType().ToString() + " checked for collisions with terrain but it does not implement ICollidable.");

            ICollidable ent = (ICollidable)this;

            int[] q = GetNearbyTileIndexes(gameWorld);

            //Solve Y collisions
            collRectangle.Y += (int)velocity.Y;
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < gameWorld.tileArray.Length)
                {
                    Tile tile = gameWorld.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant < gameWorld.tileArray.Length && tile.isSolid == true)
                    {
                        Rectangle tileRect = tile.drawRectangle;
                        if (collRectangle.Intersects(tileRect))
                        {
                            if (velocity.Y > 0)
                            {
                                while (collRectangle.Intersects(tileRect))
                                {
                                    collRectangle.Y--;
                                }
                                ent.OnCollisionWithTerrainBelow(new TerrainCollisionEventArgs(tile));
                                ent.OnCollisionWithTerrainAnywhere(new TerrainCollisionEventArgs(tile));
                            }
                            else if (velocity.Y < 0)
                            {
                                while (collRectangle.Intersects(tileRect))
                                {
                                    collRectangle.Y++;
                                }
                                ent.OnCollisionWithTerrainAbove(new TerrainCollisionEventArgs(tile));
                                ent.OnCollisionWithTerrainAnywhere(new TerrainCollisionEventArgs(tile));
                            }
                        }
                    }
                }
            }

            //Solve X Collisions
            collRectangle.X += (int)velocity.X;
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < gameWorld.tileArray.Length)
                {
                    Tile tile = gameWorld.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant < gameWorld.tileArray.Length && tile.isSolid == true)
                    {
                        Rectangle tileRect = tile.drawRectangle;
                        if (collRectangle.Intersects(tile.drawRectangle))
                        {
                            if (velocity.X > 0)
                            {
                                while (collRectangle.Intersects(tileRect))
                                {
                                    collRectangle.X--;
                                }
                                ent.OnCollisionWithTerrainRight(new TerrainCollisionEventArgs(tile));
                                ent.OnCollisionWithTerrainAnywhere(new TerrainCollisionEventArgs(tile));
                            }
                            else if (velocity.X < 0)
                            {
                                while (collRectangle.Intersects(tileRect))
                                {
                                    collRectangle.X++;
                                    UpdateXYRects();
                                }
                                ent.OnCollisionWithTerrainLeft(new TerrainCollisionEventArgs(tile));
                                ent.OnCollisionWithTerrainAnywhere(new TerrainCollisionEventArgs(tile));
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
                    if (collRectangle.Intersects(map.tileArray[quadrant].drawRectangle)) { }
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
                    spriteBatch.Draw(Main.DefaultTexture, t.drawRectangle, t.sourceRectangle, Color.Red);
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
            int indexAboveOrBelowEntity;

            if (gravity > 0)
            {
                if (isFacingRight)
                {
                    indexAboveOrBelowEntity = GetTileIndex(new Vector2(collRectangle.X, collRectangle.Bottom - Main.Tilesize)) + gameWorld.worldData.LevelWidth;
                }
                else
                {
                    indexAboveOrBelowEntity = GetTileIndex(new Vector2(collRectangle.Right, collRectangle.Bottom - 1)) + gameWorld.worldData.LevelWidth;
                }

            }
            else
            {
                if (isFacingRight)
                {
                    indexAboveOrBelowEntity = GetTileIndex(new Vector2(collRectangle.X, collRectangle.Top - Main.Tilesize)) + gameWorld.worldData.LevelWidth;
                }
                else
                {
                    indexAboveOrBelowEntity = GetTileIndex(new Vector2(collRectangle.Right, collRectangle.Top - 1)) + gameWorld.worldData.LevelWidth;
                }
            }

            if (indexAboveOrBelowEntity >= 0 && indexAboveOrBelowEntity < gameWorld.tileArray.Length)
                if (!gameWorld.tileArray[indexAboveOrBelowEntity].isSolid)
                {
                    newt.IsAboveTile = false;
                }
                else newt.IsAboveTile = true;

            if (!newt.IsAboveTile || newt.IsJumping)
            {
                velocity.Y += gravity;
            }
        }

        protected virtual void OnCollisionAbove(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.drawRectangle.Y + e.Tile.drawRectangle.Height;
            velocity.Y = 0;
        }
        protected virtual void OnCollisionBelow(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.drawRectangle.Y - collRectangle.Height;
            velocity.Y = 0;
        }
        protected virtual void OnCollisionRight(TerrainCollisionEventArgs e)
        {
            collRectangle.X = e.Tile.drawRectangle.X - collRectangle.Width;
            velocity.X = 0;
        }
        protected virtual void OnCollisionLeft(TerrainCollisionEventArgs e)
        {
            collRectangle.X = e.Tile.drawRectangle.X + e.Tile.drawRectangle.Width;
            velocity.X = 0;
        }

        public Entity GetUpdated()
        {
            return this;
        }

    }
}
