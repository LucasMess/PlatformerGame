using ThereMustBeAnotherWay.GameData;
using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.Network;
using ThereMustBeAnotherWay.PlayerCharacter;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.UI.Elements;
using ThereMustBeAnotherWay.UI.Information;
using ThereMustBeAnotherWay.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using Steamworks;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MessageBox = ThereMustBeAnotherWay.UI.MessageBox;

namespace ThereMustBeAnotherWay
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

    public partial class TMBAW_Game : Game
    {
        public delegate void UpdateHandler();

        private const bool InDebugMode = true;
        private const bool IsTestingMultiplayer = false;
        public const int Tilesize = 32;
        public const int DefaultResWidth = 960; // Default 960x540
        public const int DefaultResHeight = 540;
        public const int DefaultUiWidth = 960;
        public const int DefaultUiHeight = 540;
        public const string Version = "Version 0.10.3 Beta";
        public const string Producers = "BitBite Games";
        public const float Gravity = .8f;
        public static bool IsLoadingContent;
        public static int UserResWidth;
        public static int UserResHeight;
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
        public static bool WasEscapeButtonReleased;
        public bool IsInStoryMode = false;
        public static string UserName;
        private static bool _wantsToQuit;
        public static CSteamID SteamID;

        /// <summary>
        ///     Used to display messages to the user where he needs to press OK to continue.
        /// </summary>
        public static MessageBox MessageBox { get; set; }
        public static TextInputBox TextInputBox { get; set; }
        public static TimeFreeze TimeFreeze { get; set; } = new TimeFreeze();

        public TMBAW_Game()
        {
            // Important services that need to be instanstiated before other things.
            _graphics = new GraphicsDeviceManager(this);
            Content = new ContentManager(Services, "Content");
            GameData = new GameDataManager();

            Window.IsBorderless = true;


            DataFolder.Initialize();
            SettingsFile settings = DataFolder.GetSettingsFile();

            UserName = SteamFriends.GetPersonaName();
            SteamID = SteamUser.GetSteamID();
#if DEBUG
            SteamUserStats.ResetAllStats(true);
#endif

            MediaPlayer.Volume = settings.MusicVolume;
            MaxVolume = settings.SoundVolume;

            // Change game settings here.
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.PreferMultiSampling = false;
            IsFixedTimeStep = true;

            _graphics.ApplyChanges();

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        protected override void Initialize()
        {
            SettingsFile settings = DataFolder.GetSettingsFile();

            GraphicsRenderer.Initialize(GraphicsDevice, _graphics);
            GraphicsRenderer.ChangeResolution(settings.ResolutionWidth, settings.ResolutionHeight);
            GraphicsRenderer.SetFullscreen(settings.IsFullscreen);

            Camera = new Camera(GraphicsDevice.Viewport);
            MainMenu.Initialize(this);
            OptionsMenu.Initialize();
            Dialog = new Dialog();
            MessageBox = new MessageBox();
            TextInputBox = new TextInputBox();
            Overlay.Initialize();
            PauseMenu.Initialize();
            Session.Initialize();
            GameWorld.Initialize();
            LoadingScreen.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// Lets the game know that the user wants to quit.
        /// </summary>
        public static void Quit()
        {
            _wantsToQuit = true;
        }

        protected override void LoadContent()
        {
            _blackScreen = ContentHelper.LoadTexture("Tiles/black");

            _debugFont = Content.Load<BitmapFont>("debug");
            
            GoToMainMenu();

            if (Program.LaunchedFromInvite)
            {
                Lobby.JoinLobby(Program.GameLaunchLobbyId);
            }
        }

        public static void GoToMainMenu()
        {
            CurrentGameState = GameState.MainMenu;
            CurrentGameMode = GameMode.None;
            string basePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            try
            {
                DataFolder.LoadLevelForBackground(basePath + "/Content/Levels/MainMenu.lvl");
            }
            catch
            {

            }

        }

        public static void ChangeState(GameState desiredGameState, GameMode mode, bool reloadWorld)
        {
            CurrentGameState = GameState.LoadingScreen;
            CurrentGameMode = mode;
            _desiredGameState = desiredGameState;
            IsLoadingContent = true;

            if (reloadWorld)
            {
                _reloadThread = new Thread(BackgroundThread_FileLoad)
                {
                    IsBackground = true
                };
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

            if (_wantsToQuit)
                Exit();

            SteamAPI.RunCallbacks();

            GameTime = gameTime;
            GameUpdateCalled?.Invoke();

            _frameRateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_frameRateTimer > 1000f)
            {
                FPS = _totalFrames;
                _totalFrames = 0;
                _frameRateTimer = 0;
            }

            IsMouseVisible = false;

            UI.Elements.Cursor.Update();

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


            OptionsMenu.Update();
            if (OptionsMenu.IsActive)
                return;

            PauseMenu.Update();
            if (PauseMenu.IsActive)
                return;

            Dialog.Update();

            Overlay.Update();
            KeyPopUp.Update();

            Player player = GameWorld.GetPlayer();

            Session.Update();

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
                    GameWorld.updateTimer.Restart();
                    if (!TimeFreeze.IsTimeFrozen())
                        GameWorld.UpdateWorld();
                    GameWorld.UpdateVisual();
                    GameWorld.updateTimer.Stop();


                    if (StoryTracker.IsInStoryMode)
                    {
                        StoryTracker.Update();
                    }
                    break;
            }

            base.Update(gameTime);

        }


        protected override void Draw(GameTime gameTime)
        {
            _totalFrames++;
            GameWorld.drawTimer.Restart();
            GraphicsRenderer.Draw();
            GameWorld.drawTimer.Stop();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Session.IsActive = false;
            base.OnExiting(sender, args);
        }
    }
}