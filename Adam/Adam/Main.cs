using System;
using System.Threading;
using System.Windows.Forms;
using Adam.GameData;
using Adam.Levels;
using Adam.Misc;
using Adam.Network;
using Adam.PlayerCharacter;
using Adam.UI;
using Adam.UI.Information;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MessageBox = Adam.UI.MessageBox;

namespace Adam
{
    public enum GameState
    {
        SplashScreen,
        Cutscene,
        MainMenu,
        LoadingScreen,
        GameWorld
    }

    public enum GameMode
    {
        None,
        Edit,
        Play,
    }

    public enum LevelType
    {
        Free,
        ChaseUp,
        ChaseRight,
    }

    public class Main : Game
    {
        private const bool InDebugMode = true;
        private const bool IsTestingMultiplayer = false;
        public const int Tilesize = 48;
        public const int DefaultResWidth = 960; // Default 960x540
        public const int DefaultResHeight = 540;
        public const string Version = "Version 0.10.0 Beta";
        public const string Producers = "BitBite Games";
        public const float Gravity = .90f;

        /// <summary>
        ///     The running instance of the game.
        /// </summary>
        private static Main _instance;

        // Color presets for lighting engine.
        private static Color _sunnyPreset = new Color(255, 238, 186);
        private static Color _hellPreset = new Color(255, 129, 116);
        private static Color _winterPreset = new Color(200, 243, 255);
        private static Color _nightPreset = new Color(120, 127, 183);
        private static Color _sunsetPreset = new Color(255, 155, 13);
        // Rendering variables.
        public static SpriteBatch SpriteBatch;
        public static bool IsLoadingContent;
        public static int UserResWidth;
        public static int UserResHeight;
        public static Texture2D DefaultTexture;
        public static Dialog Dialog;
        public static GameTime GameTime;
        public static double WidthRatio;
        public static double HeightRatio;
        public static float MaxVolume = .1f;
        public static bool IsMusicMuted = false;
        public static bool HasLighting = false;
        public static ObjectiveTracker ObjectiveTracker;
        public static ContentManager Content;
        public static GraphicsDevice GraphicsDeviceInstance;
        public static DataFolder DataFolder = new DataFolder();
        public static LevelProgression LevelProgression = new LevelProgression();
        private readonly GraphicsDeviceManager _graphics;
        private Texture2D _blackScreen;
        public static Camera Camera;
        private Cutscene _cutscene;
        private GameDebug _debug;
        private SpriteFont _debugFont;
        private GameState _desiredGameState;
        private int _fps, _totalFrames;
        private double _frameRateTimer;
        //Game Variables
        private bool _hasQuacked;
        private BlendState _lightBlendState;
        private RenderTarget2D _lightingRenderTarget;
        private LoadingScreen _loadingScreen;
        private RenderTarget2D _mainRenderTarget;
        private Menu _menu;
        private Overlay _overlay;
        private SoundEffect _quack;
        private Thread _reloadThread;
        private Session _session;
        private Texture2D _splashDkd;
        public GameMode CurrentGameMode;
        //Defines the initial GameState ----- Use this variable to change the GameState
        public GameState CurrentGameState;
        public SamplerState DesiredSamplerState;
        public GameDataManager GameData;
        public bool WasPressed, DebugOn, DebugPressed;

        public delegate void UpdateHandler(GameTime gameTime);
        public static event UpdateHandler GameUpdateCalled;

