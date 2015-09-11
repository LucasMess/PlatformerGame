using Adam;
using Adam.Lights;
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
    /// <summary>
    /// Basic class that allows for collision, animation and physics.
    /// </summary>
    public abstract class Entity
    {
        protected Vector2 origin;
        protected Vector2 velocity;

        private Texture2D _texture;
        private Color _color;

        private bool _toDelete;
        private bool _isFacingRight;
        private bool _isDead;

        private DynamicPointLight _light;

        private int _health;
        private float _opacity = 1f;

        /// <summary>
        /// The index position of the entity in the game world.
        /// </summary>
        public int TileIndex
        {
            get { return GetTileIndex(); }
        }


        /// <summary>
        /// The texture of the entity.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                if (_texture == null)
                {
                    _texture = Main.DefaultTexture;
                    Console.WriteLine("Texture for: {0} is null, using default texture instead.", GetType());
                }
                return _texture;
            }
            set { _texture = value; }
        }

        /// <summary>
        /// The rectangle where the texture is drawn at.
        /// </summary>
        protected abstract Rectangle DrawRectangle
        {
            get;
        }


        /// <summary>
        /// The rectangle where position and collisions are done in.
        /// </summary>
        protected Rectangle collRectangle;

        /// <summary>
        /// Returns the Collision Rectangle of the entity.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCollRectangle()
        {
            return collRectangle;
        }

        /// <summary>
        /// The opacity of the object.
        /// </summary>
        public float Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }


        /// <summary>
        /// The color that the entity will be drawn in.
        /// </summary>
        public virtual Color Color
        {
            get
            {
                if (_color == null)
                    _color = Color.White;
                return _color * Opacity;
            }
            set
            {
                _color = value;
            }
        }

        /// <summary>
        /// The rectangle that specifies the part of the texture to draw.
        /// </summary>
        protected Rectangle sourceRectangle;

        /// <summary>
        /// The light that the entity gives off. Default is none.
        /// </summary>
        public DynamicPointLight Light
        {
            get
            {
                return _light;
            }

            set
            {
                _light = value;
            }
        }

        /// <summary>
        /// Returns true if the entity's health is equal to or below zero.
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            // If entity is not one that has health, it is not always dead.
            if (MaxHealth == 0)
                return false;
            return (Health <= 0);
        }

        /// <summary>
        /// The maximum amount of health the enemy can have. This is the value that is given to the enemy when it respawns.
        /// </summary>
        public virtual int MaxHealth
        {
            get;
        }

        int health;
        bool healthGiven;
        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public int Health
        {
            get
            {
                if (!healthGiven)
                {
                    health = MaxHealth;
                    healthGiven = true;
                }

                return health;
            }
            set
            {
                health = value;
            }
        }

        /// <summary>
        /// Returns true if the entity's texture is facing right.
        /// </summary>
        public bool IsFacingRight
        {
            get
            {
                return _isFacingRight;
            }

            set
            {
                _isFacingRight = value;
            }
        }

        /// <summary>
        /// Determines whether the entity can be deleted or not.
        /// </summary>
        public bool ToDelete
        {
            get
            {
                return _toDelete;
            }

            set
            {
                _toDelete = value;
            }
        }

        /// <summary>
        /// Base update that provides basic logic.
        /// </summary>
        public virtual void Update()
        {
            if (IsDead())
                return;

            //Check for physics, if applicable.
            if (this is INewtonian)
            {
                ApplyGravity();
            }

            //Check for collision, if applicable.
            if (this is ICollidable)
            {
                CheckTerrainCollision();
            }

            //Animate entity if applicable.
            if (this is IAnimated)
            {
                IAnimated ian = (IAnimated)this;
                if (ian.Animation == null || ian.AnimationData == null)
                {
                    throw new NullReferenceException("No animation data or animation found for object:" + GetType());
                }
                ian.Animate();
            }

        }

        /// <summary>
        /// Basic draw logic for simple objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Debugging tools
            //spriteBatch.Draw(Main.DefaultTexture, collRectangle, Color.Red);


            // If the entity has an animation.
            if (this is IAnimated)
            {
                IAnimated ian = (IAnimated)this;

                ian.Animation.Color = Color;

                //Flip sprite if facing other way.
                if (IsFacingRight)
                {
                    ian.Animation.isFlipped = true;
                }
                else ian.Animation.isFlipped = false;

                ian.Animation.Draw(spriteBatch);
            }

            // Drawing for simple entities that have a texture contained in a spritesheet.
            else if (sourceRectangle != null)
            {
                if (!IsFacingRight)
                    spriteBatch.Draw(Texture, DrawRectangle, sourceRectangle, Color * Opacity, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                else spriteBatch.Draw(Texture, DrawRectangle, sourceRectangle, Color * Opacity, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }

            // Most basic drawing when there is only one frame and it is not in a spritesheet.
            else
            {
                spriteBatch.Draw(Texture, DrawRectangle, Color * Opacity);
            }
        }

        /// <summary>
        /// Special draw method that will draw the object with its center as the main coordinate.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawFromCenter(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, null, Color.White, 0, origin, SpriteEffects.None, 0);
        }

        public Vector2 GetVelocity()
        {
            return velocity;
        }

        public virtual void Kill() { 
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
            if (GameWorld.Instance != null)
                return (int)(collRectangle.Center.Y / Main.Tilesize * GameWorld.Instance.worldData.LevelWidth) + (int)(collRectangle.Center.X / Main.Tilesize);
            else return 0;
        }

        /// <summary>
        /// Gets tile index at specified coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public int GetTileIndex(Vector2 coord)
        {
            if (GameWorld.Instance != null)
                return (int)((int)coord.Y / Main.Tilesize * GameWorld.Instance.worldData.LevelWidth) + (int)((int)coord.X / Main.Tilesize);
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
            return indexes.ToArray();
        }

        /// <summary>
        /// This will check for terrain collision if entity implements ICollidable.
        /// </summary>
        private void CheckTerrainCollision()
        {
            if (this is ICollidable) { } else throw new InvalidOperationException("The object: " + this.GetType().ToString() + " checked for collisions with terrain but it does not implement ICollidable.");

            ICollidable ent = (ICollidable)this;

            int[] q = GetNearbyTileIndexes(GameWorld.Instance);

            //Solve Y collisions
            collRectangle.Y += (int)velocity.Y;
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < GameWorld.Instance.tileArray.Length)
                {
                    Tile tile = GameWorld.Instance.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant < GameWorld.Instance.tileArray.Length && tile.isSolid == true)
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
                if (quadrant >= 0 && quadrant < GameWorld.Instance.tileArray.Length)
                {
                    Tile tile = GameWorld.Instance.tileArray[quadrant];
                    if (quadrant >= 0 && quadrant < GameWorld.Instance.tileArray.Length && tile.isSolid == true)
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
            float xDist = listener.collRectangle.Center.X - DrawRectangle.Center.X;
            float yDist = listener.collRectangle.Center.Y - DrawRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);

            if (distanceTo > 1000)
                return 0;
            else return (1 - (distanceTo / 1000)) * Main.MaxVolume;
        }

        public void DrawSurroundIndexes(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance == null) return;
            foreach (int i in GetNearbyTileIndexes(GameWorld.Instance))
            {
                if (i < GameWorld.Instance.tileArray.Length && i >= 0)
                {
                    Tile t = GameWorld.Instance.tileArray[i];
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

            velocity.Y += gravity;
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

        /// <summary>
        /// Returns the current instance of this entity.
        /// </summary>
        /// <returns></returns>
        public Entity Get()
        {
            return this;
        }

        /// <summary>
        /// Sets the health back to the entity's original health.
        /// </summary>
        public virtual void Revive()
        {
            Health = MaxHealth;
        }
    }
}
