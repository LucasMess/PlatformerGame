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

namespace Adam
{
    public enum GameState
    {
        SplashScreen,
        Cutscene,
        MainMenu,
        LoadingScreen,
        Level,
        GameOver,
        LevelGen,
        Multiplayer,
    }
    public enum Level : byte
    { 
        Level0 = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        GraphicsDeviceManager graphics;
        SpriteBatch gameSB, debugSB, mainSB, UiSB, backgroundSB, lightingSB, mainLightSB;
        SpriteFont debugFont, UIFont;
        Camera camera;
        public Vector2 monitorRes;
        RenderTarget2D mainRenderTarget, maxRenderTarget, lightingRenderTarget, interfaceRenderTarget;
        Menu menu;
        Thread reloadThread;
        Thread backgroundUpdateThread;
        Overlay overlay;
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
        public static int PrefferedResWidth;
        /// <summary>
        /// The current monitor resolution height of the user.
        /// </summary>
        public static int PrefferedResHeight;
        /// <summary>
        /// The default texture.
        /// </summary>
        public static Texture2D DefaultTexture;
        /// <summary>
        /// Default gravity.
        /// </summary>
        public const float Gravity = .8f;

#endregion

        public bool wasPressed, debugOn, debugPressed;

        public SamplerState desiredSamplerState;

        //Defines the initial GameState ----- Use this variable to change the GameState
        public GameState CurrentGameState;
        GameState desiredGameState;
        public Level CurrentLevel;

        //Game Variables
        Map map;
        Session session;
        GameDataManager gameData;
        Player player;
        LoadingScreen loadingScreen;
        public static ContentManager Content;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Get the current monitor resolution and set it as the game's resolution
            monitorRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            PrefferedResWidth = (int)monitorRes.X;
            PrefferedResHeight = (int)monitorRes.Y;
            graphics.PreferredBackBufferWidth = PrefferedResWidth;
            graphics.PreferredBackBufferHeight = PrefferedResHeight;

            //Change Game Settings Here
            //Sync with vertical retrace needs to be on until I fix the jittery jump bug
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = true;
            IsFixedTimeStep = true;

            gameData = new GameDataManager();

            graphics.IsFullScreen = gameData.Settings.IsFullscreen;
                      

            //MediaPlayer Settings
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.4f;

            //Creates ContentManager to be used by other classes
            Content = new ContentManager(Services, "Content");


        }


        protected override void Initialize()
        {            
            DefaultTexture = ContentHelper.LoadTexture("Tiles/temp");
            //Initialize all instances
            camera = new Camera(GraphicsDevice.Viewport, monitorRes, new Vector2(DefaultResWidth, DefaultResHeight));
            menu = new Menu(monitorRes);
            map = new Map(GraphicsDevice, monitorRes);
            player = new Player();
            overlay = new Overlay();
            cutscene = new Cutscene();

            //Initialize the game render target
            mainRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            maxRenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
            lightingRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            interfaceRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight,
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
            splashDKD = Content.Load<Texture2D>("Backgrounds/Splash/DKD_white_1080");
            quack = Content.Load<SoundEffect>("Backgrounds/Splash/quack");
            blackScreen = Content.Load<Texture2D>("Tiles/black");

            debugFont = Content.Load<SpriteFont>("debug");
            menu.Load(Content);
            overlay.Load();
            cutscene.Load(Content);

            debug = new GameDebug(debugFont, monitorRes, blackScreen);

            CurrentLevel = Level.Level0;
            //loadingScreen = new LoadingScreen(monitorResolution, Content);
            //loadThread = new Thread(new ThreadStart(FirstLoad));
            //loadThread.IsBackground = true;
            //loadThread.Start();

        }

        public void FirstLoad()
        {
            hasLoadedContent = true;
        }


        public void ReloadMap(GameState desiredGameState, Level desiredLevel)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentLevel = desiredLevel;
            this.desiredGameState = desiredGameState;
            hasLoadedContent = false;
            loadingScreen.Restart();

            map = new Map(GraphicsDevice, monitorRes);
            player = new Player();

