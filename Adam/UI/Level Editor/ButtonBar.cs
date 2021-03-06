﻿using ThereMustBeAnotherWay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.UI.Elements;
using ThereMustBeAnotherWay.UI.Level_Editor;

namespace ThereMustBeAnotherWay.UI.Level_Editor
{
    /// <summary>
    ///     The UI element at the top-center of the screen in the level editor.
    /// </summary>
    public class ButtonBar
    {
        private Rectangle _drawRectangle;
        private readonly Rectangle _sourceRectangle;
        private readonly Texture2D _texture;
        private readonly List<IconButton> _buttons = new List<IconButton>();

        IconButton brushButton;
        IconButton eraserButton;
        IconButton selectButton;
        IconButton lightingButton;

        // Other buttons that only need a reference to update their position on a res change.
        IconButton wallButton, expandButton, playButton, optionsButton, plusButton, minusButton;

        public ButtonBar()
        {
            _texture = GameWorld.UiSpriteSheet;
            _sourceRectangle = new Rectangle(0, 212, 382, 40);
            _drawRectangle = new Rectangle(0, 0, _sourceRectangle.Width * 2,
                _sourceRectangle.Height * 2);

            _drawRectangle.X = TMBAW_Game.UserResWidth / 2 - _drawRectangle.Width / 2;

            brushButton = new IconButton(new Vector2(11 * 2, 11 * 2), _drawRectangle, "Brush", ButtonImage.Brush);
            brushButton.ChangeColors(new Color(95, 95, 95), Color.White);

            eraserButton = new IconButton(new Vector2(29 * 2, 11 * 2), _drawRectangle, "Eraser", ButtonImage.Eraser);
            eraserButton.ChangeColors(new Color(95, 95, 95), Color.White);

            selectButton = new IconButton(new Vector2(47 * 2, 11 * 2), _drawRectangle, "Select", ButtonImage.Select);
            selectButton.ChangeColors(new Color(95, 95, 95), Color.White);

            wallButton = new IconButton(new Vector2(65 * 2, 11 * 2), _drawRectangle, "Toggle wall mode", ButtonImage.Wall);
            wallButton.ChangeColors(new Color(95, 95, 95), Color.White);

            expandButton = new IconButton(new Vector2(293 * 2, 17 * 2), _drawRectangle, "More tiles", ButtonImage.Expand);

            playButton = new IconButton(new Vector2(336 * 2, 11 * 2), _drawRectangle, "Play test level", ButtonImage.Play);

            lightingButton = new IconButton(new Vector2(318 * 2, 11 * 2), _drawRectangle, "Enable/Disable Lighting", ButtonImage.LightBulb);
            lightingButton.ChangeColors(new Color(95, 95, 95), Color.White);
            lightingButton.IsOn = true;

            optionsButton = new IconButton(new Vector2(354 * 2, 11 * 2), _drawRectangle, "More options",
                ButtonImage.Settings);
            optionsButton.ChangeColors(new Color(205, 205, 205), new Color(95, 95, 95));

            // Buttons for minimap, which will be conveniently placed here...
            plusButton = new IconButton(new Vector2(413 * 2, 158 * 2), _drawRectangle, "Zoom In", ButtonImage.Plus);
            plusButton.MouseClicked += PlusButton_MouseClicked;
            plusButton.ChangeColors(new Color(95, 95, 95), Color.White);
            _buttons.Add(plusButton);

            minusButton = new IconButton(new Vector2(413 * 2, 176 * 2), _drawRectangle, "Zoom Out", ButtonImage.Minus);
            minusButton.MouseClicked += MinusButton_MouseClicked;
            minusButton.ChangeColors(new Color(95, 95, 95), Color.White);
            _buttons.Add(minusButton);

            playButton.MouseClicked += LevelEditor.TestLevel;
            wallButton.MouseClicked += LevelEditor.ChangeToWallMode;
            expandButton.MouseClicked += Inventory.StartAnimation;
            brushButton.MouseClicked += BrushButton_MouseClicked;
            eraserButton.MouseClicked += EraserButton_MouseClicked;
            selectButton.MouseClicked += SelectButton_MouseClicked;
            lightingButton.MouseClicked += LightingButton_MouseClicked;

            _buttons.Add(wallButton);
            _buttons.Add(playButton);
            _buttons.Add(lightingButton);
            _buttons.Add(expandButton);
            _buttons.Add(brushButton);
            _buttons.Add(eraserButton);
            _buttons.Add(selectButton);
            _buttons.Add(optionsButton);

            GraphicsRenderer.OnResolutionChanged += SetElementPositions;
            SetElementPositions(TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight);
        }

