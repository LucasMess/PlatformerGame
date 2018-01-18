using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;

namespace ThereMustBeAnotherWay.Levels
{
    public class Sky
    {
        private enum Period : int
        {
            Daytime,
            Sunset_Part1,
            Sunset_Part2,
            Nighttime,
            Sunrise_Part1,
            Sunrise_Part2,
        }

        private Period _currentPeriod = Period.Daytime;

        private const int TRANSITION_LENGTH = 5000;

        private GameTimer _transitionTimer = new GameTimer();
        private GameTimer _worldClock = new GameTimer();

        private Texture2D _dayTexture;
        private Texture2D _sunsetTexture;
        private Texture2D _nightTexture;
        private Rectangle _drawRectangle;

        public Sky()
        {
            _dayTexture = ContentHelper.LoadTexture("Backgrounds/Sky/day");
            _sunsetTexture = ContentHelper.LoadTexture("Backgrounds/Sky/sunset");
            _nightTexture = ContentHelper.LoadTexture("Backgrounds/Sky/night");
            _drawRectangle = new Rectangle(0, 0, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight);
        }

        public void Update()
        {
            _worldClock.Increment();
            _transitionTimer.Increment();

            if (_transitionTimer.TimeElapsedInMilliSeconds > TRANSITION_LENGTH)
            {
                _currentPeriod++;
                _currentPeriod = (Period)((int)_currentPeriod % 6);
                _transitionTimer.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (_currentPeriod)
            {
                case Period.Daytime:
                    spriteBatch.Draw(_dayTexture, _drawRectangle, Color.White);
                    break;
                case Period.Sunset_Part1:
                    spriteBatch.Draw(_dayTexture, _drawRectangle, Color.White);
                    spriteBatch.Draw(_sunsetTexture, _drawRectangle, Color.White * GetOpacityBasedOnTransitionTime());
                    break;
                case Period.Sunset_Part2:
                    spriteBatch.Draw(_sunsetTexture, _drawRectangle, Color.White);
                    spriteBatch.Draw(_nightTexture, _drawRectangle, Color.White * GetOpacityBasedOnTransitionTime());
                    break;
                case Period.Nighttime:
                    spriteBatch.Draw(_nightTexture, _drawRectangle, Color.White);
                    break;
                case Period.Sunrise_Part1:
                    spriteBatch.Draw(_nightTexture, _drawRectangle, Color.White);
                    spriteBatch.Draw(_sunsetTexture, _drawRectangle, Color.White * GetOpacityBasedOnTransitionTime());
                    break;
                case Period.Sunrise_Part2:
                    spriteBatch.Draw(_sunsetTexture, _drawRectangle, Color.White);
                    spriteBatch.Draw(_dayTexture, _drawRectangle, Color.White * GetOpacityBasedOnTransitionTime());
                    break;
                default:
                    break;
            }
        }

        private float GetOpacityBasedOnTransitionTime()
        {
            return (float)(_transitionTimer.TimeElapsedInMilliSeconds / TRANSITION_LENGTH);
        }

        private float GetInverseOpacityBasedOnTransitionTime()
        {
            return (float)(1 - _transitionTimer.TimeElapsedInMilliSeconds / TRANSITION_LENGTH);
        }

        private Color AddColors(Color color, Color other)
        {
            int r = (int)color.R + (int)other.R;
            int g = (int)color.G + (int)other.G;
            int b = (int)color.B + (int)other.B;
            return new Color(r % 256, g % 256, b % 256);
        }

        public Color GetAmbientLight()
        {
            Color day, sunset, night;
            switch (_currentPeriod)
            {
                case Period.Daytime:
                    return _dayColor;
                case Period.Sunset_Part1:
                    day = _dayColor * GetInverseOpacityBasedOnTransitionTime();
                    sunset = _sunsetColor * GetOpacityBasedOnTransitionTime();
                    return AddColors(day, sunset);
                case Period.Sunset_Part2:
                    sunset = _sunsetColor * GetInverseOpacityBasedOnTransitionTime();
                    night = _nightColor * GetOpacityBasedOnTransitionTime();
                    return AddColors(sunset, night);
                case Period.Nighttime:
                    return _nightColor;
                case Period.Sunrise_Part1:
                    night = _nightColor * GetInverseOpacityBasedOnTransitionTime();
                    sunset = _sunsetColor * GetOpacityBasedOnTransitionTime();
                    return AddColors(sunset, night);
                case Period.Sunrise_Part2:
                    sunset = _sunsetColor * GetInverseOpacityBasedOnTransitionTime();
                    day = _dayColor * GetOpacityBasedOnTransitionTime();
                    return AddColors(sunset, day);
                default:
                    return new Color(0, 0, 1);
            }
        }

        private Color _nightColor = new Color(50, 50, 50);
        private Color _dayColor = new Color(255, 255, 255);
        private Color _sunsetColor = new Color(254, 139, 42);
    }
}
