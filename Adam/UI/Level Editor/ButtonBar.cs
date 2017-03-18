using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
            _drawRectangle = new Rectangle(0, 0, _sourceRectangle.Width * 2,
                _sourceRectangle.Height * 2);

            _drawRectangle.X = AdamGame.DefaultUiWidth / 2 - _drawRectangle.Width / 2;

            // Buttons cannot be called individually outside the constructor.
            var brushButton = new IconButton(new Vector2(11 * 2, 11 * 2), _drawRectangle, "Brush", ButtonImage.Brush);
            brushButton.ChangeColors(new Color(95, 95, 95), Color.White);

            var eraserButton = new IconButton(new Vector2(29 * 2, 11 * 2), _drawRectangle, "Eraser", ButtonImage.Eraser);
            eraserButton.ChangeColors(new Color(95, 95, 95), Color.White);

            var undoButton = new IconButton(new Vector2(47 * 2, 11 * 2), _drawRectangle, "Undo", ButtonImage.Undo);
            undoButton.ChangeColors(new Color(95, 95, 95), Color.White);

            var wallButton = new IconButton(new Vector2(65 * 2, 11 * 2), _drawRectangle, "Toggle wall mode", ButtonImage.Wall);
            wallButton.ChangeColors(new Color(95, 95, 95), Color.White);

            var expandButton = new IconButton(new Vector2(293 * 2, 17 * 2), _drawRectangle, "More tiles", ButtonImage.Expand);

            var playButton = new IconButton(new Vector2(336 * 2, 11 * 2), _drawRectangle, "Play test level", ButtonImage.Play);

            var deleteButton = new IconButton(new Vector2(318 * 2, 11 * 2), _drawRectangle, "Reset level", ButtonImage.Delete);
            deleteButton.ChangeColors(new Color(205, 205, 205), new Color(95, 95, 95));

            var optionsButton = new IconButton(new Vector2(354 * 2, 11 * 2), _drawRectangle, "More options",
                ButtonImage.Settings);
            optionsButton.ChangeColors(new Color(205,205,205), new Color(95, 95, 95));

            playButton.MouseClicked += LevelEditor.TestLevel;
            wallButton.MouseClicked += LevelEditor.ChangeToWallMode;
            expandButton.MouseClicked += Inventory.StartAnimation;
            brushButton.MouseClicked += BrushButton_MouseClicked;
            eraserButton.MouseClicked += EraserButton_MouseClicked;

            _buttons.Add(wallButton);
            _buttons.Add(playButton);
            _buttons.Add(deleteButton);
            _buttons.Add(expandButton);
            _buttons.Add(brushButton);
            _buttons.Add(eraserButton);
            _buttons.Add(undoButton);
            _buttons.Add(optionsButton);
        }

        private void EraserButton_MouseClicked(Button button)
        {
            LevelEditor.Brush.CurrentBrushMode = Brush.BrushMode.Erase;
            Cursor.ChangeCursor(Cursor.CursorType.Erase);
        }

        private void BrushButton_MouseClicked(Button button)
        {
            LevelEditor.Brush.CurrentBrushMode = Brush.BrushMode.Build;
            Cursor.ChangeCursor(Cursor.CursorType.Build);
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