using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    public class Frog : Enemy, ICollidable, INewtonian
    {

        enum AnimationState
        {
            Still, Jumping
        }
        AnimationState CurrentAnimation = AnimationState.Still;
        AnimationData still, jumping;
        double jumpTimer;
        SoundFx jumpSound;

        public Frog(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Enemies/frog");
            collRectangle = new Rectangle(x, y, 32, 32);
            drawRectangle = new Rectangle(x - 8, y - 32, 48, 64);
            sourceRectangle = new Rectangle(0, 0, 24, 32);

            still = new AnimationData(250, 4, 0, AnimationType.Loop);
            jumping = new AnimationData(125, 4, 1, AnimationType.PlayOnce);
            animation = new Animation(Texture, drawRectangle, sourceRectangle);

            jumpSound = new SoundFx("Sounds/Frog/frog_jump", this);
           
        }

        public override void Update()
        {
            if (isDead) return;

            drawRectangle.X = collRectangle.X - 8;
            drawRectangle.Y = collRectangle.Y - 32;

            damageBox = new Rectangle(collRectangle.X, collRectangle.Y - 10, collRectangle.Width, 10);

            animation.Update(gameTime, drawRectangle, still);

            Jump();
            Animate();

            base.Update();

        }

        private void Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            switch (CurrentAnimation)
            {
                case AnimationState.Still:
                    animation.Update(gameTime, drawRectangle, still);
                    break;
                case AnimationState.Jumping:
                    animation.Update(gameTime, drawRectangle, jumping);
                    break;
                default:
                    break;
            }
        }

        private void Jump()
        {
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
                    jumping.Reset();
                    CurrentAnimation = AnimationState.Jumping;
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
            CurrentAnimation = AnimationState.Still;
            velocity.X = 0;
        }

        void ICollidable.OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            if (Math.Abs(velocity.Y) < 1)
                CurrentAnimation = AnimationState.Still;
        }

        void ICollidable.OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            if (Math.Abs(velocity.Y) < 1)
                CurrentAnimation = AnimationState.Still;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {

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

        protected override int MaxHealth
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
    }
}
