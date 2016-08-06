using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.UI.Elements
{
    public class UiElement
    {

        private Timer _movementTimer = new Timer();
        private Vector2 _previousPosition;
        private Vector2 _newPosition;
        private Vector2 _delta;
        private int _duration;
        public bool IsMovingToNewPosition { get; private set; }

        private Rectangle _drawRectangle;
        public Rectangle DrawRectangle
        {
            get
            {
                if (IsMovingToNewPosition)
                {
                    _drawRectangle.X = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                        _previousPosition.X, _delta.X, _duration);
                    _drawRectangle.Y = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                        _previousPosition.Y, _delta.Y, _duration);

                    if (Math.Abs(_drawRectangle.X - _newPosition.X) < .1f &&
                        Math.Abs(_drawRectangle.Y - _newPosition.Y) < .1f)
                        IsMovingToNewPosition = false;
                }
                return _drawRectangle;
            }
            set { _drawRectangle = value; }
        }

        public virtual void MoveTo(Vector2 position, int duration)
        {
            MoveTo(position.X, position.Y, duration);
        }

        public void MoveTo(float x, float y, int duration)
        {
            if (_newPosition != new Vector2(x, y))
            {
                _previousPosition = new Vector2(DrawRectangle.X, DrawRectangle.Y);
                _newPosition = new Vector2(x, y);
                _delta = _newPosition - _previousPosition;
                _duration = duration;
                _movementTimer.Reset();
                IsMovingToNewPosition = true;
            }
        }

        /// <summary>
        /// Sets the position without animating.
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetPosition(Vector2 position)
        {
            IsMovingToNewPosition = false;
            _newPosition = position;
            _drawRectangle.X = (int)position.X;
            _drawRectangle.Y = (int)position.Y;
        }

        public virtual void SetPosition(float x, float y)
        {
            SetPosition(new Vector2(x, y));
        }

        public Vector2 GetPosition() { return new Vector2(DrawRectangle.X, DrawRectangle.Y); }
    }
}
