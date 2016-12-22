using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.Misc.Sound;
using Adam.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Adam
{
    //Defines what happens to the velocity of the entity when it collides.
    public enum CollisionType
    {
        None,
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
        public delegate void EntityHandler(Entity entity);

        public event EntityHandler HasFinishedDying;
        public event EventHandler HasTakenDamage;
        public event EventHandler HasRevived;

        public event EntityHandler UpdateCall;

        const float AirFrictionConstant = 98f / 99f;

        // Collision with terrain events.
        public event TileHandler CollidedWithTileAbove;
        public event TileHandler CollidedWithTileBelow;
        public event TileHandler CollidedWithTileToRight;
        public event TileHandler CollidedWithTileToLeft;
        public event TileHandler CollidedWithTerrain;

        // Collision with entity events.
        public event EntityHandler CollidedWithEntity;
        public event EntityHandler CollidedWithEntityAbove;
        public event EntityHandler CollidedWithEntityBelow;
        public event EntityHandler CollidedWithEntityToRight;
        public event EntityHandler CollidedWithEntityToLeft;

        protected Vector2 Origin;
        public Vector2 Position { get; set; }
        protected Vector2 Velocity;

        private Texture2D _texture;
        private Color _color = Color.White;

        private bool _healthGiven;
        public bool IsTouchingGround { get; set; }
        public bool IsDucking { get; set; }
        public bool WantsToMoveDownPlatform { get; set; }

        private int _health;
        private float _opacity = 1f;
        private float _gravityStrength = AdamGame.Gravity;
        private Timer _stompParticleTimer = new Timer(true);

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
            AdamGame.GameUpdateCalled += DefineRespawnPoint;
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
        public CollisionType CurrentCollisionType { get; set; } = CollisionType.Rigid;

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
                    if (ComplexAnim != null)
                    {
                        _texture = ComplexAnim.GetCurrentTexture();
                    }
                    else
                    {
                        _texture = AdamGame.DefaultTexture;
                        Console.WriteLine("Texture for: {0} is null, using default texture instead.", GetType());
                    }
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
        /// Retrieves the current DrawRectangle.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetDrawRectangle()
        {
            if (ComplexAnim != null)
                return ComplexAnim.GetDrawRectangle();
            return DrawRectangle;
        }

        private Rectangle _collRectangle;
        /// <summary>
        /// The rectangle that keeps track of collision.
        /// </summary>
        public Rectangle CollRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _collRectangle.Width, _collRectangle.Height);
            }
            protected set
            {
                _collRectangle = new Rectangle(0, 0, value.Width, value.Height);
            }
        }

        /// <summary>
        /// Returns the Collision Rectangle of the entity.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCollRectangle()
        {
            return CollRectangle;
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
        protected Rectangle SourceRectangle;

        /// <summary>
        /// Returns true if the entity is dead.
        /// </summary>
        /// <returns></returns>
        public bool IsDead { get; private set; }

        /// <summary>
        /// Returns true if the enemy is in the process of dying.
        /// </summary>
        public bool IsAboutToDie { get; private set; }

        /// <summary>
        /// The maximum amount of health the enemy can have. This is the value that is given to the enemy when it respawns.
        /// </summary>
        public virtual int MaxHealth { get; } = 0;

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
        public bool IsFacingRight { get; set; }

        /// <summary>
        /// Determines whether the entity can be deleted or not.
        /// </summary>
        public bool ToDelete { get; set; }

        /// <summary>
        /// Base update that provides basic logic.
        /// </summary>
        public virtual void Update()
        {
            if (AdamGame.CurrentGameMode == GameMode.Play)
            {
                if (Health <= 0 && MaxHealth > 0 && !IsAboutToDie)
                {
                    Kill();
                }

                //Check for physics, if applicable.
                if (ObeysGravity && !IsAboutToDie)
                {
                    ApplyGravity();
                }

                //Check for collision, if applicable.
                if (IsCollidable)
                {
                    CheckTerrainCollision();
                }

                ApplyAirFriction();

                UpdateCall?.Invoke(this);

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
            ComplexAnim?.Update(this);

        }

        /// <summary>
        /// Reduces the entities velocity based on what tile it is touching.
        /// </summary>
        private void ApplyAirFriction()
        {
            IsTouchingGround = false;
            Tile below = GameWorld.GetTileBelow(GetTileIndex());
            if ((CollRectangle.Y + CollRectangle.Height) - below?.GetDrawRectangle().Y < 1)
            {
                IsTouchingGround = true;
                Velocity *= below.GetFrictionConstant();
            }
        }

        /// <summary>
        /// Called right after the entity spawns to define the respawn location;
        /// </summary>
        /// <param name="gameTime"></param>
        private void DefineRespawnPoint()
        {
            AdamGame.GameUpdateCalled -= DefineRespawnPoint;
            Vector2 v = RespawnPos;
        }

        /// <summary>
        /// Basic draw logic for simple objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

            //spriteBatch.Draw(Main.DefaultTexture, GetCollRectangle(), Color.Red * .5f);

            // Complex animations.
            if (ComplexAnim != null)
            {
                ComplexAnim.Draw(spriteBatch, IsFacingRight, Color);
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
                    ian.Animation.IsFlipped = true;
                }
                else ian.Animation.IsFlipped = false;

                ian.Animation.Draw(spriteBatch);
            }

            // Drawing for simple entities that have a texture contained in a spritesheet.
            else if (SourceRectangle != null)
            {
                if (!IsFacingRight)
                    spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color * Opacity, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                else spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color * Opacity, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
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
            spriteBatch.Draw(Texture, DrawRectangle, null, Color.White, 0, Origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Returns the current velocity of the entity.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVelocity()
        {
            return Velocity;
        }

        /// <summary>
        /// Sets the X component of the entity's velocity.
        /// </summary>
        /// <param name="x"></param>
        public void SetVelX(float x)
        {
            if (!IsAboutToDie && !IsTakingDamage)
                Velocity.X = x;
        }

        /// <summary>
        /// Sets the Y component of the entity's velocity.
        /// </summary>
        /// <param name="y"></param>
        public void SetVelY(float y)
        {
            if (!IsAboutToDie && !IsTakingDamage)
                Velocity.Y = y;
        }

        /// <summary>
        /// Changes the position of the entity by the specified amount.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ChangePosBy(int x, int y)
        {
            if (!IsAboutToDie && !IsTakingDamage)
            {
                MoveBy(x, y);
            }
        }

        /// <summary>
        /// Begins the process of killing the entity. This will start the death animation, make it disappear and invoke an event when it is complete.
        /// </summary>
        private void Kill()
        {
            // Queues up death animation and waits for it to finish.
            ComplexAnim.AddToQueue("death");
            _deathAnimationTimer.ResetAndWaitFor(1000);
            _deathAnimationTimer.SetTimeReached += DeathAnimationEnded;
            IsAboutToDie = true;
            Velocity.Y = -5f;
        }

        /// <summary>
        /// Removes all references from this object.
        /// </summary>
        public virtual void Destroy()
        {
            GameWorld.Entities.Remove(this);
        }

        /// <summary>
        /// Creates death particles and calls event to do other death related things.
        /// </summary>
        private void DeathAnimationEnded()
        {
            _deathAnimationTimer.SetTimeReached -= DeathAnimationEnded;
            IsDead = true;

            for (int i = 0; i < 20; i++)
            {
                GameWorld.ParticleSystem.GetNextParticle().ChangeParticleType(ParticleType.Smoke, CalcHelper.GetRandXAndY(CollRectangle), new Vector2(0, -AdamGame.Random.Next(60, 300) / 10f), Color.White);
            }

            //Rectangle[] desinRectangles;
            //GetDisintegratedRectangles(out desinRectangles);
            //foreach (Rectangle rect in desinRectangles)
            //{
            //    EntityTextureParticle par = new EntityTextureParticle(CalcHelper.GetRandomX(CollRectangle), CalcHelper.GetRandomY(CollRectangle), rect, new Vector2(Main.Random.Next(-5, 5) / 10f, -Main.Random.Next(-5, 5) / 10f), this);
            //    GameWorld.ParticleSystem.Add(par);
            //}

            HasFinishedDying?.Invoke(this);
        }

        /// <summary>
        /// Whether the entity is simply intersecting terrain.
        /// </summary>
        /// <returns></returns>
        public bool IsTouchingTerrain()
        {
            int[] q = GetNearbyTileIndexes();

            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= GameWorld.TileArray.Length - 1 && GameWorld.TileArray[quadrant].IsSolid)
                {
                    if (CollRectangle.Intersects(GameWorld.TileArray[quadrant].DrawRectangle))
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
            return entity.CollRectangle.Intersects(this.CollRectangle);
        }

        /// <summary>
        /// Gets the entity' tile index. The tile index is used to determine an entity's position in the map.
        /// </summary>
        /// 
        /// <returns></returns>
        public int GetTileIndex()
        {
            return (int)(CollRectangle.Center.Y / AdamGame.Tilesize * GameWorld.WorldData.LevelWidth) + (int)(CollRectangle.Center.X / AdamGame.Tilesize);
        }

        /// <summary>
        /// Gets tile index at specified coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public int GetTileIndex(Vector2 coord)
        {
            return (int)((int)coord.Y / AdamGame.Tilesize * GameWorld.WorldData.LevelWidth) + (int)((int)coord.X / AdamGame.Tilesize);
        }

        /// <summary>
        /// Returns all of the tile indexes of the tiles surrounding the entity.
        /// </summary>
        /// <returns></returns>
        public int[] GetNearbyTileIndexes()
        {
            int width = GameWorld.WorldData.LevelWidth;
            int startingIndex = GetTileIndex(new Vector2(CollRectangle.Center.X, CollRectangle.Y)) - width - 1;
            int heightInTiles = (int)(Math.Ceiling((double)CollRectangle.Height / AdamGame.Tilesize) + 2);
            int widthInTiles = (int)(Math.Ceiling((double)CollRectangle.Width / AdamGame.Tilesize) + 2);

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
            int[] q = GetNearbyTileIndexes();

            //Solve Y collisions
            MoveBy(0, Velocity.Y);
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < GameWorld.TileArray.Length)
                {
                    Tile tile = GameWorld.TileArray[quadrant];
                    if (quadrant >= 0 && quadrant < GameWorld.TileArray.Length && tile.IsSolid == true)
                    {
                        Rectangle tileRect = tile.DrawRectangle;
                        if (CollRectangle.Intersects(tileRect))
                        {
                            if (Velocity.Y > 0)
                            {
                                if (tile.CurrentCollisionType == Tile.CollisionType.FromAbove)
                                {
                                    if (CollRectangle.Bottom <= tile.DrawRectangle.Center.Y) { }
                                    else continue;

                                    if (WantsToMoveDownPlatform) continue;
 
                                }
                                CollidedWithTileBelow(this, tile);
                                CollidedWithTerrain(this, tile);
                            }
                            else if (tile.CurrentCollisionType == Tile.CollisionType.FromAbove) continue;
                            else if (Velocity.Y < 0)
                            {
                                CollidedWithTileAbove(this, tile);
                                CollidedWithTerrain(this, tile);
                            }
                        }
                    }
                }
            }
            //Solve X Collisions
            MoveBy(Velocity.X, 0);
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant < GameWorld.TileArray.Length)
                {
                    Tile tile = GameWorld.TileArray[quadrant];
                    if (tile.CurrentCollisionType == Tile.CollisionType.FromAbove) continue;
                    if (quadrant >= 0 && quadrant < GameWorld.TileArray.Length && tile.IsSolid == true)
                    {
                        Rectangle tileRect = tile.DrawRectangle;
                        if (CollRectangle.Intersects(tile.DrawRectangle))
                        {
                            if (Velocity.X > 0)
                            {
                                CollidedWithTileToRight(this, tile);
                                CollidedWithTerrain(this, tile);
                            }
                            else if (Velocity.X < 0)
                            {
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
        public void CheckSimpleTerrainCollision()
        {
            int[] q = GetNearbyTileIndexes();

            foreach (int index in q)
            {
                if (index >= 0 && index <= GameWorld.TileArray.Length - 1 && GameWorld.TileArray[index].IsSolid == true)
                {
                    if (CollRectangle.Intersects(GameWorld.TileArray[index].DrawRectangle))
                    {
                        CollidedWithTerrain(this, GameWorld.TileArray[index]);
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
            ComplexAnim.AddToQueue(name);
        }

        /// <summary>
        /// Removes an animation from the lsit of aniamtions that are to be played.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAnimationFromQueue(string name)
        {
            ComplexAnim.RemoveFromQueue(name);
        }

        /// <summary>
        /// This will return the volume that the listener should be hearing from the source.
        /// </summary>
        /// <param name="listener">Who the listener is.</param>
        /// <returns>Volume of sound.</returns>
        public float GetSoundVolume(Entity listener, float maxVolume)
        {
            listener = listener.Get();
            float xDist = listener.CollRectangle.Center.X - CollRectangle.Center.X;
            float yDist = listener.CollRectangle.Center.Y - CollRectangle.Center.Y;
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
            foreach (int i in GetNearbyTileIndexes())
            {
                if (i >= GameWorld.TileArray.Length || i < 0) continue;
                Tile t = GameWorld.TileArray[i];
                if (t == null) throw new NullReferenceException("t");
                spriteBatch.Draw(AdamGame.DefaultTexture, t.DrawRectangle, t.SourceRectangle, Color.Red);
            }
        }

        /// <summary>
        /// Makes objects fall.
        /// </summary>
        private void ApplyGravity()
        {
            Velocity.Y += (GravityStrength);
            if (Velocity.Y > 12)
            {
                Velocity.Y = 12;
            }
        }

        /// <summary>
        /// Collision logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
        private void OnCollisionWithTileAbove(Entity entity, Tile tile)
        {
            SetY(tile.DrawRectangle.Y + tile.DrawRectangle.Height);
            switch (CurrentCollisionType)
            {
                case CollisionType.Rigid:
                    Velocity.Y = 0;
                    break;
                case CollisionType.Bouncy:
                    Velocity.Y = -Velocity.Y;
                    break;
                case CollisionType.SuperBouncy:
                    Velocity.Y = -Velocity.Y;
                    break;
            }
        }

        /// <summary>
        /// Collision logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
        private void OnCollisionWithTileBelow(Entity entity, Tile tile)
        {
            SetY(tile.DrawRectangle.Y - CollRectangle.Height);

            if (Math.Abs(Velocity.Y) > 8 && IsJumping)
            {
                CreateStompParticles(CollRectangle.Width / 10);
            }

            switch (CurrentCollisionType)
            {
                case CollisionType.Rigid:
                    Velocity.Y = 0;
                    break;
                case CollisionType.Bouncy:
                    Velocity.Y = -Velocity.Y;
                    break;
                case CollisionType.SuperBouncy:
                    Velocity.Y = -Velocity.Y;
                    break;
            }
        }

        /// <summary>
        /// Collision logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
        private void OnCollisionWithTileToRight(Entity entity, Tile tile)
        {
            SetX(tile.DrawRectangle.X - CollRectangle.Width);
            switch (CurrentCollisionType)
            {
                case CollisionType.None:
                    break;
                case CollisionType.Rigid:
                    Velocity.X = 0;
                    break;
                case CollisionType.Bouncy:
                    Velocity.X = -Velocity.X;
                    break;
                case CollisionType.SuperBouncy:
                    Velocity.X = -Velocity.X;
                    break;

            }

        }

        /// <summary>
        /// Collision logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
        private void OnCollisionWithTileToLeft(Entity entity, Tile tile)
        {
            SetX(tile.DrawRectangle.X + tile.DrawRectangle.Width);
            switch (CurrentCollisionType)
            {
                case CollisionType.None:

                    break;
                case CollisionType.Rigid:
                    Velocity.X = 0;
                    break;
                case CollisionType.Bouncy:
                    Velocity.X = -Velocity.X;
                    break;
                case CollisionType.SuperBouncy:
                    Velocity.X = -Velocity.X;
                    break;
            }

        }

        /// <summary>
        /// Collision logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
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
            _respawnTimer.SetTimeReached -= Revive;
            CollRectangle = new Rectangle((int)RespawnPos.X, (int)RespawnPos.Y, CollRectangle.Width,
                CollRectangle.Height);
            Console.WriteLine("Respawned at location: {0}, {1}", RespawnPos.X, RespawnPos.Y);
            Health = MaxHealth;
            Velocity = new Vector2(0, 0);
            IsDead = false;
            IsAboutToDie = false;
            ComplexAnim?.RemoveFromQueue("death");
            HasRevived?.Invoke();
        }

        /// <summary>
        /// Updates the position and velocity of the entiy based on the information received from the network packet.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public void UpdateFromPacket(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        /// <summary>
        /// Checks to see if the entity has recently taken damage.
        /// </summary>
        /// <returns></returns>
        protected bool HasTakenDamageRecently()
        {
            return (_hitRecentlyTimer.TimeElapsedInSeconds < .2);
        }

        /// <summary>
        /// Deals a certain amount of damage to the entity.
        /// </summary>
        /// <param name="damageDealer">Can be null.</param>
        /// <param name="damage"></param>
        public void TakeDamage(Entity damageDealer, int damage)
        {
            if (IsTakingDamage || IsAboutToDie)
                return;

            // Main.TimeFreeze.AddFrozenTime(damage*3);

            IsTakingDamage = true;
            Health -= damage;
            _hitRecentlyTimer.ResetAndWaitFor(500);
            _hitRecentlyTimer.SetTimeReached += HitByPlayerTimer_SetTimeReached;

            //Creates damage particles.
            int particleCount = damage / 2;
            if (particleCount > 100)
                particleCount = 100;
            for (int i = 0; i < particleCount; i++)
            {
                GameWorld.ParticleSystem.GetNextParticle().ChangeParticleType(ParticleType.Round_Common, CalcHelper.GetRandXAndY(CollRectangle), new Vector2(0, -AdamGame.Random.Next(1, 5) / 10f), Color.Red);
            }

            if (damageDealer == null)
                return;

            Velocity.Y = -8f;
            Velocity.X = (Weight / 2f);
            if (!damageDealer.IsFacingRight)
                Velocity.X *= -1;

            HasTakenDamage?.Invoke();

        }

        /// <summary>
        ///  When timer ends let enemy take damage.
        /// </summary>
        private void HitByPlayerTimer_SetTimeReached()
        {
            IsTakingDamage = false;
        }

        /// <summary>
        /// Returns the texture of the entity as small rectangles for particle effects.
        /// </summary>
        /// <param name="rectangles"></param>
        private void GetDisintegratedRectangles(out Rectangle[] rectangles)
        {
            Vector2 size = new Vector2(GetDrawRectangle().Width / AdamGame.Tilesize, GetDrawRectangle().Height / AdamGame.Tilesize);
            int xSize = 4 * (int)size.X;
            int ySize = 4 * (int)size.Y;

            if (xSize == 0) xSize = 1;
            if (ySize == 0) ySize = 1;

            int width = SourceRectangle.Width / xSize;
            int height = SourceRectangle.Height / ySize;
            rectangles = new Rectangle[xSize * ySize];

            int i = 0;
            for (int h = 0; h < ySize; h++)
            {
                for (int w = 0; w < xSize; w++)
                {
                    rectangles[i] = new Rectangle(w * width, h * height, width, height);
                    i++;
                }
            }
        }

        /// <summary>
        /// Creates and adds stomp particles where the entity is.
        /// </summary>
        /// <param name="count"></param>
        public void CreateStompParticles(int count)
        {
            if (_stompParticleTimer.TimeElapsedInMilliSeconds > 100)
            {
                for (int i = 0; i < count; i++)
                {
                    GameWorld.ParticleSystem.GetNextParticle().ChangeParticleType(ParticleType.Smoke, new Vector2(CalcHelper.GetRandomX(GetCollRectangle()), GetCollRectangle().Bottom), new Vector2(AdamGame.Random.Next(-300, 300) / 10f, -AdamGame.Random.Next(60, 300) / 10f), Color.White);
                }
                _stompParticleTimer.Reset();
            }
        }

        public bool IsPlayerToRight()
        {
            if (this is PlayerCharacter.Player)
            {
                throw new Exception("Cannot ask if player is to the right if you are the player!");
            }

            return (Position.X < GameWorld.GetPlayer().Position.X);
        }

        public ComplexAnimation ComplexAnimation => ComplexAnim;

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void MoveBy(float x, float y)
        {
            Position += new Vector2(x, y);
        }

        public void SetX(float x)
        {
            Position = new Vector2(x, Position.Y);
        }

        public void SetY(float y)
        {
            Position = new Vector2(Position.X, y);
        }

    }
}
