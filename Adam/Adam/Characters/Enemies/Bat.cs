using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    class Bat : Enemy, IAnimated
    {
        bool isLookingForRefuge;
        bool isSleeping;

        Rectangle rangeRect;
        Vector2 maxVelocity;

        public override byte ID
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
                    _respawnRect = collRectangle;
                }
                return _respawnRect;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDB.Bat_MaxHealth;
            }
        }

        SoundFx meanSound;
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
                return new Rectangle(collRectangle.X -16,collRectangle.Y,64,64);
            }
        }

        Animation _animation;
        public Animation Animation
        {
            get
            {
                if (_animation == null)
                {
                    _animation = new Animation(Texture, DrawRectangle, sourceRectangle);
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
            collRectangle = new Rectangle(x, y, 32, 32);
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            maxVelocity = new Vector2(2, 2);
            Texture = ContentHelper.LoadTexture("Enemies/bat");

            CollidedWithTileAbove += OnCollisionWithTerrainAbove;
        }

        public void OnCollisionWithTerrainAbove(Entity entity, Tile tile)
        {
            if (isLookingForRefuge)
            {
                isSleeping = true;
            }
            else
            {
                velocity.Y = 0;
            }
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.GetPlayer();

            rangeRect = new Rectangle(collRectangle.X - 100, collRectangle.Y - 100, collRectangle.Width + 200, collRectangle.Height + 200);

            if (player.GetCollRectangle().Intersects(rangeRect))
            {
                isSleeping = false;
                isLookingForRefuge = false;
            }
            else
            {
                isLookingForRefuge = true;
            }

            if (!isLookingForRefuge)
            {
                int buffer = 5;
                if (collRectangle.Y < player.GetCollRectangle().Y - buffer)
                {
                    velocity.Y = maxVelocity.Y;
                }
                else if (collRectangle.Y > player.GetCollRectangle().Y + buffer)
                {
                    velocity.Y = -maxVelocity.Y;
                }
                else
                {
                    velocity.Y = 0;
                }

                if (collRectangle.X < player.GetCollRectangle().X - buffer)
                {
                    velocity.X = maxVelocity.X;
                }
                else if (collRectangle.X > player.GetCollRectangle().X + buffer)
                {
                    velocity.X = -maxVelocity.X;
                }
                else
                {
                    velocity.X = 0;
                }
            }
            else
            {
                velocity.X = 0;
                velocity.Y = -maxVelocity.Y;
            }

            if (isSleeping)
            {
                CurrentAnimationState = AnimationState.Sleeping;
                velocity = Vector2.Zero;
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
