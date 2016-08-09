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
        private readonly Texture2D _antiTexture;
        private readonly Color[] _pixels;
        private readonly Rectangle _rectangle;
        private readonly Texture2D _texture;
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
            int size = CalcHelper.ApplyUiRatio(74);
            _temp = Main.DefaultTexture;
            _texture = new Texture2D(Main.GraphicsDeviceInstance, GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight);
            _antiTexture = new Texture2D(Main.GraphicsDeviceInstance, GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight);
            _pixels = new Color[_texture.Width * _texture.Height];
            _rectangle = new Rectangle(Main.UserResWidth - size, Main.UserResHeight - size,
                size, size);

            minimapToWorldRatio = (float)(_texture.Width)/size;

            int width = CalcHelper.ApplyUiRatio(_uiSourceRect.Width);
            int height = CalcHelper.ApplyUiRatio(_uiSourceRect.Height);
            _uiDrawRect = new Rectangle(Main.UserResWidth - width, Main.UserResHeight - height, width, height);

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
                if (Main.IsLoadingContent)
                    continue;

                var tileArray = GameWorld.TileArray;
                var wallArray = GameWorld.WallArray;

                Texture2D texture = GameWorld.SpriteSheet;
                Color[] colors = new Color[texture.Width * texture.Height];
                try
                {
                    texture.GetData(colors);

                }
                catch{
                    return;
                }

                for (var i = 0; i < tileArray.Length; i++)
                {
                    _pixels[i] = Color.Transparent;
                    if (tileArray[i].Id != 0 && tileArray[i].Id < 200)
                    {
                        _pixels[i] = colors[tileArray[i].SourceRectangle.Center.X + tileArray[i].SourceRectangle.Center.Y * texture.Width];
                    }
                    else if (wallArray[i].Id != 0)
                    {
                        _pixels[i] = colors[wallArray[i].SourceRectangle.Center.X + wallArray[i].SourceRectangle.Center.Y * texture.Width];
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

            _viewDrawRect = new Rectangle((int)(Main.Camera.LeftTopGameCoords.X / Main.Tilesize / minimapToWorldRatio), (int)(Main.Camera.LeftTopGameCoords.Y / Main.Tilesize / minimapToWorldRatio), (int)(23f * (16/9f)), 23);
            _viewDrawRect.X += _rectangle.X;
            _viewDrawRect.Y += _rectangle.Y;
            spriteBatch.Draw(GameWorld.SpriteSheet, _viewDrawRect, _viewSourceRect, Color.Black);

        }
    }
}