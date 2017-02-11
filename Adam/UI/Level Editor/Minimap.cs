using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Adam.UI
{
    /// <summary>
    ///     This creates a minimap of the level for the level editor.
    /// </summary>
    internal class Minimap
    {
        private Texture2D _antiTexture;
        private Color[] _pixels;
        private Rectangle _rectangle;
        private Texture2D _texture;
        private bool _isAnti;
        private Texture2D _temp;
        private Thread _thread;
        private Rectangle _uiSourceRect = new Rectangle(205, 95, 77, 114);
        private Rectangle _uiDrawRect;
        private Rectangle _viewSourceRect = new Rectangle(352, 160, 32, 32);
        private Rectangle _viewDrawRect;
        private float minimapToWorldRatio;

        public Minimap()
        {
            int size = (74 * 2);
            _temp = AdamGame.DefaultTexture;
            _texture = new Texture2D(AdamGame.GraphicsDeviceInstance, GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight);
            _antiTexture = new Texture2D(AdamGame.GraphicsDeviceInstance, GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight);
            _pixels = new Color[_texture.Width * _texture.Height];
            _rectangle = new Rectangle(AdamGame.DefaultUiWidth - size, AdamGame.DefaultUiHeight - size,
                size, size);

            minimapToWorldRatio = (float)(_texture.Width) / size;

            int width = (_uiSourceRect.Width * 2);
            int height = (_uiSourceRect.Height * 2);
            _uiDrawRect = new Rectangle(AdamGame.DefaultUiWidth - width, AdamGame.DefaultUiHeight - height, width, height);

        }

        /// <summary>
        ///     Initializes the thread that will be updating the minimap in the background.
        /// </summary>
        public void StartUpdating()
        {
            _thread = new Thread(Update);
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        ///     Updates the minimap. This is a background thread method.
        /// </summary>
        private void Update()
        {
            var isGoing = true;
            while (isGoing)
            {
                if (AdamGame.IsLoadingContent)
                    continue;

                var tileArray = GameWorld.TileArray;
                var wallArray = GameWorld.WallArray;

                for (var i = 0; i < tileArray.Length; i++)
                {
                    _pixels[i] = Color.Transparent;
                    if (tileArray[i].Id != 0 && tileArray[i].Id < AdamGame.TileType.Player)
                    {
                        Color color = GameWorld.SpriteSheetColorData[tileArray[i].SourceRectangle.Center.X + tileArray[i].SourceRectangle.Center.Y * GameWorld.SpriteSheet.Width];
                        _pixels[i] = color;
                    }
                    else if (wallArray[i].Id != 0)
                    {
                        _pixels[i] = GameWorld.SpriteSheetColorData[wallArray[i].SourceRectangle.Center.X + wallArray[i].SourceRectangle.Center.Y * GameWorld.SpriteSheet.Width];
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
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _uiDrawRect, _uiSourceRect, Color.White);

            if (_isAnti)
                spriteBatch.Draw(_texture, _rectangle, Color.White);
            else
            {
                spriteBatch.Draw(_antiTexture, _rectangle, Color.White);
            }

            _viewDrawRect = new Rectangle((int)(AdamGame.Camera.LeftTopGameCoords.X / AdamGame.Tilesize / minimapToWorldRatio), (int)(AdamGame.Camera.LeftTopGameCoords.Y / AdamGame.Tilesize / minimapToWorldRatio), 24, 17);
            _viewDrawRect.X += _rectangle.X;
            _viewDrawRect.Y += _rectangle.Y;
            spriteBatch.Draw(GameWorld.SpriteSheet, _viewDrawRect, _viewSourceRect, Color.Black);

        }
    }
}