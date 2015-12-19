using Adam;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Interactables
{
    public class CdPlayer : Item, IAnimated
    {
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
                    _animationData = new AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                    };
                return _animationData;
            }
        }

        public AnimationState CurrentAnimationState
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public CdPlayer(Vector2 position)
        {
            Texture = ContentHelper.LoadTexture("Objects/CDplayer_new");
            CollRectangle = new Rectangle((int)position.X, (int)position.Y, 32, 32);
            SourceRectangle = new Rectangle(0, 0, 32, 32);

            _animation = new Animation(Texture, DrawRectangle, 70, 0, AnimationType.Loop);
            LoopSound = new Misc.SoundFx("Sounds/loop");
            Velocity.Y = -10f;
        }

        public override void Update()
        {
            GameWorld gameWorld = GameWorld.Instance;
            GameTime gameTime = gameWorld.GameTime;

            Velocity.Y += .3f;
            if (Velocity.Y > 5f)
                Velocity.Y = 5f;

            EffectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EffectTimer > 500)
            {
                Particle eff = new Particle();
                eff.CreateMusicNotesEffect(this);
                GameWorld.Instance.Particles.Add(eff);
                EffectTimer = 0;
            }

            base.Update();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch);
        }

        public void Animate()
        {
            throw new NotImplementedException();
        }
    }
}
