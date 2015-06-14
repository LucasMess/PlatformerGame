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
    class PotatoEnemy : Enemy
    {
        enum AnimationState
        {
            Planted,
            Emerging,
            Walking,
            Planting,
        }

        AnimationState CurrentAnimationState = AnimationState.Planted;
        bool hasEmerged;
        bool hasPlanted;

        public PotatoEnemy(int x, int y, ContentManager Content)
        {
            CurrentEnemyType = EnemyType.Potato;
            health = EnemyDB.Potato_MaxHealth;
            canPassThroughWalls = true;

            texture = Content.Load<Texture2D>("Enemies/Potato_Spritesheet");

            drawRectangle = new Rectangle(x, y, 32, 32);
            collRectangle = new Rectangle(x + 6, y + 8, 20, 22);
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            frameCount = new Vector2(4, 4);

            Initialize();

        }

        public override void Update(Player player, GameTime gameTime)
        {
            base.Update(player, gameTime);
            Animate();

            if (!isInRange)
            {
                CurrentAnimationState = AnimationState.Planted;
                return;
            }

            if (hasEmerged)
            {
                CurrentAnimationState = AnimationState.Walking;
                hasEmerged = false;
            }
            if (hasPlanted)
            {
                CurrentAnimationState = AnimationState.Planted;
                hasPlanted = false;
            }

            if (player.collRectangle.Intersects(collRectangle))
            {
                CurrentAnimationState = AnimationState.Emerging;
            }
        }

        private void Animate()
        {
            switch (CurrentAnimationState)
            {
                case AnimationState.Planted:
                    switchFrame = 0;
                    sourceRectangle = new Rectangle(0, 0, sourceRectangle.Width, sourceRectangle.Height);
                    break;
                case AnimationState.Emerging:
                    switchFrame = 200;
                    sourceRectangle.Y = 0;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer > switchFrame)
                    {
                        sourceRectangle.X += sourceRectangle.Width;
                        frameTimer = 0;
                        currentFrame++;

                        if (currentFrame >= frameCount.X)
                        {
                            currentFrame = 0;
                            sourceRectangle.X = 0;
                            hasEmerged = true;
                        }
                    }

                    break;
                case AnimationState.Walking:
                    switchFrame = 200;
                    sourceRectangle.Y = sourceRectangle.Height;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer > switchFrame)
                    {
                        sourceRectangle.X += sourceRectangle.Width;
                        frameTimer = 0;
                        currentFrame++;

                        if (currentFrame >= frameCount.X)
                        {
                            currentFrame = 0;
                            sourceRectangle.X = 0;
                        }
                    }
                    break;
                case AnimationState.Planting:
                    switchFrame = 200;
                    sourceRectangle.Y = sourceRectangle.Height * 2;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer > switchFrame)
                    {
                        sourceRectangle.X += sourceRectangle.Width;
                        frameTimer = 0;
                        currentFrame++;

                        if (currentFrame >= frameCount.X)
                        {
                            currentFrame = 0;
                            sourceRectangle.X = 0;
                            hasPlanted = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isInRange)
                return;
            if (isDead)
                return;

            if (isFacingRight)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            else spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);

            base.Draw(spriteBatch);
        }
    }
}
