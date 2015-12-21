using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

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

        NewParticle[] _particles;

        public ParticleSystem()
        {
            _particles = new NewParticle[10000];

            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new NewParticle();
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

        public void Add(NewParticle par)
        {
            _particles[NextIndex] = par;
        }


    }

    public class NewParticle
    {
        public bool InUse { get; set; } = true;
        protected Vector2 Position { get; set; }
        protected Rectangle SourceRectangle { get; set; }
        protected Vector2 Velocity { get; set; }
        protected float Opacity { get; set; } = 1;
        protected Color Color { get; set; } = Color.White;
        protected Texture2D Texture { get; set; } = Main.DefaultTexture;
        protected int Width { get; set; } = 8;
        protected int Height { get; set; } = 8;

        public virtual void Update()
        {
            DefaultBehavior();
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
            Position = new Vector2(Position.X, Position.Y + Main.Gravity);
            DefaultBehavior();
        }

        protected void NoOpacityDefaultBehavior()
        {
            Position += Velocity;
        }

        protected void GravityNoOpacityDefaultBehavior()
        {
            Position = new Vector2(Position.X, Position.Y + Main.Gravity);
            NoOpacityDefaultBehavior();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), SourceRectangle, Color * Opacity);
        }

    }

    class SpeedParticle : NewParticle
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
            Color = Color.Blue * (float)GameWorld.RandGen.NextDouble();
            Texture = texture;
            Width = sourceRectangle.Width * 2;
            Height = sourceRectangle.Height * 2;
        }
    }

    class SmokeParticle : NewParticle
    {
        Timer _animationTimer = new Timer();
        int _currentFrame;
        int _frames;

        public SmokeParticle(int x, int y, Vector2 velocity)
        {
            Position = new Vector2(x - 4, y);
            SourceRectangle = new Rectangle(256, 104, 8, 8);
            Velocity = velocity;
            Opacity = 1;
            Color = Color.White;
            Texture = GameWorld.SpriteSheet;

            _frames = 4;
        }

        public override void Update()
        {
            if (_animationTimer.TimeElapsedInMilliSeconds > 500)
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

    class RoundCommonParticle : NewParticle
    {
        Timer _animationTimer = new Timer();
        int _currentFrame;
        int _frames;

        public RoundCommonParticle(int x, int y, Vector2 vel, Color color)
        {
            Position = new Vector2(x - 4, y - 4);
            SourceRectangle = new Rectangle(288, 104, 8, 8);
            Velocity = vel;
            Opacity = 1;
            Color = color;
            Texture = GameWorld.SpriteSheet;

            _frames = 4;

        }

        public override void Update()
        {
            if (_animationTimer.TimeElapsedInMilliSeconds > 250)
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

    class EntityTextureParticle :NewParticle
    {
        public EntityTextureParticle(int x, int y, Rectangle rect, Vector2 vel, Entity entity)
        {
            Position = new Vector2(x,y);
            SourceRectangle = rect;
            Texture = entity.Texture;
            Velocity = vel;
        }
    }
}
