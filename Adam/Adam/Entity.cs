﻿using Adam;
using Adam.Lights;
using Adam.Misc.Errors;
using Adam.Misc.Interfaces;
using Adam.Misc.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    //Defines what happens to the velocity of the entity when it collides.
    public enum CollisionType
    {
        Rigid,
        Bouncy,
        SuperBouncy,
    }

    /// <summary>
    /// Basic class that allows for collision, animation and physics.
    /// </summary>
    public abstract partial class Entity
    {
        public delegate void EventHandler();
        public delegate void TileHandler(Entity entity, Tile tile);
        public delegate void Entityhandler(Entity entity);

        const float FrictionConstant = 95f / 90f;

        // Collision with terrain events.
        public event TileHandler CollidedWithTileAbove;
        public event TileHandler CollidedWithTileBelow;
        public event TileHandler CollidedWithTileToRight;
        public event TileHandler CollidedWithTileToLeft;
        public event TileHandler CollidedWithTerrain;

        // Collision with entity events.
        public event Entityhandler CollidedWithEntity;
        public event Entityhandler CollidedWithEntityAbove;
        public event Entityhandler CollidedWithEntityBelow;
        public event Entityhandler CollidedWithEntityToRight;
        public event Entityhandler CollidedWithEntityToLeft;

        protected Vector2 origin;
        protected Vector2 velocity;

        private Texture2D _texture;
        private Color _color = Color.White;

        private bool _toDelete;
        private bool _isFacingRight;
        private bool _isDead;
        private bool _healthGiven;

        private DynamicPointLight _light;

        private int _health;
        private float _opacity = 1f;
        private float _gravityStrength = Main.Gravity;

        /// <summary>
        /// Subscribes to events and initializes other variables.
        /// </summary>
        public Entity()
        {
            Sounds = new SoundFxManager(this);

            // Subscribes to default collision handling.
            CollidedWithTileAbove += OnCollisionWithTileAbove;
            CollidedWithTileBelow += OnCollisionWithTileBelow;
            CollidedWithTileToLeft += OnCollisionWithTileToLeft;
            CollidedWithTileToRight += OnCollisionWithTileToRight;
            CollidedWithTerrain += OnCollisionWithTerrain;
        }

        /// <summary>
        /// The index position of the entity in the game world.
        /// </summary>
        public int TileIndex
        {
            get { return GetTileIndex(); }
        }

        /// <summary>
        /// Defines what happens to the velocity of the entity when it collides with something.
        /// </summary>
        protected CollisionType CurrentCollisionType { get; set; } = CollisionType.Rigid;

        /// <summary>
        /// How much gravity is applied to the entity
        /// </summary>
        public float GravityStrength
        {
            get
            {
                if (!ObeysGravity)
                {
                    return 0;
                }
                else return _gravityStrength;
            }
            set
            {
                _gravityStrength = value;
            }
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
                if (HasTakenDamageRecently())
                    return Color.Red;
                else
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

        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public int Health
        {
            get
            {
                if (!_healthGiven)
                {
                    _health = MaxHealth;
                    _healthGiven = true;
                }

                return _health;
            }
            set
            {
                _health = value;
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
            if (ObeysGravity)
            {
                ApplyGravity();
            }

            //Check for collision, if applicable.
            if (IsCollidable)
            {
                CheckTerrainCollision();

                // y = (499/45) * (x / (x + 1)
                float friction = FrictionConstant * ((float)Weight/((float)Weight +1));
                velocity *= friction;
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

            // Update complex animations if this entity has it.
            complexAnim?.Update(this);

        }

        /// <summary>
        /// Basic draw logic for simple objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            hitRecentlyTimer.Increment();

            // Debugging tools
            //spriteBatch.Draw(Main.DefaultTexture, collRectangle, Color.Red);

            // Complex animations.
            if (complexAnim != null)
            {
                complexAnim.Draw(spriteBatch, IsFacingRight, Color);
                return;
            }


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

        public void SetVelX(float x)
        {
            velocity.X = x;
        }

        public void SetVelY(float y)
        {
            velocity.Y = y;
        }

        public void ChangePosBy(int x, int y)
        {
            collRectangle.X += x;
            collRectangle.Y += y;
        }

        public virtual void Kill()
        {
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
                                CollidedWithTileBelow(this, tile);
                                CollidedWithTerrain(this, tile);
                            }
                            else if (velocity.Y < 0)
                            {
                                while (collRectangle.Intersects(tileRect))
                                {
                                    collRectangle.Y++;
                                }
                                CollidedWithTileAbove(this, tile);
                                CollidedWithTerrain(this, tile);
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
                                CollidedWithTileToRight(this, tile);
                                CollidedWithTerrain(this, tile);
                            }
                            else if (velocity.X < 0)
                            {
                                while (collRectangle.Intersects(tileRect))
                                {
                                    collRectangle.X++;
                                }
                                CollidedWithTileToLeft(this, tile);
                                CollidedWithTerrain(this, tile);
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

            foreach (int index in q)
            {
                if (index >= 0 && index <= map.tileArray.Length - 1 && map.tileArray[index].isSolid == true)
                {
                    if (collRectangle.Intersects(map.tileArray[index].drawRectangle))
                    {
                        CollidedWithTerrain(this, map.tileArray[index]);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds an animation to the list of animations that are to be played.
        /// </summary>
        /// <param name="name"></param>
        public void AddAnimationToQueue(string name)
        {
            complexAnim.AddToQueue(name);
        }

        /// <summary>
        /// Removes an animation from the lsit of aniamtions that are to be played.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAnimationFromQueue(string name)
        {
            complexAnim.RemoveFromQueue(name);
        }

        /// <summary>
        /// This will return the volume that the listener should be hearing from the source.
        /// </summary>
        /// <param name="listener">Who the listener is.</param>
        /// <returns>Volume of sound.</returns>
        public float GetSoundVolume(Entity listener, float maxVolume)
        {
            listener = listener.Get();
            float xDist = listener.collRectangle.Center.X - collRectangle.Center.X;
            float yDist = listener.collRectangle.Center.Y - collRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);

            if (distanceTo < 64)
                return maxVolume;
            else return (float)(1 / Math.Sqrt(distanceTo)) * maxVolume;
        }

        /// <summary>
        /// Highlights the surrounding tiles and draws them in a different color for debugging.
        /// </summary>
        /// <param name="spriteBatch"></param>
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
            velocity.Y += GravityStrength;
        }

        private void OnCollisionWithTileAbove(Entity entity, Tile tile)
        {
            collRectangle.Y = tile.drawRectangle.Y + tile.drawRectangle.Height;
            switch (CurrentCollisionType)
            {
                case CollisionType.Rigid:
                    velocity.Y = 0;
                    break;
                case CollisionType.Bouncy:
                    velocity.Y = -velocity.Y;
                    break;
            }
        }
        private void OnCollisionWithTileBelow(Entity entity, Tile tile)
        {
            collRectangle.Y = tile.drawRectangle.Y - collRectangle.Height;
            switch (CurrentCollisionType)
            {
                case CollisionType.Rigid:
                    velocity.Y = 0;
                    break;
                case CollisionType.Bouncy:
                    velocity.Y = -velocity.Y;
                    break;
            }
        }
        private void OnCollisionWithTileToRight(Entity entity, Tile tile)
        {
            collRectangle.X = tile.drawRectangle.X - collRectangle.Width;
            switch (CurrentCollisionType)
            {
                case CollisionType.Rigid:
                    velocity.X = 0;
                    break;
                case CollisionType.Bouncy:
                    velocity.X = -velocity.X;
                    break;
            }

        }
        private void OnCollisionWithTileToLeft(Entity entity, Tile tile)
        {
            collRectangle.X = tile.drawRectangle.X + tile.drawRectangle.Width;
            switch (CurrentCollisionType)
            {
                case CollisionType.Rigid:
                    velocity.X = 0;
                    break;
                case CollisionType.Bouncy:
                    velocity.X = -velocity.X;
                    break;
            }

        }

        private void OnCollisionWithTerrain(Entity entity, Tile tile)
        {

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

        public void UpdateFromPacket(Vector2 position, Vector2 velocity)
        {
            collRectangle.X = (int)position.X;
            collRectangle.Y = (int)position.Y;
            this.velocity = velocity;
        }

        /// <summary>
        /// Checks to see if the entity has recently taken damage.
        /// </summary>
        /// <returns></returns>
        protected bool HasTakenDamageRecently()
        {
            return (hitRecentlyTimer.TimeElapsedInSeconds < .2);
        }

        /// <summary>
        /// Deals a certain amount of damage to the entity.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Entity damageDealer, int damage)
        {
            if (IsDead() || IsTakingDamage)
                return;

            //Main.TimeFreeze.AddFrozenTime(50);

            IsTakingDamage = true;
            Health -= damage;
            hitRecentlyTimer.ResetAndWaitFor(500);
            hitRecentlyTimer.SetTimeReached += HitByPlayerTimer_SetTimeReached;

            //Creates damage particles.
            for (int i = 0; i < damage; i++)
            {
                Particle par = new Particle();
                par.CreateTookDamage(this);
                GameWorld.Instance.particles.Add(par);
            }

            if (damageDealer == null)
                return;

            velocity.Y = -5f;
            velocity.X = damage / Weight;
            if (!damageDealer.IsFacingRight)
                velocity.X *= -1;

        }

        // When timer ends let enemy take damage.
        private void HitByPlayerTimer_SetTimeReached()
        {
            IsTakingDamage = false;
        }

    }
}
