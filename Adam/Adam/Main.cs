using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Adam;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Adam.GameData;
using Adam.Network;
using Adam.UI;
using Adam.UI.Information;

namespace Adam
{
    public enum GameState
    {
        SplashScreen,
        Cutscene,
        MainMenu,
        LoadingScreen,
        GameWorld,
    }
    public enum GameMode
    {
        None,
        Edit,
        Play,
    }

    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables
        static Color SunnyPreset = new Color(255, 238, 186);
        static Color HellPreset = new Color(255, 129, 116);
        static Color WinterPreset = new Color(200, 243, 255);
        static Color NightPreset = new Color(120, 127, 183);
        static Color SunsetPreset = new Color(255, 155, 13);

        GraphicsDeviceManager graphics;
        SpriteBatch gameSB, debugSB, mainSB, UiSB, backgroundSB, lightingSB, mainLightSB;
        SpriteFont debugFont;
        Camera camera;
        public Vector2 monitorRes;
        RenderTarget2D mainRenderTarget, lightingRenderTarget;
        Menu menu;
        Thread reloadThread;
        Overlay overlay;
        public static Dialog Dialog;
        Texture2D splashDKD, blackScreen;
        SoundEffect quack;
        GameDebug debug;
        Cutscene cutscene;
        bool hasLoadedContent, hasQuacked;
        bool isDebug = true;
        Stopwatch updateWatch, drawWatch, renderWatch, lightWatch, loadWatch;
        double splashTimer, updateTime, drawTime, lightTime, renderTime;
        double frameRateTimer;
        int fps, totalFrames;
        int updateCount, drawCount;

        public const int Tilesize = 32;
        public const int DefaultResWidth = 960;
        public const int DefaultResHeight = 540;
        public const string Version = "Version 0.6.1.0 Beta";
        public const string Producers = "Duck Knight Duel Games";
        public static int UserResWidth;
        public static int UserResHeight;
        public static Texture2D DefaultTexture;
        public const float Gravity = .5f;
        public static double WidthRatio;
        public static double HeightRatio;

        public bool wasPressed, debugOn, debugPressed;

        public SamplerState desiredSamplerState;

        //Defines the initial GameState ----- Use this variable to change the GameState
        public GameState CurrentGameState;
        GameState desiredGameState;
        public GameMode CurrentGameMode;

