using Adam.GameData;
using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Network;
using Adam.UI;
using Adam.UI.Information;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Threading;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MessageBox = Adam.UI.MessageBox;
using Timer = Adam.Misc.Timer;

namespace Adam
{
    public enum GameState
    {
        MainMenu,
        LoadingScreen,
        GameWorld
    }

    public enum GameMode
    {
        None,
        Edit,
        Play
    }

    public class Main : Game
    {
        public delegate void UpdateHandler(GameTime gameTime);

        private const bool InDebugMode = true;
        private const bool IsTestingMultiplayer = false;
        public const int Tilesize = 32;
        public const int DefaultResWidth = 960; // Default 960x540
        public const int DefaultResHeight = 540;
        public const string Version = "Version 0.10.0 Beta";
        public const string Producers = "BitBite Games";
        public const float Gravity = 3400;
        // Color presets for lighting engine.
        private static Color _sunnyPreset = new Color(255, 238, 186);
        private static Color _hellPreset = new Color(255, 129, 116);
        private static Color _winterPreset = new Color(200, 243, 255);
        private static Color _nightPreset = new Color(120, 127, 183);
        private static Color _sunsetPreset = new Color(255, 155, 13);
        // Rendering variables.
        private static SpriteBatch _spriteBatch;
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
        public static readonly Random Random = new Random();
        public static ContentManager Content;
        public static GraphicsDevice GraphicsDeviceInstance;
        public static DataFolder DataFolder = new DataFolder();
        public static LevelProgression LevelProgression = new LevelProgression();
        public static Camera Camera;
        private static GameState _desiredGameState;
        private static Thread _reloadThread;
        public static GameMode CurrentGameMode;
        //Defines the initial GameState ----- Use this variable to change the GameState
        public static GameState CurrentGameState;
        public static GameDataManager GameData;
        public static bool WasPressed, DebugOn, DebugPressed;
        private readonly GraphicsDeviceManager _graphics;
        private Texture2D _blackScreen;
        private GameDebug _debug;
        private SpriteFont _debugFont;
        private int _fps, _totalFrames;
        private double _frameRateTimer;
        //Game Variables
        private BlendState _lightBlendState;
        private LoadingScreen _loadingScreen;
        private RenderTarget2D _frontRT;
        private RenderTarget2D _shadowRT;
        private RenderTarget2D _backRT;
        private RenderTarget2D _lightRT;
        public static float TimeSinceLastUpdate;
        private Menu _menu;
        private Session _session;
        public SamplerState DesiredSamplerState;
        public static event UpdateHandler GameUpdateCalled;

        public Main()
        {
            // Get the current monitor resolution and set it as the game's resolution
            var monitorRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

#pragma warning disable 0162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (IsTestingMultiplayer)
            {
                monitorRes = new Vector2(1366, 768);
            }
            UserResWidth = (int)monitorRes.X;
            UserResHeight = (int)monitorRes.Y;

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
            _graphics.PreferMultiSampling = false;
            IsFixedTimeStep = false;
            _graphics.IsFullScreen = true;
            if (IsTestingMultiplayer) _graphics.IsFullScreen = false;

            // Set window to borderless.
            //            var hWnd = Window.Handle;
            //            var control = Control.FromHandle(hWnd);
            //            var form = control.FindForm();
            //#pragma warning disable
            //            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            //            if (IsTestingMultiplayer)
            //            {
            //                form.WindowState = FormWindowState.Normal;
            //            }
            //            // ReSharper disable once PossibleNullReferenceException
            //            form.FormBorderStyle = FormBorderStyle.None;
            //            form.WindowState = FormWindowState.Maximized;
            //#pragma warning restore


            _graphics.ApplyChanges();

            //MediaPlayer Settings
            MediaPlayer.Volume = MaxVolume;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        public static Session Session { get; set; }

        /// <summary>
        ///     Used to display messages to the user where he needs to press OK to continue.
        /// </summary>
        public static MessageBox MessageBox { get; set; }

        public static TextInputBox TextInputBox { get; set; }
        public static TimeFreeze TimeFreeze { get; set; } = new TimeFreeze();

        protected override void Initialize()
        {
            Camera = new Camera(GraphicsDevice.Viewport);
            _menu = new Menu(this);
            Dialog = new Dialog();
            MessageBox = new MessageBox();
            TextInputBox = new TextInputBox();

            DefaultTexture = ContentHelper.LoadTexture("Tiles/black");
            GraphicsDeviceInstance = _graphics.GraphicsDevice;

            //Initialize the game render target
            _frontRT = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _backRT = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
               GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
               GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _shadowRT = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _lightRT = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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
            _loadingScreen = new LoadingScreen(new Vector2(UserResWidth, UserResHeight), Content);
            _blackScreen = ContentHelper.LoadTexture("Tiles/black");

            _debugFont = Content.Load<SpriteFont>("debug");

            _debug = new GameDebug(_debugFont, new Vector2(UserResWidth, UserResHeight), _blackScreen);

            CurrentGameMode = GameMode.None;
        }

        public static void ChangeState(GameState desiredGameState, GameMode mode)
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

        private static void LoadWorldFromFile(GameMode mode)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentGameMode = mode;
            _desiredGameState = GameState.GameWorld;
            IsLoadingContent = true;

            _reloadThread = new Thread(BackgroundThread_FileLoad);
            _reloadThread.IsBackground = true;
            _reloadThread.Start();
        }

