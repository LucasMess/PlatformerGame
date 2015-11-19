using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                if (_nextInt >= particles.Length)
                    _nextInt = 0;
                return _nextInt;
            }
        }

        NewParticle[] particles;

        public ParticleSystem()
        {
            particles = new NewParticle[10000];

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new NewParticle();
            }
        }

        public void Update()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Draw(spriteBatch);
            }
        }

        public void Add(NewParticle par)
        {
            particles[NextIndex] = par;
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
        Timer animationTimer = new Timer();
        int currentFrame;
        int frames;

        public SmokeParticle(int x, int y)
        {
            Position = new Vector2(x - 4, y);
            SourceRectangle = new Rectangle(256, 104, 8, 8);
            float randX = 0;
            float randY = (float)(GameWorld.RandGen.Next(-1, 0) * GameWorld.RandGen.NextDouble());
            Velocity = new Vector2(randX, randY);
            Opacity = 1;
            Color = Color.White;
            Texture = GameWorld.SpriteSheet;

            frames = 4;
        }

        public override void Update()
        {
            animationTimer.Increment();
            if (animationTimer.TimeElapsedInMilliSeconds > 500)
            {
                SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
                currentFrame++;
                animationTimer.Reset();
            }
            if (currentFrame >= frames)
            {
                Opacity = 0;
            }

            NoOpacityDefaultBehavior();
        }
    }

    class SparkleParticle : NewParticle
    {
        Timer animationTimer = new Timer();
        int currentFrame;
        int frames;

        public SparkleParticle(int x, int y, float velX, float velY, Color color)
        {
            Position = new Vector2(x - 4, y - 4);
            SourceRectangle = new Rectangle(288, 104, 8, 8);
            Velocity = new Vector2(velX, velY);
            Opacity = 1;
            Color = color;
            Texture = GameWorld.SpriteSheet;

            frames = 4;

        }

        public override void Update()
        {
            animationTimer.Increment();
            if (animationTimer.TimeElapsedInMilliSeconds > 250)
            {
                SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
                currentFrame++;
                animationTimer.Reset();
            }
            if (currentFrame >= frames)
            {
                Opacity = 0;
            }

            NoOpacityDefaultBehavior();
        }
    }
}