        private void SetElementPositions(int width, int height)
        {
            _drawRectangle.X = TMBAW_Game.UserResWidth / 2 - _drawRectangle.Width / 2;
            _drawRectangle.Y = 0;

            brushButton.SetPosition(new Vector2(11 * 2, 11 * 2), _drawRectangle);
            eraserButton.SetPosition(new Vector2(29 * 2, 11 * 2), _drawRectangle);
            selectButton.SetPosition(new Vector2(47 * 2, 11 * 2), _drawRectangle);
            wallButton.SetPosition(new Vector2(65 * 2, 11 * 2), _drawRectangle);
            expandButton.SetPosition(new Vector2(293 * 2, 17 * 2), _drawRectangle);
            playButton.SetPosition(new Vector2(336 * 2, 11 * 2), _drawRectangle);
            lightingButton.SetPosition(new Vector2(318 * 2, 11 * 2), _drawRectangle);
            optionsButton.SetPosition(new Vector2(354 * 2, 11 * 2), _drawRectangle);

            // Minimap buttons.
            plusButton.SetPosition(new Vector2(TMBAW_Game.UserResWidth - 18 * 2, TMBAW_Game.UserResHeight - 112 * 2));
            minusButton.SetPosition(new Vector2(TMBAW_Game.UserResWidth - 18 * 2, TMBAW_Game.UserResHeight - 94 * 2));
        }

        private void LightingButton_MouseClicked(Button button)
        {
            button.IsOn = !button.IsOn;
            LevelEditor.IsLightingEnabled = !LevelEditor.IsLightingEnabled;
        }

        public static void MinusButton_MouseClicked(Button button)
        {
            TMBAW_Game.Camera.SetZoomTo(.5f);
        }

        public static void PlusButton_MouseClicked(Button button)
        {
            TMBAW_Game.Camera.ResetZoom();
        }

        public void OnSelectButtonClicked() => SelectButton_MouseClicked(selectButton);
        public void OnEraserButtonClicked() => EraserButton_MouseClicked(eraserButton);
        public void OnBrushButtonClicked() => BrushButton_MouseClicked(brushButton);

        private void SelectButton_MouseClicked(Button button)
        {
            LevelEditor.Brush.CurrentBrushMode = Brush.BrushMode.Select;
            Cursor.ChangeCursor(Cursor.Type.Select);
            selectButton.IsOn = true;
            brushButton.IsOn = false;
            eraserButton.IsOn = false;
        }

        private void EraserButton_MouseClicked(Button button)
        {
            LevelEditor.Brush.CurrentBrushMode = Brush.BrushMode.Erase;
            Cursor.ChangeCursor(Cursor.Type.Erase);
            selectButton.IsOn = false;
            brushButton.IsOn = false;
            eraserButton.IsOn = true;
        }

        private void BrushButton_MouseClicked(Button button)
        {
            LevelEditor.Brush.CurrentBrushMode = Brush.BrushMode.Build;
            Cursor.ChangeCursor(Cursor.Type.Build);
            selectButton.IsOn = false;
            brushButton.IsOn = true;
            eraserButton.IsOn = false;
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

            if (GameWorld.GetPlayers()[0].IsBrushButtonPressed())
                BrushButton_MouseClicked(brushButton);
            if (GameWorld.GetPlayers()[0].IsEraserButtonPressed())
                EraserButton_MouseClicked(eraserButton);

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