        private static void BackgroundThread_FileLoad()
        {
            IsLoadingContent = true;
            if (!GameWorld.TryLoadFromFile(CurrentGameMode))
            {
                CurrentGameMode = GameMode.None;
                CurrentGameState = GameState.MainMenu;
            }

            IsLoadingContent = false;
            WasPressed = false;
        }

        protected override void Update(GameTime gameTime)
        {
            TimeSinceLastUpdate = (float)(gameTime.ElapsedGameTime.TotalSeconds);

            GameUpdateCalled?.Invoke(gameTime);

            GameTime = gameTime;

            if (TimeFreeze.IsTimeFrozen())
            {
                return;
            }

            _frameRateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_frameRateTimer > 1000f)
            {
                _fps = _totalFrames;
                _totalFrames = 0;
                _frameRateTimer = 0;
            }

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    IsMouseVisible = true;
                    break;
                case GameState.GameWorld:
                    IsMouseVisible = true;
                    break;
            }

            MessageBox.Update();
            if (MessageBox.IsActive)
            {
                return;
            }

            TextInputBox.Update();
            if (TextInputBox.IsActive)
            {
                return;
            }

            //if (GameData.Settings.HasChanged)
            //{
            //    GameData.SaveSettings();
            //    if (GameData.Settings.NeedsRestart)
            //    {
            //        _graphics.IsFullScreen = GameData.Settings.IsFullscreen;
            //        _graphics.ApplyChanges();
            //        GameData.Settings.NeedsRestart = false;
            //    }
            //    GameData.Settings.HasChanged = false;
            //}

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && CurrentGameState != GameState.MainMenu &&
                CurrentGameState != GameState.LoadingScreen)
            {
                if (CurrentGameState == GameState.GameWorld && GameWorld.IsTestingLevel)
                {
                    //ChangeState(GameState.GameWorld, GameMode.Edit);
                }
                else
                {
                    GameData.SaveGame();
                    Menu.CurrentMenuState = Menu.MenuState.Main;
                    ChangeState(GameState.MainMenu, GameMode.None);
                }
            }

            if (InputHelper.IsKeyDown(Keys.Enter) && CurrentGameState == GameState.GameWorld &&
                CurrentGameMode == GameMode.Play && GameWorld.IsTestingLevel)
            {
                LevelEditor.GoBackToEditing();
            }

            //Update the game based on what GameState it is
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    _menu.Update();
                    break;
                case GameState.LoadingScreen:
                    _loadingScreen.Update();

                    if (!IsLoadingContent)
                    {
                        CurrentGameState = _desiredGameState;
                    }
                    break;
                case GameState.GameWorld:
                    if (IsLoadingContent) return;
                    if (GameWorld.IsOnDebug)
                        break;

                    GameWorld.Update();
                    Dialog.Update();
                    break;
            }

            base.Update(gameTime);
            _debug.Update(this, DebugOn);
            Overlay.Update();
        }

        private void DrawToMainRenderTarget(RenderTarget2D renderTarget)
        {
            //Change RenderTarget to this from the default
            GraphicsDevice.SetRenderTarget(renderTarget);
            //Set up the background color
            GraphicsDevice.Clear(Color.Transparent);

            //Draw what is needed based on GameState
            switch (CurrentGameState)
            {
                case GameState.GameWorld:
                    if (IsLoadingContent)
                        break;





                    break;
            }

            //Return the current RenderTarget to the default
            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            _totalFrames++;
            //DrawToMainRenderTarget(_mainRenderTarget);

            //Set background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw what is needed based on GameState
            switch (CurrentGameState)
            {
                case GameState.LoadingScreen:
                    _spriteBatch.Begin();
                    _loadingScreen.Draw(_spriteBatch);
                    TextInputBox.Draw(_spriteBatch);
                    MessageBox.Draw(_spriteBatch);
                    _spriteBatch.End();
                    break;
                case GameState.MainMenu:
                    var rs2 = new RasterizerState { ScissorTestEnable = true };
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, rs2);
                    _spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, UserResWidth, UserResWidth);
                    _menu.Draw(_spriteBatch);
                    TextInputBox.Draw(_spriteBatch);
                    MessageBox.Draw(_spriteBatch);
                    _spriteBatch.End();
                    break;
                case GameState.GameWorld:

                    // Draw background and walls to normal render target.
                    GraphicsDevice.SetRenderTarget(_backRT);
                    GraphicsDevice.Clear(Color.Green);

                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                    GameWorld.DrawBackground(_spriteBatch);
                    _spriteBatch.End();

                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                        null, null, Camera.Translate);
                    GameWorld.DrawWalls(_spriteBatch);
                    _spriteBatch.End();



                    ////Draw the front tiles as usual.
                    GraphicsDevice.SetRenderTarget(_frontRT);
                    GraphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                        null, null, Camera.Translate);
                    GameWorld.Draw(_spriteBatch);
                    _spriteBatch.End();


                    //// Draw walls to another render target so that the shadows are only drawn when there is a wall.
                    GraphicsDevice.SetRenderTarget(_shadowRT);
                    GraphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                        null, null, Camera.Translate);
                    GameWorld.DrawWalls(_spriteBatch);
                    _spriteBatch.End();

                    // Draw shadow of front tiles and entities when there is a wall.
                    BlendState bs = new BlendState
                    {
                        AlphaSourceBlend = Blend.DestinationAlpha,
                        AlphaDestinationBlend = Blend.Zero,
                        ColorSourceBlend = Blend.SourceColor,
                        ColorDestinationBlend = Blend.Zero
                    };
                    _spriteBatch.Begin(SpriteSortMode.Deferred, bs, SamplerState.PointClamp, null,
                        null, null);
                    _spriteBatch.Draw(_frontRT, new Rectangle(6, 6, DefaultResWidth, DefaultResHeight), Color.Black * 1f);
                    _spriteBatch.End();

                    GraphicsDevice.SetRenderTarget(_lightRT);
                    GraphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                       null, null, Camera.Translate);
                    GameWorld.DrawLights(_spriteBatch);
                    _spriteBatch.End();


                    GraphicsDevice.SetRenderTarget(null);

                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, RasterizerState.CullNone);
                    _spriteBatch.Draw(_backRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    _spriteBatch.Draw(_shadowRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.Black * .5f);
                    _spriteBatch.Draw(_frontRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    _spriteBatch.End();


                    BlendState bsLight = new BlendState
                    {
                        AlphaSourceBlend = Blend.Zero,
                        AlphaDestinationBlend = Blend.InverseSourceAlpha,
                        ColorSourceBlend = Blend.DestinationColor,
                    };
                    _spriteBatch.Begin(SpriteSortMode.Deferred, bsLight, SamplerState.PointClamp,
                        DepthStencilState.None, RasterizerState.CullNone);
                    _spriteBatch.Draw(_lightRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    _spriteBatch.End();


                    /////////////////////////////////////////////////////

                    //_spriteBatch.Begin(SpriteSortMode.Immediate, _lightBlendState,
                    //    GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    //_spriteBatch.Draw(_lightingRenderTarget, new Rectangle(0, 0, UserResWidth, UserResHeight),
                    //    Color.White);
                    //_spriteBatch.End();


                    var rs = new RasterizerState { ScissorTestEnable = true };
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, rs);
                    GameWorld.DrawUi(_spriteBatch);
                    Overlay.Draw(_spriteBatch);
                    Dialog.Draw(_spriteBatch);
                    TextInputBox.Draw(_spriteBatch);
                    MessageBox.Draw(_spriteBatch);
                    _spriteBatch.End();

                    break;
            }
            base.Draw(gameTime);

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
                _spriteBatch.Begin();
                _spriteBatch.Draw(_blackScreen, new Rectangle(0, 0, UserResWidth, 360), Color.White * .3f);
                _spriteBatch.DrawString(_debugFont, Version + " FPS: " + _fps, new Vector2(0, 0), Color.White);
                _spriteBatch.DrawString(_debugFont, "", new Vector2(0, 20), Color.White);
                _spriteBatch.DrawString(_debugFont,
                    "Camera Position:" + Camera.CenterGameCoords.X + "," + Camera.CenterGameCoords.Y,
                    new Vector2(0, 40), Color.White);
                _spriteBatch.DrawString(_debugFont,
                    "Editor Rectangle Position:" + LevelEditor.EditorRectangle.X + "," +
                    LevelEditor.EditorRectangle.Y, new Vector2(0, 60), Color.White);
                _spriteBatch.DrawString(_debugFont, "Camera DrawRect:" + Camera.DrawRectangle, new Vector2(0, 80),
                    Color.White);
                _spriteBatch.DrawString(_debugFont, "Times Updated: " + GameWorld.TimesUpdated, new Vector2(0, 100),
                    Color.White);
                _spriteBatch.DrawString(_debugFont, "Player is jumping: " + GameWorld.GetPlayer().IsJumping,
                    new Vector2(0, 120),
                    Color.White);
                _spriteBatch.DrawString(_debugFont, "Player is touching ground:" + GameWorld.GetPlayer().IsTouchingGround, new Vector2(0, 140), Color.White);
                _spriteBatch.DrawString(_debugFont, "Time since last update:" + TimeSinceLastUpdate, new Vector2(0, 160), Color.White);
                _spriteBatch.DrawString(_debugFont, "Player Velocity" + GameWorld.GetPlayer().GetVelocity(),
                    new Vector2(0, 180),
                    Color.White);
                _spriteBatch.DrawString(_debugFont,
                    "Total Chunks: " + GameWorld.ChunkManager.GetNumberOfChunks() + " Active Chunk: " +
                    GameWorld.ChunkManager.GetActiveChunkIndex(), new Vector2(0, 200), Color.White);
                _spriteBatch.DrawString(_debugFont,
                    "Particle Index: " + GameWorld.ParticleSystem.GetCurrentParticleIndex(),
                    new Vector2(0, 240), Color.White);
                _spriteBatch.DrawString(_debugFont, "Entity Count: " + GameWorld.Entities?.Count,
                    new Vector2(0, 260), Color.White);
                _spriteBatch.DrawString(_debugFont,
                    "Visible Tiles: " + GameWorld.ChunkManager.GetVisibleIndexes()?.Length,
                    new Vector2(0, 280), Color.White);
                _spriteBatch.DrawString(_debugFont, "", new Vector2(0, 300), Color.White);
                _spriteBatch.DrawString(_debugFont, "Is TextInputBox Active: " + TextInputBox.IsActive,
                    new Vector2(0, 320), Color.White);
                _spriteBatch.DrawString(_debugFont, "Is MessageBox Active: " + MessageBox.IsActive,
                    new Vector2(0, 340), Color.White);
                _spriteBatch.DrawString(_debugFont, "Timers called: " + Timer.ActiveTimers,
                    new Vector2(0, 360), Color.White);

                _debug.Draw(_spriteBatch);
                _spriteBatch.End();
            }

        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Session.IsActive = false;
            base.OnExiting(sender, args);
        }
    }
}