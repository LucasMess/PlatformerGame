using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    class Bat : Enemy, IAnimated
    {
        bool _isLookingForRefuge;
        bool _isSleeping;

        Rectangle _rangeRect;
        Vector2 _maxVelocity;

        public override byte Id
        {
            get
            {
                return 207;
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
                return EnemyDb.BatMaxHealth;
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

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X -16,CollRectangle.Y,64,64);
            }
        }

        Animation _animation;
        public Animation Animation
        {
            get
            {
                if (_animation == null)
                {
                    _animation = new Animation(Texture, DrawRectangle, SourceRectangle);
                }
                return _animation;
            }
        }

        AnimationData[] _animationData;
        public AnimationData[] AnimationData
        {
            get
            {
               if (_animationData == null)
                {
                    _animationData = new Adam.AnimationData[]
                    {
                        new Adam.AnimationData(200,5,0,AnimationType.Loop),
                        new Adam.AnimationData(85,5,1,AnimationType.Loop),
                    };
                }
                return _animationData;
            }
        }

        public AnimationState CurrentAnimationState
        {
            get; set;
        }

        public Bat(int x, int y)
        {
            CollRectangle = new Rectangle(x, y, 32, 32);
            SourceRectangle = new Rectangle(0, 0, 32, 32);
            _maxVelocity = new Vector2(2, 2);
            Texture = ContentHelper.LoadTexture("Enemies/bat");

            CollidedWithTileAbove += OnCollisionWithTerrainAbove;
        }

        public void OnCollisionWithTerrainAbove(Entity entity, Tile tile)
        {
            if (_isLookingForRefuge)
            {
                _isSleeping = true;
            }
            else
            {
                Velocity.Y = 0;
            }
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.GetPlayer();

            _rangeRect = new Rectangle(CollRectangle.X - 100, CollRectangle.Y - 100, CollRectangle.Width + 200, CollRectangle.Height + 200);

            if (player.GetCollRectangle().Intersects(_rangeRect))
            {
                _isSleeping = false;
                _isLookingForRefuge = false;
            }
            else
            {
                _isLookingForRefuge = true;
            }

            if (!_isLookingForRefuge)
            {
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
            }
            else
            {
                Velocity.X = 0;
                Velocity.Y = -_maxVelocity.Y;
            }

            if (_isSleeping)
            {
                CurrentAnimationState = AnimationState.Sleeping;
                Velocity = Vector2.Zero;
            }
            else
            {
                CurrentAnimationState = AnimationState.Flying;
            }

            base.Update();
        }

        public void Animate()
        {
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    break;
                case AnimationState.Walking:
                    break;
                case AnimationState.Jumping:
                    break;
                case AnimationState.Charging:
                    break;
                case AnimationState.Talking:
                    break;
                case AnimationState.Sleeping:
                    Animation.Update(Main.GameTime, DrawRectangle, AnimationData[0]);
                    break;
                case AnimationState.Flying:
                    Animation.Update(Main.GameTime, DrawRectangle, AnimationData[1]);
                    break;
                case AnimationState.Transforming:
                    break;
                default:
                    break;
            }
        }
    }
}
