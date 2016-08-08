using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
{

    /// <summary>
    /// Used to display information about the player on screen and also to make transitions such as screen flashes and fade to black.
    /// </summary>
    public static class Overlay
    {
        private const float OpacityDelta = .01f;
        private static float _whiteOpacity;
        private static bool _isFlashingWhite;
        private static bool _hasReachedMaxWhite;
        private static Texture2D _white = ContentHelper.LoadTexture("Tiles/white");

        public static void Update()
        {
            if (_isFlashingWhite)
            {
                if (!_hasReachedMaxWhite)
                {
                    _whiteOpacity += OpacityDelta * 5;
                    if (_whiteOpacity > 1f)
                    {
                        _hasReachedMaxWhite = true;
                    }
                }
                else
                {
                    _whiteOpacity -= OpacityDelta;
                    if (_whiteOpacity < 0)
                    {
                        _whiteOpacity = 0;
                        _hasReachedMaxWhite = false;
                        _isFlashingWhite = false;
                    }
                }
            }
        }

        public static void FlashWhite()
        {
            _isFlashingWhite = true;
            _hasReachedMaxWhite = false;
            _whiteOpacity =1f;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_white, new Rectangle(0,0, Main.UserResWidth, Main.UserResHeight), Color.White * _whiteOpacity);
        }
    }
}
