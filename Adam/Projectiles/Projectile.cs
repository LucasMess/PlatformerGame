using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;

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
            PlayerShuriken,
            SnakeVenom,
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
            Velocity = velocity;
            ObeysGravity = true;
            CurrentType = type;

            switch (CurrentType)
            {
                case Type.PlayerShuriken:
                    SourceRectangle = new Rectangle(288, 104, 8, 8);
                    break;
                case Type.SnakeVenom:
                    SourceRectangle = new Rectangle(256, 240, 16, 16);
                    break;
                default:
                    break;
            }

            CollRectangle = new Rectangle(0, 0, SourceRectangle.Width * 2, SourceRectangle.Height * 2);
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
            other.TakeDamage(_source, _source.DamagePointsProj);
            ToDelete = true;
        }

    }
}
