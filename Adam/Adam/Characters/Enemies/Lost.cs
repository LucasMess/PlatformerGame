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

namespace Adam
{
    public class Lost : Enemy, IAnimated
    {
        SoundFx ghost, ghost2;
        double ghostTimer;
        int timerEnd;
        Vector2 maxVelocity;

        public override byte ID
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
                    _respawnRect = collRectangle;
                }
                return _respawnRect;
            }
        }


        public override int MaxHealth
        {
            get
            {
                return EnemyDB.Lost_MaxHealth;
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

        Animation animation;
        public Animation Animation
        {
            get
            {
                if (animation == null)
                    animation = new Animation(Texture, DrawRectangle, sourceRectangle);
                return animation;
            }
        }

        AnimationData[] animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (animationData == null)
                    animationData = new Adam.AnimationData[]
                    {
                        new AnimationData(250,4,0,AnimationType.Loop),
                    };
                return animationData;
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
                return new Rectangle(collRectangle.X - 8, collRectangle.Y - 12, 48, 80);
            }
        }

        public Lost(int x, int y)
        {
            maxVelocity = new Vector2(1, 1);

            Texture = ContentHelper.LoadTexture("Enemies/lost");
            ghost = new SoundFx("Sounds/Lost/ghost");
            ghost2 = new SoundFx("Sounds/Lost/ghost2");

            collRectangle = new Rectangle(x, y, 48 - 8, 80 - 12);
            sourceRectangle = new Rectangle(0, 0, 24, 40);

            timerEnd = GameWorld.RandGen.Next(3, 8);
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.GetPlayer();
            GameTime gameTime = GameWorld.Instance.GetGameTime();

            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

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

            // Set the opacity back to normal before checking if is hiding;
            Opacity = 1f;
            CurrentAnimationState = AnimationState.Flying;

            if (IsPlayerToTheRight() && !player.IsFacingRight)
            {
                CurrentAnimationState = AnimationState.Flying;
                velocity = new Vector2(0, 0);
                Opacity = .5f;
            }
            if (!IsPlayerToTheRight() && player.IsFacingRight)
            {
                CurrentAnimationState = AnimationState.Flying;
                velocity = new Vector2(0, 0);
                Opacity = .5f;
            }

            IsFacingRight = IsPlayerToTheRight();

            ghostTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (ghostTimer > timerEnd)
            {
                ghostTimer = 0;
                if (GameWorld.RandGen.Next(0, 2) == 0)
                {
                    ghost.PlayIfStopped();
                }
                else ghost2.PlayIfStopped();
            }

            base.Update();

        }

        void IAnimated.Animate()
        {
            animation.Update(GameWorld.Instance.gameTime, DrawRectangle, animationData[0]);
        }
    }
}
