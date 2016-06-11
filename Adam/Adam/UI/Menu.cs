using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Adam.Network;
using Adam.UI;
using Adam.GameData;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Sound;
using Adam.UI.Elements;

namespace Adam
{
    public class Menu
    {
        private Texture2D background;

        Vector2 _first;
        Vector2 _second;
        Vector2 _third;
        Vector2 _fourth;
        Vector2 _fifth;

        //Main Menu
        Button _chooseLevel;
        Button _options;
        Button _quit;
        Button _multiplayer;
        Button _storyMode;

        //Level Selector
        Button _save1;
        Button _save2;
        Button _save3;

        //Options
        Button _smoothPixels;
        Button _lighting;
        Button _fullscreen;

        //Multiplayer
        Button _hostGame;
        Button _joinGame;
        Button _startMultiplayerGame;

        Button _backButton;
        List<Button> _buttons = new List<Button>();

        bool _isSongPlaying;
        SpriteFont _font32, _font64;
        Main _game1;

        LevelSelection _levelSelection;

        public enum MenuState { Main, Options, LevelSelector, HostJoin, MultiplayerSession  }
        public static MenuState CurrentMenuState = MenuState.Main;

        public Menu(Main game1)
        {
            this._game1 = game1;

            int width = (int)(Main.DefaultResWidth / 2f / Main.WidthRatio);
            int height = (int)(200 / Main.HeightRatio);
            int diff = (int)(40 / Main.HeightRatio);
            _first = new Vector2(width, height + (diff * 0));
            _second = new Vector2(width, height + (diff * 1));
            _third = new Vector2(width, height + (diff * 2));
            _fourth = new Vector2(width, height + (diff * 3));
            _fifth = new Vector2(width, height + (diff * 4));

            background = ContentHelper.LoadTexture("Backgrounds/Main Menu/menu_background_temp");
            _font32 = ContentHelper.LoadFont("Fonts/x32");
            _font64 = ContentHelper.LoadFont("Fonts/x64");

            _chooseLevel = new TextButton(_second, "Choose a Level");
            _chooseLevel.MouseClicked += chooseLevel_MouseClicked;
            _buttons.Add(_chooseLevel);

            _quit = new TextButton(_fifth, "Quit");
            _quit.MouseClicked += quit_MouseClicked;
            _buttons.Add(_quit);

            _options = new TextButton(_third, "Options");
            _options.MouseClicked += options_MouseClicked;
            _buttons.Add(_options);

            _multiplayer = new TextButton(_fourth, "Multiplayer");
            _multiplayer.MouseClicked += multiplayer_MouseClicked;
            _buttons.Add(_multiplayer);

            _storyMode = new TextButton(_first, "Story Mode");
            _storyMode.MouseClicked += storyMode_MouseClicked;
            _buttons.Add(_storyMode);

            //smoothPixels = new Button(first, "Smooth Pixels: ");
            //smoothPixels.MouseClicked += smoothPixels_MouseClicked;
            //buttons.Add(smoothPixels);

            //lighting = new Button(second, "Lighting: ");
            //lighting.MouseClicked += lighting_MouseClicked;
            //buttons.Add(lighting);

            _fullscreen = new TextButton(_first, "Borderless Mode: ");
            _fullscreen.MouseClicked += fullscreen_MouseClicked;
            _fullscreen.IsOn = Main.GameData.Settings.IsFullscreen;
            _buttons.Add(_fullscreen);

            _backButton = new TextButton(_fifth, "Back");
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

            _hostGame = new TextButton(_first, "Host Game");
            _hostGame.MouseClicked += hostGame_MouseClicked;
            _buttons.Add(_hostGame);

            _joinGame = new TextButton(_second, "Join Game");
            _joinGame.MouseClicked += joinGame_MouseClicked;
            _buttons.Add(_joinGame);

            _startMultiplayerGame = new TextButton(_third, "Start Game");
            _startMultiplayerGame.MouseClicked += StartMultiplayerGame_MouseClicked;
            _buttons.Add(_startMultiplayerGame);

            _levelSelection = new LevelSelection();
        }

        private void StartMultiplayerGame_MouseClicked()
        {
           if (Session.IsHost)
            {
                Main.Session.Start();
            }
        }

        private void storyMode_MouseClicked()
        {
            Main.MessageBox.Show("Coming Soon...");
        }

