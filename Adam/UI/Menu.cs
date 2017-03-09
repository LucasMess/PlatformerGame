using Adam.Misc.Helpers;
using Adam.Misc.Sound;
using Adam.Network;
using Adam.UI;
using Adam.UI.Elements;
using Adam.UI.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

namespace Adam
{
    public static class MainMenu
    {
        static Vector2 _first;
        static Vector2 _second;
        static Vector2 _third;
        static Vector2 _fourth;
        static Vector2 _fifth;

        //Main Menu
        static TextButton _chooseLevel;
        static TextButton _options;
        static TextButton _quit;
        static TextButton _multiplayer;
        static TextButton _storyModeButton;

        //Level Selector
        static TextButton _save1;
        static TextButton _save2;
        static TextButton _save3;

        //Options
        static TextButton _smoothPixels;
        static TextButton _lighting;
        static TextButton _fullscreen;

        //Multiplayer
        static TextButton _hostGame;
        static TextButton _joinGame;
        static TextButton _startMultiplayerGame;

        static TextButton _backButton;
        static List<TextButton> _buttons = new List<TextButton>();

        static bool _isSongPlaying;
        static BitmapFont _font8, _font32;

        static StoryMode storyMode = new StoryMode();

        static LevelSelection _levelSelection;

        public enum MenuState { Main, Options, LevelSelector, HostJoin, MultiplayerSession, StoryMode }
        public static MenuState CurrentMenuState = MenuState.Main;

        public static void Initialize(AdamGame game1)
        {
            int width = AdamGame.DefaultUiWidth / 2 - TextButton.Width;
            int height = AdamGame.DefaultUiHeight * 2 / 5;
            int diff = (TextButton.Height + 2) * 2;
            _first = new Vector2(width, height + (diff * 0));
            _second = new Vector2(width, height + (diff * 1));
            _third = new Vector2(width, height + (diff * 2));
            _fourth = new Vector2(width, height + (diff * 3));
            _fifth = new Vector2(width, height + (diff * 4));

            _font8 = ContentHelper.LoadFont("Fonts/x8");
            _font32 = ContentHelper.LoadFont("Fonts/x32");

            _chooseLevel = new TextButton(_second, "Choose a Level", false);
            _chooseLevel.MouseClicked += chooseLevel_MouseClicked;
            _buttons.Add(_chooseLevel);

            _quit = new TextButton(_fifth, "Quit", false);
            _quit.MouseClicked += quit_MouseClicked;
            _buttons.Add(_quit);

            _options = new TextButton(_third, "Options", false);
            _options.MouseClicked += options_MouseClicked;
            _buttons.Add(_options);

            _multiplayer = new TextButton(_fourth, "Multiplayer", false);
            _multiplayer.MouseClicked += multiplayer_MouseClicked;
            _buttons.Add(_multiplayer);

            _storyModeButton = new TextButton(_first, "Story Mode", false);
            _storyModeButton.MouseClicked += storyMode_MouseClicked;
            _buttons.Add(_storyModeButton);

            //smoothPixels = new Button(first, "Smooth Pixels: ");
            //smoothPixels.MouseClicked += smoothPixels_MouseClicked;
            //buttons.Add(smoothPixels);

            //lighting = new Button(second, "Lighting: ");
            //lighting.MouseClicked += lighting_MouseClicked;
            //buttons.Add(lighting);

            _fullscreen = new TextButton(_first, "Borderless Mode: ", false);
            _fullscreen.MouseClicked += fullscreen_MouseClicked;
            //_fullscreen.IsOn = Main.GameData.Settings.IsFullscreen;
            _buttons.Add(_fullscreen);

            _backButton = new TextButton(_fifth, "Back", false);
            _backButton.MouseClicked += backButton_MouseClicked;
            _buttons.Add(_backButton);

            //save1 = new Button(first, "Save 1");
            //save1.MouseClicked += level1_MouseClicked;
            //buttons.Add(save1);

            //save2 = new Button(second, "Save 2");
            //save2.MouseClicked += level2_MouseClicked;
            //buttons.Add(save2);

            //save3 = new Button(third, "Save 3");
            //save3.MouseClicked += level3_MouseClicked;
            //buttons.Add(save3);

            _hostGame = new TextButton(_first, "Host Game", false);
            _hostGame.MouseClicked += hostGame_MouseClicked;
            _buttons.Add(_hostGame);

            _joinGame = new TextButton(_second, "Join Game", false);
            _joinGame.MouseClicked += joinGame_MouseClicked;
            _buttons.Add(_joinGame);

            _startMultiplayerGame = new TextButton(_third, "Start Game", false);
            _startMultiplayerGame.MouseClicked += StartMultiplayerGame_MouseClicked;
            _buttons.Add(_startMultiplayerGame);

            foreach (var button in _buttons)
            {
                button.ChangeDimensions(new Vector2(TextButton.Width * 2, TextButton.Height * 2));
                button.Color = new Color(196, 69, 69);
            }

            _levelSelection = new LevelSelection();
        }

        private static void StartMultiplayerGame_MouseClicked(Button button)
        {
            if (Session.IsHost)
            {
                Session.Start();
            }
        }

        private static void storyMode_MouseClicked(Button button)
        {
            CurrentMenuState = MenuState.StoryMode;
        }

        static void joinGame_MouseClicked(Button button)
        {
            Session.Join();
            CurrentMenuState = MenuState.MultiplayerSession;
        }

        static void hostGame_MouseClicked(Button button)
        {
            Session.HostGame();
            CurrentMenuState = MenuState.MultiplayerSession;
        }

