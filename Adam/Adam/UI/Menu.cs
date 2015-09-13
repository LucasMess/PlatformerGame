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

namespace Adam
{
    public class Menu
    {
        Vector2 first;
        Vector2 second;
        Vector2 third;
        Vector2 fourth;
        Vector2 fifth;

        //Main Menu
        Button chooseLevel;
        Button options;
        Button quit;
        Button multiplayer;
        Button storyMode;

        //Level Selector
        Button save1;
        Button save2;
        Button save3;

        //Options
        Button smoothPixels;
        Button lighting;
        Button fullscreen;

        //Multiplayer
        Button hostGame;
        Button joinGame;
        Button connect;

        Button backButton;
        List<Button> buttons = new List<Button>();

        Texture2D background, foreground, adam, apple;
        Song theme;
        Rectangle adamRect, sourceRect;
        Rectangle appleRect, appleSource;
        GameTime gameTime;
        double frameTimer;
        double appleTimer;
        int switchFrame, currentFrame;
        bool isSongPlaying;
        SpriteFont font;
        Main game1;

        LevelSelection levelSelection;

        List<Particle> zzzList = new List<Particle>();
        double zzzTimer;

        public enum MenuState { Main, Options, LevelSelector, MultiplayerSession }
        public static MenuState CurrentMenuState = MenuState.Main;

        public Menu(Main game1)
        {
            this.game1 = game1;

            int width = (int)(530 / Main.WidthRatio);
            int height = (int)(200 / Main.HeightRatio);
            int diff = (int)(40 / Main.HeightRatio);
            first = new Vector2(width, height + (diff * 0));
            second = new Vector2(width, height + (diff * 1));
            third = new Vector2(width, height + (diff * 2));
            fourth = new Vector2(width, height + (diff * 3));
            fifth = new Vector2(width, height + (diff * 4));

            chooseLevel = new Button(second, "Choose a Level");
            chooseLevel.MouseClicked += chooseLevel_MouseClicked;
            buttons.Add(chooseLevel);

            quit = new Button(fifth, "Quit");
            quit.MouseClicked += quit_MouseClicked;
            buttons.Add(quit);

            options = new Button(third, "Options");
            options.MouseClicked += options_MouseClicked;
            buttons.Add(options);

            //multiplayer = new Button(fourth, "Multiplayer");
            //multiplayer.MouseClicked += multiplayer_MouseClicked;
            //buttons.Add(multiplayer);

            storyMode = new Button(first, "Story Mode");
            storyMode.MouseClicked += storyMode_MouseClicked;
            buttons.Add(storyMode);

            //smoothPixels = new Button(first, "Smooth Pixels: ");
            //smoothPixels.MouseClicked += smoothPixels_MouseClicked;
            //buttons.Add(smoothPixels);

            //lighting = new Button(second, "Lighting: ");
            //lighting.MouseClicked += lighting_MouseClicked;
            //buttons.Add(lighting);

            fullscreen = new Button(first, "Borderless Mode: ");
            fullscreen.MouseClicked += fullscreen_MouseClicked;
            fullscreen.IsActive = game1.GameData.Settings.IsFullscreen;
            buttons.Add(fullscreen);

            backButton = new Button(fifth, "Back");
            backButton.MouseClicked += backButton_MouseClicked;
            buttons.Add(backButton);

            //save1 = new Button(first, "Save 1");
            //save1.MouseClicked += level1_MouseClicked;
            //buttons.Add(save1);

            //save2 = new Button(second, "Save 2");
            //save2.MouseClicked += level2_MouseClicked;
            //buttons.Add(save2);

            //save3 = new Button(third, "Save 3");
            //save3.MouseClicked += level3_MouseClicked;
            //buttons.Add(save3);

            hostGame = new Button(first, "Host Game");
            hostGame.MouseClicked += hostGame_MouseClicked;
            buttons.Add(hostGame);

            joinGame = new Button(second, "Join Game");
            joinGame.MouseClicked += joinGame_MouseClicked;
            buttons.Add(joinGame);

            levelSelection = new LevelSelection();
        }

        private void storyMode_MouseClicked()
        {
            Main.MessageBox.Show("Coming Soon...");
        }

        void joinGame_MouseClicked()
        {
            throw new NotImplementedException();
        }

        void hostGame_MouseClicked()
        {
            throw new NotImplementedException();
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
                case MenuState.MultiplayerSession:
                    CurrentMenuState = MenuState.Main;
                    break;
                default:
                    break;
            }
        }

        void fullscreen_MouseClicked()
        {

            switch (fullscreen.IsActive)
            {
                case true:
                    fullscreen.IsActive = false;
                    game1.GameData.Settings.IsFullscreen = false;
                    game1.GameData.Settings.NeedsRestart = true;
                    game1.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    fullscreen.IsActive = true;
                    game1.GameData.Settings.IsFullscreen = true;
                    game1.GameData.Settings.NeedsRestart = true;
                    game1.GameData.Settings.HasChanged = true;
                    break;
                default:
                    break;
            }
        }

