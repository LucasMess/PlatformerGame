using Adam.GameData;
using Adam.Graphics;
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
using Steamworks;
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

    public partial class AdamGame : Game
    {
        public delegate void UpdateHandler();

        private const bool InDebugMode = true;
        private const bool IsTestingMultiplayer = false;
        public const int Tilesize = 32;
        public const int DefaultResWidth = 960; // Default 960x540
        public const int DefaultResHeight = 540;
        public const int DefaultUiWidth = 960;
        public const int DefaultUiHeight = 540;
        public const string Version = "Version 0.10.0 Beta";
        public const string Producers = "BitBite Games";
        public const float Gravity = .8f;
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
        public static PlayerProfile LevelProgression = new PlayerProfile();
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
        public SamplerState DesiredSamplerState;
        public static event UpdateHandler GameUpdateCalled;
        private bool _wasEscapeReleased;
        public bool IsInStoryMode = false;
        public static string UserName;

        /// <summary>
        ///     Used to display messages to the user where he needs to press OK to continue.
        /// </summary>
        public static MessageBox MessageBox { get; set; }
        public static TextInputBox TextInputBox { get; set; }
        public static TimeFreeze TimeFreeze { get; set; } = new TimeFreeze();

        public AdamGame()
        {
            // Get the current monitor resolution and set it as the game's resolution
            var monitorRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            DataFolder.Initialize();


            SteamAPI.Init();
            UserName = SteamFriends.GetPersonaName();

#pragma warning disable 0162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (IsTestingMultiplayer)
            {
                monitorRes = new Vector2(1366, 768);
            }
            //UserResWidth = (int)monitorRes.X;
            //UserResHeight = (int)monitorRes.Y;
            UserResWidth = DefaultResWidth;
            UserResHeight = DefaultResHeight;

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
            _graphics.IsFullScreen = false;
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

        protected override void Initialize()
        {
            Camera = new Camera(GraphicsDevice.Viewport);
            MainMenu.Initialize(this);
            Dialog = new Dialog();
            MessageBox = new MessageBox();
            TextInputBox = new TextInputBox();
            Overlay.Initialize();

            DefaultTexture = ContentHelper.LoadTexture("Tiles/black");

            GraphicsRenderer.Initialize(GraphicsDevice);

            base.Initialize();
        }

        internal static void Quit()
        {
            //TODO: Quit.
        }

        protected override void LoadContent()
        {
            LoadingScreen.Initialize();
            _blackScreen = ContentHelper.LoadTexture("Tiles/black");

            _debugFont = Content.Load<BitmapFont>("debug");

            CurrentGameMode = GameMode.None;

            string basePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            //DataFolder.LoadLevelForBackground(basePath + "/Content/Levels/MainMenu.lvl");

            GameWorld.Initialize();
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
            if (!IsActive) return;

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

            GameDebug.Update();
            if (GameDebug.IsTyping)
            {
                return;
            }

            Dialog.Update();

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
                        MainMenu.CurrentMenuState = MainMenu.MenuState.Main;
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
                    MainMenu.Update();
                    if (GameWorld.TileArray != null && GameWorld.TileArray.Length != 0)
                    {
                        goto case GameState.GameWorld;
                    }
                    break;
                case GameState.LoadingScreen:
                    LoadingScreen.Update();

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
                    break;
            }

            base.Update(gameTime);
            Overlay.Update();
        }


        protected override void Draw(GameTime gameTime)
        {
            _totalFrames++;
            GraphicsRenderer.Draw();           
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Session.IsActive = false;
            base.OnExiting(sender, args);
        }
    }
}