        public Main()
        {
            _instance = this;

            // Get the current monitor resolution and set it as the game's resolution
            var monitorRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

#pragma warning disable 0162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (IsTestingMultiplayer)
            {
                UserResWidth = 960;
                UserResHeight = 540;
            }
            else
            {
                UserResWidth = (int)monitorRes.X;
                UserResHeight = (int)monitorRes.Y;
            }
#pragma warning restore 0162

            WidthRatio = (DefaultResWidth / (double)UserResWidth);
            HeightRatio = (DefaultResHeight / (double)UserResHeight);

            // Important services that need to be instanstiated before other things.
            _graphics = new GraphicsDeviceManager(this);
            Content = new ContentManager(Services, "Content");
            GameData = new GameDataManager();

            // Sets the current screen resolution to the user resolution.
            _graphics.PreferredBackBufferWidth = UserResWidth;
            _graphics.PreferredBackBufferHeight = UserResHeight;

            // Change game settings here.
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.PreferMultiSampling = true;
            IsFixedTimeStep = true;
            _graphics.IsFullScreen = GameData.Settings.IsFullscreen;

            // Set window to borderless.
            var hWnd = Window.Handle;
            var control = Control.FromHandle(hWnd);
            var form = control.FindForm();
#pragma warning disable
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (IsTestingMultiplayer)
            {
                form.WindowState = FormWindowState.Normal;
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
            }
#pragma warning restore


            //MediaPlayer Settings
            MediaPlayer.Volume = MaxVolume;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        public static Main Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("The instance of Gameworld has not yet been created.");
                return _instance;
            }
        }

        public static Session Session { get; set; }

        /// <summary>
        ///     Used to display messages to the user where he needs to press OK to continue.
        /// </summary>
        public static MessageBox MessageBox { get; set; }

        public static TextInputBox TextInputBox { get; set; }
        public static TimeSpan DefaultTimeLapse { get; set; }
        public static TimeFreeze TimeFreeze { get; set; } = new TimeFreeze();

        protected override void Initialize()
        {
            GameWorld.Initialize();
            Camera = new Camera(GraphicsDevice.Viewport);
            _menu = new Menu(this);
            _overlay = new Overlay();
            _cutscene = new Cutscene();
            Dialog = new Dialog();
            ObjectiveTracker = new ObjectiveTracker();
            MessageBox = new MessageBox();
            TextInputBox = new TextInputBox();

            DefaultTexture = ContentHelper.LoadTexture("Tiles/temp tile");
            GraphicsDeviceInstance = _graphics.GraphicsDevice;

            //Initialize the game render target
            _mainRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _lightingRenderTarget = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);

            SpriteBatch = new SpriteBatch(GraphicsDevice);


            _lightBlendState = new BlendState
            {
                AlphaSourceBlend = Blend.DestinationColor,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            CurrentGameState = GameState.SplashScreen;
            _loadingScreen = new LoadingScreen(new Vector2(UserResWidth, UserResHeight), Content);
            _splashDkd = ContentHelper.LoadTexture("Backgrounds/Splash/DKD_new");
            _quack = Content.Load<SoundEffect>("Backgrounds/Splash/quack");
            _blackScreen = ContentHelper.LoadTexture("Tiles/black");

            _debugFont = Content.Load<SpriteFont>("debug");
            _cutscene.Load(Content);

            _debug = new GameDebug(_debugFont, new Vector2(UserResWidth, UserResHeight), _blackScreen);

            CurrentGameMode = GameMode.None;

            DefaultTimeLapse = TargetElapsedTime;
        }

        public void ChangeState(GameState desiredGameState, GameMode mode)
        {
            CurrentGameState = GameState.LoadingScreen;
            _desiredGameState = desiredGameState;
            IsLoadingContent = true;

            if (desiredGameState == GameState.GameWorld)
            {
                LoadWorldFromFile(mode);
            }
            else
            {
                IsLoadingContent = false;
            }
        }

        public void LoadWorldFromFile(GameMode mode)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentGameMode = mode;
            _desiredGameState = GameState.GameWorld;
            IsLoadingContent = true;

