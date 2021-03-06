﻿using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.Misc.Sound;
using ThereMustBeAnotherWay.Network;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.UI.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.UI.Elements;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay
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

        //Multiplayer
        static TextButton _startMultiplayerGame;

        static TextButton _backButton;
        static List<TextButton> _buttons = new List<TextButton>();

        static SpriteFont _font8, _font32;

        static LevelSelection _levelSelection;

        public enum MenuState { Main, Options, LevelSelector, MultiplayerLobby, MultiplayerSession, StoryMode }
        public static MenuState CurrentMenuState = MenuState.Main;

        public static void Initialize(TMBAW_Game game1)
        {
            _font8 = ContentHelper.LoadFont("Fonts/x8");
            _font32 = ContentHelper.LoadFont("Fonts/x32");

            _chooseLevel = new TextButton(Vector2.Zero, "Choose a Level", false);
            _chooseLevel.MouseClicked += chooseLevel_MouseClicked;
            _buttons.Add(_chooseLevel);

            _quit = new TextButton(Vector2.Zero, "Quit", false);
            _quit.MouseClicked += quit_MouseClicked;
            _buttons.Add(_quit);

            _options = new TextButton(Vector2.Zero, "Options", false);
            _options.MouseClicked += options_MouseClicked;
            _buttons.Add(_options);

            _multiplayer = new TextButton(Vector2.Zero, "Multiplayer", false);
            _multiplayer.MouseClicked += multiplayer_MouseClicked;
            _buttons.Add(_multiplayer);

            _storyModeButton = new TextButton(Vector2.Zero, "Story Mode", false);
            _storyModeButton.MouseClicked += storyMode_MouseClicked;
            _buttons.Add(_storyModeButton);

            _backButton = new TextButton(Vector2.Zero, "Back", false);
            _backButton.MouseClicked += backButton_MouseClicked;
            _buttons.Add(_backButton);

            _startMultiplayerGame = new TextButton(Vector2.Zero, "Start Game", false);
            _startMultiplayerGame.MouseClicked += StartMultiplayerGame_MouseClicked;
            _buttons.Add(_startMultiplayerGame);

            foreach (var button in _buttons)
            {
                button.ChangeDimensions(new Vector2(TextButton.Width * 2, TextButton.Height * 2));
                button.Color = new Color(196, 69, 69);
            }

            _levelSelection = new LevelSelection();
            SaveSelector.Initialize();

            GraphicsRenderer.OnResolutionChanged += SetElementPositions;
            SetElementPositions(TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight);
        }

        private static void SetElementPositions(int width, int height)
        {
            int middle = width / 2 - TextButton.Width;
            int startingY = height * 2 / 5;
            int diff = (TextButton.Height + 2) * 2;

            _first = new Vector2(middle, startingY + (diff * 0));
            _second = new Vector2(middle, startingY + (diff * 1));
            _third = new Vector2(middle, startingY + (diff * 2));
            _fourth = new Vector2(middle, startingY + (diff * 3));
            _fifth = new Vector2(middle, startingY + (diff * 4));

            _chooseLevel.SetPosition(_second);
            _quit.SetPosition(_fifth);
            _options.SetPosition(_third);
            _multiplayer.SetPosition(_fourth);
            _storyModeButton.SetPosition(_first);
            _backButton.SetPosition(_fifth);
            _startMultiplayerGame.SetPosition(_third);
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
                case MenuState.MultiplayerLobby:
                    CurrentMenuState = MenuState.Main;
                    break;
                case MenuState.MultiplayerSession:
                    CurrentMenuState = MenuState.Main;
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
            CurrentMenuState = MenuState.MultiplayerLobby;
            Lobby.CreateLobby();
        }

        static void quit_MouseClicked(Button button)
        {
            // game1.GameData.SaveGame();
            TMBAW_Game.Quit();
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
                    SaveSelector.Update();
                    break;
                case MenuState.LevelSelector:
                    _levelSelection.Update();
                    break;
                case MenuState.Options:
                    break;

                case MenuState.MultiplayerLobby:
                    Lobby.Update();
                    break;
                case MenuState.MultiplayerSession:
                    _startMultiplayerGame.Update();
                    break;
            }

            Cursor.Show();
            Cursor.ChangeCursor(Cursor.Type.Normal);

            if (TMBAW_Game.CurrentGameState == GameState.MainMenu)
                SoundtrackManager.PlayMainTheme();

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background, new Rectangle(0, 0, AdamGame.UserResWidth, AdamGame.UserResHeight), Color.White);

            FontHelper.DrawWithOutline(spriteBatch, _font8, TMBAW_Game.Producers, new Vector2(5, 5), 3, Color.White, Color.Black);
            FontHelper.DrawWithOutline(spriteBatch, _font8, TMBAW_Game.Version, new Vector2(5, 20), 3, Color.White, Color.Black);


            string title = "There Must Be Another Way";
            FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[2], title, new Vector2(TMBAW_Game.UserResWidth / 2 - FontHelper.Fonts[2].MeasureString(title).X / 2, 120), 1, Color.White, Color.DarkGray);

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
                    SaveSelector.Draw(spriteBatch);
                    break;
                case MenuState.LevelSelector:
                    _levelSelection.Draw(spriteBatch);
                    break;
                case MenuState.Options:
                    if (!OptionsMenu.IsActive)
                        CurrentMenuState = MenuState.Main;
                    break;
                case MenuState.MultiplayerLobby:
                    Lobby.Draw(spriteBatch);
                    break;
                case MenuState.MultiplayerSession:
                    _startMultiplayerGame.Draw(spriteBatch);
                    _backButton.Draw(spriteBatch);
                    break;
            }

        }

    }
}
