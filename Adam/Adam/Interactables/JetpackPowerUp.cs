using Adam;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Interactables
{
    public class JetpackPowerUp : Item, IAnimated
    {
        GameTime _gameTime;
        bool _isHovering;
        double _hoverTimer;

        Animation _animation;
        public Animation Animation
        {
            get
            {
                if (_animation == null)
                    _animation = new Animation(Texture, DrawRectangle, SourceRectangle);
                return _animation;
            }
        }

        AnimationData[] _animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (_animationData == null)
                    _animationData = new Adam.AnimationData[]
                    {
                        new AnimationData(250,4,0,AnimationType.Loop),
                    };
                return _animationData;
            }
        }

        public AnimationState CurrentAnimationState
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public JetpackPowerUp(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Objects/jetpack");
            LoopSound = new Misc.SoundFx("Sounds/jetpack_engine");
            CollRectangle = new Rectangle(x, y, 32, 32);
            _animation = new Animation(Texture, DrawRectangle, 100, 0, AnimationType.Loop);
            Velocity.Y = -10f;
        }

        public override void Update()
        {
            _gameTime = Main.GameTime;

            _animation.UpdateRectangle(DrawRectangle);
            _animation.Update(_gameTime);

            Hover();

            EffectTimer += _gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EffectTimer > 100)
            {
                Particle eff = new Particle();
                eff.CreateJetPackSmokeParticle(this);
                GameWorld.Particles.Add(eff);
                EffectTimer = 0;
            }

            base.Update();
        }

        private void Hover()
        {
            if (_isHovering)
            {
                _hoverTimer += _gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_hoverTimer > 100)
                {
                    Velocity.Y = -Velocity.Y;
                    _hoverTimer = 0;
                }
            }
            else
            {
                if (Velocity.Y < 0)
                {
                    Velocity.Y += .3f;
                }
                else
                {
                    _isHovering = true;
                    Velocity.Y = 1f;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch);
        }

        public void Animate()
        {
            throw new NotImplementedException();
        }
    }
}
