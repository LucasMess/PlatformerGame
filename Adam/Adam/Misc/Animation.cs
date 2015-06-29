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
        public Rectangle drawRectangle, sourceRectangle;

        public int switchFrame, restart;
        int currentFrameCount;
        double frameTimer;
        double waitTimer;
        public bool canStart;
        public bool isFlipped;
        Vector2 frameCount;
        public Color Color = Color.White;

        AnimationType type;

        public Animation(Texture2D texture, Rectangle drawRectangle, int switchFrame, int restart, AnimationType type)
        {
            this.type = type;
            sourceRectangle = new Rectangle(0, 0, drawRectangle.Width, drawRectangle.Height);
            this.texture = texture;
            this.drawRectangle = drawRectangle;
            this.switchFrame = switchFrame;
            this.restart = restart;
            frameCount = new Vector2(texture.Width / drawRectangle.Width, texture.Height / drawRectangle.Height);
        }

        /// <summary>
        /// For complex character animations
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="drawRectangle"></param>
        /// <param name="sourceRectangle"></param>
        public Animation(Texture2D texture, Rectangle drawRectangle, Rectangle sourceRectangle)
        {
            this.texture = texture;
            this.drawRectangle = drawRectangle;
            this.sourceRectangle = sourceRectangle;
        }

        /// <summary>
        /// For complex character animations.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="drawRectangle"></param>
        /// <param name="animationData"></param>
        public void Update(GameTime gameTime, Rectangle drawRectangle, AnimationData animationData)
        {
            this.drawRectangle = drawRectangle;
            sourceRectangle.Y = animationData.StartingY * sourceRectangle.Height;
            animationData.FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;         

            switch (animationData.AnimationType)
            {
                case AnimationType.Loop:
                    if (animationData.FrameTimer > animationData.SwitchFrame)
                    {
                        animationData.FrameTimer = 0;
                        animationData.CurrentFrame++;
                        sourceRectangle.X = animationData.CurrentFrame * sourceRectangle.Width;
                        if (animationData.CurrentFrame > animationData.FrameCount.X)
                        {
                            animationData.CurrentFrame = 0;
                            sourceRectangle.X = 0;
                        }
                    }
                    break;
                case AnimationType.PlayOnce:
                    if (animationData.FrameTimer > animationData.SwitchFrame && !animationData.Done)
                    {
                        animationData.FrameTimer = 0;
                        animationData.CurrentFrame++;
                        sourceRectangle.X = animationData.CurrentFrame * sourceRectangle.Width;
                        if (animationData.CurrentFrame >= animationData.FrameCount.X)
                        {
                            animationData.Done = true;
                        }
                    }
                    break;
            }
            

        }

        public void Update(GameTime gameTime, Rectangle rectangle)
        {
            this.drawRectangle = rectangle;
            Update(gameTime);
        }

        /// <summary>
        /// Use this to update the animation sprite's position
        /// </summary>
        /// <param name="rectangle"></param>
        public void UpdateRectangle(Rectangle rectangle)
        {
            this.drawRectangle = rectangle;
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
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color);
            else spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
        }



    }

    class AnimationData
    {
        public AnimationData(int switchFrame, int frames, int startingY, AnimationType type)
        {
            this.SwitchFrame = switchFrame;
            this.FrameCount = new Vector2(frames - 1, 0);
            this.StartingY = startingY;
        }

        public AnimationType AnimationType { get; set; }
        public int SwitchFrame { get; set; }
        public int CurrentFrame { get; set; }
        public Vector2 FrameCount { get; set; }
        public double FrameTimer { get; set; }
        public int StartingY { get; set; }
        public bool Done { get; set; }

        public void Reset()
        {
            CurrentFrame = 0;
            Done = false;
        }

    }
}
