using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Particles
{
    /// <summary>
    /// This particle system allocates memory once for all particles.
    /// </summary>
    public class ParticleSystem
    {
        int _nextInt = 0;

        /// <summary>
        /// The next available index for creating a particle.
        /// </summary>
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

        public Particle GetNextParticle()
        {
            return _particles[NextIndex];
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

    public enum ParticleType
    {
        Smoke,
        Flame,
        Speed,
        Round_Common,
    }

    public class Particle
    {
        private static Texture2D _texture => GameWorld.SpriteSheet;
        private Rectangle _sourceRectangle;
        private Timer _animationTimer = new Timer();
        private int _frameChange;
        private int _frames;
        private int _currentFrame;

        public ParticleType CurrentParticleType { get; private set; }


        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Opacity { get; set; } = 1;
        public Color Color { get; set; } = Color.White;
        public int Width { get; set; } = 8;
        public int Height { get; set; } = 8;
        public float Scale { get; set; } = 1;
        public bool IsAnimated { get; set; }

        private void Reset()
        {
            Width = 8;
            Height = 8;
            Scale = 1;
            Opacity = 1;
            IsAnimated = false;
            Color = Color.White;
            _frameChange = 0;
            _frames = 1;
            _currentFrame = 0;
            _animationTimer.Reset();
        }

        public void ChangeParticleType(ParticleType type, Vector2 position, Vector2 velocity, Color color)
        {
            Reset();
            CurrentParticleType = type;

            switch (CurrentParticleType)
            {

                case ParticleType.Smoke:
                    Position = new Vector2(position.X - 4, position.Y - AdamGame.Random.Next(0, 80) / 10f);
                    _sourceRectangle = new Rectangle(256, 104, 8, 8);
                    Velocity = velocity;
                    Color = color;
                    Scale = AdamGame.Random.Next(5, 30) / 10f;
                    Position = new Vector2(Position.X - (Scale * Width) / 2, Position.Y - (Scale * Height) / 2);
                    _frameChange = AdamGame.Random.Next(200, 300);
                    _frames = 4;
                    IsAnimated = true;
                    break;
                case ParticleType.Flame:
                    Position = new Vector2(position.X - 4, position.Y - AdamGame.Random.Next(0, 80) / 10f);
                    _sourceRectangle = new Rectangle(288, 96, 8, 8);
                    Velocity = velocity;
                    Scale = AdamGame.Random.Next(5, 30) / 10f;
                    Position = new Vector2(position.X - (Scale * Width) / 2, position.Y - (Scale * Height) / 2);
                    _frameChange = AdamGame.Random.Next(100, 200);
                    _frames = 4;
                    IsAnimated = true;
                    break;
                case ParticleType.Round_Common:
                    Position = new Vector2(position.X - 4, position.Y - 4);
                    _sourceRectangle = new Rectangle(288, 104, 8, 8);
                    Velocity = velocity;
                    Color = color;
                    Scale = AdamGame.Random.Next(5, 30) / 10f;
                    _frameChange = AdamGame.Random.Next(100, 200);
                    _frames = 4;
                    IsAnimated = true;
                    break;
                default:
                    _sourceRectangle = new Rectangle(0, 0, 0, 0);
                    break;
            }
        }

        public virtual void Update()
        {
            switch (CurrentParticleType)
            {
                case ParticleType.Smoke:
                    NoOpacityDefaultBehavior();
                    break;
                case ParticleType.Flame:
                    NoOpacityDefaultBehavior();
                    break;
                default:
                    DefaultBehavior();
                    break;
            }

            if (IsAnimated)
            {
                _animationTimer.Increment();
                if (_animationTimer.TimeElapsedInMilliSeconds > _frameChange)
                {
                    _sourceRectangle = new Rectangle(_sourceRectangle.X + _sourceRectangle.Width, _sourceRectangle.Y, _sourceRectangle.Width, _sourceRectangle.Height);
                    _currentFrame++;
                    _animationTimer.Reset();
                }
                if (_currentFrame >= _frames)
                {
                    Opacity = 0;
                }
            }
        }

        /// <summary>
        /// Velocity will be applied and particle will fade.
        /// </summary>
        private void DefaultBehavior()
        {
            Position += Velocity;
            Opacity -= .01f;
        }

        /// <summary>
        /// Velocity will be applied along with gravity, and particle will fade.
        /// </summary>
        protected void GravityDefaultBehavior()
        {
            Position = new Vector2(Position.X, Position.Y + AdamGame.Gravity);
            DefaultBehavior();
        }

        protected void NoOpacityDefaultBehavior()
        {
            Position += Velocity;
        }

        protected void GravityNoOpacityDefaultBehavior()
        {
            Position = new Vector2(Position.X, Position.Y + AdamGame.Gravity);
            NoOpacityDefaultBehavior();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, (int)(Width * Scale), (int)(Height * Scale)), _sourceRectangle, Color * Opacity);
        }

    }

    //class SpeedParticle : Particle
    //{
    //    public SpeedParticle(Texture2D texture, int x, int y, Rectangle sourceRectangle, bool isFacingRight)
    //    {
    //        Position = new Vector2(x, y);
    //        SourceRectangle = sourceRectangle;
    //        float randX = -3f;
    //        if (isFacingRight)
    //            randX *= -1;
    //        Velocity = new Vector2(randX, 0);
    //        Opacity = .2f;
    //        Color = Color.Blue * (float)Main.Random.NextDouble();
    //        Texture = texture;
    //        Width = sourceRectangle.Width * 2;
    //        Height = sourceRectangle.Height * 2;
    //    }
    //}

    //class SmokeParticle : Particle
    //{
    //    Timer _animationTimer = new Timer();
    //    int _currentFrame;
    //    int _frames;
    //    private int frameChange;

    //    public SmokeParticle(int x, int y, Vector2 velocity, Color color)
    //    {

    //    }

    //    public override void Update()
    //    {
    //        _animationTimer.Increment();
    //        if (_animationTimer.TimeElapsedInMilliSeconds > frameChange)
    //        {
    //            SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
    //            _currentFrame++;
    //            _animationTimer.Reset();
    //        }
    //        if (_currentFrame >= _frames)
    //        {
    //            Opacity = 0;
    //        }

    //        NoOpacityDefaultBehavior();
    //    }
    //}

    //class FlameParticle : Particle
    //{
    //    Timer _animationTimer = new Timer();
    //    int _currentFrame;
    //    int _frames;
    //    private int frameChange;

    //    public FlameParticle(int x, int y, Vector2 velocity)
    //    {
    //        Position = new Vector2(x - 4, y - Main.Random.Next(0, 80) / 10f);
    //        SourceRectangle = new Rectangle(288, 96, 8, 8);
    //        Velocity = velocity;
    //        Opacity = 1;
    //        Color = Color.White;
    //        Texture = GameWorld.SpriteSheet;
    //        Scale = Main.Random.Next(5, 30) / 10f;
    //        Position = new Vector2(x - (Scale * Width) / 2, y - (Scale * Height) / 2);
    //        frameChange = Main.Random.Next(100, 200);
    //        _frames = 4;
    //    }

    //    public override void Update()
    //    {
    //        _animationTimer.Increment();
    //        if (_animationTimer.TimeElapsedInMilliSeconds > frameChange)
    //        {
    //            SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
    //            _currentFrame++;
    //            _animationTimer.Reset();
    //        }
    //        if (_currentFrame >= _frames)
    //        {
    //            Opacity = 0;
    //        }

    //        NoOpacityDefaultBehavior();
    ////    }
    ////}


    //class RoundCommonParticle : Particle
    //{
    //    Timer _animationTimer = new Timer();
    //    int _currentFrame;
    //    int _frames;
    //    private int _animationTime = Main.Random.Next(50, 200);

    //    public RoundCommonParticle(int x, int y, Vector2 vel, Color color)
    //    {

    //    }

    //    public override void Update()
    //    {
    //        _animationTimer.Increment();
    //        if (_animationTimer.TimeElapsedInMilliSeconds > _animationTime)
    //        {
    //            SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
    //            _currentFrame++;
    //            _animationTimer.Reset();
    //        }
    //        if (_currentFrame >= _frames)
    //        {
    //            Opacity = 0;
    //        }

    //        NoOpacityDefaultBehavior();
    //    }
    //}

    //class EntityTextureParticle : Particle
    //{
    //    public EntityTextureParticle(int x, int y, Rectangle rect, Vector2 vel, Entity entity)
    //    {
    //        Position = new Vector2(x, y);
    //        SourceRectangle = rect;
    //        Texture = entity.Texture;
    //        Velocity = vel;
    //    }
    //}
}