            _reloadThread = new Thread(BackgroundThread_FileLoad);
            _reloadThread.IsBackground = true;
            _reloadThread.Start();
        }

        private void BackgroundThread_FileLoad()
        {
            IsLoadingContent = true;
            if (GameWorld.TryLoadFromFile(CurrentGameMode))
            {
                ObjectiveTracker = GameData.CurrentSave.ObjTracker;
            }
            else
            {
                CurrentGameMode = GameMode.None;
                CurrentGameState = GameState.MainMenu;
            }

            IsLoadingContent = false;
            WasPressed = false;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            GameUpdateCalled?.Invoke(gameTime);

            GameTime = gameTime;

            if (TimeFreeze.IsTimeFrozen())
            {
                return;
            }

            if (InputHelper.IsKeyDown(Keys.P))
            {
                TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1000 / 10);
            }
            else
            {
                TargetElapsedTime = DefaultTimeLapse;
            }

            _frameRateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_frameRateTimer > 1000f)
            {
                _fps = _totalFrames;
                _totalFrames = 0;
                _frameRateTimer = 0;
            }

            #region Mouse Settings

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    IsMouseVisible = true;
                    break;
                case GameState.GameWorld:
                    IsMouseVisible = true;
                    break;
            }

            #endregion

            if (MessageBox.IsActive)
            {
                MessageBox.Update();
                return;
            }
            if (TextInputBox.IsActive)
            {
                TextInputBox.Update();
                return;
            }

            if (GameData.Settings.HasChanged)
            {
                GameData.SaveSettings();
                if (GameData.Settings.NeedsRestart)
                {
                    _graphics.IsFullScreen = GameData.Settings.IsFullscreen;
                    _graphics.ApplyChanges();
                    GameData.Settings.NeedsRestart = false;
                }
                GameData.Settings.HasChanged = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && CurrentGameState != GameState.MainMenu &&
                CurrentGameState != GameState.LoadingScreen)
            {
                if (CurrentGameState == GameState.GameWorld && GameWorld.DebuggingMode)
                {
                    ChangeState(GameState.GameWorld, GameMode.Edit);
                    GameWorld.DebuggingMode = false;
                }
                else
                {
                    GameData.SaveGame();
                    Menu.CurrentMenuState = Menu.MenuState.Main;
                    ChangeState(GameState.MainMenu, GameMode.None);
                }
            }

            //Update the game based on what GameState it is
            switch (CurrentGameState)
            {
                case GameState.SplashScreen:
                    CurrentGameState = GameState.Cutscene;
                    break;
                case GameState.Cutscene:
                    if (InDebugMode)
                    {
                        CurrentGameState = GameState.MainMenu;
                        _cutscene.Stop();
                    }
                    if (_cutscene.WasPlayed == false)
                    {
                        _cutscene.Update(CurrentGameState);
                    }
                    else CurrentGameState = GameState.MainMenu;
                    if (InputHelper.IsAnyInputPressed())
                    {
                        CurrentGameState = GameState.MainMenu;
                        _cutscene.Stop();
                    }
                    break;
                case GameState.MainMenu:
                    _menu.Update(this, gameTime, GameData.Settings);
                    break;
                case GameState.LoadingScreen:
                    _loadingScreen.Update();

                    if (!IsLoadingContent)
                    {
                        CurrentGameState = _desiredGameState;
                        _overlay.FadeIn();
                    }
                    break;
                case GameState.GameWorld:
                    if (IsLoadingContent) return;
                    if (GameWorld.IsOnDebug)
                        break;

                    GameWorld.Update(gameTime, CurrentGameMode, Camera);
                    _overlay.Update();
                    Dialog.Update();
                    ObjectiveTracker.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
            _debug.Update(this, DebugOn);
        }

        protected void PrepareRenderTargets()
        {
            //Does all rendertarget work
            switch (CurrentGameState)
            {
                case GameState.GameWorld:
                    DrawToMainRenderTarget(_mainRenderTarget);

                    if (HasLighting)
                    {
                        DrawLightingRenderTarget(_lightingRenderTarget);
                    }
                    break;
                case GameState.Cutscene:
                    DrawToMainRenderTarget(_mainRenderTarget);
                    break;
                case GameState.MainMenu:
                    DrawToMainRenderTarget(_mainRenderTarget);
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
                case GameState.Cutscene:
                    SpriteBatch.Begin();
                    _cutscene.Draw(SpriteBatch);
                    SpriteBatch.End();
                    break;
                case GameState.GameWorld:
                    if (IsLoadingContent)
                        break;
                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    GameWorld.DrawBackground(SpriteBatch);
                    GameWorld.DrawClouds(SpriteBatch);
                    SpriteBatch.End();

                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                        null, null, Camera.Translate);
                    GameWorld.DrawInBack(SpriteBatch);
                    GameWorld.Draw(SpriteBatch);
                    GameWorld.DrawParticles(SpriteBatch);

                    SpriteBatch.End();


                    //SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                    //    null, null);
                    //_overlay.Draw(SpriteBatch);

                    //if (!GameWorld.LevelEditor.OnInventory)
                    //    ObjectiveTracker.Draw(SpriteBatch);

                    //GameWorld.DrawUi(SpriteBatch);
                    //Dialog.Draw(SpriteBatch);
                    //TextInputBox.Draw(SpriteBatch);
                    //MessageBox.Draw(SpriteBatch);

                    //SpriteBatch.End();

                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null,
                        null, Camera.Translate);
                    GameWorld.DrawGlows(SpriteBatch);
                    SpriteBatch.End();
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
                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null,
                        null);
                    SpriteBatch.Draw(ContentHelper.LoadTexture("Tiles/max_shadow"),
                        new Rectangle(0, 0, DefaultResWidth, DefaultResHeight), Color.White);
                    SpriteBatch.End();

                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null,
                        null, Camera.Translate);
                    GameWorld.DrawLights(SpriteBatch);
                    SpriteBatch.End();
                    break;
            }

            //Return the current RenderTarget to the default
            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            _totalFrames++;
            PrepareRenderTargets();

            //Set background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw what is needed based on GameState
            switch (CurrentGameState)
            {
                case GameState.SplashScreen:
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(_splashDkd, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    SpriteBatch.End();
                    break;
                case GameState.Cutscene:
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(_mainRenderTarget, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    SpriteBatch.End();
                    break;
                case GameState.LoadingScreen:
                    SpriteBatch.Begin();
                    _loadingScreen.Draw(SpriteBatch);
                    TextInputBox.Draw(SpriteBatch);
                    MessageBox.Draw(SpriteBatch);
                    SpriteBatch.End();
                    break;
                case GameState.MainMenu:
                    var rs2 = new RasterizerState { ScissorTestEnable = true };
                    SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, rs2);
                    _menu.Draw(SpriteBatch);
                    TextInputBox.Draw(SpriteBatch);
                    MessageBox.Draw(SpriteBatch);
                    SpriteBatch.End();
                    break;
                case GameState.GameWorld:

                    Overlay.CornerColor = Color.Black;
                    if (TimeFreeze.IsTimeFrozen())
                    {
                        if (GameWorld.GetPlayer().IsTakingDamage)
                            Overlay.CornerColor = Color.Red;
                        else Overlay.CornerColor = Color.White;
                    }

                    //Draw the rendertarget
                    SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, RasterizerState.CullNone);
                    SpriteBatch.Draw(_mainRenderTarget, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    SpriteBatch.End();

                    if (HasLighting)
                    {
                        SpriteBatch.Begin(SpriteSortMode.Immediate, _lightBlendState,
                            GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                        SpriteBatch.Draw(_lightingRenderTarget, new Rectangle(0, 0, UserResWidth, UserResHeight),
                            Color.White);
                        SpriteBatch.End();
                    }

                    var rs = new RasterizerState { ScissorTestEnable = true };
                    SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, rs);
                   // _overlay.Draw(SpriteBatch);

                    //if (!GameWorld.LevelEditor.OnInventory)
                    //    ObjectiveTracker.Draw(SpriteBatch);

                    GameWorld.DrawUi(SpriteBatch);
                    Dialog.Draw(SpriteBatch);
                    TextInputBox.Draw(SpriteBatch);
                    MessageBox.Draw(SpriteBatch);

                    SpriteBatch.End();

                    break;
            }
            base.Draw(gameTime);

            if (CurrentGameState != GameState.SplashScreen)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.F3) && !DebugPressed && !DebugOn)
                {
                    DebugOn = true;
                    DebugPressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F3) && !DebugPressed && DebugOn)
                {
                    DebugOn = false;
                    DebugPressed = true;
                }

                if (Keyboard.GetState().IsKeyUp(Keys.F3))
                {
                    DebugPressed = false;
                }

                if (DebugOn)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(_blackScreen, new Rectangle(0, 0, UserResWidth, 360), Color.White * .3f);
                    SpriteBatch.DrawString(_debugFont, Version + " FPS: " + _fps, new Vector2(0, 0), Color.White);
                    SpriteBatch.DrawString(_debugFont, "", new Vector2(0, 20), Color.White);
                    SpriteBatch.DrawString(_debugFont,
                        "Camera Position:" + Camera.InvertedCoords.X + "," + Camera.InvertedCoords.Y,
                        new Vector2(0, 40), Color.White);
                    SpriteBatch.DrawString(_debugFont,
                        "Editor Rectangle Position:" + LevelEditor.EditorRectangle.X + "," +
                        LevelEditor.EditorRectangle.Y, new Vector2(0, 60), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Camera Zoom:" + Camera.GetZoom(), new Vector2(0, 80),
                        Color.White);
                    SpriteBatch.DrawString(_debugFont, "Times Updated: " + GameWorld.TimesUpdated, new Vector2(0, 100),
                        Color.White);
                    SpriteBatch.DrawString(_debugFont, "Player is dead: " + GameWorld.GetPlayer().IsDead, new Vector2(0, 120),
                        Color.White);
                    //SpriteBatch.DrawString(debugFont, "AnimationState:" + player.CurrentAnimation, new Vector2(0, 140), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Level:" + CurrentGameMode, new Vector2(0, 160), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Player Velocity" + GameWorld.GetPlayer().GetVelocity(), new Vector2(0, 180),
                        Color.White);
                    SpriteBatch.DrawString(_debugFont,
                        "Total Chunks: " + GameWorld.ChunkManager.GetNumberOfChunks() + " Active Chunk: " +
                        GameWorld.ChunkManager.GetActiveChunkIndex(), new Vector2(0, 200), Color.White);
                    SpriteBatch.DrawString(_debugFont,
                        "Dynamic Lights Count: " + GameWorld.LightEngine?.DynamicLights.Count, new Vector2(0, 220),
                        Color.White);
                    SpriteBatch.DrawString(_debugFont, "Particle Index: " + GameWorld.ParticleSystem.GetCurrentParticleIndex(),
                        new Vector2(0, 240), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Entity Count: " + GameWorld.Entities?.Count,
                        new Vector2(0, 260), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Visible Tiles: " + GameWorld.VisibleTileArray?.Length,
                        new Vector2(0, 280), Color.White);
                    SpriteBatch.DrawString(_debugFont, "", new Vector2(0, 300), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Is TextInputBox Active: " + TextInputBox.IsActive,
                        new Vector2(0, 320), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Is MessageBox Active: " + MessageBox.IsActive,
                        new Vector2(0, 340), Color.White);
                    SpriteBatch.DrawString(_debugFont, "Timers called: " + Adam.Misc.Timer.ActiveTimers,
                        new Vector2(0, 360), Color.White);

                    _debug.Draw(SpriteBatch);
                    SpriteBatch.End();
                }
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Session.IsActive = false;
            base.OnExiting(sender, args);
        }
    }
}