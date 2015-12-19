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

namespace Adam
{
    public class Menu
    {
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

        Texture2D _background, _foreground, _adam, _apple;
        Rectangle _adamRect, _sourceRect;
        Rectangle _appleRect, _appleSource;
        GameTime _gameTime;
        double _frameTimer;
        double _appleTimer;
        int _switchFrame, _currentFrame;
        bool _isSongPlaying;
        SpriteFont _font;
        Main _game1;

        LevelSelection _levelSelection;

        List<Particle> _zzzList = new List<Particle>();
        double _zzzTimer;

        public enum MenuState { Main, Options, LevelSelector, HostJoin, MultiplayerSession  }
        public static MenuState CurrentMenuState = MenuState.Main;

        public Menu(Main game1)
        {
            this._game1 = game1;

            int width = (int)(530 / Main.WidthRatio);
            int height = (int)(200 / Main.HeightRatio);
            int diff = (int)(40 / Main.HeightRatio);
            _first = new Vector2(width, height + (diff * 0));
            _second = new Vector2(width, height + (diff * 1));
            _third = new Vector2(width, height + (diff * 2));
            _fourth = new Vector2(width, height + (diff * 3));
            _fifth = new Vector2(width, height + (diff * 4));

            _chooseLevel = new Button(_second, "Choose a Level");
            _chooseLevel.MouseClicked += chooseLevel_MouseClicked;
            _buttons.Add(_chooseLevel);

            _quit = new Button(_fifth, "Quit");
            _quit.MouseClicked += quit_MouseClicked;
            _buttons.Add(_quit);

            _options = new Button(_third, "Options");
            _options.MouseClicked += options_MouseClicked;
            _buttons.Add(_options);

            _multiplayer = new Button(_fourth, "Multiplayer");
            _multiplayer.MouseClicked += multiplayer_MouseClicked;
            _buttons.Add(_multiplayer);

            _storyMode = new Button(_first, "Story Mode");
            _storyMode.MouseClicked += storyMode_MouseClicked;
            _buttons.Add(_storyMode);

            //smoothPixels = new Button(first, "Smooth Pixels: ");
            //smoothPixels.MouseClicked += smoothPixels_MouseClicked;
            //buttons.Add(smoothPixels);

            //lighting = new Button(second, "Lighting: ");
            //lighting.MouseClicked += lighting_MouseClicked;
            //buttons.Add(lighting);

            _fullscreen = new Button(_first, "Borderless Mode: ");
            _fullscreen.MouseClicked += fullscreen_MouseClicked;
            _fullscreen.IsActive = game1.GameData.Settings.IsFullscreen;
            _buttons.Add(_fullscreen);

            _backButton = new Button(_fifth, "Back");
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

            _hostGame = new Button(_first, "Host Game");
            _hostGame.MouseClicked += hostGame_MouseClicked;
            _buttons.Add(_hostGame);

            _joinGame = new Button(_second, "Join Game");
            _joinGame.MouseClicked += joinGame_MouseClicked;
            _buttons.Add(_joinGame);

            _startMultiplayerGame = new Button(_third, "Start Game");
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

            switch (_fullscreen.IsActive)
            {
                case true:
                    _fullscreen.IsActive = false;
                    _game1.GameData.Settings.IsFullscreen = false;
                    _game1.GameData.Settings.NeedsRestart = true;
                    _game1.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    _fullscreen.IsActive = true;
                    _game1.GameData.Settings.IsFullscreen = true;
                    _game1.GameData.Settings.NeedsRestart = true;
                    _game1.GameData.Settings.HasChanged = true;
                    break;
                default:
                    break;
            }
        }

        void lighting_MouseClicked()
        {
            switch (_lighting.IsActive)
            {
                case true:
                    _lighting.IsActive = false;
                    _game1.GameData.Settings.DesiredLight = false;
                    _game1.GameData.Settings.NeedsRestart = true;
                    _game1.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    _lighting.IsActive = true;
                    _game1.GameData.Settings.DesiredLight = true;
                    _game1.GameData.Settings.NeedsRestart = true;
                    _game1.GameData.Settings.HasChanged = true;
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
            //GameWorld.Instance.worldData.OpenLevelLocally(false);
        }

        void options_MouseClicked()
        {
            CurrentMenuState = MenuState.Options;
        }

        public void Load(ContentManager content)
        {
            _background = ContentHelper.LoadTexture("Menu/menu_back");
            _adam = ContentHelper.LoadTexture("Menu/menu_adam");
            _foreground = ContentHelper.LoadTexture("Menu/menu_front");
            _apple = ContentHelper.LoadTexture("Menu/menu_apple");
            _font = content.Load<SpriteFont>("Fonts/x32");



            double scaleWidth = 8 / Main.WidthRatio;
            double scaleHeight = 8 / Main.HeightRatio;

            _appleRect = new Rectangle((int)(5 * scaleWidth), (int)(36 * scaleHeight), (int)(16 * scaleWidth), (int)(16 * scaleHeight));
            _appleSource = new Rectangle(0, 0, 16, 16);
            _adamRect = new Rectangle((int)(20 * scaleWidth), (int)(16 * scaleHeight), (int)(_adam.Width*.25* scaleWidth), (int)(_adam.Height * scaleHeight));
            _sourceRect = new Rectangle(0, 0, 24, 36);

        }

        public void Update(Main game1, GameTime gameTime, Settings settings)
        {
            _zzzTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_zzzTimer > 1)
            {
                _zzzList.Add(new Particle(_adamRect));
                _zzzTimer = 0;
            }

            foreach (var z in _zzzList)
            {
                z.Update(gameTime);
            }
            foreach (var z in _zzzList)
            {
                if (z.ToDelete)
                {
                    _zzzList.Remove(z);
                    break;
                }
            }


            this._gameTime = gameTime;
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
                    _fullscreen.Text = "Borderless Mode: " + !_fullscreen.IsActive;

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

            AnimateSprites();

            if (game1.CurrentGameState == GameState.MainMenu)
            SoundtrackManager.PlayMainTheme();

        }

        int _appleFrame;
        public void AnimateSprites()
        {
            _switchFrame = 600;
            _frameTimer += _gameTime.ElapsedGameTime.TotalMilliseconds;
            _appleTimer += _gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_frameTimer > _switchFrame)
            {
                _frameTimer = 0;
                _sourceRect.X += _sourceRect.Width;
                _currentFrame++;
            }

            if (_currentFrame > 3)
            {
                _sourceRect.X = 0;
                _currentFrame = 0;
            }

            if (_appleTimer > 150)
            {
                _appleSource.X += _appleSource.Width;
                _appleTimer = 0;
                _appleFrame++;
            }

            if (_appleFrame > 3)
            {
                _appleSource.X = 0;
                _appleFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.White);
            spriteBatch.Draw(_foreground, new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.White);
            spriteBatch.Draw(_adam, _adamRect, _sourceRect, Color.White);
            spriteBatch.Draw(_apple, _appleRect, _appleSource, Color.White);

            spriteBatch.DrawString(_font, Main.Producers, new Vector2(5, (float)(5 / Main.HeightRatio)), Color.White, 0, new Vector2(0, 0), (float)(.5/Main.HeightRatio), SpriteEffects.None, 0);
            spriteBatch.DrawString(_font, Main.Version, new Vector2(5, (float)(30/Main.HeightRatio)), Color.White, 0, new Vector2(0, 0), (float)(.5 / Main.HeightRatio), SpriteEffects.None, 0);


            foreach (var z in _zzzList)
                z.Draw(spriteBatch);

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
