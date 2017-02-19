using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.Projectiles
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
            PlayerShuriken
        }

        Entity _source;

        private Timer _stayAliveTimer;


        /// <summary>
        /// The amount of time a projectile is alive for before being deleted.
        /// </summary>
        private const int StayAliveDuration = 10000;

        public Type CurrentType { get; private set; }

        public Projectile(Type type, Vector2 position, Vector2 velocity, Entity source)
        {
            _stayAliveTimer = new Timer();
            Weight = 10;
            Texture = GameWorld.SpriteSheet;
            CurrentCollisionType = CollisionType.SuperBouncy;
            _source = source;
            Position = position;
            CollRectangle = new Rectangle(0, 0, 16, 16);
            Velocity = velocity;
            ObeysGravity = true;

            switch (CurrentType)
            {
                case Type.PlayerShuriken:
                    SourceRectangle = new Rectangle(288, 104, 8, 8);
                    break;
                default:
                    break;
            }
        }

        public override void Update()
        {
            _stayAliveTimer.Increment();
            if (_stayAliveTimer.TimeElapsedInMilliSeconds > StayAliveDuration)
            {
                ToDelete = true;
            }

            switch (CurrentType)
            {
                case Type.PlayerShuriken:
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.Flame, Position, Vector2.Zero, Color.White);
                    GameWorld.ParticleSystem.Add(Particles.ParticleType.Round_Common, Position, null, new Color(255, 233, 198));
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
            other.TakeDamage(_source, _source.DamagePointsProj);
            ToDelete = true;
        }

    }
}