        //void level4_MouseClicked()
        //{
        //    game1.ChangeState(GameState.GameWorld);
        //}

        //void level3_MouseClicked()
        //{
        //    game1.ChangeState(GameState.GameWorld);
        //}

        //void level2_MouseClicked()
        //{
        //    game1.ChangeState(GameState.GameWorld);
        //}

        //void level1_MouseClicked()
        //{
        //    game1.GameData.SelectedSave = 0;
        //    game1.ChangeState(GameState.GameWorld);
        //}

        static void backButton_MouseClicked(Button button)
        {
            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    break;
                case MenuState.Options:
                    CurrentMenuState = MenuState.Main;
                    break;
                case MenuState.LevelSelector:
                    CurrentMenuState = MenuState.Main;
                    break;
                case MenuState.HostJoin:
                    CurrentMenuState = MenuState.Main;
                    break;
                case MenuState.MultiplayerSession:
                    CurrentMenuState = MenuState.Main;
                    break;
                default:
                    break;
            }
        }

        static void fullscreen_MouseClicked(Button button)
        {

            switch (_fullscreen.IsOn)
            {
                case true:
                    _fullscreen.IsOn = false;
                    AdamGame.GameData.Settings.IsFullscreen = false;
                    AdamGame.GameData.Settings.NeedsRestart = true;
                    AdamGame.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    _fullscreen.IsOn = true;
                    AdamGame.GameData.Settings.IsFullscreen = true;
                    AdamGame.GameData.Settings.NeedsRestart = true;
                    AdamGame.GameData.Settings.HasChanged = true;
                    break;
                default:
                    break;
            }
        }

        static void lighting_MouseClicked()
        {
            switch (_lighting.IsOn)
            {
                case true:
                    _lighting.IsOn = false;
                    AdamGame.GameData.Settings.DesiredLight = false;
                    AdamGame.GameData.Settings.NeedsRestart = true;
                    AdamGame.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    _lighting.IsOn = true;
                    AdamGame.GameData.Settings.DesiredLight = true;
                    AdamGame.GameData.Settings.NeedsRestart = true;
                    AdamGame.GameData.Settings.HasChanged = true;
                    break;
                default:
                    break;
            }
        }

        static void smoothPixels_MouseClicked()
        {
            throw new NotImplementedException();
        }

        static void multiplayer_MouseClicked(Button button)
        {
            CurrentMenuState = MenuState.HostJoin;
        }

        static void quit_MouseClicked(Button button)
        {
            // game1.GameData.SaveGame();
            AdamGame.Quit();
        }

        static void chooseLevel_MouseClicked(Button button)
        {
            _levelSelection.LoadLevels();
            CurrentMenuState = MenuState.LevelSelector;

            //game1.CurrentGameMode = GameMode.Play;
            //GameWorld.worldData.OpenLevelLocally(false);
        }

        static void options_MouseClicked(Button button)
        {
            OptionsMenu.Show();
            CurrentMenuState = MenuState.Options;
        }

        static public void Update()
        {
            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    _chooseLevel.Update();
                    _quit.Update();
                    _options.Update();
                    _multiplayer.Update();
                    _storyModeButton.Update();
                    break;
                case MenuState.StoryMode:
                    storyMode.Update();
                    break;
                case MenuState.LevelSelector:
                    _levelSelection.Update();
                    break;
                case MenuState.Options:
                    break;

                case MenuState.HostJoin:
                    _hostGame.Update();
                    _joinGame.Update();

                    _backButton.Update();
                    break;
                case MenuState.MultiplayerSession:
                    _startMultiplayerGame.Update();
                    break;
            }

            if (AdamGame.CurrentGameState == GameState.MainMenu)
                SoundtrackManager.PlayMainTheme();

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background, new Rectangle(0, 0, AdamGame.UserResWidth, AdamGame.UserResHeight), Color.White);

            FontHelper.DrawWithOutline(spriteBatch, _font8, AdamGame.Producers, new Vector2(5, 5), 3, Color.White, Color.Black);
            FontHelper.DrawWithOutline(spriteBatch, _font8, AdamGame.Version, new Vector2(5, 30), 3, Color.White, Color.Black);
            //FontHelper.DrawWithOutline(spriteBatch, _font8, "Another Way", new Vector2((AdamGame.DefaultUiWidth / 2f) - _font32.MeasureString("Adam").X / 2, (AdamGame.DefaultUiHeight * 1 / 5f)), 3, new Color(196, 69, 69), new Color(147,52,52));

            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    _chooseLevel.Draw(spriteBatch);
                    _quit.Draw(spriteBatch);
                    _options.Draw(spriteBatch);
                    _multiplayer.Draw(spriteBatch);
                    _storyModeButton.Draw(spriteBatch);
                    break;
                case MenuState.StoryMode:
                    storyMode.Draw(spriteBatch);
                    break;
                case MenuState.LevelSelector:
                    _levelSelection.Draw(spriteBatch);
                    break;
                case MenuState.Options:
                    if (!OptionsMenu.IsActive)
                        CurrentMenuState = MenuState.Main;
                    break;
                case MenuState.HostJoin:
                    _hostGame.Draw(spriteBatch);
                    _joinGame.Draw(spriteBatch);
                    _backButton.Draw(spriteBatch);
                    break;
                case MenuState.MultiplayerSession:
                    _startMultiplayerGame.Draw(spriteBatch);
                    _backButton.Draw(spriteBatch);
                    break;
            }

        }

    }
}
