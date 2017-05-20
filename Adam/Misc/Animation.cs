using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay
{
    public enum AnimationType { Loop, PlayOnce, PlayInIntervals, SlowPanVertical }

    public class Animation
    {
        public Texture2D Texture;
        public Rectangle DrawRectangle, SourceRectangle;

        public int SwitchFrame, Restart;
        int _currentFrameCount;
        double _frameTimer;
        double _waitTimer;
        public bool CanStart;
        public bool IsFlipped;
        Vector2 _frameCount;
        public Color Color = Color.White;
        int startingX = 0;

        AnimationType _type;

        public Animation(Texture2D texture, Rectangle drawRectangle, int switchFrame, int restart, AnimationType type)
        {
            this._type = type;
            SourceRectangle = new Rectangle(0, 0, drawRectangle.Width, drawRectangle.Height);
            this.Texture = texture;
            this.DrawRectangle = drawRectangle;
            this.SwitchFrame = switchFrame;
            this.Restart = restart;
            _frameCount = new Vector2(texture.Width / drawRectangle.Width, texture.Height / drawRectangle.Height);
        }

        /// <summary>
        /// For simple animations where the animation texture has other sprites that do not belong to the animation.
        /// </summary>
        /// <param name=""></param>
        /// <param name="drawRectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="switchFrame"></param>
        /// <param name="frameCount"></param>
        /// <param name="restart"></param>
        /// <param name="type"></param>
        public Animation(Texture2D texture, Rectangle drawRectangle, Rectangle sourceRectangle, int switchFrame, int frameCount, int restart, AnimationType type) {
            this._type = type;
            SourceRectangle = sourceRectangle;
            this.Texture = texture;
            this.DrawRectangle = drawRectangle;
            this.SwitchFrame = switchFrame;
            this.Restart = restart;
            startingX = sourceRectangle.X;
            _frameCount = new Vector2(frameCount, 0);
        }

        /// <summary>
        /// For complex character animations
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="drawRectangle"></param>
        /// <param name="sourceRectangle"></param>
        public Animation(Texture2D texture, Rectangle drawRectangle, Rectangle sourceRectangle)
        {
            this.Texture = texture;
            this.DrawRectangle = drawRectangle;
            this.SourceRectangle = sourceRectangle;
        }

        /// <summary>
        /// For complex character animations.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="drawRectangle"></param>
        /// <param name="animationData"></param>
        public void Update(GameTime gameTime, Rectangle drawRectangle, AnimationData animationData)
        {
            this.DrawRectangle = drawRectangle;
            SourceRectangle.Y = animationData.StartingY * SourceRectangle.Height;
            animationData.FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;         

            switch (animationData.AnimationType)
            {
                case AnimationType.Loop:
                    if (animationData.FrameTimer > animationData.SwitchFrame)
                    {
                        animationData.FrameTimer = 0;
                        animationData.CurrentFrame++;
                        SourceRectangle.X = startingX + animationData.CurrentFrame * SourceRectangle.Width;
                        if (animationData.CurrentFrame > animationData.FrameCount.X)
                        {
                            animationData.CurrentFrame = 0;
                        }
                    }
                    break;
                case AnimationType.PlayOnce:
                    if (animationData.FrameTimer > animationData.SwitchFrame && !animationData.Done)
                    {
                        animationData.FrameTimer = 0;
                        animationData.CurrentFrame++;
                        SourceRectangle.X = animationData.CurrentFrame * SourceRectangle.Width;
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
            this.DrawRectangle = rectangle;
            Update(gameTime);
        }

        /// <summary>
        /// Use this to update the animation sprite's position
        /// </summary>
        /// <param name="rectangle"></param>
        public void UpdateRectangle(Rectangle rectangle)
        {
            this.DrawRectangle = rectangle;
        }
        
        public void Update(GameTime gameTime)
        {
            switch (_type)
            {
                case AnimationType.Loop:
                    _frameTimer += gameTime.ElapsedGameTime.Milliseconds;

                    if (_frameTimer > SwitchFrame)
                    {
                        _currentFrameCount++;
                        SourceRectangle.X += SourceRectangle.Width;
                        _frameTimer = 0;
                    }
                    if (_currentFrameCount >= _frameCount.X)
                    {
                        _currentFrameCount = 0;
                        SourceRectangle.X = startingX;
                    }

                    break;
                case AnimationType.PlayInIntervals:
                    _frameTimer += gameTime.ElapsedGameTime.Milliseconds;
                    _waitTimer += gameTime.ElapsedGameTime.Milliseconds;

                    if (_waitTimer > Restart)
                    {
                        if (_frameTimer > SwitchFrame)
                        {
                            _currentFrameCount++;
                            SourceRectangle.X += SourceRectangle.Width;
                            _frameTimer = 0;
                        }
                        if (_currentFrameCount >= _frameCount.X)
                        {
                            _currentFrameCount = 0;
                            SourceRectangle.X = 0;
                            _waitTimer = 0;
                        }
                    }

                    break;
                case AnimationType.PlayOnce:

                    break;
                case AnimationType.SlowPanVertical:
                    if (CanStart)
                    {
                        _frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (_frameTimer > SwitchFrame)
                        {
                            _frameTimer = 0;
                            SourceRectangle.Y += 1;
                        }
                        if (SourceRectangle.Y >= SourceRectangle.Y * _frameCount.Y)
                        {
                            CanStart = false;
                        }
                    }
                    break;
            }


        }



        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsFlipped)
                spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color);
            else spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
        }



    }

    public class AnimationData
    {
        public AnimationData(int switchFrame, int frames, int startingY, AnimationType type)
        {
            this.SwitchFrame = switchFrame;
            this.FrameCount = new Vector2(frames - 1, 0);
            this.StartingY = startingY;
            this.AnimationType = type;
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
