using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using System;

namespace ThereMustBeAnotherWay.Projectiles
{

    /// <summary>
    /// Base class for all projectile. Provides basic functionality for simple projectiles, but can be extended to include more complex logic and rendering.
    /// </summary>
    public class Projectile : Entity
    {

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        /// <summary>
        /// Decides what update logic and what texture to use.
        /// </summary>
        public enum Type
        {
            PlayerTimePunch,
            SnakeVenom,
        }

        Entity _source;

        private Timer _stayAliveTimer;

        private SoundFx _onHitSound;

        /// <summary>
        /// The amount of time a projectile is alive for before being deleted.
        /// </summary>
        private const int StayAliveDuration = 10000;

        public Type CurrentType { get; private set; }

        public Projectile(Type type, Vector2 position, Vector2 velocity, Entity source)
        {
            // This prevents the projectile from hitting the floor when the player is standing still.
            Vector2 velSource = source.GetVelocity();
            if (Math.Abs(velSource.Y) < 1)
            {
                velSource.Y = 0;
            }

            CanTakeDamage = false;
            _stayAliveTimer = new Timer();
            Weight = 100;
            Texture = GameWorld.SpriteSheet;
            CurrentCollisionType = CollisionType.SuperBouncy;
            _source = source;
            Position = position;
            Velocity = velocity + velSource;
            ObeysGravity = true;
            CurrentType = type;
            CollidedWithTerrain += Projectile_CollidedWithTerrain;

            switch (CurrentType)
            {
                case Type.PlayerTimePunch:
                    SourceRectangle = new Rectangle(256, 176, 16, 16);
                    ObeysGravity = false;
                    CurrentCollisionType = CollisionType.Rigid;
                    Weight = 0;
                    _onHitSound = new SoundFx("Sounds/Player/bulletHit");
                    break;
                case Type.SnakeVenom:
                    SourceRectangle = new Rectangle(256, 240, 16, 16);
                    break;
                default:
                    break;
            }

            CollRectangle = new Rectangle(0, 0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);
        }

        private void Projectile_CollidedWithTerrain(Entity entity, Tile tile)
        {
            switch (CurrentType) {
                case Type.PlayerTimePunch:
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.Explosion, new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y), Vector2.Zero, Color.White);
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.TilePiece, new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y), new Vector2(tile.SourceRectangle.X, tile.SourceRectangle.Y), Color.White);
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.TilePiece, new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y), new Vector2(tile.SourceRectangle.X, tile.SourceRectangle.Y), Color.White);
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.TilePiece, new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y), new Vector2(tile.SourceRectangle.X, tile.SourceRectangle.Y), Color.White);
                    break;
            }

            _onHitSound?.Play();

            CollidedWithTerrain -= Projectile_CollidedWithTerrain;
            ToDelete = true;
        }

        public override void Update()
        {
            _stayAliveTimer.Increment();
            if (_stayAliveTimer.TimeElapsedInMilliSeconds > StayAliveDuration)
            {
                ToDelete = true;
            }

            IsFacingRight = Velocity.X < 0;

            switch (CurrentType)
            {
                case Type.PlayerTimePunch:
                    for (int i = 0; i < 1; i++) {
                        Vector2 randPos = CalcHelper.GetRandXAndY(new Rectangle(Position.ToPoint(), new Point(DrawRectangle.Width - 4, DrawRectangle.Height - 4)));
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.Tiny, randPos, new Vector2(TMBAW_Game.Random.Next(-10, 10) / 10f, TMBAW_Game.Random.Next(-30,30)/10f), new Color(0, 246, 255));
                    }
                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 randPos = CalcHelper.GetRandXAndY(new Rectangle(Position.ToPoint(), new Point(DrawRectangle.Width - 4, DrawRectangle.Height - 4)));
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.Tiny, randPos, Vector2.Zero,Color.White);
                    }

                    break;
                case Type.SnakeVenom:
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.Round_Common, Position, CalcHelper.GetRandXAndY(new Rectangle(0,-2,0,4)), new Color(143, 219, 116));
                    break;
                default:
                    break;
            }
            base.Update();
        }

        /// <summary>
        /// Called when this entity collides with an entity it can collide with.
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnCollisionWithEntity(Entity other)
        {
            // Collisions do not occur with items.
            if (other is Interactables.Item)
                return;
            other.TakeDamage(_source, _source.DamagePointsProj);
            ToDelete = true;
        }


    }
}