        void joinGame_MouseClicked()
        {
            Main.Session = new Session(false, "Client");
            CurrentMenuState = MenuState.MultiplayerSession;
        }

        void hostGame_MouseClicked()
        {
            Main.Session = new Session(true, "Host");
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

        void backButton_MouseClicked()
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

        void fullscreen_MouseClicked()
        {

            switch (_fullscreen.IsOn)
            {
                case true:
                    _fullscreen.IsOn = false;
                    Main.GameData.Settings.IsFullscreen = false;
                    Main.GameData.Settings.NeedsRestart = true;
                    Main.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    _fullscreen.IsOn = true;
                    Main.GameData.Settings.IsFullscreen = true;
                    Main.GameData.Settings.NeedsRestart = true;
                    Main.GameData.Settings.HasChanged = true;
                    break;
                default:
                    break;
            }
        }

        void lighting_MouseClicked()
        {
            switch (_lighting.IsOn)
            {
                case true:
                    _lighting.IsOn = false;
                    Main.GameData.Settings.DesiredLight = false;
                    Main.GameData.Settings.NeedsRestart = true;
                    Main.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    _lighting.IsOn = true;
                    Main.GameData.Settings.DesiredLight = true;
                    Main.GameData.Settings.NeedsRestart = true;
                    Main.GameData.Settings.HasChanged = true;
                    break;
                default:
                    break;
            }
        }

        void smoothPixels_MouseClicked()
        {
            throw new NotImplementedException();
        }

        void multiplayer_MouseClicked()
        {
            CurrentMenuState = MenuState.HostJoin;
        }

        void quit_MouseClicked()
        {
            // game1.GameData.SaveGame();
            _game1.Exit();
        }

        void chooseLevel_MouseClicked()
        {
            _levelSelection.LoadLevels();
            CurrentMenuState = MenuState.LevelSelector;

            //game1.CurrentGameMode = GameMode.Play;
            //GameWorld.worldData.OpenLevelLocally(false);
        }

        void options_MouseClicked()
        {
            CurrentMenuState = MenuState.Options;
        }

        public void Update()
        {
            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    _chooseLevel.Update();
                    _quit.Update();
                    _options.Update();
                    _multiplayer.Update();
                    _storyMode.Update();
                    break;
                case MenuState.LevelSelector:
                    _levelSelection.Update();
                    break;
                case MenuState.Options:
                    //smoothPixels.Text = "Smooth Pixels: " + smoothPixels.IsActive;
                    //lighting.Text = "Lighting: " + lighting.IsActive;
                    _fullscreen.Text = "Borderless Mode: " + !_fullscreen.IsOn;

                    //smoothPixels.Update();
                    //lighting.Update();
                    _fullscreen.Update();

                    _backButton.Update();
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

            if (Main.CurrentGameState == GameState.MainMenu)
            SoundtrackManager.PlayMainTheme();

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background,new Rectangle(0,0,Main.UserResWidth,Main.UserResHeight),Color.White);

            FontHelper.DrawWithOutline(spriteBatch,_font32,Main.Producers,new Vector2((float)(5 / Main.WidthRatio), (float)(5 / Main.HeightRatio)),3,Color.White,Color.Black);
            FontHelper.DrawWithOutline(spriteBatch, _font32, Main.Version, new Vector2((float)(5 / Main.WidthRatio), (float)(30 / Main.HeightRatio)), 3, Color.White, Color.Black);
            FontHelper.DrawWithOutline(spriteBatch, _font64, "Adam", new Vector2((float)(400 / Main.WidthRatio), (float)(60 / Main.HeightRatio)), 3, Color.DarkRed, Color.MediumVioletRed);

            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    _chooseLevel.Draw(spriteBatch);
                    _quit.Draw(spriteBatch);
                    _options.Draw(spriteBatch);
                    _multiplayer.Draw(spriteBatch);
                    _storyMode.Draw(spriteBatch);
                    break;
                case MenuState.LevelSelector:
                    _levelSelection.Draw(spriteBatch);
                    break;
                case MenuState.Options:
                    //smoothPixels.Draw(spriteBatch);
                    //lighting.Draw(spriteBatch);
                    _fullscreen.Draw(spriteBatch);
                    _backButton.Draw(spriteBatch);
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
