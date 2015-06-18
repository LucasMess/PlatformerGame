using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public enum AnimationType { Loop, PlayOnce, PlayInIntervals, SlowPanVertical }

    class Animation
    {
        public Texture2D texture;
        public Rectangle rectangle, sourceRectangle;

        public int switchFrame, restart;
        int currentFrameCount;
        double frameTimer;
        double waitTimer;
        public bool canStart;
        public bool isFlipped;
        Vector2 frameCount;

        AnimationType type;

        public Animation(Texture2D texture, Rectangle rectangle, int switchFrame, int restart, AnimationType type)
        {
            this.type = type;
            sourceRectangle = new Rectangle(0, 0, rectangle.Width, rectangle.Height);
            this.texture = texture;
            this.rectangle = rectangle;
            this.switchFrame = switchFrame;
            this.restart = restart;
            frameCount = new Vector2(texture.Width / rectangle.Width, texture.Height / rectangle.Height);
        }

        public void Update(GameTime gameTime, Rectangle rectangle)
        {
            this.rectangle = rectangle;
            Update(gameTime);
        }

        //Use this to update the animation sprite's position
        public void UpdateRectangle(Rectangle rectangle)
        {
            this.rectangle = rectangle;
        }
        
        public void Update(GameTime gameTime)
        {
            switch (type)
            {
                case AnimationType.Loop:
                    frameTimer += gameTime.ElapsedGameTime.Milliseconds;

                    if (frameTimer > switchFrame)
                    {
                        currentFrameCount++;
                        sourceRectangle.X += sourceRectangle.Width;
                        frameTimer = 0;
                    }
                    if (currentFrameCount >= frameCount.X)
                    {
                        currentFrameCount = 0;
                        sourceRectangle.X = 0;
                    }

                    break;
                case AnimationType.PlayInIntervals:
                    frameTimer += gameTime.ElapsedGameTime.Milliseconds;
                    waitTimer += gameTime.ElapsedGameTime.Milliseconds;

                    if (waitTimer > restart)
                    {
                        if (frameTimer > switchFrame)
                        {
                            currentFrameCount++;
                            sourceRectangle.X += sourceRectangle.Width;
                            frameTimer = 0;
                        }
                        if (currentFrameCount >= frameCount.X)
                        {
                            currentFrameCount = 0;
                            sourceRectangle.X = 0;
                            waitTimer = 0;
                        }
                    }

                    break;
                case AnimationType.PlayOnce:

                    break;
                case AnimationType.SlowPanVertical:
                    if (canStart)
                    {
                        frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (frameTimer > switchFrame)
                        {
                            frameTimer = 0;
                            sourceRectangle.Y += 1;
                        }
                        if (sourceRectangle.Y >= sourceRectangle.Y * frameCount.Y)
                        {
                            canStart = false;
                        }
                    }
                    break;
            }


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isFlipped)
                spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
            else spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
        }



    }

    struct AnimationData
    {
        public int SwitchFrame { get; set; }
        public int CurrentFrame { get; set; }
        public Vector2 FrameCount { get; set; }
        public double FrameTimer { get; set; }

    }
}
