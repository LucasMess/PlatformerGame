using Adam;
using Microsoft.Xna.Framework;
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

        public Lost(int x, int y)
        {
            health = EnemyDB.Shade_MaxHealth;
            maxVelocity = new Vector2(2, 2);

            texture = Content.Load<Texture2D>("Enemies/BlueShade_Single");

            drawRectangle = new Rectangle(x, y, 48, 80);
            collRectangle = new Rectangle(x, y, 48, 80);
            sourceRectangle = new Rectangle(0, 0, 48, 80);

            Initialize();
        }

        public override void Update(Player player, GameTime gameTime)
        {
            base.Update(player, gameTime);
            Animate(gameTime);
            DetectCollision();

            if (!isInRange)
                return;

            //Calculating unit vector for velocity of shade
            double xVector = (double)(player.collRectangle.Center.X - collRectangle.Center.X);
            double yVector = (double)(player.collRectangle.Center.Y - collRectangle.Center.Y);

            double magnitude = Math.Sqrt((Math.Pow(xVector, 2.0)) + (Math.Pow(yVector, 2.0)));
            Vector2 newVelocity = new Vector2(maxVelocity.X * (float)(xVector / magnitude), maxVelocity.Y * (float)(yVector / magnitude));
            //newVelocity = new Vector2(1, 1);
            
            velocity = newVelocity;

            drawRectangle = collRectangle;

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

        }

        private void Animate(GameTime gameTime)
        {
            switch (CurrentAnimationState)
            {
                case AnimationState.Flying:

                    break;
                case AnimationState.Hiding:

                    break;
            }
        }

        private void DetectCollision()
        {
            if (player.collRectangle.Intersects(drawRectangle) && !isDead && !player.isInvincible && !player.isInvulnerable && !player.isGhost)
            {
                player.TakeDamageAndKnockBack(EnemyDB.Shade_TouchDamage);
                this.Kill();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isInRange)
                return;
            if (isDead)
                return;

            if (isFacingRight)
            {
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            }
            else spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
        }
    }
}
