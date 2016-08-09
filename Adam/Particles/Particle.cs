using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Particles
{
    public class ParticleSystem
    {
        int _nextInt = 0;
        int NextIndex
        {
            get
            {
                _nextInt++;
                if (_nextInt >= _particles.Length)
                    _nextInt = 0;
                return _nextInt;
            }
        }

        readonly Particle[] _particles;

        public ParticleSystem()
        {
            _particles = new Particle[10000];

            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new Particle();
            }
        }

        public void Update()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Draw(spriteBatch);
            }
        }

        public void Add(Particle par)
        {
            _particles[NextIndex] = par;
        }

        public int GetCurrentParticleIndex()
        {
            return _nextInt;
        }

    }

    public class Particle
    {
        protected Vector2 Position { get; set; }
        protected Rectangle SourceRectangle { get; set; }
        protected Vector2 Velocity { get; set; }
        protected float Opacity { get; set; } = 1;
        protected Color Color { get; set; } = Color.White;
        protected Texture2D Texture { get; set; } = Main.DefaultTexture;
        protected int Width { get; set; } = 8;
        protected int Height { get; set; } = 8;
        protected float Scale { get; set; } = 1;

        public virtual void Update()
        {
            DefaultBehavior();
        }

        /// <summary>
        /// Velocity will be applied and particle will fade.
        /// </summary>
        private void DefaultBehavior()
        {
            Position += Velocity * Main.TimeSinceLastUpdate;
            Opacity -= .01f;
        }

        /// <summary>
        /// Velocity will be applied along with gravity, and particle will fade.
        /// </summary>
        protected void GravityDefaultBehavior()
        {
            Position = new Vector2(Position.X, Position.Y + Main.Gravity);
            DefaultBehavior();
        }

        protected void NoOpacityDefaultBehavior()
        {
            Position += Velocity * Main.TimeSinceLastUpdate;
        }

        protected void GravityNoOpacityDefaultBehavior()
        {
            Position = new Vector2(Position.X, Position.Y + Main.Gravity);
            NoOpacityDefaultBehavior();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)(Width * Scale), (int)(Height * Scale)), SourceRectangle, Color * Opacity);
        }

    }

    class SpeedParticle : Particle
    {
        public SpeedParticle(Texture2D texture, int x, int y, Rectangle sourceRectangle, bool isFacingRight)
        {
            Position = new Vector2(x, y);
            SourceRectangle = sourceRectangle;
            float randX = -3f;
            if (isFacingRight)
                randX *= -1;
            Velocity = new Vector2(randX, 0);
            Opacity = .2f;
            Color = Color.Blue * (float)Main.Random.NextDouble();
            Texture = texture;
            Width = sourceRectangle.Width * 2;
            Height = sourceRectangle.Height * 2;
        }
    }

    class SmokeParticle : Particle
    {
        Timer _animationTimer = new Timer();
        int _currentFrame;
        int _frames;
        private int frameChange;

        public SmokeParticle(int x, int y, Vector2 velocity, Color color)
        {
            Position = new Vector2(x - 4, y - Main.Random.Next(0, 80) / 10f);
            SourceRectangle = new Rectangle(256, 104, 8, 8);
            Velocity = velocity;
            Opacity = 1;
            Color = color;
            Texture = GameWorld.SpriteSheet;
            Scale = Main.Random.Next(5, 30) / 10f;
            Position = new Vector2(x - (Scale * Width) / 2, y - (Scale * Height) / 2);
            frameChange = Main.Random.Next(100, 200);
            _frames = 4;
        }

        public override void Update()
        {
            _animationTimer.Increment();
            if (_animationTimer.TimeElapsedInMilliSeconds > frameChange)
            {
                SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
                _currentFrame++;
                _animationTimer.Reset();
            }
            if (_currentFrame >= _frames)
            {
                Opacity = 0;
            }

            NoOpacityDefaultBehavior();
        }
    }

    class FlameParticle : Particle
    {
        Timer _animationTimer = new Timer();
        int _currentFrame;
        int _frames;
        private int frameChange;

        public FlameParticle(int x, int y, Vector2 velocity)
        {
            Position = new Vector2(x - 4, y - Main.Random.Next(0, 80) / 10f);
            SourceRectangle = new Rectangle(288, 96, 8, 8);
            Velocity = velocity;
            Opacity = 1;
            Color = Color.White;
            Texture = GameWorld.SpriteSheet;
            Scale = Main.Random.Next(5, 30) / 10f;
            Position = new Vector2(x - (Scale * Width) / 2, y - (Scale * Height) / 2);
            frameChange = Main.Random.Next(100, 200);
            _frames = 4;
        }

        public override void Update()
        {
            _animationTimer.Increment();
            if (_animationTimer.TimeElapsedInMilliSeconds > frameChange)
            {
                SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
                _currentFrame++;
                _animationTimer.Reset();
            }
            if (_currentFrame >= _frames)
            {
                Opacity = 0;
            }

            NoOpacityDefaultBehavior();
        }
    }


    class RoundCommonParticle : Particle
    {
        Timer _animationTimer = new Timer();
        int _currentFrame;
        int _frames;
        private int _animationTime = Main.Random.Next(50, 200);

        public RoundCommonParticle(int x, int y, Vector2 vel, Color color)
        {
            Position = new Vector2(x - 4, y - 4);
            SourceRectangle = new Rectangle(288, 104, 8, 8);
            Velocity = vel;
            Opacity = 1;
            Color = color;
            Texture = GameWorld.SpriteSheet;
            Scale = Main.Random.Next(5, 30) / 10f;

            _frames = 4;

        }

        public override void Update()
        {
            if (_animationTimer.TimeElapsedInMilliSeconds > _animationTime)
            {
                SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
                _currentFrame++;
                _animationTimer.Reset();
            }
            if (_currentFrame >= _frames)
            {
                Opacity = 0;
            }

            NoOpacityDefaultBehavior();
        }
    }

    class EntityTextureParticle : Particle
    {
        public EntityTextureParticle(int x, int y, Rectangle rect, Vector2 vel, Entity entity)
        {
            Position = new Vector2(x, y);
            SourceRectangle = rect;
            Texture = entity.Texture;
            Velocity = vel;
        }
    }
}
