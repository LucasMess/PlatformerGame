using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Adam.Levels;

namespace Adam.UI
{
    /// <summary>
    /// This creates a minimap of the level for the level editor.
    /// </summary>
    class Minimap
    {
        Color[] _pixels;
        Texture2D _texture, _antiTexture, _temp;
        Thread _thread;
        Rectangle _rectangle;
        bool _isAnti;

        public Minimap()
        {
            _temp = Main.DefaultTexture;
            _texture = new Texture2D(Main.GraphicsDeviceInstance, GameWorld.Instance.WorldData.LevelWidth, GameWorld.Instance.WorldData.LevelHeight);
            _antiTexture = new Texture2D(Main.GraphicsDeviceInstance, GameWorld.Instance.WorldData.LevelWidth, GameWorld.Instance.WorldData.LevelHeight);
            _pixels = new Color[_texture.Width * _texture.Height];
            _rectangle = new Rectangle(Main.UserResWidth - _texture.Width, Main.UserResHeight - _texture.Height, _texture.Width, _texture.Height);
        }

        /// <summary>
        /// Initializes the thread that will be updating the minimap in the background.
        /// </summary>
        public void StartUpdating()
        {
            _thread = new Thread(new ThreadStart(Update));
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Updates the minimap. This is a background thread method.
        /// </summary>
        private void Update()
        {

            bool isGoing = true;
            while (isGoing)
            {
                if (Main.IsLoadingContent)
                    continue;

                Tile[] tileArray = GameWorld.Instance.TileArray;
                Tile[] wallArray = GameWorld.Instance.WallArray;

                for (int i = 0; i < tileArray.Length; i++)
                {
                    _pixels[i] = new Color(0, 0, 0, 50);
                    if (tileArray[i].Id != 0 && tileArray[i].Id < 200)
                    {
                        _pixels[i] = Color.ForestGreen;
                    }
                    else if (wallArray[i].Id != 0)
                    {
                        _pixels[i] = Color.DarkGreen;
                    }
                }

                if (!_isAnti)
                {
                    try
                    {
                        _texture?.SetData(_pixels);
                    }
                    catch
                    {
                        isGoing = false;
                    }
                }
                else
                {
                    try
                    {
                        _antiTexture?.SetData(_pixels);
                    }
                    catch
                    {
                        isGoing = false;
                    }
                }
                _isAnti = !_isAnti;

                Thread.Sleep(1000);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isAnti)
                spriteBatch.Draw(_texture, _rectangle, Color.White);
            else
            {
                spriteBatch.Draw(_antiTexture, _rectangle, Color.White);
            }
        }
    }
}
