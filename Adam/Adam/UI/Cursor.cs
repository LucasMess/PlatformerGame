using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    public class Cursor
    {
        public Texture2D Texture;
        public Rectangle Rectangle, SourceRectangle;
        MouseState _mouseState;
        Vector2 _origin;
        public Vector2 PositionOnScreen;
        public Vector2 PositionOnGame;
        Vector2 _monitorRes;

        public bool IsPressed;
        double _scrollTimer;
        int _scrollWheel, _lastScrollWheel;

        public Cursor()
        {
        }

        public void Load(ContentManager content)
        {
            Texture = ContentHelper.LoadTexture("Tiles/spritemap_3");
            Rectangle = new Rectangle(0, 0, Main.Tilesize, Main.Tilesize);
            SourceRectangle = Rectangle;
            _origin = new Vector2(Main.Tilesize / 2, Main.Tilesize / 2);
        }

        public void Update(GameTime gameTime, Vector2 cameraPos)
        {
            _mouseState = Mouse.GetState();
            PositionOnScreen.X = _mouseState.X;
            PositionOnScreen.Y = _mouseState.Y;

            PositionOnGame = new Vector2(cameraPos.X - Main.DefaultResWidth / 2 + PositionOnScreen.X, cameraPos.Y - Main.DefaultResHeight + PositionOnScreen.Y);

            _scrollWheel = _mouseState.ScrollWheelValue;

            //if (scrollWheel < lastScrollWheel)
            //{
            //    sourceRectangle.X += Game1.tileSize;
            //}
            //if (scrollWheel > lastScrollWheel)
            //{
            //    sourceRectangle.X -= Game1.tileSize;
            //}

            _scrollTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            int scrollBuffer = 20;
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && _scrollTimer > scrollBuffer)
            {
                SourceRectangle.X += Main.Tilesize;
                _scrollTimer = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E) && _scrollTimer > scrollBuffer)
            {
                SourceRectangle.X -= Main.Tilesize;
                _scrollTimer = 0;
            }


            if (SourceRectangle.X < 0)
                SourceRectangle = new Rectangle(Texture.Width - Main.Tilesize, Texture.Height - Main.Tilesize, Main.Tilesize, Main.Tilesize);
            if (SourceRectangle.X > Texture.Width)
            {
                SourceRectangle.X = 0;
                SourceRectangle.Y += Main.Tilesize;
            }
            if (SourceRectangle.Y > Texture.Height)
                SourceRectangle.Y = 0;

            _lastScrollWheel = _scrollWheel;

            if (_mouseState.LeftButton == ButtonState.Pressed)
                IsPressed = true;
            else IsPressed = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, positionOnGame, sourceRectangle, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
