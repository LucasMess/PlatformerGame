using Adam.GameData;
using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Network;
using Adam.UI;
using Adam.UI.Information;
using Adam.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using System;
using System.IO;
using System.Threading;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MessageBox = Adam.UI.MessageBox;

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

    public class AdamGame : Game
    {
        public delegate void UpdateHandler();

        private const bool InDebugMode = true;
        private const bool IsTestingMultiplayer = false;
        public const int Tilesize = 32;
        public const int DefaultResWidth = 960; // Default 960x540
        public const int DefaultResHeight = 540;
        public const int DefaultUiWidth = 480;
        public const int DefaultUiHeight = 270;
        public const string Version = "Version 0.10.0 Beta";
        public const string Producers = "BitBite Games";
        public const float Gravity = .8f;
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
        public static double UiWidthRatio;
        public static double UiHeightRatio;
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
        private BitmapFont _debugFont;
        public static int FPS { get; set; }
        private int _totalFrames;
        private double _frameRateTimer;
        //Game Variables
        private BlendState _lightBlendState;
        private LoadingScreen _loadingScreen;
        private RenderTarget2D _frontRT;
        private RenderTarget2D _shadowRT;
        private RenderTarget2D _backRT;
        private RenderTarget2D _lightRT;
        private RenderTarget2D _sunlightRT;
        private RenderTarget2D _uiRT;
        private Menu _menu;
        private Session _session;
        public SamplerState DesiredSamplerState;
        public static event UpdateHandler GameUpdateCalled;
        private bool _wasEscapeReleased;

        public AdamGame()
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

            UiWidthRatio = (DefaultUiWidth / (double)UserResWidth);
            UiHeightRatio = (DefaultUiHeight / (double)UserResHeight);

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
            IsFixedTimeStep = true;
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
            Overlay.Initialize();

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
            _sunlightRT = new RenderTarget2D(GraphicsDevice, DefaultResWidth, DefaultResHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _uiRT = new RenderTarget2D(GraphicsDevice, DefaultUiWidth, DefaultUiHeight, false,
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

            _debugFont = Content.Load<BitmapFont>("debug");

            CurrentGameMode = GameMode.None;

            string basePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            DataFolder.LoadLevelForBackground(basePath + "/Content/Levels/Main Menu.lvl");
        }

        public static void ChangeState(GameState desiredGameState, GameMode mode, bool reloadWorld)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentGameMode = mode;
            _desiredGameState = desiredGameState;
            IsLoadingContent = true;

            if (reloadWorld)
            {
                _reloadThread = new Thread(BackgroundThread_FileLoad);
                _reloadThread.IsBackground = true;
                _reloadThread.Start();
            }
            else
            {
                IsLoadingContent = false;
            }
        }

        private static void BackgroundThread_FileLoad()
        {
            if (!GameWorld.TryLoadFromFile(CurrentGameMode))
            {
                // If loading fails, return to main menu to avoid errors.
                CurrentGameMode = GameMode.None;
                CurrentGameState = GameState.MainMenu;
            }

            IsLoadingContent = false;
            WasPressed = false;
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            GameUpdateCalled?.Invoke();

            _frameRateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_frameRateTimer > 1000f)
            {
                FPS = _totalFrames;
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

            if (InputHelper.IsKeyUp(Keys.Escape))
                _wasEscapeReleased = true;

            if (_wasEscapeReleased)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && CurrentGameState != GameState.MainMenu &&
                    CurrentGameState != GameState.LoadingScreen)
                {
                    if (CurrentGameState == GameState.GameWorld && GameWorld.IsTestingLevel)
                    {
                        _wasEscapeReleased = false;
                        LevelEditor.GoBackToEditing();
                    }
                    else if (CurrentGameState == GameState.GameWorld && CurrentGameMode == GameMode.Edit && Inventory.IsOpen)
                    {
                        _wasEscapeReleased = false;
                        Inventory.OpenOrClose();
                    }
                    else
                    {
                        //GameData.SaveGame();
                        _wasEscapeReleased = false;
                        Menu.CurrentMenuState = Menu.MenuState.Main;
                        ChangeState(GameState.MainMenu, GameMode.None, false);
                    }
                }

                if (InputHelper.IsKeyDown(Keys.Enter) && CurrentGameState == GameState.GameWorld &&
                    CurrentGameMode == GameMode.Play && GameWorld.IsTestingLevel)
                {
                    _wasEscapeReleased = false;
                    LevelEditor.GoBackToEditing();
                }
            }

            //Update the game based on what GameState it is
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    _menu.Update();
                    goto case GameState.GameWorld;
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
                    if (!TimeFreeze.IsTimeFrozen())
                        GameWorld.UpdateWorld();
                    GameWorld.UpdateVisual();
                    Dialog.Update();
                    break;
            }

            base.Update(gameTime);
            GameDebug.Update();
            Overlay.Update();
        }


        protected override void Draw(GameTime gameTime)
        {
            _totalFrames++;

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
                    goto case GameState.GameWorld;
                    GraphicsDevice.SetRenderTarget(_uiRT);
                    GraphicsDevice.Clear(Color.Black);
                    var rs2 = new RasterizerState { ScissorTestEnable = true };
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, rs2);
                    _spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, UserResWidth, UserResWidth);
                    _menu.Draw(_spriteBatch);
                    TextInputBox.Draw(_spriteBatch);
                    MessageBox.Draw(_spriteBatch);
                    _spriteBatch.End();

                    GraphicsDevice.SetRenderTarget(null);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                      DepthStencilState.None, RasterizerState.CullNone);
                    _spriteBatch.Draw(_uiRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
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
                    LightingEngine.DrawGlows(_spriteBatch);
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

                    GraphicsDevice.SetRenderTarget(_uiRT);
                    GraphicsDevice.Clear(Color.Transparent);
                    var rs = new RasterizerState { ScissorTestEnable = true };
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, rs);

                    // For the new main menu that has a world simulated in the background.
                    if (CurrentGameState == GameState.MainMenu)
                        _menu.Draw(_spriteBatch);
                    else
                        GameWorld.DrawUi(_spriteBatch);


                    Overlay.Draw(_spriteBatch);
                    Dialog.Draw(_spriteBatch);
                    TextInputBox.Draw(_spriteBatch);
                    MessageBox.Draw(_spriteBatch);
                    _spriteBatch.End();


                    GraphicsDevice.SetRenderTarget(null);

                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, RasterizerState.CullNone);
                    _spriteBatch.Draw(_backRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    _spriteBatch.Draw(_shadowRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.Black * .5f);

                    int count = 0;
                    if (TimeFreeze.IsTimeFrozen())
                    {
                        count = 10;
                    }
                    Color color = Color.White;
                    for (int i = 0; i <= count; i++)
                    {
                        int dist = i * 5;
                        _spriteBatch.Draw(_frontRT, new Rectangle(0 + dist, 0 + dist, UserResWidth - dist * 2, UserResHeight - dist * 2), color);
                        color *= .8f;
                    }

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

                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                       DepthStencilState.None, RasterizerState.CullNone);
                    _spriteBatch.Draw(_uiRT, new Rectangle(0, 0, UserResWidth, UserResHeight), Color.White);
                    _spriteBatch.End();

                    /////////////////////////////////////////////////////

                    //_spriteBatch.Begin(SpriteSortMode.Immediate, _lightBlendState,
                    //    GameData.Settings.DesiredSamplerState, DepthStencilState.None, RasterizerState.CullNone);
                    //_spriteBatch.Draw(_lightingRenderTarget, new Rectangle(0, 0, UserResWidth, UserResHeight),
                    //    Color.White);
                    //_spriteBatch.End();               

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

            _spriteBatch.Begin();
            GameDebug.Draw(_spriteBatch);
            _spriteBatch.End();


        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Session.IsActive = false;
            base.OnExiting(sender, args);
        }
    }
}