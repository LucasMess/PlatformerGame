using Adam;
using Adam.Characters.Enemies;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam
{
    public class Lost : Enemy, IAnimated
    {
        SoundFx _ghost, _ghost2;
        double _ghostTimer;
        int _timerEnd;
        Vector2 _maxVelocity;

        public override byte Id
        {
            get
            {
                return 204;
            }
        }

        private Rectangle _respawnRect;
        public override Rectangle RespawnLocation
        {
            get
            {
                if (_respawnRect == new Rectangle(0, 0, 0, 0))
                {
                    _respawnRect = CollRectangle;
                }
                return _respawnRect;
            }
        }


        public override int MaxHealth
        {
            get
            {
                return EnemyDb.LostMaxHealth;
            }
        }

        SoundFx _meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                return null;
            }
        }

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
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X - 8, CollRectangle.Y - 12, 48, 80);
            }
        }

        public Lost(int x, int y)
        {
            _maxVelocity = new Vector2(1, 1);

            Texture = ContentHelper.LoadTexture("Enemies/lost");
            _ghost = new SoundFx("Sounds/Lost/ghost");
            _ghost2 = new SoundFx("Sounds/Lost/ghost2");

            CollRectangle = new Rectangle(x, y, 48 - 8, 80 - 12);
            SourceRectangle = new Rectangle(0, 0, 24, 40);

            _timerEnd = GameWorld.RandGen.Next(3, 8);
        }

        public override void Update()
        {
            Player.Player player = GameWorld.Instance.GetPlayer();
            GameTime gameTime = GameWorld.Instance.GetGameTime();

            CollRectangle.X += (int)Velocity.X;
            CollRectangle.Y += (int)Velocity.Y;

            int buffer = 5;
            if (CollRectangle.Y < player.GetCollRectangle().Y - buffer)
            {
                Velocity.Y = _maxVelocity.Y;
            }
            else if (CollRectangle.Y > player.GetCollRectangle().Y + buffer)
            {
                Velocity.Y = -_maxVelocity.Y;
            }
            else
            {
                Velocity.Y = 0;
            }

            if (CollRectangle.X < player.GetCollRectangle().X - buffer)
            {
                Velocity.X = _maxVelocity.X;
            }
            else if (CollRectangle.X > player.GetCollRectangle().X + buffer)
            {
                Velocity.X = -_maxVelocity.X;
            }
            else
            {
                Velocity.X = 0;
            }

            // Set the opacity back to normal before checking if is hiding;
            Opacity = 1f;
            CurrentAnimationState = AnimationState.Flying;

            if (IsPlayerToTheRight() && !player.IsFacingRight)
            {
                CurrentAnimationState = AnimationState.Flying;
                Velocity = new Vector2(0, 0);
                Opacity = .5f;
            }
            if (!IsPlayerToTheRight() && player.IsFacingRight)
            {
                CurrentAnimationState = AnimationState.Flying;
                Velocity = new Vector2(0, 0);
                Opacity = .5f;
            }

            IsFacingRight = IsPlayerToTheRight();

            _ghostTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_ghostTimer > _timerEnd)
            {
                _ghostTimer = 0;
                if (GameWorld.RandGen.Next(0, 2) == 0)
                {
                    _ghost.PlayIfStopped();
                }
                else _ghost2.PlayIfStopped();
            }

            base.Update();

        }

        void IAnimated.Animate()
        {           
            _animation.Update(GameWorld.Instance.GameTime, DrawRectangle, _animationData[0]);
        }
    }
}
