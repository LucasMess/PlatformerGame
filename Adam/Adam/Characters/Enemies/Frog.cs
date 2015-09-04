using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    public class Frog : Enemy, ICollidable, INewtonian, IAnimated
    {
        double jumpTimer;
        SoundFx jumpSound;

        public Frog(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Enemies/frog");
            collRectangle = new Rectangle(x, y, 32, 32);            
            sourceRectangle = new Rectangle(0, 0, 24, 32);

            jumpSound = new SoundFx("Sounds/Frog/frog_jump", this);

        }

        public override void Update()
        {
            if (IsDead()) return;

            GameTime gameTime = GameWorld.Instance.GetGameTime();

            Jump();

            base.Update();

        }

        private void Jump()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            jumpTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (jumpTimer > 3)
            {
                if (!IsJumping)
                {
                    if (GameWorld.RandGen.Next(0, 2) == 0)
                        velocity.X = 2f;
                    else velocity.X = -2f;

                    jumpTimer = 0;
                    velocity.Y = -11f;
                    collRectangle.Y -= 1;
                    IsJumping = true;
                    animationData[1].Reset();
                    CurrentAnimationState = AnimationState.Jumping;
                    jumpSound.Play();
                }
            }

        }


        void ICollidable.OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0f;
            velocity.X = 0;
        }

        void ICollidable.OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0f;
            IsJumping = false;
            IsFlying = false;
            CurrentAnimationState = AnimationState.Still;
            velocity.X = 0;
        }

        void ICollidable.OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            if (Math.Abs(velocity.Y) < 1)
                CurrentAnimationState = AnimationState.Still;
        }

        void ICollidable.OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            if (Math.Abs(velocity.Y) < 1)
                CurrentAnimationState = AnimationState.Still;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {

        }

        void IAnimated.Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    Animation.Update(gameTime, DrawRectangle, animationData[0]);
                    break;
                case AnimationState.Jumping:
                    Animation.Update(gameTime, DrawRectangle, animationData[1]);
                    break;
                default:
                    break;
            }
        }

        public float GravityStrength
        {
            get { return Main.Gravity; }
            set
            {
                GravityStrength = value;
            }
        }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public bool IsAboveTile { get; set; }

        public override byte ID
        {
            get
            {
                return 202;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDB.Frog_MaxHealth;
            }
        }

        SoundFx meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                if (meanSound == null)
                    meanSound = new SoundFx("Sounds/Frog/frog_croak");
                return meanSound;
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
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                        new Adam.AnimationData(250,4,1,AnimationType.PlayInIntervals),
                    };
                return animationData;
            }
        }

        public Misc.Interfaces.AnimationState CurrentAnimationState
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(collRectangle.X - 8, collRectangle.Y - 32, 48, 64);
            }
        }
    }
}
