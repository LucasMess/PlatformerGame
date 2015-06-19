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

namespace Adam
{
    class Menu
    {
        //Main Menu
        Button play;
        Button options;
        Button quit;
        Button multiplayer;

        //Level Selector
        Button level1;
        Button level2;
        Button level3;
        Button level4;

        //Options
        Button smoothPixels;
        Button lighting;
        Button fullscreen;

        //Multiplayer
        Button hostGame;
        Button joinGame;
        Button connect;

        Button backButton;

        ContentManager Content;
        Vector2 monitorResolution;

        Texture2D background, foreground, adam, apple;
        Song theme;
        Rectangle adamRect, sourceRect;
        Rectangle appleRect, appleSource;
        GameTime gameTime;
        double frameTimer;
        int switchFrame, currentFrame;
        bool isSongPlaying;
        bool mouseButtonReleased;
        SoundEffect confirmSound, cursorSound, backSound, errorSound;
        SpriteFont font;

        List<Particle> zzzList = new List<Particle>();
        double zzzTimer;

        enum MenuState { Main, Options, LevelSelector, MultiplayerSession }
        MenuState CurrentMenuState = MenuState.Main;

        public Menu(Vector2 monitorResolution)
        {
            play = new Button();
            quit = new Button();
            options = new Button();
            multiplayer = new Button();

            smoothPixels = new Button();
            lighting = new Button();
            fullscreen = new Button();

            backButton = new Button();

            level1 = new Button();
            level2 = new Button();
            level3 = new Button();
            level4 = new Button();

            hostGame = new Button();
            joinGame = new Button();
            this.monitorResolution = monitorResolution;

        }

        public void Load(ContentManager Content)
        {
            this.Content = Content;
            play.Load(Content);
            quit.Load(Content);
            options.Load(Content);
            multiplayer.Load(Content);

            smoothPixels.Load(Content);
            lighting.Load(Content);
            fullscreen.Load(Content);

            backButton.Load(Content);

            level1.Load(Content);
            level2.Load(Content);
            level3.Load(Content);
            level4.Load(Content);

            SetPosition();
            SetText();

            background = Content.Load<Texture2D>("Menu/menu_back");
            adam = Content.Load<Texture2D>("Menu/menu_adam");
            foreground = Content.Load<Texture2D>("Menu/menu_front");
            apple = ContentHelper.LoadTexture("Menu/menu_apple");

            theme = Content.Load<Song>("Music/Alchemists Tower");
            font = Content.Load<SpriteFont>("Fonts/button");

            errorSound = Content.Load<SoundEffect>("Sounds/Menu/error_style_2_001");
            confirmSound = Content.Load<SoundEffect>("Sounds/Menu/confirm_style_4_001");
            cursorSound = Content.Load<SoundEffect>("Sounds/Menu/cursor_style_2");
            backSound = Content.Load<SoundEffect>("Sounds/Menu/back_style_2_001");

            int scale = 8;
            appleRect = new Rectangle(5 * scale, 36 * scale, 16 * scale, 16 * scale);
            appleSource = new Rectangle(0, 0, 16, 16);
            adamRect = new Rectangle(20 * scale, 17 * scale, adam.Width / 4 * scale, adam.Height * scale);
            sourceRect = new Rectangle(0, 0, 24, 36);

        }

        void SetText()
        {
            play.SetText("Play");
            quit.SetText("Quit");
            options.SetText("Options");
            multiplayer.SetText("Multiplayer");

            smoothPixels.SetText("Smooth Pixels: " + smoothPixels.currentSettingsState);
            lighting.SetText("Lighting: " + lighting.currentSettingsState);
            fullscreen.SetText("Fullscreen: " + fullscreen.currentSettingsState);

            backButton.SetText("Back");

            level1.SetText("Level 1 - Garden of Eden");
            level2.SetText("Level 2 - The Desert");
            level3.SetText("Level 3 - Debug");
            level4.SetText("Level 4 - None");
        }

        void SetPosition()
        {
            Vector2 first = new Vector2(550, 180);
            Vector2 second = new Vector2(550, 220);
            Vector2 third = new Vector2(550, 260);
            Vector2 fourth = new Vector2(550, 300);
            Vector2 fifth = new Vector2(550, 340);

            play.SetPosition(first);
            quit.SetPosition(second);
            options.SetPosition(third);
            multiplayer.SetPosition(fourth);

            smoothPixels.SetPosition(first);
            lighting.SetPosition(second);
            fullscreen.SetPosition(third);
            
            backButton.SetPosition(fifth);

            level1.SetPosition(first);
            level2.SetPosition(second);
            level3.SetPosition(third);
            level4.SetPosition(fourth);

            hostGame.SetPosition(first);
            joinGame.SetPosition(second);
        }

        public void Update(Game1 game1, GameTime gameTime, Settings settings)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                mouseButtonReleased = true;

            zzzTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (zzzTimer > 1)
            {
                zzzList.Add(new Particle(adamRect, Content));
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

                    if (play.IsPressed())
                    {
                        CurrentMenuState = MenuState.LevelSelector;
                        mouseButtonReleased = false;
                        cursorSound.Play();
                    }
                    if (quit.IsPressed())
                    {
                        cursorSound.Play();
                        game1.Exit();
                    }
                    if (options.IsPressed())
                    {
                        mouseButtonReleased = false;
                        CurrentMenuState = MenuState.Options;
                        cursorSound.Play();
                    }
                    //if (levelEditor.IsPressed())
                    //{
                    //    game1.ReloadLevelGen(GameState.LevelGen);
                    //    cursorSound.Play();
                    //}
                    break;
                case MenuState.LevelSelector:
                    level1.Update();
                    level2.Update();
                    level3.Update();
                    level4.Update();
                    backButton.Update();

                    if (level1.IsPressed() && mouseButtonReleased == true)
                    {
                        game1.ReloadMap(GameState.Level, Level.Level1);
                        confirmSound.Play();
                    }
                    if (level2.IsPressed())
                    {
                        game1.ReloadMap(GameState.Level, Level.Level2);
                        confirmSound.Play();
                    }
                    if (level3.IsPressed())
                    {
                        game1.ReloadMap(GameState.Level, Level.Level3);
                        confirmSound.Play();
                    }
                    if (level4.IsPressed())
                    {
                        game1.ReloadMap(GameState.Level, Level.Level4);
                        confirmSound.Play();
                    }

                    if (backButton.IsPressed())
                    {
                        CurrentMenuState = MenuState.Main;
                        backSound.Play();
                    }

                    break;
                case MenuState.Options:
                    smoothPixels.SetText("Smooth Pixels: " + smoothPixels.currentSettingsState);
                    lighting.SetText("Lighting: " + lighting.currentSettingsState);
                    fullscreen.SetText("Fullscreen: " + fullscreen.currentSettingsState);

                    smoothPixels.Update();
                    lighting.Update();
                    fullscreen.Update();

                    backButton.Update();

                    //Smooth Pixels
                    if (smoothPixels.IsPressed() && smoothPixels.wasPressed == false && smoothPixels.currentSettingsState == Button.SettingsState.OFF)
                    {
                        smoothPixels.wasPressed = true;
                        settings.DesiredSamplerState = SamplerState.LinearClamp;
                        settings.HasChanged = true;
                        cursorSound.Play();
                    }

                    if (smoothPixels.IsPressed() && smoothPixels.wasPressed == false && smoothPixels.currentSettingsState == Button.SettingsState.ON)
                    {
                        smoothPixels.wasPressed = true;
                        settings.DesiredSamplerState = SamplerState.PointClamp;
                        settings.HasChanged = true;
                        cursorSound.Play();
                    }

                    if (settings.DesiredSamplerState == SamplerState.PointClamp)
                        smoothPixels.currentSettingsState = Button.SettingsState.OFF;
                    else smoothPixels.currentSettingsState = Button.SettingsState.ON;

                    if (smoothPixels.IsPressed() == false)
                        smoothPixels.wasPressed = false;

                    //Lighting
                    if (lighting.IsPressed() && lighting.wasPressed == false && lighting.currentSettingsState == Button.SettingsState.OFF)
                    {
                        lighting.wasPressed = true;
                        settings.DesiredLight = true;
                        settings.HasChanged = true;
                        cursorSound.Play();
                    }
                    if (lighting.IsPressed() && lighting.wasPressed == false && lighting.currentSettingsState == Button.SettingsState.ON)
                    {
                        lighting.wasPressed = true;
                        settings.DesiredLight = false;
                        settings.HasChanged = true;
                        cursorSound.Play();
                    }

                    if (settings.DesiredLight == false)
                        lighting.currentSettingsState = Button.SettingsState.OFF;
                    else lighting.currentSettingsState = Button.SettingsState.ON;

                    if (lighting.IsPressed() == false)
                        lighting.wasPressed = false;

                    //fullscreen
                    if (fullscreen.IsPressed() && fullscreen.wasPressed == false && fullscreen.currentSettingsState == Button.SettingsState.OFF && mouseButtonReleased)
                    {
                        fullscreen.wasPressed = true;
                        settings.IsFullscreen = true;
                        settings.HasChanged = true;
                        settings.NeedsRestart = true;
                        cursorSound.Play();
                    }
                    if (fullscreen.IsPressed() && fullscreen.wasPressed == false && fullscreen.currentSettingsState == Button.SettingsState.ON && mouseButtonReleased)
                    {
                        fullscreen.wasPressed = true;
                        settings.IsFullscreen = false;
                        settings.HasChanged = true;
                        settings.NeedsRestart = true;
                        cursorSound.Play();
                    }

                    if (settings.IsFullscreen == false)
                        fullscreen.currentSettingsState = Button.SettingsState.OFF;
                    else fullscreen.currentSettingsState = Button.SettingsState.ON;

                    if (fullscreen.IsPressed() == false)
                        fullscreen.wasPressed = false;

                    if (backButton.IsPressed())
                    {
                        backSound.Play();
                        CurrentMenuState = MenuState.Main;
                    }
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

            AnimateAdam();
            SleepEffect();

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

        public void AnimateAdam()
        {
            switchFrame = 600;
            frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameTimer > switchFrame)
            {
                frameTimer = 0;
                sourceRect.X += adamRect.Width;
                currentFrame++;
            }

            if (currentFrame > 3)
            {
                sourceRect.X = 0;
                currentFrame = 0;
            }
        }

        public void SleepEffect()
        {
            //zzzTimer += gameTime.ElapsedGameTime.TotalSeconds;
            //if (zzzTimer > 1)
            //{

            //}
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           
        }


        public void DrawBackground(SpriteBatch spriteBatch)
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
                    level1.Draw(spriteBatch);
                    level2.Draw(spriteBatch);
                    level3.Draw(spriteBatch);
                    level4.Draw(spriteBatch);
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

        public bool WantsToExit()
        {
            if (quit.IsPressed())
                return true;
            else return false;
        }
    }
}
