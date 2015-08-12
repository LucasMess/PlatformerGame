using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Elements
{
    class FunctionButton : Button
    {
        protected Vector2 relativePosition;

        protected void Initialize()
        {
            MouseOver += OnMouseOver;
            MouseOut += OnMouseOut;
            collRectangle = new Rectangle(0, 0, (int)(Game1.Tilesize / Game1.WidthRatio), (int)(Game1.Tilesize / Game1.HeightRatio));
            sourceRectangle = new Rectangle(0, 0, 16, 16);
        }

        public void Update(Rectangle box)
        {
            base.Update();

            collRectangle.X = (int)relativePosition.X + box.X;
            collRectangle.Y = (int)relativePosition.Y + box.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UI_SpriteSheet, collRectangle, sourceRectangle, color);
        }
    }

    class PlayButton : FunctionButton
    {
        public PlayButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 0;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Game1.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Game1.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Game1.WidthRatio), (float)(position.Y / Game1.HeightRatio));
        }
    }

    class SaveButton : FunctionButton
    {
        public SaveButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 32;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Game1.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Game1.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Game1.WidthRatio), (float)(position.Y / Game1.HeightRatio));
        }
    }

    class OpenButton : FunctionButton
    {
        public OpenButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 48;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Game1.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Game1.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Game1.WidthRatio), (float)(position.Y / Game1.HeightRatio));
        }
    }
}
