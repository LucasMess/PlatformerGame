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
        public static bool IsRightMousePressed()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.RightButton == ButtonState.Pressed)
                return true;
            else return false;
        }
        public static bool IsMiddleMousePressed()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.MiddleButton == ButtonState.Pressed)
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
        public static Rectangle MouseRectangleRenderTarget
        {
            get
            {
                MouseState mouseState = Mouse.GetState();
                double widthRatio = Game1.WidthRatio;
                double heightRatio = Game1.HeightRatio;
                Rectangle mouseRect = new Rectangle((int)(mouseState.X * widthRatio), (int)(mouseState.Y * heightRatio), 1, 1);
                return mouseRect;
            }
        }
        public static Rectangle MouseRectangle
        {
            get
            {
                MouseState mouseState = Mouse.GetState();
                Rectangle mouseRect = new Rectangle((int)(mouseState.X), (int)(mouseState.Y), 1, 1);
                return mouseRect;
            }
        }
        public static Rectangle MouseRectangleGameWorld
        {
            get
            {
                Rectangle rect = MouseRectangleRenderTarget;
                rect.X -= (int)GameWorld.Instance.camera.lastCameraLeftCorner.X;
                rect.Y -= (int)GameWorld.Instance.camera.lastCameraLeftCorner.Y;
                return rect;
            }
        }

    }
}