        void lighting_MouseClicked()
        {
            switch (lighting.IsActive)
            {
                case true:
                    lighting.IsActive = false;
                    game1.GameData.Settings.DesiredLight = false;
                    game1.GameData.Settings.NeedsRestart = true;
                    game1.GameData.Settings.HasChanged = true;
                    break;
                case false:
                    lighting.IsActive = true;
                    game1.GameData.Settings.DesiredLight = true;
                    game1.GameData.Settings.NeedsRestart = true;
                    game1.GameData.Settings.HasChanged = true;
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
            CurrentMenuState = MenuState.MultiplayerSession;
        }

        void quit_MouseClicked()
        {
            // game1.GameData.SaveGame();
            game1.Exit();
        }

        void chooseLevel_MouseClicked()
        {
            levelSelection.LoadLevels();
            CurrentMenuState = MenuState.LevelSelector;

            //game1.CurrentGameMode = GameMode.Play;
            //GameWorld.Instance.worldData.OpenLevelLocally(false);
        }

        void options_MouseClicked()
        {
            CurrentMenuState = MenuState.Options;
        }

        public void Load(ContentManager Content)
        {
            background = ContentHelper.LoadTexture("Menu/menu_back");
            adam = ContentHelper.LoadTexture("Menu/menu_adam");
            foreground = ContentHelper.LoadTexture("Menu/menu_front");
            apple = ContentHelper.LoadTexture("Menu/menu_apple");

            theme = Content.Load<Song>("Music/Force Reunite");
            font = Content.Load<SpriteFont>("Fonts/button");



            double scaleWidth = 8 / Main.WidthRatio;
            double scaleHeight = 8 / Main.HeightRatio;

            appleRect = new Rectangle((int)(5 * scaleWidth), (int)(36 * scaleHeight), (int)(16 * scaleWidth), (int)(16 * scaleHeight));
            appleSource = new Rectangle(0, 0, 16, 16);
            adamRect = new Rectangle((int)(20 * scaleWidth), (int)(16 * scaleHeight), (int)(adam.Width*.25* scaleWidth), (int)(adam.Height * scaleHeight));
            sourceRect = new Rectangle(0, 0, 24, 36);

        }

        public void Update(Main game1, GameTime gameTime, Settings settings)
        {
            zzzTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (zzzTimer > 1)
            {
                zzzList.Add(new Particle(adamRect));
                zzzTimer = 0;
            }

            foreach (var z in zzzList)
            {
                z.Update(gameTime);
            }
            foreach (var z in zzzList)
            {
                if (z.ToDelete)
                {
                    zzzList.Remove(z);
                    break;
                }
            }


            this.gameTime = gameTime;
            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    chooseLevel.Update();
                    quit.Update();
                    options.Update();
                    //multiplayer.Update();
                    storyMode.Update();
                    break;
                case MenuState.LevelSelector:
                    levelSelection.Update();
                    break;
                case MenuState.Options:
                    //smoothPixels.Text = "Smooth Pixels: " + smoothPixels.IsActive;
                    //lighting.Text = "Lighting: " + lighting.IsActive;
                    fullscreen.Text = "Borderless Mode: " + !fullscreen.IsActive;

                    //smoothPixels.Update();
                    //lighting.Update();
                    fullscreen.Update();

                    backButton.Update();
                    break;

                case MenuState.MultiplayerSession:
                    //if (hostGame.IsPressed() && hostGame.wasPressed == false && mouseButtonReleased)
                    //{
                    //    game1.session = new Session(true, "Host");
                    //}
                    //if (joinGame.IsPressed() && joinGame.wasPressed == false && mouseButtonReleased)
                    //{
                    //    game1.session = new Session(false, "Random Player");
                    //    game1.session.ConnectTo(address, 42555);
                    //}
                    break;
            }

            AnimateSprites();

            if (!isSongPlaying && game1.CurrentGameState == GameState.MainMenu)
            {
                MediaPlayer.Play(theme);
                isSongPlaying = true;
            }

            if (game1.CurrentGameState != GameState.MainMenu)
            {
                MediaPlayer.Stop();
                isSongPlaying = false;
            }

        }

        int appleFrame;
        public void AnimateSprites()
        {
            switchFrame = 600;
            frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            appleTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameTimer > switchFrame)
            {
                frameTimer = 0;
                sourceRect.X += sourceRect.Width;
                currentFrame++;
            }

            if (currentFrame > 3)
            {
                sourceRect.X = 0;
                currentFrame = 0;
            }

            if (appleTimer > 150)
            {
                appleSource.X += appleSource.Width;
                appleTimer = 0;
                appleFrame++;
            }

            if (appleFrame > 3)
            {
                appleSource.X = 0;
                appleFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.White);
            spriteBatch.Draw(foreground, new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.White);
            spriteBatch.Draw(adam, adamRect, sourceRect, Color.White);
            spriteBatch.Draw(apple, appleRect, appleSource, Color.White);

            spriteBatch.DrawString(font, Main.Producers, new Vector2(5, (float)(5 / Main.HeightRatio)), Color.White, 0, new Vector2(0, 0), (float)(.5/Main.HeightRatio), SpriteEffects.None, 0);
            spriteBatch.DrawString(font, Main.Version, new Vector2(5, (float)(30/Main.HeightRatio)), Color.White, 0, new Vector2(0, 0), (float)(.5 / Main.HeightRatio), SpriteEffects.None, 0);


            foreach (var z in zzzList)
                z.Draw(spriteBatch);

            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    chooseLevel.Draw(spriteBatch);
                    quit.Draw(spriteBatch);
                    options.Draw(spriteBatch);
                   // multiplayer.Draw(spriteBatch);
                    storyMode.Draw(spriteBatch);
                    break;
                case MenuState.LevelSelector:
                    levelSelection.Draw(spriteBatch);
                    break;
                case MenuState.Options:
                    //smoothPixels.Draw(spriteBatch);
                    //lighting.Draw(spriteBatch);
                    fullscreen.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);
                    break;
            }

        }

    }
}
