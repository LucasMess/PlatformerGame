using System.Collections.Generic;
using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    ///     The UI element at the top-center of the screen in the level editor.
    /// </summary>
    internal class ButtonBar
    {
        private readonly Rectangle _drawRectangle;
        private readonly Rectangle _sourceRectangle;
        private readonly Texture2D _texture;
        private readonly List<IconButton> _buttons = new List<IconButton>();

        public ButtonBar()
        {
            _texture = GameWorld.UiSpriteSheet;
            _sourceRectangle = new Rectangle(0, 212, 382, 40);
            _drawRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(_sourceRectangle.Width),
                CalcHelper.ApplyUiRatio(_sourceRectangle.Height));

            _drawRectangle.X = Main.UserResWidth / 2 - _drawRectangle.Width / 2;

            // Buttons cannot be called individually outside the constructor.
            var brushButton = new IconButton(new Vector2(11, 11), _drawRectangle, "Brush", ButtonImage.Brush);
            var eraserButton = new IconButton(new Vector2(29, 11), _drawRectangle, "Eraser", ButtonImage.Eraser);
            var undoButton = new IconButton(new Vector2(47, 11), _drawRectangle, "Undo", ButtonImage.Undo);
            var wallButton = new IconButton(new Vector2(65, 11), _drawRectangle, "Toggle wall mode", ButtonImage.Wall);
            var expandButton = new IconButton(new Vector2(293, 17), _drawRectangle, "More tiles", ButtonImage.Expand);
            var playButton = new IconButton(new Vector2(336, 11), _drawRectangle, "Play test level", ButtonImage.Play);
            var deleteButton = new IconButton(new Vector2(318, 11), _drawRectangle, "Reset level", ButtonImage.Delete);
            var optionsButton = new IconButton(new Vector2(354, 11), _drawRectangle, "More options",
                ButtonImage.Settings);

            playButton.MouseClicked += LevelEditor.TestLevel;
            wallButton.MouseClicked += LevelEditor.ChangeToWallMode;
            expandButton.MouseClicked += Inventory.StartAnimation;

            _buttons.Add(wallButton);
            _buttons.Add(playButton);
            _buttons.Add(deleteButton);
            _buttons.Add(expandButton);
            _buttons.Add(brushButton);
            _buttons.Add(eraserButton);
            _buttons.Add(undoButton);
            _buttons.Add(optionsButton);
        }

        /// <summary>
        /// Returns the space occupied by this element where it cannot be clicked through.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCollRectangle()
        {
            return _drawRectangle;
        }

        public void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);

            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }

            // Tooltips are always drawn on top.
            foreach (var button in _buttons)
            {
                button.DrawTooltip(spriteBatch);
            }
        }
    }
}