using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Interfaces;
using ThereMustBeAnotherWay.Misc.Sound;
using ThereMustBeAnotherWay.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.Interactables;

namespace ThereMustBeAnotherWay
{
    //Defines what happens to the velocity of the entity when it collides.
    public enum CollisionType
    {
        None,
        Rigid,
        Bouncy,
        SuperBouncy,
        Sticky,
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

        const float AirFrictionConstant = 98f / 99f;

        // Collision with terrain events.
        public event TileHandler CollidedWithTileAbove;
        public event TileHandler CollidedWithTileBelow;
        public event TileHandler CollidedWithTileToRight;
        public event TileHandler CollidedWithTileToLeft;
        public event TileHandler CollidedWithTerrain;

        protected Vector2 Origin;
        public Vector2 Position { get; set; }
        private Vector2 _positionInLastFrame;
        protected Vector2 Velocity;
        protected Animation SimpleAnimation;
        protected Light Light;

        private Texture2D _texture;
        private Color _color = Color.White;

        private const float BouncyFallOff = .8f;
        private bool _healthGiven;
        public bool IsTouchingGround { get; set; }
        public bool IsDucking { get; set; }
        public bool WantsToMoveDownPlatform { get; set; }
        public bool CanTakeDamage { get; set; } = true;

        /// <summary>
        /// Unique identifier using in multiplayer to update the entity.
        /// </summary>
        public string Id { get; set; }

        public bool IsInWater { get; set; }

        public Timer SwimTimer = new Timer();

        /// <summary>
        /// The amount of damage this entity deals by touching.
        /// </summary>
        public virtual int DamagePointsTouch { get; set; }

        /// <summary>
        /// The amount of damage this entity deals from their projectiles.
        /// </summary>
        public virtual int DamagePointsProj { get; set; }

        private int _health;
        private float _opacity = 1f;
        private float _gravityStrength = TMBAW_Game.Gravity;
        private Timer _stompParticleTimer = new Timer(true);


        /// <summary>
        /// The sound that the entity makes when it jumps on another entity.
        /// </summary>
        private static SoundFx jumpOnEntitySound = new SoundFx("Sounds/Player/enemy_jumpedOn");

        /// <summary>
        /// Sets the rotation of the sprite.
        /// </summary>
        public float Rotation { get; set; }

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

            RespawnPos = new Vector2(CollRectangle.X, CollRectangle.Y);

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
                    if (_complexAnimation != null)
                    {
                        _texture = _complexAnimation.GetCurrentTexture();
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
            if (_complexAnimation != null)
                return _complexAnimation.GetDrawRectangle();
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
        public bool IsPlayingDeathAnimation { get; private set; }

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
            _hitRecentlyTimer.Increment();

            if (IsPlayingDeathAnimation)
            {
                Rotation += .09f;
            }

            if (TMBAW_Game.CurrentGameMode == GameMode.Play)
            {
                _positionInLastFrame = new Vector2(CollRectangle.X, CollRectangle.Y);


                if (Health <= 0 && MaxHealth > 0 && !IsPlayingDeathAnimation)
                {
                    Kill();
                }



                //Check for physics, if applicable.
                if (ObeysGravity && !GameWorld.WorldData.IsTopDown)
                {
                    ApplyGravity();
                }

                //Check for collision, if applicable.
                if (IsCollidable)
                {
                    CheckTerrainCollision();
                }
                else
                {
                    // If the entity is not collidable, it should still update the position based on velocity,
                    // which is normally done in the terrain collision method.
                    MoveBy(0, Velocity.Y);
                    MoveBy(Velocity.X, 0);
                }

                // Update light position.
                Light?.Update(Center);

                if (Weight != 0)
                    ApplyAirFriction();

                // Reset this every update so that the tile update does not have to.
                IsInWater = false;

            }

            //Animate entity if applicable.
            if (this is IAnimated ian)
            {
                if (ian.Animation == null || ian.AnimationData == null)
                {
                    throw new NullReferenceException("No animation data or animation found for object:" + GetType());
                }
                ian.Animate();
            }

            if (SimpleAnimation != null)
            {
                SimpleAnimation.Update(TMBAW_Game.GameTime, CollRectangle);
            }

            // Update complex animations if this entity has it.
            _complexAnimation?.Update(this);

        }

