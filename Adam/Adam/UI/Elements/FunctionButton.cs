using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc.Helpers;

namespace Adam.UI.Elements
{
    public class FunctionButton : Button
    {
        protected Vector2 relativePosition;
        protected string hoverText="";
        bool showHoverText;
        SpriteFont font;

        protected void Initialize()
        {
            MouseOver += OnMouseOver;
            MouseOut += OnMouseOut;
            collRectangle = new Rectangle(0, 0, (int)(Main.Tilesize / Main.WidthRatio), (int)(Main.Tilesize / Main.HeightRatio));
            sourceRectangle = new Rectangle(0, 0, 16, 16);
            font = ContentHelper.LoadFont("Fonts/objectiveText");
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

            if (showHoverText)
            {
                Rectangle mouse = InputHelper.MouseRectangle;
                FontHelper.DrawWithOutline(spriteBatch, font, hoverText, new Vector2(mouse.X, mouse.Y - 50), 1, Color.White, Color.Black);
            }
        }

        protected override void OnMouseOver()
        {
            base.OnMouseOver();

            showHoverText = true;
        }

        protected override void OnMouseOut()
        {
            base.OnMouseOut();

            showHoverText = false;
        }
    }

    public class PlayButton : FunctionButton
    {
        public PlayButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 0;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Test level [F5]";
        }
    }

    public class SaveButton : FunctionButton
    {
        public SaveButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 32;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Save level [F2]";
        }
    }

    public class OpenButton : FunctionButton
    {
        public OpenButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 48;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Open level [F1]";
        }
    }

    public class NewButton : FunctionButton
    {
        public NewButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 16;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "New level";
        }
    }

    public class DeleteButton : FunctionButton
    {
        public DeleteButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 64;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Erase level [Ctrl + W]";
        }
    }

    public class WallButton : FunctionButton
    {
        bool isActive;
        Rectangle active;
        Rectangle inactive;

        public WallButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 64;
            sourceRectangle.Y = 16;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Switch to Wall Mode [L]";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.levelEditor.onWallMode)
            {
                hoverText = "Switch to Tile Mode [L]";
            }
            else hoverText = "Switch to Wall Mode [L]";

            base.Draw(spriteBatch);
        }
    }

    public class RenameButton : FunctionButton
    {
        public RenameButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 80;
            sourceRectangle.Y = 0;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Rename Level";
        }
    }

    public class EditButton : FunctionButton
    {
        public EditButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 80;
            sourceRectangle.Y = 16;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Edit level in Level Editor";
        }
    }

    public class BackButton : FunctionButton
    {
        public BackButton(Vector2 position, Rectangle box)
        {
            Initialize();
            sourceRectangle.X = 80;
            sourceRectangle.Y = 32;
            sourceRectangle.Width = sourceRectangle.Width * 2;
            collRectangle.X = (int)(position.X / Main.WidthRatio) + box.X;
            collRectangle.Y = (int)(position.Y / Main.HeightRatio) + box.Y;
            collRectangle.Width = collRectangle.Width * 2;
            relativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            hoverText = "Return";
        }
    }
}
