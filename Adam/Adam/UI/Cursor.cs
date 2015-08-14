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
        public Texture2D texture;
        public Rectangle rectangle, sourceRectangle;
        MouseState mouseState;
        Vector2 origin;
        public Vector2 positionOnScreen;
        public Vector2 positionOnGame;
        Vector2 monitorRes;

        public bool isPressed;
        double scrollTimer;
        int scrollWheel, lastScrollWheel;

        public Cursor()
        {
        }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Tiles/spritemap_3");
            rectangle = new Rectangle(0, 0, Main.Tilesize, Main.Tilesize);
            sourceRectangle = rectangle;
            origin = new Vector2(Main.Tilesize / 2, Main.Tilesize / 2);
        }

        public void Update(GameTime gameTime, Vector2 cameraPos)
        {
            mouseState = Mouse.GetState();
            positionOnScreen.X = mouseState.X;
            positionOnScreen.Y = mouseState.Y;

            positionOnGame = new Vector2(cameraPos.X - Main.DefaultResWidth / 2 + positionOnScreen.X, cameraPos.Y - Main.DefaultResHeight + positionOnScreen.Y);

            scrollWheel = mouseState.ScrollWheelValue;

            //if (scrollWheel < lastScrollWheel)
            //{
            //    sourceRectangle.X += Game1.tileSize;
            //}
            //if (scrollWheel > lastScrollWheel)
            //{
            //    sourceRectangle.X -= Game1.tileSize;
            //}

            scrollTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            int scrollBuffer = 20;
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && scrollTimer > scrollBuffer)
            {
                sourceRectangle.X += Main.Tilesize;
                scrollTimer = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E) && scrollTimer > scrollBuffer)
            {
                sourceRectangle.X -= Main.Tilesize;
                scrollTimer = 0;
            }


            if (sourceRectangle.X < 0)
                sourceRectangle = new Rectangle(texture.Width - Main.Tilesize, texture.Height - Main.Tilesize, Main.Tilesize, Main.Tilesize);
            if (sourceRectangle.X > texture.Width)
            {
                sourceRectangle.X = 0;
                sourceRectangle.Y += Main.Tilesize;
            }
            if (sourceRectangle.Y > texture.Height)
                sourceRectangle.Y = 0;

            lastScrollWheel = scrollWheel;

            if (mouseState.LeftButton == ButtonState.Pressed)
                isPressed = true;
            else isPressed = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, positionOnGame, sourceRectangle, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
