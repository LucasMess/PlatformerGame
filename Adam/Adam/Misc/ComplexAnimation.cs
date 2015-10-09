using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    /// <summary>
    /// Used for complex aniamtions that contain several different textures, states and variables.
    /// </summary>
    public class ComplexAnimation
    {
        ComplexAnimData currentAnimationData;
        int currentFrame;

        public delegate void FrameHandler(FrameArgs e);
        public delegate void EventHandler();
        
        /// <summary>
        /// Fires whenever the animation data was switched to another one.
        /// </summary>
        public event EventHandler AnimationStateChanged;
        /// <summary>
        /// Fires whenever the aniamtion has reached the end of its loop and it is not a repeating animation.
        /// </summary>
        public event EventHandler AnimationEnded;
        /// <summary>
        /// Fires whenever the frame was incremented to the next available frame.
        /// </summary>
        public event FrameHandler FrameChanged;

        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        Dictionary<string, ComplexAnimData> animationData = new Dictionary<string, ComplexAnimData>();

        Timer frameTimer = new Timer();

        Texture2D texture;
        Rectangle sourceRectangle;
        Rectangle drawRectangle;
        Rectangle collRectangle;

        /// <summary>
        /// Update the aniamtion, timers and fire events.
        /// </summary>
        /// <param name="collRectangle"></param>
        public void Update(Rectangle collRectangle)
        {

            frameTimer.Increment();

            if (frameTimer.TimeElapsedInMilliSeconds > currentAnimationData.Speed)
            {
                frameTimer.Reset();
                currentFrame++;

                if (currentFrame >= currentAnimationData.FrameCount)
                {
                    // Send notice that animation has ended.
                    if (currentAnimationData.IsRepeating)
                    {
                        AnimationEnded();
                    }

                    currentFrame = 0;
                }

                FrameChanged(new FrameArgs(currentFrame));
            }

            sourceRectangle.X = currentFrame * currentAnimationData.Width;

        }

        /// <summary>
        /// Change the current animation to the specified animation.
        /// </summary>
        /// <param name="name">The unique identifier of the animation.</param>
        public void ChangeAnimation(string name)
        {
            ComplexAnimData animData;
            if (!animationData.TryGetValue(name,out animData))
            {
                throw new Exception("Animation not found.");
            }

            // If the current animation has a larger priority, do not change the animation.
            if (animData.Priority < currentAnimationData.Priority)
                return;

            // Reset animation and change current animation being used.
            currentFrame = 0;
            frameTimer.Reset();
            currentAnimationData = animData;

            // Switch to the new texture and size.
            texture = currentAnimationData.Texture;
            sourceRectangle.Width = currentAnimationData.Width;
            sourceRectangle.Height = currentAnimationData.Height;
            sourceRectangle.Y = currentAnimationData.StartingY;

            AnimationStateChanged();

        }

        /// <summary>
        /// Draws the animated texture with the specified color and flip.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="isFlipped">Whether the animation should be flipped horizontally or not.</param>
        /// <param name="color">The color and opacity of the texture.</param>
        public void Draw(SpriteBatch spriteBatch, bool isFlipped, Color color)
        {
            if (isFlipped)
            {
                spriteBatch.Draw(currentAnimationData.Texture, drawRectangle, sourceRectangle, color, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(currentAnimationData.Texture, drawRectangle, sourceRectangle, color, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Add an animation data to the list of avalaible ADs.
        /// </summary>
        /// <param name="name">The unique name that identifies this data.</param>
        /// <param name="data">The data that is linked to that name.</param>
        public void AddAnimationData(string name, ComplexAnimData data)
        {
            // Check to see if identifier already exists.
            ComplexAnimData value;
            if (animationData.TryGetValue(name, out value))
            {
                throw new Exception("Identifier for this animation data already exists.");
            }

            animationData.Add(name, data);
        }

    }

    /// <summary>
    /// Used to give the current frame of an animation when an event is fired.
    /// </summary>
    public class FrameArgs
    {
        public int CurrentFrame
        {
            get; set;
        }

        public FrameArgs(int currentFrame)
        {
            CurrentFrame = currentFrame;
        }

    }

    /// <summary>
    /// Used to contain the data that is tied to an animation.
    /// </summary>
    public class ComplexAnimData
    {
        public Texture2D Texture
        {
            get; set;
        }
        public int StartingY
        {
            get; set;
        }
        public int Width
        {
            get; set;
        }
        public int Height
        {
            get; set;
        }
        public int Speed
        {
            get; set;
        }
        public int FrameCount
        {
            get; set;
        }
        public bool IsRepeating
        {
            get; set;
        }
        public Rectangle DeltaRectangle
        {
            get; set;
        }
        public float Priority
        {
            get; set;
        }

        /// <summary>
        /// Creates a new object containing the data required to animate.
        /// </summary>
        /// <param name="priority">Animatins with larger priorities cancel this animation.</param>
        /// <param name="texture">Texture where animation frames are.</param>
        /// <param name="deltaRectangle">The rectangle inside the source rectangle that defines the drawRectangle.</param>
        /// <param name="startingY">The Y-coordinates of the first frame.</param>
        /// <param name="width">The width of each frame.</param>
        /// <param name="height">The height of each frame.</param>
        /// <param name="speed">The duration of each frame.</param>
        /// <param name="frameCount">The number of frames in this animation.</param>
        /// <param name="isRepeating">Whether the animation should loop.</param>
        public ComplexAnimData(int priority, Texture2D texture, Rectangle deltaRectangle, int startingY, int width, int height, int speed,int frameCount, bool isRepeating)
        {
            Priority = priority;
            Texture = texture;
            DeltaRectangle = deltaRectangle;
            StartingY = startingY;
            Width = width;
            Height = height;
            Speed = speed;
            FrameCount = frameCount;
            IsRepeating = isRepeating;
        }
    }
}
