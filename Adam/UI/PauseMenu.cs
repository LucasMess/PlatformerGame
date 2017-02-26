using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Adam.UI
{
    /// <summary>
    /// Shown when the player presses escape on a level.
    /// </summary>
    public static class PauseMenu
    {
        private static TextButton _returnToMainMenu;
        private static TextButton _returnToLevelEditor;
        private static TextButton _resumeGame;
        private static TextButton _showOptions;
        private static TextButton _quitGame;

        private static List<TextButton> _buttons = new List<TextButton>();

        public static bool IsActive { get; private set; }
        private static bool _buttonReleased;

        public static void Initialize()
        {
            _returnToLevelEditor = new TextButton(new Vector2(), "Return to Level Editor", false);
            _returnToLevelEditor.MouseClicked += _returnToLevelEditor_MouseClicked;

            _returnToMainMenu = new TextButton(new Vector2(), "Return to Main Menu");
            _returnToMainMenu.MouseClicked += _returnToMainMenu_MouseClicked;

            _resumeGame = new TextButton(new Vector2(), "Resume", false);
            _resumeGame.MouseClicked += _resumeGame_MouseClicked;

            _showOptions = new TextButton(new Vector2(), "Options", false);
            _showOptions.MouseClicked += _showOptions_MouseClicked;

            _quitGame = new TextButton(new Vector2(), "Quit to Desktop", false);
            _quitGame.MouseClicked += _quitGame_MouseClicked;

            _buttons.Add(_resumeGame);
            _buttons.Add(_showOptions);
            _buttons.Add(_returnToLevelEditor);
            _buttons.Add(_returnToMainMenu);
            _buttons.Add(_quitGame);

            foreach (var button in _buttons)
            {
                button.ChangeDimensions(new Vector2(TextButton.Width * 2, TextButton.Height * 2));
                button.Color = new Color(196, 69, 69);
            }

            int startingY = 200;
            int x = AdamGame.DefaultUiWidth / 2 - TextButton.Width;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].SetPosition(new Vector2(x, startingY + (TextButton.Height * 2 + 10) * i));
            }
        }

        private static void _quitGame_MouseClicked(Button button)
        {
            LevelEditor.SaveLevel();
            AdamGame.Quit();
        }

        private static void _showOptions_MouseClicked(Button button)
        {
            OptionsMenu.Show();
        }

        private static void _resumeGame_MouseClicked(Button button)
        {
            IsActive = false;
        }

        private static void _returnToMainMenu_MouseClicked(Button button)
        {
            if (AdamGame.CurrentGameState == GameState.GameWorld)
                LevelEditor.SaveLevel();
            AdamGame.ChangeState(GameState.MainMenu, GameMode.None, true);
            IsActive = false;
        }

        private static void _returnToLevelEditor_MouseClicked(Button button)
        {
            if (GameWorld.IsTestingLevel)
            {
                LevelEditor.GoBackToEditing();
                IsActive = false;
            }
            else
            {
                AdamGame.MessageBox.Show("You cannot edit a level you are playing!");
            }
        }

        public static void Update()
        {

            if (GameWorld.GetPlayer().IsPauseButtonDown() && _buttonReleased && AdamGame.CurrentGameState == GameState.GameWorld)
            {
                _buttonReleased = false;
                IsActive = !IsActive;
            }

            if (!GameWorld.GetPlayer().IsPauseButtonDown())
            {
                _buttonReleased = true;
            }

            if (OptionsMenu.IsActive)
                return;

            if (IsActive)
            {
                foreach (var button in _buttons)
                {
                    button.Update();
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (OptionsMenu.IsActive)
                return;

            if (IsActive)
            {
                foreach (var button in _buttons)
                {
                    button.Draw(spriteBatch);
                }
            }
        }
    }
}