        /// <summary>
        /// Reduces the entities velocity based on what tile it is touching.
        /// </summary>
        private void ApplyAirFriction()
        {
            IsTouchingGround = false;
            Tile below;
            if (GameWorld.WorldData.IsTopDown)
            {
                below = GameWorld.GetTile(GetTileIndex());
                IsTouchingGround = true;
                Velocity *= below.GetFrictionConstant();
            }
            else
            {
                below = GameWorld.GetTileBelow(GetTileIndex());
                if ((CollRectangle.Y + CollRectangle.Height) - below?.GetDrawRectangle().Y < 1)
                {
                    IsTouchingGround = true;
                    Velocity *= below.GetFrictionConstant();
                }
            }

            if (IsInWater)
            {
                Velocity *= .97f;
            }
        }

        /// <summary>
        /// Basic draw logic for simple objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

            if (IsDead) return;

            // Complex animations.
            if (_complexAnimation != null && _complexAnimation.AnimationDataCount != 0)
            {
                _complexAnimation.Draw(spriteBatch, IsFacingRight, Color);
            }
            else if (SimpleAnimation != null)
            {
                SimpleAnimation.Draw(spriteBatch);
            }
            // If the entity has an animation.
            else if (this is IAnimated ian)
            {
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
            else if (SourceRectangle != null && Texture != null)
            {
                if (!IsFacingRight)
                    spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color * Opacity, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                else spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color * Opacity, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }

            // Most basic drawing when there is only one frame and it is not in a spritesheet.
            else if (Texture != null)
            {
                spriteBatch.Draw(Texture, DrawRectangle, Color * Opacity);
            }

            if (GameDebug.IsDebugOn)
                DrawSurroundIndexes(spriteBatch);
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
            Velocity.X = x;
        }

        /// <summary>
        /// Sets the Y component of the entity's velocity.
        /// </summary>
        /// <param name="y"></param>
        public void SetVelY(float y)
        {
            Velocity.Y = y;
        }

        /// <summary>
        /// Changes the position of the entity by the specified amount.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ChangePosBy(int x, int y)
        {
            MoveBy(x, y);
        }

        /// <summary>
        /// Begins the process of killing the entity. This will start the death animation, make it disappear and invoke an event when it is complete.
        /// </summary>
        protected virtual void Kill()
        {
            // Queues up death animation and waits for it to finish.
            _complexAnimation.AddToQueue("death");
            _deathAnimationTimer.ResetAndWaitFor(500);
            _deathAnimationTimer.SetTimeReached += DeathAnimationEnded;
            IsPlayingDeathAnimation = true;
            SetVelY(-5f);
        }

        /// <summary>
        /// Removes all references from this object.
        /// </summary>
        public virtual void Destroy()
        {
            LightingEngine.RemoveDynamicLight(Light);
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
                GameWorld.ParticleSystem.Add(ParticleType.Smoke, CalcHelper.GetRandXAndY(CollRectangle), new Vector2(0, -TMBAW_Game.Random.Next(1, 5) / 10f), Color.White);
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
            return CalcHelper.GetIndexInGameWorld(CollRectangle.Center.X, CollRectangle.Center.Y);
            //if (GameWorld.WorldData == null) return -1;
            //return (int)(CollRectangle.Center.Y / AdamGame.Tilesize * GameWorld.WorldData.LevelWidth) + (int)(CollRectangle.Center.X / AdamGame.Tilesize);
        }

        /// <summary>
        /// Gets tile index at specified coordinate.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public int GetTileIndex(Vector2 coord)
        {
            return (int)((int)coord.Y / TMBAW_Game.Tilesize * GameWorld.WorldData.LevelWidth) + (int)((int)coord.X / TMBAW_Game.Tilesize);
        }

