using Adam;
using Adam.Misc;
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
    public class Lost : Enemy
    {
        enum AnimationState
        {
            Hiding,
            Flying,
        }
        AnimationState CurrentAnimationState = AnimationState.Hiding;
        AnimationData still;
        SoundFx ghost, ghost2;
        double ghostTimer;
        int timerEnd;

        public Lost(int x, int y)
        {
            health = EnemyDB.Shade_MaxHealth;
            maxVelocity = new Vector2(1, 1);

            texture = Content.Load<Texture2D>("Enemies/lost");
            ghost = new SoundFx("Lost/ghost");
            ghost2 = new SoundFx("Lost/ghost2");
            deathSound = ContentHelper.LoadSound("Lost/scream");
            deathSoundInstance = deathSound.CreateInstance();

            collRectangle = new Rectangle(x, y, 48 - 8, 80 - 12);
            drawRectangle = new Rectangle(collRectangle.X - 8, collRectangle.Y - 12, 48, 80);
            sourceRectangle = new Rectangle(0, 0, 24, 40);

            still = new AnimationData(200, 4, 0, AnimationType.Loop);
            animation = new Animation(texture, drawRectangle, sourceRectangle);

            timerEnd = GameWorld.RandGen.Next(3, 8);

            Initialize();
        }

        public override void Update(Player player, GameTime gameTime)
        {
            DetectCollision();
            base.Update(player, gameTime);
            Animate(gameTime);


            if (!isInRange)
                return;
            if (isDead)
                return;

            //Calculating unit vector for velocity of shade
            //double xVector = (double)(player.collRectangle.Center.X - collRectangle.Center.X);
            //double yVector = (double)(player.collRectangle.Center.Y - collRectangle.Center.Y);

            //double magnitude = Math.Sqrt((Math.Pow(xVector, 2.0)) + (Math.Pow(yVector, 2.0)));
            //Vector2 newVelocity = new Vector2(maxVelocity.X * (float)(xVector / magnitude), maxVelocity.Y * (float)(yVector / magnitude));

            //velocity = newVelocity;

            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            int buffer = 5;
            if (collRectangle.Y < player.collRectangle.Y - buffer)
            {
                velocity.Y = maxVelocity.Y;
            }
            else if (collRectangle.Y > player.collRectangle.Y + buffer)
            {
                velocity.Y = -maxVelocity.Y;
            }
            else
            {
                velocity.Y = 0;
            }

            if (collRectangle.X < player.collRectangle.X - buffer)
            {
                velocity.X = maxVelocity.X;
            }
            else if (collRectangle.X > player.collRectangle.X + buffer)
            {
                velocity.X = -maxVelocity.X;
            }
            else
            {
                velocity.X = 0;
            }

            drawRectangle = new Rectangle(collRectangle.X - 8, collRectangle.Y - 12, 48, 80);

            opacity = 1f;
            CurrentAnimationState = AnimationState.Flying;

            if (isPlayerToTheRight && !player.isFacingRight)
            {
                CurrentAnimationState = AnimationState.Hiding;
                velocity = new Vector2(0, 0);
                opacity = .5f;
            }
            if (!isPlayerToTheRight && player.isFacingRight)
            {
                CurrentAnimationState = AnimationState.Hiding;
                velocity = new Vector2(0, 0);
                opacity = .5f;
            }

            isFacingRight = isPlayerToTheRight;

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

        }

        private void Animate(GameTime gameTime)
        {
            animation.Update(gameTime, drawRectangle, still);
        }

        private void DetectCollision()
        {
            if (player.collRectangle.Intersects(drawRectangle) && !isDead && !player.isInvincible && !player.isInvulnerable && !player.isGhost)
            {
                player.TakeDamageAndKnockBack(EnemyDB.Shade_TouchDamage);
                Kill();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isInRange)
                return;
            if (isDead)
                return;
            Color color = Color.White * opacity;
            animation.Color = color;
            animation.Draw(spriteBatch);

            if (isFacingRight)
            {
                animation.isFlipped = false;
                //spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            }
            else
            {
                animation.isFlipped = true;
                // spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}
