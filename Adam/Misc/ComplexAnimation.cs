using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Adam.Misc
{
    /// <summary>
    /// Used for complex aniamtions that contain several different textures, states and variables.
    /// </summary>
    public class ComplexAnimation
    {
        ComplexAnimData _currentAnimationData = new ComplexAnimData();
        int _currentFrame;
        string _currentName;
        public bool IsActive { get; set; } = true;

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

        Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        Dictionary<string, ComplexAnimData> _animationData = new Dictionary<string, ComplexAnimData>();
        List<string> _queue = new List<string>();

        Timer _frameTimer = new Timer();

        Texture2D _texture;
        Rectangle _sourceRectangle;
        Rectangle _drawRectangle;

        /// <summary>
        /// Update the aniamtion, timers and fire events.
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Entity entity)
        {
            //SpeedParticle speed = new SpeedParticle(texture, drawRectangle.X, drawRectangle.Y, sourceRectangle, entity.IsFacingRight);
            //GameWorld.ParticleSystem.Add(speed);

            FindHighestPriorityAnimation();

            _drawRectangle = new Rectangle(entity.GetCollRectangle().X - _currentAnimationData.DeltaRectangle.X, entity.GetCollRectangle().Y - _currentAnimationData.DeltaRectangle.Y, _currentAnimationData.Width * 2, _currentAnimationData.Height * 2);

            if (_currentName == "walk" || _currentName == "run")
            {
                // y = 1020/(x + 1) - 20
                _currentAnimationData.Speed = (int)(-20 + 500f / (Math.Abs(entity.GetVelocity().X) + 1));
            }
            if (_currentName == "climb")
            {
                _currentAnimationData.Speed = (int)(-20 + 1020f / (Math.Abs(entity.GetVelocity().Y) + 1));
            }

           

            if (_frameTimer.TimeElapsedInMilliSeconds > _currentAnimationData.Speed)
            {
                _frameTimer.Reset();
                _currentFrame++;

                if (_currentFrame >= _currentAnimationData.FrameCount)
                {
                    AnimationEnded?.Invoke();
                    // Send notice that animation has ended.
                    if (!_currentAnimationData.IsRepeating)
                    { 
                        _currentFrame = _currentAnimationData.FrameCount - 1;
                    }
                    else
                    {
                        _currentFrame = 0;
                    }
                }
                FrameChanged?.Invoke(new FrameArgs(_currentFrame));
            }

            _sourceRectangle.X = _currentFrame * _currentAnimationData.Width;

        }

        /// <summary>
        /// Iterate through all animations that have been added to the queue. Choose the animation which has highest priority then empty the queue.
        /// </summary>
        private void FindHighestPriorityAnimation()
        {
            if (_queue.Count == 0)
                return;

            float highestPriority = 0;
            string best = "";

            //Console.WriteLine("There are {0} animations in queue: {1}", queue.Count, queue);

            foreach (string s in _queue)
            {
                if (CheckIfHighestPriority(s, ref highestPriority))
                {
                    best = s;
                }
            }
            ChangeAnimation(best);
        }

        /// <summary>
        /// Check if this animation is higher priority.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="highestPriority"></param>
        /// <returns></returns>
        private bool CheckIfHighestPriority(string name, ref float highestPriority)
        {
            ComplexAnimData animData;
            if (!_animationData.TryGetValue(name, out animData))
            {
                //throw new Exception("Animation not found.");
                return false;
            }

            // If the current animation has a larger priority, do not change the animation.
            if (animData.Priority < highestPriority)
            {
                return false;
            }
            else
            {
                highestPriority = animData.Priority;
                return true;
            }
        }

        /// <summary>
        /// Change the current animation to the specified animation.
        /// </summary>
        /// <param name="name">The unique identifier of the animation.</param>
        private void ChangeAnimation(string name)
        {
            ComplexAnimData animData;
            if (!_animationData.TryGetValue(name, out animData))
            {
                throw new Exception("Animation not found.");
            }

            if (name == _currentName)
                return;

            // Reset animation and change current animation being used.
            _currentFrame = 0;
            _frameTimer.Reset();
            _currentAnimationData = animData;
            _currentName = name;

            // Switch to the new texture and size.
            _texture = _currentAnimationData.Texture;
            _sourceRectangle.Width = _currentAnimationData.Width;
            _sourceRectangle.Height = _currentAnimationData.Height;
            _sourceRectangle.Y = _currentAnimationData.StartingY;

            if (AnimationStateChanged != null)
                AnimationStateChanged();

        }

        /// <summary>
        /// Adds an animation to the list of animations that want to be played.
        /// </summary>
        /// <param name="name"></param>
        public void AddToQueue(string name)
        {
            if (!_queue.Contains(name))
                _queue.Add(name);
        }

        /// <summary>
        /// Removes an animation from the list of animations that want to be played.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveFromQueue(string name)
        {
            if (_queue.Contains(name))
                _queue.Remove(name);
        }

        public void RemoveAllFromQueue()
        {
            _queue = new List<string>();
            AddToQueue("idle");
        }

        /// <summary>
        /// Draws the animated texture with the specified color and flip.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="isFacingRight">Whether the animation should be flipped horizontally or not.</param>
        /// <param name="color">The color and opacity of the texture.</param>
        public void Draw(SpriteBatch spriteBatch, bool isFacingRight, Color color)
        {
            if (_currentAnimationData.Texture == null)
                return;

            if (!isFacingRight)
            {
                spriteBatch.Draw(_currentAnimationData.Texture, _drawRectangle, _sourceRectangle, color, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(_currentAnimationData.Texture, _drawRectangle, _sourceRectangle, color, 0, new Vector2(0, 0), SpriteEffects.None, 0);
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
            if (_animationData.TryGetValue(name, out value))
            {
                throw new Exception("Identifier for this animation data already exists.");
            }

            _animationData.Add(name, data);
        }

        /// <summary>
        /// Returns the texture being used currently.
        /// </summary>
        /// <returns></returns>
        public Texture2D GetCurrentTexture()
        {
            return _currentAnimationData.Texture;
        }

        /// <summary>
        /// Returns the current draw rectangle set by the animation settings.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetDrawRectangle()
        {
            return _drawRectangle;
        }

        public Rectangle GetSourceRectangle()
        {
            return _sourceRectangle;
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

        public ComplexAnimData() { }

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
        public ComplexAnimData(int priority, Texture2D texture, Rectangle deltaRectangle, int startingY, int width, int height, int speed, int frameCount, bool isRepeating)
        {
            Priority = priority;
            Texture = texture;
            DeltaRectangle = new Rectangle(deltaRectangle.X * 2, deltaRectangle.Y * 2, deltaRectangle.Width * 2, deltaRectangle.Height * 2);
            StartingY = startingY;
            Width = width;
            Height = height;
            Speed = speed;
            FrameCount = frameCount;
            IsRepeating = isRepeating;
        }
    }
}