        //Game Variables
        GameWorld gameWorld;
        Session session;
        public static ObjectiveTracker ObjectiveTracker;
        public GameDataManager GameData;
        public Player player;
        LoadingScreen loadingScreen;
        public static ContentManager Content;
        #endregion

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);

            //Get the current monitor resolution and set it as the game's resolution
            monitorRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            UserResWidth = (int)monitorRes.X;
            UserResHeight = (int)monitorRes.Y;
            WidthRatio = ((double)Main.DefaultResWidth / (double)Main.UserResWidth);
            HeightRatio = ((double)Main.DefaultResHeight / (double)Main.UserResHeight);

            graphics.PreferredBackBufferWidth = UserResWidth;
            graphics.PreferredBackBufferHeight = UserResHeight;


            //Change Game Settings Here
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = false;
            IsFixedTimeStep = true;

            Content = new ContentManager(Services, "Content");

            GameData = new GameDataManager();
            graphics.IsFullScreen = GameData.Settings.IsFullscreen;

            //Set window to borderless
            IntPtr hWnd = this.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;



            //MediaPlayer Settings
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .5f;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        protected override void Initialize()
        {
            DefaultTexture = ContentHelper.LoadTexture("Tiles/temp tile");
            //Initialize all instances
            camera = new Camera(GraphicsDevice.Viewport, monitorRes, new Vector2(DefaultResWidth, DefaultResHeight));
            menu = new Menu(this);
            gameWorld = new GameWorld(this);
            player = new Player(this);
            overlay = new Overlay();
            cutscene = new Cutscene();
            Dialog = new Dialog();
            ObjectiveTracker = new ObjectiveTracker();

            //Initialize the game render target
            mainRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            lightingRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);

            //Initialize Spritebatches
            gameSB = new SpriteBatch(GraphicsDevice);
            debugSB = new SpriteBatch(GraphicsDevice);
            backgroundSB = new SpriteBatch(GraphicsDevice);
            UiSB = new SpriteBatch(GraphicsDevice);
            mainSB = new SpriteBatch(GraphicsDevice);
            lightingSB = new SpriteBatch(GraphicsDevice);
            mainLightSB = new SpriteBatch(GraphicsDevice);

            updateWatch = new Stopwatch();
            drawWatch = new Stopwatch();
            lightWatch = new Stopwatch();
            renderWatch = new Stopwatch();
            loadWatch = new Stopwatch();

            base.Initialize();

        }

        protected override void LoadContent()
        {
            CurrentGameState = GameState.SplashScreen;
            loadingScreen = new LoadingScreen(monitorRes, Content);
            splashDKD = Content.Load<Texture2D>("Backgrounds/Splash/DKD_new");
            quack = Content.Load<SoundEffect>("Backgrounds/Splash/quack");
            blackScreen = Content.Load<Texture2D>("Tiles/black");

            debugFont = Content.Load<SpriteFont>("debug");
            menu.Load(Content);
            cutscene.Load(Content);

            debug = new GameDebug(debugFont, monitorRes, blackScreen);

            CurrentGameMode = GameMode.None;

        }

        public void ChangeState(GameState desiredGameState, GameMode mode)
        {
            CurrentGameState = GameState.LoadingScreen;
            this.desiredGameState = desiredGameState;
            hasLoadedContent = false;
            loadingScreen.Restart();

            if (desiredGameState == GameState.GameWorld)
            {
                LoadWorldFromFile(mode);
            }
            else
            {
                hasLoadedContent = true;
            }

        }

        public void LoadWorldFromFile(GameMode mode)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentGameMode = mode;
            desiredGameState = GameState.GameWorld;
            hasLoadedContent = false;
            loadingScreen.Restart();

            reloadThread = new Thread(new ThreadStart(BackgroundThread_FileLoad));
            reloadThread.IsBackground = true;
            reloadThread.Start();
        }

        private void BackgroundThread_FileLoad()
        {
            hasLoadedContent = false;
            gameWorld.LoadFromFile(CurrentGameMode);
            ObjectiveTracker = GameData.CurrentSave.ObjTracker;
            hasLoadedContent = true;
            wasPressed = false;
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            updateWatch.Start();

            frameRateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (frameRateTimer > 1000f)
            {
                fps = totalFrames;
                totalFrames = 0;
                frameRateTimer = 0;
            }

            #region Mouse Settings

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    this.IsMouseVisible = true;
                    break;
                case GameState.GameWorld:
                    this.IsMouseVisible = true;
                    break;
            }
            #endregion

            if (GameData.Settings.HasChanged)
            {
                GameData.SaveSettings();
                if (GameData.Settings.NeedsRestart)
                {
                    graphics.IsFullScreen = GameData.Settings.IsFullscreen;
                    graphics.ApplyChanges();
                    GameData.Settings.NeedsRestart = false;
                }
                GameData.Settings.HasChanged = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && CurrentGameState != GameState.MainMenu && CurrentGameState != GameState.LoadingScreen)
            {
                if (CurrentGameMode == GameMode.Edit)
                {
                    if (GameWorld.Instance.levelEditor.actionBar.AskSaveDialog())
                    {
                        GameWorld.Instance.levelEditor.actionBar.SaveButton_MouseClicked();
                    }
                }
                GameData.SaveGame();
                ChangeState(GameState.MainMenu, GameMode.None);

            }




            //Update the game based on what GameState it is
            switch (CurrentGameState)
            {
                case GameState.SplashScreen:
                    splashTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (splashTimer > 2)
                        CurrentGameState = GameState.Cutscene;
                    if (splashTimer > 1 && !hasQuacked)
                    {
                        hasQuacked = true;
                        quack.Play();
                    }
                    break;
                case GameState.Cutscene:
                    if (isDebug)
                    {
                        CurrentGameState = GameState.MainMenu;
                        cutscene.Stop();
                    }
                    if (cutscene.wasPlayed == false)
                    {
                        cutscene.Update(CurrentGameState);
                    }
                    else CurrentGameState = GameState.MainMenu;
                    if (InputHelper.IsAnyInputPressed())
                    {
                        CurrentGameState = GameState.MainMenu;
                        cutscene.Stop();
                    }
                    break;
                case GameState.MainMenu:
                    menu.Update(this, gameTime, GameData.Settings);
                    break;
                case GameState.LoadingScreen:
                    loadingScreen.Update(gameTime);

                    if (hasLoadedContent && loadingScreen.isReady)
                    {
                        CurrentGameState = desiredGameState;
                        overlay.FadeIn();
                    }
                    break;
                case GameState.GameWorld:
                    if (!hasLoadedContent) return;
                    if (gameWorld.isOnDebug)
                        break;



                    //if (gameWorld.SimulationPaused)
                    //    break;

                    gameWorld.Update(gameTime, CurrentGameMode, camera);
                    player.Update(gameTime);
                    overlay.Update(gameTime, player, gameWorld);
                    Dialog.Update(gameTime);
                    ObjectiveTracker.Update(gameTime);

                    if (player.returnToMainMenu)
                        ChangeState(GameState.MainMenu, GameMode.None);
                    break;
            }

            base.Update(gameTime);
            debug.Update(this, player, gameWorld, debugOn);

            if (drawCount > 100000 || updateCount > 100000)
            {
                drawCount = 0;
                updateCount = 0;
            }

            updateCount++;
            updateWatch.Stop();
            updateTime = updateWatch.ElapsedMilliseconds;
            updateWatch.Reset();
        }

        protected void PrepareRenderTargets()
        {
            //Does all rendertarget work
            switch (CurrentGameState)
            {
                case GameState.GameWorld:
                    renderWatch.Start();
                    DrawToMainRenderTarget(mainRenderTarget);
                    renderTime = renderWatch.ElapsedMilliseconds;
                    renderWatch.Reset();

                    lightWatch.Start();
                    DrawLightingRenderTarget(lightingRenderTarget);
                    lightTime = lightWatch.ElapsedMilliseconds;
                    lightWatch.Reset();

                    break;
                case GameState.Cutscene:
                    DrawToMainRenderTarget(mainRenderTarget);
                    break;
                case GameState.MainMenu:
                    DrawToMainRenderTarget(mainRenderTarget);
                    break;
            }

        }

        protected void DrawToMainRenderTarget(RenderTarget2D renderTarget)
        {
            //Change RenderTarget to this from the default
            GraphicsDevice.SetRenderTarget(renderTarget);
            //Set up the background color
            GraphicsDevice.Clear(Color.Black);

            //Draw what is needed based on GameState
            switch (CurrentGameState)
            {
                case GameState.LoadingScreen:
                    break;
                case GameState.Cutscene:
                    gameSB.Begin();
                    cutscene.Draw(gameSB);
                    gameSB.End();
                    break;
                case GameState.MainMenu:
                    //backgroundSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    //menu.Draw(backgroundSB);
                    //backgroundSB.End();
                    break;
                case GameState.GameWorld:
                    backgroundSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    gameWorld.DrawBackground(backgroundSB);
                    gameWorld.DrawClouds(backgroundSB);
                    backgroundSB.End();

                    gameSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Translate);
                    gameWorld.DrawInBack(gameSB);
                    gameWorld.Draw(gameSB);
                    player.Draw(gameSB);
                    gameSB.End();
                    gameSB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null, camera.Translate);
                    gameWorld.DrawGlows(gameSB);
                    gameSB.End();
                    break;
            }

            //Return the current RenderTarget to the default
            GraphicsDevice.SetRenderTarget(null);
        }

        protected void DrawLightingRenderTarget(RenderTarget2D renderTarget)
        {
            //Change RenderTarget to this from the default
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);

            switch (CurrentGameState)
            {
                case GameState.GameWorld:
                    lightingSB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null);
                    lightingSB.Draw(ContentHelper.LoadTexture("Tiles/max_shadow"), new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);
                    lightingSB.End();

                    lightingSB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null, camera.Translate);
                    gameWorld.DrawLights(lightingSB);
                    lightingSB.End();
                    break;
            }

            //Return the current RenderTarget to the default
            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            drawWatch.Start();
            totalFrames++;
            PrepareRenderTargets();

            //Set background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw what is needed based on GameState
            switch (CurrentGameState)
            {
                case GameState.SplashScreen:
                    gameSB.Begin();
                    gameSB.Draw(splashDKD, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    gameSB.End();
                    break;
                case GameState.Cutscene:
                    gameSB.Begin();
                    gameSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    gameSB.End();
                    break;
                case GameState.LoadingScreen:
                    gameSB.Begin();
                    loadingScreen.Draw(gameSB);
                    gameSB.End();
                    break;
                case GameState.MainMenu:
                    gameSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    menu.Draw(gameSB);
                    gameSB.End();
                    break;
                case GameState.GameWorld:
                    //Draw the rendertarget
                    mainSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    mainSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    mainSB.End();

                    if (GameData.Settings.DesiredLight)
                    {

                        BlendState LightBlendState = new BlendState();
                        LightBlendState.AlphaSourceBlend = Blend.DestinationColor;
                        LightBlendState.ColorSourceBlend = Blend.DestinationColor;
                        LightBlendState.ColorDestinationBlend = Blend.Zero;
                        mainLightSB.Begin(SpriteSortMode.Immediate, LightBlendState, GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                        mainLightSB.Draw(lightingRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), SunnyPreset);
                        mainLightSB.End();
                    }

                    UiSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    overlay.Draw(UiSB);

                    if (!gameWorld.levelEditor.onInventory)
                        ObjectiveTracker.Draw(UiSB);

                    gameWorld.DrawUI(UiSB);
                    Dialog.Draw(UiSB);

                    UiSB.End();

                    break;
            }
            base.Draw(gameTime);

            if (CurrentGameState != GameState.SplashScreen)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.F3) && !debugPressed && !debugOn)
                {
                    debugOn = true;
                    debugPressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F3) && !debugPressed && debugOn)
                {
                    debugOn = false;
                    debugPressed = true;
                }

                if (Keyboard.GetState().IsKeyUp(Keys.F3))
                {
                    debugPressed = false;
                }

                if (debugOn)
                {
                    debugSB.Begin();
                    debugSB.Draw(blackScreen, new Rectangle(0, 0, (int)monitorRes.X, 280), Color.White * .3f);
                    debugSB.DrawString(debugFont, Main.Version + " FPS: " + fps, new Vector2(0, 0), Color.White);
                    debugSB.DrawString(debugFont, "Is Player Above Tile: " + player.IsAboveTile, new Vector2(0, 20), Color.White);
                    debugSB.DrawString(debugFont, "Camera Position:" + camera.lastCameraLeftCorner.X + "," + camera.lastCameraLeftCorner.Y, new Vector2(0, 40), Color.White);
                    debugSB.DrawString(debugFont, "Editor Rectangle Position:" + gameWorld.levelEditor.editorRectangle.X + "," + gameWorld.levelEditor.editorRectangle.Y, new Vector2(0, 60), Color.White);
                    debugSB.DrawString(debugFont, "Total Draw Time:" + drawTime, new Vector2(0, 80), Color.White);
                    debugSB.DrawString(debugFont, "Times Updated: " + gameWorld.TimesUpdated, new Vector2(0, 100), Color.White);
                    debugSB.DrawString(debugFont, "Player Source Rectangle: " + player.sourceRectangle, new Vector2(0, 120), Color.White);
                    debugSB.DrawString(debugFont, "AnimationState:" + player.CurrentAnimation, new Vector2(0, 140), Color.White);
                    debugSB.DrawString(debugFont, "Level:" + CurrentGameMode, new Vector2(0, 160), Color.White);
                    debugSB.DrawString(debugFont, "Player Velocity" + player.velocity, new Vector2(0, 180), Color.White);
                    debugSB.DrawString(debugFont, "Tile Index Visible: " + gameWorld.visibleTileArray[0], new Vector2(0, 200), Color.White);
                    debugSB.DrawString(debugFont, "Dynamic Lights Count: " + gameWorld.lightEngine?.dynamicLights.Count, new Vector2(0, 220), Color.White);
                    debugSB.DrawString(debugFont, "Particle Count: " + gameWorld.particles?.Count, new Vector2(0, 240), Color.White);
                    debugSB.DrawString(debugFont, "Entity Count: " + gameWorld.entities?.Count, new Vector2(0, 260), Color.White);
                    debug.Draw(debugSB);
                    debugSB.End();
                }

            }

            drawCount++;
            drawWatch.Stop();
            drawTime = drawWatch.ElapsedMilliseconds;
            drawWatch.Reset();
        }
    }
}