            reloadThread = new Thread(new ThreadStart(BackgroundMapLoad));
            reloadThread.IsBackground = true;
            reloadThread.Start();

        }

        protected void BackgroundMapLoad()
        {
            loadWatch.Reset();
            loadWatch.Start();
            hasLoadedContent = false;
            map.Load(Content, monitorRes, player, CurrentLevel);
            player.Load();
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
                case GameState.Level:
                    this.IsMouseVisible = true;
                    break;
            }
            #endregion

            if (gameData.Settings.HasChanged)
            {
                gameData.SaveSettings();
                if (gameData.Settings.NeedsRestart)
                {
                    graphics.IsFullScreen = gameData.Settings.IsFullscreen;
                    graphics.ApplyChanges();
                    gameData.Settings.NeedsRestart = false;
                }
                gameData.Settings.HasChanged = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && CurrentGameState != GameState.MainMenu && CurrentGameState != GameState.LoadingScreen)
                ReloadMap(GameState.MainMenu, Level.Level0);

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
                    menu.Update(this, gameTime, gameData.Settings);
                    break;
                case GameState.LoadingScreen:
                    loadingScreen.Update(gameTime);
                    if (hasLoadedContent && loadingScreen.isReady)
                        CurrentGameState = desiredGameState;
                    break;
                case GameState.Level:
                    if (map.isPaused)
                        break;

                    map.Update(gameTime, CurrentLevel, camera);              
                    player.Update(gameTime, map);
                    overlay.Update(gameTime, player, map);                 
                    camera.UpdateSmoothly(player, map);
                    //camera.UpdateWithZoom(player.position);

                    if (player.returnToMainMenu)
                        ReloadMap(GameState.MainMenu, Level.Level0);
                    break;
                case GameState.Multiplayer:

                    break;               
            }

            base.Update(gameTime);
            debug.Update(this, player, map, debugOn);

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
                case GameState.Level:
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
                    DrawToMaxRenderTarget(maxRenderTarget);
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
                    break;
                case GameState.Level:
                    backgroundSB.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.PointClamp, DepthStencilState.Default,RasterizerState.CullCounterClockwise);
                    map.DrawBackground(backgroundSB);
                    map.DrawClouds(backgroundSB);
                    backgroundSB.End();

                    gameSB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Translate);
                    map.DrawInBack(gameSB);
                    map.Draw(gameSB);
                    player.Draw(gameSB);
                    gameSB.End();


                    UiSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    map.DrawAfterEffects(UiSB);
                    UiSB.End();

                    break;
            }

            //Return the current RenderTarget to the default
            GraphicsDevice.SetRenderTarget(null);
        }

        protected void DrawToMaxRenderTarget(RenderTarget2D renderTarget)
        {
            //Change RenderTarget to this from the default
            GraphicsDevice.SetRenderTarget(renderTarget);
            //Set up the background color
            GraphicsDevice.Clear(Color.Peru);

            backgroundSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, null);
            menu.DrawBackground(backgroundSB);
            backgroundSB.End();

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
                case GameState.Level:
                    lightingSB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null);
                    lightingSB.Draw(ContentHelper.LoadTexture("Tiles/max_shadow"), new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight), Color.White);
                    lightingSB.End();

                    lightingSB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null, camera.Translate);
                    map.DrawLights(lightingSB);
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
                    backgroundSB.Begin();
                    backgroundSB.Draw(maxRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    backgroundSB.End();

                    mainSB.Begin();
                    menu.Draw(mainSB);
                    mainSB.End();
                    break;
                case GameState.Level:
                    //Draw the rendertarget
                    mainSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, gameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    mainSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    mainSB.End();

                    if (gameData.Settings.DesiredLight)
                    {
                        Color sunny = new Color(255,238,186);
                        Color hell = new Color(255, 129, 116);
                        BlendState WHATTHEFUCK = new BlendState();
                        WHATTHEFUCK.AlphaSourceBlend = Blend.DestinationColor;
                        WHATTHEFUCK.ColorSourceBlend = Blend.DestinationColor;
                        WHATTHEFUCK.ColorDestinationBlend = Blend.Zero;
                        mainLightSB.Begin(SpriteSortMode.Immediate, WHATTHEFUCK, gameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                        mainLightSB.Draw(lightingRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), sunny);
                        mainLightSB.End();
                    }

                    UiSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    overlay.Draw(UiSB);
                    map.DrawUI(UiSB);
                    UiSB.End();

                    break;
                case GameState.LevelGen:
                    mainSB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, gameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    mainSB.Draw(mainRenderTarget, new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y), Color.White);
                    mainSB.End();

                    UiSB.Begin();
                    overlay.Draw(UiSB);
                    map.DrawUI(UiSB);
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
                    debugSB.Draw(blackScreen, new Rectangle(0, 0, (int)monitorRes.X, 240), Color.White * .3f);
                    debugSB.DrawString(debugFont, "Frames Per Second:" + fps, new Vector2(0, 0), Color.White);
                    debugSB.DrawString(debugFont, "Tile Below Player: " + player.IsAboveTile, new Vector2(0, 20), Color.White);
                    debugSB.DrawString(debugFont, "Player Position:" + player.position.X + "," + player.position.Y, new Vector2(0, 40), Color.White);
                    debugSB.DrawString(debugFont, "Player Rectangle Position:" + player.collRectangle.X +","+player.collRectangle.Y, new Vector2(0, 60), Color.White);
                    debugSB.DrawString(debugFont, "Total Draw Time:" + drawTime, new Vector2(0, 80), Color.White);
                    debugSB.DrawString(debugFont, "Game World Render Time:" + renderTime, new Vector2(0, 100), Color.White);
                    debugSB.DrawString(debugFont, "Light Render Time:" + lightTime, new Vector2(0, 120), Color.White);
                    debugSB.DrawString(debugFont, "AnimationState:" + player.CurrentAnimation, new Vector2(0, 140), Color.White);
                    debugSB.DrawString(debugFont, "Level:" + CurrentLevel, new Vector2(0, 160), Color.White);
                    debugSB.DrawString(debugFont, "Player Velocity" + player.velocity, new Vector2(0, 180), Color.White);
                    debugSB.DrawString(debugFont, "Load time: " +loadWatch.ElapsedMilliseconds, new Vector2(0, 200), Color.White);
                    debugSB.DrawString(debugFont, "Tile Index Camera:" + camera.tileIndex, new Vector2(0, 220), Color.White);
                    debugSB.DrawString(debugFont, "Number of items in effects: " + player.particles.Count ,new Vector2(0, 240), Color.White);
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
