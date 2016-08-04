using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.UI.Elements
{
    class UiElement
    {

        private Timer _movementTimer = new Timer();
        private Vector2 _previousPosition;
        private Vector2 _newPosition;
        private Vector2 _delta;
        private int _duration;
        private bool _isMovingToNewPosition;

        private Rectangle _drawRectangle;
        public Rectangle DrawRectangle
        {
            get
            {
                if (_isMovingToNewPosition)
                {
                    _drawRectangle.X = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                        _previousPosition.X, _delta.X, _duration);
                    _drawRectangle.Y = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                        _previousPosition.Y, _delta.Y, _duration);

                    if (Math.Abs(_drawRectangle.X - _newPosition.X) < .1f &&
                        Math.Abs(_drawRectangle.Y - _newPosition.Y) < .1f)
                        _isMovingToNewPosition = false;
                }
                return _drawRectangle;
            }
            set { _drawRectangle = value; }
        }

        public void MoveTo(Vector2 position, int duration)
        {
            MoveTo(position.X, position.Y, duration);
        }

        public void MoveTo(float x, float y, int duration)
        {
            _previousPosition = new Vector2(DrawRectangle.X, DrawRectangle.Y);
            _newPosition = new Vector2(x, y);
            _delta = _newPosition - _previousPosition;
            _duration = duration;
            _movementTimer.Reset();
            _isMovingToNewPosition = true;
        }

        /// <summary>
        /// Sets the position without animating.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2 position)
        {
            _isMovingToNewPosition = false;
            _drawRectangle.X = (int) position.X;
            _drawRectangle.Y = (int) position.Y;
        }
    }
}
