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
    class Menu
    {
        Vector2 first = new Vector2(550, 200);
        Vector2 second = new Vector2(550, 240);
        Vector2 third = new Vector2(550, 280);
        Vector2 fourth = new Vector2(550, 320);
        Vector2 fifth = new Vector2(550, 360);

        //Main Menu
        NewButton play;
        NewButton options;
        NewButton quit;
        NewButton multiplayer;

        //Level Selector
        NewButton save1;
        NewButton save2;
        NewButton save3;

        //Options
        NewButton smoothPixels;
        NewButton lighting;
        NewButton fullscreen;

        //Multiplayer
        NewButton hostGame;
        NewButton joinGame;
        NewButton connect;

        NewButton backButton;
        List<NewButton> buttons = new List<NewButton>();

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
        Game1 game1;

        List<Particle> zzzList = new List<Particle>();
        double zzzTimer;

        enum MenuState { Main, Options, LevelSelector, MultiplayerSession }
        MenuState CurrentMenuState = MenuState.Main;

        public Menu(Game1 game1)
        {
            this.game1 = game1;
            
            play = new NewButton(first, "Play");
            play.MouseClicked += play_MouseClicked;
            buttons.Add(play);

            quit = new NewButton(second, "Quit");
            quit.MouseClicked += quit_MouseClicked;
            buttons.Add(quit);

            options = new NewButton(third, "Options");
            options.MouseClicked += options_MouseClicked;
            buttons.Add(options);

            multiplayer = new NewButton(fourth, "Multiplayer");
            multiplayer.MouseClicked += multiplayer_MouseClicked;
            buttons.Add(multiplayer);

            smoothPixels = new NewButton(first, "Smooth Pixels: ");
            smoothPixels.MouseClicked += smoothPixels_MouseClicked;
            buttons.Add(smoothPixels);

            lighting = new NewButton(second, "Lighting: ");
            lighting.MouseClicked += lighting_MouseClicked;
            buttons.Add(lighting);

            fullscreen = new NewButton(third, "Fullscreen: ");
            fullscreen.MouseClicked += fullscreen_MouseClicked;
            fullscreen.IsActive = game1.GameData.Settings.IsFullscreen;
            buttons.Add(fullscreen);

            backButton = new NewButton(fifth, "Back");
            backButton.MouseClicked += backButton_MouseClicked;
            buttons.Add(backButton);

            save1 = new NewButton(first, "Save 1");
            save1.MouseClicked += level1_MouseClicked;
            buttons.Add(save1);

            save2 = new NewButton(second, "Save 2");
            save2.MouseClicked += level2_MouseClicked;
            buttons.Add(save2);

            save3 = new NewButton(third, "Save 3");
            save3.MouseClicked += level3_MouseClicked;
            buttons.Add(save3);

            hostGame = new NewButton(first, "Host Game");
            hostGame.MouseClicked += hostGame_MouseClicked;
            buttons.Add(hostGame);

            joinGame = new NewButton(second, "Join Game");
            joinGame.MouseClicked += joinGame_MouseClicked;
            buttons.Add(joinGame);
        }

        void joinGame_MouseClicked()
        {
            throw new NotImplementedException();
        }

        void hostGame_MouseClicked()
        {
            throw new NotImplementedException();
        }

        void level4_MouseClicked()
        {
            game1.ChangeState(GameState.GameWorld, Level.Level4and1);
        }

        void level3_MouseClicked()
        {
            game1.ChangeState(GameState.GameWorld, Level.Level3and1);
        }

        void level2_MouseClicked()
        {
            game1.ChangeState(GameState.GameWorld, Level.Level2and1);
        }

        void level1_MouseClicked()
        {
            game1.GameData.SelectedSave = 0;
            game1.ChangeState(GameState.GameWorld, game1.GameData.CurrentSave.CurrentLevel);
        }

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
            game1.Exit();
        }

        void play_MouseClicked()
        {
            CurrentMenuState = MenuState.LevelSelector;
        }

        void options_MouseClicked()
        {
            CurrentMenuState = MenuState.Options;
        }

        public void Load(ContentManager Content)
        {
            background = Content.Load<Texture2D>("Menu/menu_back");
            adam = Content.Load<Texture2D>("Menu/menu_adam");
            foreground = Content.Load<Texture2D>("Menu/menu_front");
            apple = ContentHelper.LoadTexture("Menu/menu_apple");

            theme = Content.Load<Song>("Music/Anguish");
            font = Content.Load<SpriteFont>("Fonts/button");

            

            int scale = 8;
            appleRect = new Rectangle(5 * scale, 36 * scale, 16 * scale, 16 * scale);
            appleSource = new Rectangle(0, 0, 16, 16);
            adamRect = new Rectangle(20 * scale, 16 * scale, (adam.Width / 4) * scale, adam.Height * scale);
            sourceRect = new Rectangle(0, 0, 24, 36);

        }

        public void Update(Game1 game1, GameTime gameTime, Settings settings)
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
                if (z.ToDelete())
                {
                    zzzList.Remove(z);
                    break;
                }
            }


            this.gameTime = gameTime;
            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    play.Update();
                    quit.Update();
                    options.Update();
                    multiplayer.Update();
                    break;
                case MenuState.LevelSelector:
                    save1.Text = "Save 1: " + game1.GameData.saves[0].Completeness;
                    save2.Text = "Save 2: " + game1.GameData.saves[1].Completeness;
                    save3.Text = "Save 3: " + game1.GameData.saves[2].Completeness;

                    save1.Update();
                    save2.Update();
                    save3.Update();
                    backButton.Update();
                    break;
                case MenuState.Options:
                    smoothPixels.Text = "Smooth Pixels: " + smoothPixels.IsActive;
                    lighting.Text = "Lighting: " + lighting.IsActive;
                    fullscreen.Text = "Fullscreen: " + fullscreen.IsActive;

                    smoothPixels.Update();
                    lighting.Update();
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
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight), Color.White);
            spriteBatch.Draw(foreground, new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight), Color.White);
            spriteBatch.Draw(adam, adamRect, sourceRect, Color.White);
            spriteBatch.Draw(apple, appleRect, appleSource, Color.White);

            spriteBatch.DrawString(font, Game1.Producers, new Vector2(5, 5), Color.White, 0, new Vector2(0, 0), .3f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, Game1.Version, new Vector2(5, 30), Color.White, 0, new Vector2(0, 0), .3f, SpriteEffects.None, 0);

            switch (CurrentMenuState)
            {
                case MenuState.Main:
                    play.Draw(spriteBatch);
                    quit.Draw(spriteBatch);
                    options.Draw(spriteBatch);
                    multiplayer.Draw(spriteBatch);
                    break;
                case MenuState.LevelSelector:
                    save1.Draw(spriteBatch);
                    save2.Draw(spriteBatch);
                    save3.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);
                    break;
                case MenuState.Options:
                    smoothPixels.Draw(spriteBatch);
                    lighting.Draw(spriteBatch);
                    fullscreen.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);
                    break;
            }

            foreach (var z in zzzList)
                z.Draw(spriteBatch);
        }

    }
}
