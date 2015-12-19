using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Interactables
{
    public class CheckPoint : Entity, IAnimated
    {
        AnimationData _opening;
        SoundFx _quack, _openSound;
        bool _isOpen;

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
                        new AnimationData(250,4,0,AnimationType.PlayOnce),
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
                return new Rectangle(CollRectangle.X + 50, CollRectangle.Y, 32, 96);
            }
        }

        public CheckPoint(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Objects/checkPoint");
            SourceRectangle = new Rectangle(0, 0, 16, 48);
            CollRectangle = new Rectangle(x - 50, y - Main.Tilesize * 2, 100, DrawRectangle.Height);

            _opening = new AnimationData(32, 4, 0, AnimationType.PlayOnce);
            _animation = new Animation(Texture, DrawRectangle, SourceRectangle);

            _quack = new SoundFx("Backgrounds/Splash/quack");
            _openSound = new SoundFx("Sounds/Menu/checkPoint");
        }
        public override void Update()
        {
            if (GameWorld.Instance.Player.GetCollRectangle().Intersects(CollRectangle))
            {
                if (!_isOpen)
                {
                    Open();
                }
            }

            if (_isOpen)
            {
                _animation.Update(GameWorld.Instance.GameTime, DrawRectangle, _opening);
            }
        }

        private void Open()
        {
            //Closes all other checkpoints.
            for (int i = 0; i < GameWorld.Instance.Entities.Count; i++)
            {
                Entity en = GameWorld.Instance.Entities[i];
                if (en is CheckPoint)
                {
                    if (en == this)
                        continue;
                    CheckPoint ch = (CheckPoint)en;
                    ch.Close();
                }
            }

            //Open this checkpoint.
            _isOpen = true;
            _quack.PlayIfStopped();
            _openSound.PlayIfStopped();

            //Sets respawn point;
            Player.Player player = GameWorld.Instance.Player;
            player.RespawnPos = new Vector2(this.DrawRectangle.X, this.DrawRectangle.Y);

            //Particle effects
            for (int i = 0; i < 100; i++)
            {
                Particle par = new Particle();
                par.CreateSparkles(this);
                GameWorld.Instance.Particles.Add(par);
            }

        }

        public void Close()
        {
            _isOpen = false;
            _opening.Reset();
            _animation = new Animation(Texture, DrawRectangle, SourceRectangle);
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