        /// <summary>
        /// Returns all of the tile indexes of the tiles surrounding the entity.
        /// </summary>
        /// <returns></returns>
        public int[] GetNearbyTileIndexes()
        {
            int width = GameWorld.WorldData.LevelWidth;
            int heightInTiles = (int)(Math.Ceiling((double)CollRectangle.Height / TMBAW_Game.Tilesize) + 2);
            int widthInTiles = (int)(Math.Ceiling((double)CollRectangle.Width / TMBAW_Game.Tilesize) + 3);
            int startingIndex = GetTileIndex(new Vector2(CollRectangle.X, CollRectangle.Y)) - width - 1;

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
                    if (quadrant >= 0 && quadrant < GameWorld.TileArray.Length && GameWorld.WorldData.IsTopDown ? tile.IsSolidTopDown : tile.IsSolid)
                    {
                        Rectangle tileRect = tile.DrawRectangle;
                        if (CollRectangle.Intersects(tileRect))
                        {
                            if (Velocity.Y > 0)
                            {
                                if (tile.CurrentCollisionType == Tile.CollisionType.FromAbove)
                                {
                                    // Do I know why I have to use the last position instead of the current? No. But does it work? Yes.
                                    if (_positionInLastFrame.Y + CollRectangle.Height * 2 / 3 > tile.DrawRectangle.Y)
                                        continue;

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
                    if (quadrant >= 0 && quadrant < GameWorld.TileArray.Length && GameWorld.WorldData.IsTopDown ? tile.IsSolidTopDown : tile.IsSolid)
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

            // Check for interactables updates.
            foreach (int quadrant in q)
            {
                Tile tile = GameWorld.GetTile(quadrant);
                if (tile.GetDrawRectangle().Intersects(CollRectangle))
                    tile.OnEntityTouch(this);
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
            _complexAnimation.AddToQueue(name);
        }

        /// <summary>
        /// Removes an animation from the lsit of aniamtions that are to be played.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAnimationFromQueue(string name)
        {
            _complexAnimation.RemoveFromQueue(name);
        }

        /// <summary>
        /// This will return the volume that the listener should be hearing from the source.
        /// </summary>
        /// <param name="listener">Who the listener is.</param>
        /// <returns>Volume of sound.</returns>
        public float GetSoundVolume(Entity listener, float maxVolume)
        {
            // Anything further than 20 tiles cannot be heard.
            listener = listener.Get();
            int maxDist = 20;
            float xDist = (listener.CollRectangle.Center.X - CollRectangle.Center.X) / TMBAW_Game.Tilesize;
            float yDist = (listener.CollRectangle.Center.Y - CollRectangle.Center.Y) / TMBAW_Game.Tilesize;
            float retVal = maxVolume * (1 - (xDist + yDist) / maxDist);
            if (retVal > 1) retVal = 1;
            if (retVal < 0) retVal = 0;
            return retVal;
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
                spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), t.DrawRectangle, Color.Red * .5f);
            }
        }

        /// <summary>
        /// Makes objects fall.
        /// </summary>
        private void ApplyGravity()
        {
            float gravity = GravityStrength;
            if (IsInWater)
            {
                gravity = GravityStrength / 10f;
            }

            Velocity.Y += gravity * Weight / 100;

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
                    Velocity.Y = -Velocity.Y * BouncyFallOff;
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
                    Velocity.Y = -Velocity.Y * BouncyFallOff;
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
                    Velocity.X = -Velocity.X * BouncyFallOff;
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
                    Velocity.X = -Velocity.X * BouncyFallOff;
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
            if (CurrentCollisionType == CollisionType.Sticky)
            {
                SetVelX(0);
                SetVelY(0);
                ObeysGravity = false;
            }
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
            SetPosition(RespawnPos);
            Console.WriteLine("Respawned at location: {0}, {1}", RespawnPos.X, RespawnPos.Y);
            Health = MaxHealth;
            Velocity = new Vector2(0, 0);
            IsDead = false;
            IsPlayingDeathAnimation = false;
            _complexAnimation?.RemoveFromQueue("death");
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

        public Vector2 Center
        {
            get { return new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y); }
        }

        public Vector2 SpriteCenter
        {
            get
            {
                if (Texture != null)
                    return new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2);
                return new Vector2();
            }
        }

        /// <summary>
        /// Deals a certain amount of damage to the entity.
        /// </summary>
        /// <param name="damageDealer">Can be null.</param>
        /// <param name="damage"></param>
        public virtual void TakeDamage(Entity damageDealer, int damage)
        {
            if (TMBAW_Game.CurrentGameMode == GameMode.Play)
            {
                if (IsTakingDamage || IsPlayingDeathAnimation || !CanTakeDamage)
                    return;

                if (this == GameWorld.GetPlayer())
                {
                    Overlay.ColoredCorners.FlashColor(Color.Red);
                }

                // Main.TimeFreeze.AddFrozenTime(damage*3);

                IsTakingDamage = true;
                Health -= damage;
                GameWorld.ParticleSystem.Add("-" + damage, Center, new Vector2(TMBAW_Game.Random.Next(0, 2) * -2 + 1, -15), new Color(255, 108, 108));
                _hitRecentlyTimer.ResetAndWaitFor(500);
                _hitRecentlyTimer.SetTimeReached += HitByPlayerTimer_SetTimeReached;

                //Creates damage particles.
                int particleCount = damage / 2;
                if (particleCount > 100)
                    particleCount = 100;
                for (int i = 0; i < particleCount; i++)
                {
                    GameWorld.ParticleSystem.Add(ParticleType.Round_Common, CalcHelper.GetRandXAndY(CollRectangle), null, Color.Red);
                }

                //if (damageDealer == null)
                //    return;

                //Velocity.Y = -8f;
                //Velocity.X = (Weight / 2f);
                //if (!damageDealer.IsFacingRight)
                //    Velocity.X *= -1;

                HasTakenDamage?.Invoke();
            }
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
            Vector2 size = new Vector2(GetDrawRectangle().Width / TMBAW_Game.Tilesize, GetDrawRectangle().Height / TMBAW_Game.Tilesize);
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
                    GameWorld.ParticleSystem.Add(ParticleType.Smoke, new Vector2(CalcHelper.GetRandomX(GetCollRectangle()), GetCollRectangle().Bottom), new Vector2(TMBAW_Game.Random.Next(-5, 5) / 10f, -TMBAW_Game.Random.Next(1, 5) / 10f), Color.White);
                }
                _stompParticleTimer.Reset();
            }
        }


