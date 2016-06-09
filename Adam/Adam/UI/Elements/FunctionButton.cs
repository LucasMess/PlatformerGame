using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc.Helpers;

namespace Adam.UI.Elements
{
    public class FunctionButton : Button
    {
        private const int buttonSize = 32;
        protected Vector2 RelativePosition;
        protected string HoverText="";
        bool _showHoverText;
        SpriteFont _font;
        private Texture2D _black = ContentHelper.LoadTexture("Tiles/black");

        protected void Initialize()
        {
            MouseHover += OnMouseHover;
            MouseOut += OnMouseOut;
            CollRectangle = new Rectangle(0, 0, (int)(buttonSize / Main.WidthRatio), (int)(buttonSize / Main.HeightRatio));
            SourceRectangle = new Rectangle(0, 0, 16, 16);
            _font = ContentHelper.LoadFont("Fonts/x32");
        }

        public void Update(Rectangle box)
        {
            base.Update();

            CollRectangle.X = (int)RelativePosition.X + box.X;
            CollRectangle.Y = (int)RelativePosition.Y + box.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color);
        }

        public void DrawOnTop(SpriteBatch spriteBatch)
        {
            if (_showHoverText)
            {
                Rectangle mouse = InputHelper.MouseRectangle;
                spriteBatch.Draw(_black, new Rectangle(mouse.X - 5, mouse.Y - 52, (int)_font.MeasureString(HoverText).X + 10, (int)_font.MeasureString(HoverText).Y + 4), Color.Black);
                FontHelper.DrawWithOutline(spriteBatch, _font, HoverText, new Vector2(mouse.X, mouse.Y - 50), 1, Color.White, Color.Black);
            }
        }

        protected override void OnMouseHover()
        {
            base.OnMouseHover();

            _showHoverText = true;
        }

        protected override void OnMouseOut()
        {
            base.OnMouseOut();

            _showHoverText = false;
        }
    }

    public class PlayButton : FunctionButton
    {
        public PlayButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 0;
            SourceRectangle.Y = 0;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Play level";
        }
    }

    public class SaveButton : FunctionButton
    {
        public SaveButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 32;
            SourceRectangle.Y = 0;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Save level";
        }
    }

    public class OpenButton : FunctionButton
    {
        public OpenButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 48;
            SourceRectangle.Y = 0;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Open level";
        }
    }

    public class NewButton : FunctionButton
    {
        public NewButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 16;
            SourceRectangle.Y = 0;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "New level";
        }
    }

    public class DeleteButton : FunctionButton
    {
        public DeleteButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 64;
            SourceRectangle.Y = 0;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Erase level";
        }
    }

    public class WallButton : FunctionButton
    {
        bool _isActive;
        Rectangle _active;
        Rectangle _inactive;

        public WallButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 64;
            SourceRectangle.Y = 16;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Switch to Wall Mode";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.LevelEditor.OnWallMode)
            {
                HoverText = "Switch to Tile Mode";
            }
            else HoverText = "Switch to Wall Mode";

            base.Draw(spriteBatch);
        }
    }

    public class RenameButton : FunctionButton
    {
        public RenameButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 80;
            SourceRectangle.Y = 0;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Rename Level";
        }
    }

    public class EditButton : FunctionButton
    {
        public EditButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 80;
            SourceRectangle.Y = 16;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Edit level in Level Editor";
        }
    }

    public class BackButton : FunctionButton
    {
        public BackButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 80;
            SourceRectangle.Y = 32;
            SourceRectangle.Width = SourceRectangle.Width * 2;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            CollRectangle.Width = CollRectangle.Width * 2;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Return";
        }
    }

    public class ExpandButton : FunctionButton
    {
        public ExpandButton(Vector2 position, Rectangle box)
        {
            Initialize();
            SourceRectangle.X = 0;
            SourceRectangle.Y = 48;
            CollRectangle.X = CalcHelper.ApplyUiRatio((int)position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int)position.Y) + box.Y;
            RelativePosition = new Vector2((float)(position.X / Main.WidthRatio), (float)(position.Y / Main.HeightRatio));
            HoverText = "Expand";
        }
    }
}
