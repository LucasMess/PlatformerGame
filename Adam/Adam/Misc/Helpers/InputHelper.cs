using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public static class InputHelper
    {
        public static bool IsAnyInputPressed()
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.A) ||
                keyboard.IsKeyDown(Keys.W) ||
                keyboard.IsKeyDown(Keys.S) ||
                keyboard.IsKeyDown(Keys.D) ||
                keyboard.IsKeyDown(Keys.Space))
            {
                return true;
            }
            else return false;
        }

        public static bool IsKeyDown(Keys key)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(key))
                return true;
            else return false;
        }
        public static bool IsKeyUp(Keys key)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyUp(key))
                return true;
            else return false;
        }
        public static bool IsLeftMousePressed()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
                return true;
            else return false;
        }
        public static bool IsLeftMouseReleased()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Released)
                return true;
            else return false;
        }
        public static Rectangle MouseRectangle
        {
            get
            {
                MouseState mouseState = Mouse.GetState();
                double widthRatio = ((double)Game1.DefaultResWidth / (double)Game1.PrefferedResWidth);
                double heightRatio = ((double)Game1.DefaultResHeight / (double)Game1.PrefferedResHeight);
                Rectangle mouseRect = new Rectangle((int)(mouseState.X * widthRatio), (int)(mouseState.Y * heightRatio), 1, 1);
                return mouseRect;
            }
        }

    }
}
