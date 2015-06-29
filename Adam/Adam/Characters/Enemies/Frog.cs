using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    class Frog : Enemy, ICollidable, INewtonian
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
            texture = ContentHelper.LoadTexture("Enemies/frog");
            collRectangle = new Rectangle(x, y, 32, 32);
            drawRectangle = new Rectangle(x - 8, y - 32, 48, 64);
            sourceRectangle = new Rectangle(0, 0, 24, 32);
            CurrentEnemyType = EnemyType.Frog;
            health = EnemyDB.Frog_MaxHealth;

            still = new AnimationData(250, 4, 0, AnimationType.Loop);
            jumping = new AnimationData(125, 4, 1, AnimationType.PlayOnce);
            animation = new Animation(texture, drawRectangle, sourceRectangle);

            jumpSound = new SoundFx("Sounds/Frog/frog_jump");
            meanSound = ContentHelper.LoadSound("Sounds/Frog/frog_croak");

            Initialize();
           
        }

        public override void Update(Player player, GameTime gameTime)
        {
            if (isDead) return;

            this.gameTime = gameTime;

            if (tookDamage)
                goto BeingHit;

            drawRectangle.X = collRectangle.X - 8;
            drawRectangle.Y = collRectangle.Y - 32;

            damageBox = new Rectangle(collRectangle.X, collRectangle.Y - 10, collRectangle.Width, 10);

            animation.Update(gameTime, drawRectangle, still);

            Jump();
            Animate();

            BeingHit:
            base.Update(player, gameTime);

        }

        private void Animate()
        {
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

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (!isDead)
            {
                if (tookDamage) animation.Color = Color.Red;
                else animation.Color = Color.White;
                animation.Draw(spriteBatch);

               // spriteBatch.Draw(Game1.DefaultTexture, collRectangle, Color.Red);
                //spriteBatch.Draw(Game1.DefaultTexture, xRect, Color.Red);
                //spriteBatch.Draw(Game1.DefaultTexture, yRect, Color.Blue);
            }
        }

        void ICollidable.OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.drawRectangle.Y + e.Tile.drawRectangle.Height;
            velocity.Y = 0f;
            velocity.X = 0;
        }

        void ICollidable.OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.drawRectangle.Y - collRectangle.Height;
            velocity.Y = 0f;
            IsJumping = false;
            IsFlying = false;
            CurrentAnimation = AnimationState.Still;
            velocity.X = 0;
        }

        void ICollidable.OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            collRectangle.X = e.Tile.drawRectangle.X - collRectangle.Width;

            if (Math.Abs(velocity.Y) < 1)
                CurrentAnimation = AnimationState.Still;
        }

        void ICollidable.OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            collRectangle.X = e.Tile.drawRectangle.X + e.Tile.drawRectangle.Width;
            if (Math.Abs(velocity.Y) < 1)
                CurrentAnimation = AnimationState.Still;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {

        }

        public float GravityStrength
        {
            get { return Game1.Gravity; }
        }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public bool IsAboveTile { get; set; }
    }
}