        /// <summary>
        /// Returns true if the player is to the right of this entity in the game world.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerToRight()
        {
            return (Position.X < GameWorld.GetPlayer().Position.X);
        }

        /// <summary>
        /// Returns a reference to the Complex Animation object of this entity, responsible for keeping track 
        /// of all animations and sorting them based on priorities.
        /// </summary>
        public ComplexAnimation ComplexAnimation => _complexAnimation;

        /// <summary>
        /// Sets the position of the entity to the give position.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        /// <summary>
        /// Changes the position of the entity by the specified amount.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveBy(float x, float y)
        {
            Position += new Vector2(x, y);
        }

        /// <summary>
        /// Sets the x-coordinate of the entity to the given quantity.
        /// </summary>
        /// <param name="x"></param>
        public void SetX(float x)
        {
            Position = new Vector2(x, Position.Y);
        }

        /// <summary>
        /// Sets the y-coordinate of the entity to the given quantity.
        /// </summary>
        /// <param name="y"></param>
        public void SetY(float y)
        {
            Position = new Vector2(Position.X, y);
        }

        /// <summary>
        /// The damage dealt by the entity when jumping on top of another entity.
        /// </summary>
        /// <returns></returns>
        public virtual int GetDamage()
        {
            return 20;
        }

        /// <summary>
        /// Determines what happens to this entity when it jumps on another entity.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnJumpOnAnotherEntity(Entity other)
        {
            jumpOnEntitySound.Play();
            SetVelY(MushroomBooster.WeakBoost);
            SetY(other.GetCollRectangle().Y - GetCollRectangle().Height);
        }

    }
}
