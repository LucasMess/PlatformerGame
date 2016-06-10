using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// The UI element at the top-center of the screen in the level editor.
    /// </summary>
    class ButtonBar
    {
        private Container _container;
        List<FunctionButton> buttons = new List<FunctionButton>();


        private Texture2D _texture;
        private Rectangle _drawRectangle;
        private Rectangle _sourceRectangle;

        public ButtonBar()
        {
            _texture = GameWorld.UiSpriteSheet;
            _sourceRectangle = new Rectangle(0, 212, 382, 40);
            _drawRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(_sourceRectangle.Width), CalcHelper.ApplyHeightRatio(_sourceRectangle.Height));

            int x = Main.UserResWidth / 2 - _drawRectangle.Width / 2;
            _drawRectangle.X = x;

            var brushButton = new FunctionButton(new Vector2(11, 11), _drawRectangle, "Brush", ButtonImage.Brush);
            var eraserButton = new FunctionButton(new Vector2(29, 11), _drawRectangle, "Eraser", ButtonImage.Eraser);
            var undoButton = new FunctionButton(new Vector2(47, 11), _drawRectangle, "Undo", ButtonImage.Undo);
            var wallButton = new FunctionButton(new Vector2(65, 11), _drawRectangle, "Toggle wall mode", ButtonImage.Wall);
            var expandButton = new FunctionButton(new Vector2(293, 17), _drawRectangle, "More tiles", ButtonImage.Expand);
            var playButton = new FunctionButton(new Vector2(336, 11), _drawRectangle, "Play test level", ButtonImage.Play);
            var deleteButton = new FunctionButton(new Vector2(318, 11), _drawRectangle, "Reset level", ButtonImage.Delete);
            var optionsButton = new FunctionButton(new Vector2(354, 11), _drawRectangle, "More options",
                ButtonImage.Settings);

            expandButton.MouseClicked += OpenInventory;

            buttons.Add(wallButton);
            buttons.Add(playButton);
            buttons.Add(deleteButton);
            buttons.Add(expandButton);
            buttons.Add(brushButton);
            buttons.Add(eraserButton);
            buttons.Add(undoButton);
            buttons.Add(optionsButton);

            _container = new Container(0, 0, 100, 200);
        }

        private void OpenInventory()
        {
            Inventory.StartAnimation();
        }

        public void Update()
        {
            foreach (var button in buttons)
            {
                button.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);

            foreach (var button in buttons)
            {
                button.Draw(spriteBatch);
            }
            foreach (var button in buttons)
            {
                button.DrawOnTop(spriteBatch);
            }
        }
    }
}
