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
        GameOver,
        LevelGen,
        Multiplayer,
    }
    public enum Level
    {
        Level0,
        Level1and1, Level1and2, Level1and3,
        Level2and1,
        Level3and1,
        Level4and1,
        Level8and1,
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        Color sunny = new Color(255, 238, 186);
        Color hell = new Color(255, 129, 116);
        Color winter = new Color(200, 243, 255);
        Color night = new Color(120, 127, 183);
        Color sunset = new Color(255, 155, 13);

        GraphicsDeviceManager graphics;
        SpriteBatch gameSB, debugSB, mainSB, UiSB, backgroundSB, lightingSB, mainLightSB;
        SpriteFont debugFont, UIFont;
        Camera camera;
        public Vector2 monitorRes;
        RenderTarget2D mainRenderTarget, lightingRenderTarget;
        Menu menu;
        Thread reloadThread;
        Thread backgroundUpdateThread;
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

        #region Constants
        /// <summary>
        /// The default tilesize.
        /// </summary>
        public const int Tilesize = 32;
        /// <summary>
        /// The resolution width the game is scaled to.
        /// </summary>
        public const int DefaultResWidth = 960;
        /// <summary>
        /// The resolution height the game is scale to.
        /// </summary>
        public const int DefaultResHeight = 540;
        /// <summary>
        /// Current version of the game.
        /// </summary>
        public const string Version = "Version 0.5.3";
        /// <summary>
        /// Producers of the game.
        /// </summary>
        public const string Producers = "Duck Knight Duel Games";
        /// <summary>
        /// The current monitor resolution of the user.
        /// </summary>
        public static int UserResWidth;
        /// <summary>
        /// The current monitor resolution height of the user.
        /// </summary>
        public static int UserResHeight;
        /// <summary>
        /// The default texture.
        /// </summary>
        public static Texture2D DefaultTexture;
        /// <summary>
        /// Default gravity.
        /// </summary>
        public const float Gravity = .5f;

        #endregion

        public bool wasPressed, debugOn, debugPressed;

        public SamplerState desiredSamplerState;

        //Defines the initial GameState ----- Use this variable to change the GameState
        public GameState CurrentGameState;
        GameState desiredGameState;
        public Level CurrentLevel;

        //Game Variables
        GameWorld gameWorld;
        Session session;
        public static ObjectiveTracker ObjectiveTracker;
        public GameDataManager GameData;
        Player player;
        LoadingScreen loadingScreen;
        public static ContentManager Content;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Get the current monitor resolution and set it as the game's resolution
            monitorRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            UserResWidth = (int)monitorRes.X;
            UserResHeight = (int)monitorRes.Y;
            graphics.PreferredBackBufferWidth = UserResWidth;
            graphics.PreferredBackBufferHeight = UserResHeight;

            //Change Game Settings Here
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = true;
            IsFixedTimeStep = true;

            Content = new ContentManager(Services, "Content");

            GameData = new GameDataManager();
            graphics.IsFullScreen = GameData.Settings.IsFullscreen;



            //MediaPlayer Settings
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.4f;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;


            //Creates ContentManager to be used by other classes



        }


        protected override void Initialize()
        {
            DefaultTexture = ContentHelper.LoadTexture("Tiles/temp");
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

            CurrentLevel = Level.Level0;

        }

        public void ChangeState(GameState desiredGameState, Level desiredLevel)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentLevel = desiredLevel;
            this.desiredGameState = desiredGameState;
            hasLoadedContent = false;
            loadingScreen.Restart();

            reloadThread = new Thread(new ThreadStart(BackgroundMapLoad));
            reloadThread.IsBackground = true;
            reloadThread.Start();

        }

        protected void BackgroundMapLoad()
        {
            loadWatch.Reset();
            loadWatch.Start();
            hasLoadedContent = false;
            gameWorld.Load(Content, monitorRes, player, CurrentLevel);
            ObjectiveTracker = GameData.CurrentSave.ObjTracker;
            hasLoadedContent = true;
            wasPressed = false;
            loadWatch.Stop();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
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
                GameData.SaveGame();
                ChangeState(GameState.MainMenu, Level.Level0);

            }

            //if (InputHelper.IsKeyDown(Keys.I))
            //{
            //    Objective obj = new Objective();
            //    obj.Create("Test Objective", 0);
            //    ObjectiveTracker.AddObjective(obj);
            //}
            //if (InputHelper.IsKeyDown(Keys.U))
            //{
            //    ObjectiveTracker.CompleteObjective(0);
            //}

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
                    if (gameWorld.isOnDebug)
                        break;

                    player.Update(gameTime);
                    overlay.Update(gameTime, player, gameWorld);

                    //if (gameWorld.SimulationPaused)
                    //    break;

                    gameWorld.Update(gameTime, CurrentLevel, camera);
                    camera.UpdateSmoothly(player, gameWorld);
                    Dialog.Update(gameTime);
                    ObjectiveTracker.Update(gameTime);

                    if (player.returnToMainMenu)
                        ChangeState(GameState.MainMenu, Level.Level0);
                    break;
                case GameState.Multiplayer:

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
                case GameState.LevelGen:
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
                    backgroundSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    menu.Draw(backgroundSB);
                    backgroundSB.End();
                    break;
                case GameState.GameWorld:
                    backgroundSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    gameWorld.DrawBackground(backgroundSB);
                    gameWorld.DrawClouds(backgroundSB);
                    backgroundSB.End();

                    gameSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Translate);
                    gameWorld.DrawInBack(gameSB);
                    gameSB.End();

                    gameSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Translate);
                    gameWorld.Draw(gameSB);
                    player.Draw(gameSB);
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
                    lightingSB.Draw(ContentHelper.LoadTexture("Tiles/max_shadow"), new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight), Color.White);
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
                    //Draw the MaxRenderTarget
                    backgroundSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    backgroundSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    backgroundSB.End();
                    break;
                case GameState.GameWorld:
                    //Draw the rendertarget
                    mainSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    mainSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    mainSB.End();

                    if (GameData.Settings.DesiredLight)
                    {

                        BlendState WHATTHEFUCK = new BlendState();
                        WHATTHEFUCK.AlphaSourceBlend = Blend.DestinationColor;
                        WHATTHEFUCK.ColorSourceBlend = Blend.DestinationColor;
                        WHATTHEFUCK.ColorDestinationBlend = Blend.Zero;
                        mainLightSB.Begin(SpriteSortMode.Immediate, WHATTHEFUCK, GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                        mainLightSB.Draw(lightingRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), sunny);
                        mainLightSB.End();
                    }

                    UiSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    overlay.Draw(UiSB);
                    gameWorld.DrawUI(UiSB);
                    Dialog.Draw(UiSB);
                    ObjectiveTracker.Draw(UiSB);
                    UiSB.End();

                    break;
                case GameState.LevelGen:
                    mainSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    mainSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    mainSB.End();

                    UiSB.Begin();
                    overlay.Draw(UiSB);
                    gameWorld.DrawUI(UiSB);
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
                    debugSB.DrawString(debugFont, "Frames Per Second:" + fps, new Vector2(0, 0), Color.White);
                    debugSB.DrawString(debugFont, "Tile Below Player: " + player.IsAboveTile, new Vector2(0, 20), Color.White);
                    debugSB.DrawString(debugFont, "Player Position:" + player.position.X + "," + player.position.Y, new Vector2(0, 40), Color.White);
                    debugSB.DrawString(debugFont, "Player Rectangle Position:" + player.collRectangle.X + "," + player.collRectangle.Y, new Vector2(0, 60), Color.White);
                    debugSB.DrawString(debugFont, "Total Draw Time:" + drawTime, new Vector2(0, 80), Color.White);
                    debugSB.DrawString(debugFont, "Times Updated: " + gameWorld.TimesUpdated, new Vector2(0, 100), Color.White);
                    debugSB.DrawString(debugFont, "Times Background Updated: " + gameWorld.TimesBackgroundUpdated, new Vector2(0, 120), Color.White);
                    debugSB.DrawString(debugFont, "AnimationState:" + player.CurrentAnimation, new Vector2(0, 140), Color.White);
                    debugSB.DrawString(debugFont, "Level:" + CurrentLevel, new Vector2(0, 160), Color.White);
                    debugSB.DrawString(debugFont, "Player Velocity" + player.velocity, new Vector2(0, 180), Color.White);
                    debugSB.DrawString(debugFont, "Load time: " + loadWatch.ElapsedMilliseconds, new Vector2(0, 200), Color.White);
                    debugSB.DrawString(debugFont, "Tile Index Camera:" + camera.tileIndex, new Vector2(0, 220), Color.White);
                    debugSB.DrawString(debugFont, "Particle Count: " + gameWorld.particles.Count, new Vector2(0, 240), Color.White);
                    debugSB.DrawString(debugFont, "Entity Count: " + gameWorld.entities.Count, new Vector2(0, 260), Color.White);
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