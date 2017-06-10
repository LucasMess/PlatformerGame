using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Helpers;

namespace ThereMustBeAnotherWay.Particles
{
    public partial class ParticleSystem
    {
        private class Particle
        {
            private static Texture2D _texture => GameWorld.SpriteSheet;
            public Rectangle SourceRectangle;
            private ThereMustBeAnotherWay.Misc.Timer _animationTimer = new Misc.Timer();
            public int _frameChange;
            public int _frames;
            public int _currentFrame;

            public ParticleType CurrentParticleType { get; set; }


            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            public float Opacity { get; set; } = 1;
            public Color Color { get; set; } = Color.White;
            public int Width => SourceRectangle.Width * 2;
            public int Height => SourceRectangle.Height * 2;
            public float Scale { get; set; } = 1;
            public bool IsAnimated { get; set; }
            public bool IsRippleEffect { get; set; } = false;
            public int TimeToLive { get; set; } = -1;
            public bool IsImmuneToTimeEffects { get; internal set; }
            public bool IsText { get; set; }
            public string Text { get; set; }

            public bool IsDead()
            {
                if (Opacity <= 0)
                {
                    return true;
                }
                if (TimeToLive < 0)
                    return true;
                return false;
            }

            public void Reset()
            {
                Scale = 1;
                Opacity = 1;
                IsAnimated = false;
                IsRippleEffect = false;
                Color = Color.White;
                _frameChange = 0;
                _frames = 1;
                _currentFrame = 0;
                TimeToLive = 60 * 5;
                _animationTimer.Reset();
                IsText = false;
                Text = null;
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
                    case ParticleType.Snow:
                        NoOpacityDefaultBehavior();
                        break;
                    case ParticleType.Rain:
                        NoOpacityDefaultBehavior();
                        break;
                    case ParticleType.RewindFire:
                        NoOpacityDefaultBehavior();
                        break;
                    case ParticleType.FireBall:
                        NoOpacityDefaultBehavior();
                        break;
                    case ParticleType.Explosion:
                        NoOpacityDefaultBehavior();
                        break;
                    case ParticleType.TilePiece:
                        GravityDefaultBehavior();
                        break;
                    case ParticleType.SplashText:
                        GravityDefaultBehavior();
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
                        SourceRectangle = new Rectangle(SourceRectangle.X + SourceRectangle.Width, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
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
                Velocity = new Vector2(Velocity.X, Velocity.Y + TMBAW_Game.Gravity);
                DefaultBehavior();
            }

            protected void NoOpacityDefaultBehavior()
            {
                Position += Velocity;
                TimeToLive--;
            }

            protected void GravityNoOpacityDefaultBehavior()
            {
                Position = new Vector2(Position.X, Position.Y + TMBAW_Game.Gravity);
                NoOpacityDefaultBehavior();
            }

            public virtual void Draw(SpriteBatch spriteBatch)
            {
                if (IsText && Text != null)
                {
                    FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[2], Text, Position, 1, Color, Color.Black);
                }
                else if (_texture != null)
                    spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, (int)(Width * Scale), (int)(Height * Scale)), SourceRectangle, Color * Opacity);

            }

        }
    }
}
