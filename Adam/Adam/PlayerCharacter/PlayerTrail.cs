using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private Timer _addTimer = new Timer();
        private Timer _drawTimer = new Timer();
        private int _currentlyShown = 0;
        private const int TimeBetweenShots = 200;
        private const int NumberOfTrails = 20;
        private const int TimeBetweenDraws = 100;

        public void Add(Player player)
        {
            if (_addTimer.TimeElapsedInMilliSeconds < TimeBetweenShots)
                return;
            
            _addTimer.Reset();

            _textures.Add(player.ComplexAnimation.GetCurrentTexture());
            _drawRectangles.Add(player.ComplexAnimation.GetDrawRectangle());
            _sourceRectangles.Add(player.ComplexAnimation.GetSourceRectangle());

            if (_textures.Count > NumberOfTrails)
            {
                _textures.RemoveAt(0);
                _drawRectangles.RemoveAt(0);
                _sourceRectangles.RemoveAt(0);
            }
        }

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
                spriteBatch.Draw(_textures.ElementAt(i), _drawRectangles.ElementAt(i), _sourceRectangles.ElementAt(i), Color.White * .5f);
            }
        }
    }
}
