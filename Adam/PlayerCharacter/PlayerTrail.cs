using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Adam.PlayerCharacter
{
    /// <summary>
    /// Trail seen in level editor after level is edited.
    /// </summary>
    public class PlayerTrail
    {
        private List<Texture2D> _textures = new List<Texture2D>();
        private List<Rectangle> _drawRectangles = new List<Rectangle>();
        private List<Rectangle> _sourceRectangles = new List<Rectangle>();
        private List<bool> _isFacingRight = new List<bool>();
        private Timer _drawTimer = new Timer(true);
        private int _currentlyShown = 0;
        private const int DistanceBetweenShots = 64;
        private const int NumberOfTrails = 20;
        private const int TimeBetweenDraws = 100;
        private Vector2 _oldPosition;

        /// <summary>
        /// Adds a new trail if the player has moved sufficiently.
        /// </summary>
        /// <param name="player"></param>
        public void Add(Player player)
        {
            Vector2 position = player.Position;
            if (CalcHelper.GetPythagoras(position.X - _oldPosition.X, position.Y - _oldPosition.Y) > DistanceBetweenShots)
            {
                _oldPosition = position;

                _textures.Add(player.ComplexAnimation.GetCurrentTexture());
                _drawRectangles.Add(player.ComplexAnimation.GetDrawRectangle());
                _sourceRectangles.Add(player.ComplexAnimation.GetSourceRectangle());
                _isFacingRight.Add(player.IsFacingRight);

                if (_textures.Count > NumberOfTrails)
                {
                    _textures.RemoveAt(0);
                    _drawRectangles.RemoveAt(0);
                    _sourceRectangles.RemoveAt(0);
                    _isFacingRight.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Draws the trails one at a time until they are all drawn.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_drawTimer.TimeElapsedInMilliSeconds > TimeBetweenDraws)
            {
                _drawTimer.Reset();
                _currentlyShown++;
            }

            if (_currentlyShown > _textures.Count)
                _currentlyShown = _textures.Count;

            for (int i = 0; i < _currentlyShown; i++)
            {
                spriteBatch.Draw(_textures.ElementAt(i), _drawRectangles.ElementAt(i), _sourceRectangles.ElementAt(i), Color.White * .5f, 0, Vector2.Zero, _isFacingRight[i] ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }
        }

    }
}
