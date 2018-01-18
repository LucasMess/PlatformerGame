using ThereMustBeAnotherWay.GameData;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ThereMustBeAnotherWay.Particles;
using BattleBoss.Utilities;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay.Graphics
{
    /// <summary>
    /// Responsible for rendering everything the screen. Keeps track of all the render targets and does special effects.
    /// </summary>
    public static class GraphicsRenderer
    {

        public static event ResolutionHandler OnResolutionChanged;
        public delegate void ResolutionHandler(int width, int height);

        public static bool ShadowsEnabled { get; set; } = true;
        /// <summary>
        /// Enable if static lights from tiles should be drawn.
        /// </summary>
        public static bool StaticLightsEnabled { get; set; } = true;

        private static bool IsDarkOutline => GameWorld.WorldData.IsDarkOutline;

        /// <summary>
        /// When drawing the lights on top of the world, any opaque area becomes transparent.
        /// </summary>
        private static BlendState LightingBlend = new BlendState
        {
            AlphaSourceBlend = Blend.DestinationAlpha,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.InverseSourceAlpha,
        };

        private static BlendState OnAlphaBlend = new BlendState
        {
            AlphaSourceBlend = Blend.DestinationAlpha,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha,
        };

        /// <summary>
        /// Rasterizer state for drawing inside a scissor rectangle.
        /// </summary>
        public static RasterizerState ScissorRectRasterizer = new RasterizerState()
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = FillMode.Solid,
            DepthBias = 0,
            MultiSampleAntiAlias = false,
            ScissorTestEnable = true,
            SlopeScaleDepthBias = 0,
        };

        /// <summary>
        /// Default Rasterizer State
        /// </summary>
        public static RasterizerState DefaultRasterizer = new RasterizerState()
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = FillMode.Solid,
            DepthBias = 0,
            MultiSampleAntiAlias = false,
            ScissorTestEnable = false,
            SlopeScaleDepthBias = 0,
        };

        public static DepthStencilState DefaultDepthStencil = new DepthStencilState()
        {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = true,
            DepthBufferFunction = CompareFunction.LessEqual,
            StencilEnable = false,
            StencilFunction = CompareFunction.Always,
            StencilPass = StencilOperation.Keep,
            StencilFail = StencilOperation.Keep,
            StencilDepthBufferFail = StencilOperation.Keep,
            TwoSidedStencilMode = false,
            CounterClockwiseStencilFunction = CompareFunction.Always,
            CounterClockwiseStencilFail = StencilOperation.Keep,
            CounterClockwiseStencilPass = StencilOperation.Keep,
            CounterClockwiseStencilDepthBufferFail = StencilOperation.Keep,
            StencilMask = Int32.MaxValue,
            StencilWriteMask = Int32.MaxValue,
            ReferenceStencil = 0,
        };

        private static SpriteBatch _spriteBatch;
        private static GraphicsDevice _graphicsDevice;
        private static GraphicsDeviceManager _graphicsManager;

        private static RenderTarget2D _sunlightRenderTarget;
        private static RenderTarget2D _otherLightsRenderTarget;
        private static RenderTarget2D _userInterfaceRenderTarget;
        private static RenderTarget2D _backgroundRenderTarget;
        private static RenderTarget2D _wallRenderTarget;
        private static RenderTarget2D _mainRenderTarget;
        private static RenderTarget2D _rippleRenderTarget;
        private static RenderTarget2D _combinedLightingRenderTarget;
        private static RenderTarget2D _combinedWorldRenderTarget;

        private static Effect testEffect;

        public static AverageStopwatch LightDrawTimer = new AverageStopwatch();
        public static AverageStopwatch TileDrawTimer = new AverageStopwatch();
        public static AverageStopwatch ShadowsDrawTimer = new AverageStopwatch();
        public static AverageStopwatch UserInterfaceDrawTimer = new AverageStopwatch();

        /// <summary>
        /// Initializes all of the components used for rendering, such as rendertargets and spritebatches.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsManager)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _graphicsDevice = graphicsDevice;
            _graphicsManager = graphicsManager;

            _mainRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                    _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _wallRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                    _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _backgroundRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
               graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                   _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _otherLightsRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
               graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _sunlightRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
               graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _combinedLightingRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
               graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _rippleRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                   graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _combinedWorldRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                   graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            testEffect = ContentHelper.LoadEffect("Effects/testEffect");

        }

        /// <summary>
        /// Draws everything to the screen.
        /// </summary>
        public static void Draw()
        {
            TileDrawTimer.Start();

            _graphicsDevice.SetRenderTarget(_backgroundRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawBackground();

            _graphicsDevice.SetRenderTarget(_wallRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawWalls();

            _graphicsDevice.SetRenderTarget(_mainRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawGameWorld();

            TileDrawTimer.Stop();

            UserInterfaceDrawTimer.Start();

            _graphicsDevice.SetRenderTarget(_userInterfaceRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawUserInterface();

            UserInterfaceDrawTimer.Stop();

            LightDrawTimer.Start();

            _graphicsDevice.SetRenderTarget(_sunlightRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawSunlight();

            _graphicsDevice.SetRenderTarget(_otherLightsRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawOtherLights();

            _graphicsDevice.SetRenderTarget(_combinedLightingRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer);
            int width = TMBAW_Game.DefaultResWidth;
            int height = TMBAW_Game.DefaultResHeight;
            _spriteBatch.Draw(_otherLightsRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(_sunlightRenderTarget, new Rectangle(0, 0, width, height), GetMainRenderTargetColor());
            _spriteBatch.End();

            LightDrawTimer.Stop();

            TileDrawTimer.Start();

            _graphicsDevice.SetRenderTarget(_rippleRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawRipples();

            TileDrawTimer.Stop();

            _graphicsDevice.SetRenderTarget(_combinedWorldRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            CombineRenderTargets();

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Transparent);
            FinalRender();

            TileDrawTimer.Measure();
            UserInterfaceDrawTimer.Measure();
            LightDrawTimer.Measure();
        }

        /// <summary>
        /// Draw the background of the level in a static position.
        /// </summary>
        private static void DrawBackground()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null);
            GameWorld.DrawBackground(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draw the special effect ripples that use a shader.
        /// </summary>
        private static void DrawRipples()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawRipples(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null);
            Overlay.DrawRipples(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Everything drawn here will not be translated by a camera.
        /// </summary>
        private static void DrawUserInterface()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, ScissorRectRasterizer);
            Overlay.Draw(_spriteBatch);
            switch (TMBAW_Game.CurrentGameState)
            {
                case GameState.MainMenu:
                    MainMenu.Draw(_spriteBatch);
                    break;
                case GameState.LoadingScreen:
                    LoadingScreen.Draw(_spriteBatch);
                    break;
                case GameState.GameWorld:
                    GameWorld.DrawUi(_spriteBatch);
                    break;
                default:
                    break;
            }

            PauseMenu.Draw(_spriteBatch);
            OptionsMenu.Draw(_spriteBatch);
            TMBAW_Game.Dialog.Draw(_spriteBatch);
            GameDebug.Draw(_spriteBatch);
            TMBAW_Game.MessageBox.Draw(_spriteBatch);
            TMBAW_Game.TextInputBox.Draw(_spriteBatch);

            Cursor.Draw(_spriteBatch);

            _spriteBatch.End();
        }


        /// <summary>
        /// Draws all the walls in the gameworld along with the shadows on them..
        /// </summary>
        private static void DrawWalls()
        {
            if (TMBAW_Game.CurrentGameState == GameState.LoadingScreen)
                return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawWalls(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, OnAlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawWallShadows(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draws things that are only seen by the camera in the gameworld.
        /// </summary>
        private static void DrawGameWorld()
        {
            int width = TMBAW_Game.UserResWidth;
            int height = TMBAW_Game.UserResHeight;

            if (TMBAW_Game.CurrentGameState == GameState.LoadingScreen)
                return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null);
            _spriteBatch.Draw(_wallRenderTarget, new Rectangle(0, 0, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), Color.White);
            _spriteBatch.End();


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            GameWorld.Draw(_spriteBatch);
            ParticleSystem.DrawNormalParticles(_spriteBatch);
            KeyPopUp.Draw(_spriteBatch);
            _spriteBatch.End();
            //GameWorld.DrawRipples(_spriteBatch);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawGlows(_spriteBatch);
            _spriteBatch.End();


            _spriteBatch.End();
        }

        /// <summary>
        /// Draws just the sunlight from the lighting engine.
        /// </summary>
        private static void DrawSunlight()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null);
            _spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), _otherLightsRenderTarget.Bounds, Color.White);
            _spriteBatch.End();

            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            //LightingSystem.DrawSunlight(_spriteBatch);
            //_spriteBatch.End();
        }


        /// <summary>
        /// Draw all the other lights that are not static, such as lights from projectiles or from the player.
        /// </summary>
        private static void DrawOtherLights()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawLights(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draws all the rendertargets in the correct order.
        /// </summary>
        private static void CombineRenderTargets()
        {
            int width = TMBAW_Game.DefaultResWidth;
            int height = TMBAW_Game.DefaultResHeight;

            testEffect.Parameters["InputTexture"].SetValue(_rippleRenderTarget);
            testEffect.Parameters["LastTexture"].SetValue(_mainRenderTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, testEffect);
            _spriteBatch.Draw(_mainRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.End();

            if (TMBAW_Game.CurrentGameMode == GameMode.Play || LevelEditor.IsLightingEnabled)
            {
                if (StaticLightsEnabled && !GameWorld.WorldData.IsTopDown)
                {
                    _spriteBatch.Begin(SpriteSortMode.Deferred, LightingBlend, SamplerState.AnisotropicClamp);
                    _spriteBatch.Draw(_combinedLightingRenderTarget, new Rectangle(0, 0, width, height), Color.White);
                    _spriteBatch.End();
                }
            }

        }

        /// <summary>
        /// Final render routine, the three different layers are combined.
        /// </summary>
        private static void FinalRender()
        {
            int width = TMBAW_Game.UserResWidth;
            int height = TMBAW_Game.UserResHeight;
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DefaultDepthStencil, DefaultRasterizer, null);
            _spriteBatch.Draw(_backgroundRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(_combinedWorldRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(_userInterfaceRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        /// Returns a reference to the current graphics device.
        /// </summary>
        /// <returns></returns>
        public static GraphicsDevice GetGraphicsDevice()
        {
            return _graphicsDevice;
        }

        /// <summary>
        /// Returns the color that the main render target should be colored as, which is specified in the level's world data.
        /// </summary>
        /// <returns></returns>
        private static Color GetMainRenderTargetColor()
        {
            if (IsDarkOutline) return Color.Black;

            // Good color for night time.
            //return new Color(63, 37, 140);

            return GameWorld.WorldData.SunLightColor;
        }

        /// <summary>
        /// Changes the window resolution and applies settings.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ChangeResolution(int width, int height)
        {
            Point max = GetMonitorResolution();
            if (width > max.X || height > max.Y)
            {
                Console.WriteLine("Too big: {0}x{1}", width, height);
                throw new ArgumentOutOfRangeException("width", "This resolution is bigger than the max resolution of: " + max.X + "x" + max.Y);
            }
            _graphicsManager.PreferredBackBufferWidth = width;
            _graphicsManager.PreferredBackBufferHeight = height;
            TMBAW_Game.UserResWidth = width;
            TMBAW_Game.UserResHeight = height;

            TMBAW_Game.WidthRatio = ((double)TMBAW_Game.UserResWidth / TMBAW_Game.DefaultUiWidth);
            TMBAW_Game.HeightRatio = ((double)TMBAW_Game.UserResHeight / TMBAW_Game.DefaultUiHeight);

            TMBAW_Game.UiWidthRatio = ((double)TMBAW_Game.UserResWidth / TMBAW_Game.DefaultUiWidth);
            TMBAW_Game.UiHeightRatio = ((double)TMBAW_Game.UserResHeight / TMBAW_Game.DefaultUiHeight);

            _graphicsManager.ApplyChanges();
            ChangeUserInterfaceResolution();

            OnResolutionChanged?.Invoke(width, height);
        }

        /// <summary>
        /// Changes the resolution of the game to be the monitor's resolution.
        /// </summary>
        public static void ChangeToNativeResolution()
        {
            ChangeResolution(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        }

        /// <summary>
        /// Changes the size of the user interface rendertarget so that the user interface scales well with the screen resolution.
        /// </summary>
        private static void ChangeUserInterfaceResolution()
        {
            _userInterfaceRenderTarget = new RenderTarget2D(_graphicsDevice, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight, false,
                 _graphicsDevice.PresentationParameters.BackBufferFormat, _graphicsDevice.PresentationParameters.DepthStencilFormat,
                    _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Sets the game to full screen and changes the resolution to match.
        /// </summary>
        /// <param name="value"></param>
        public static void SetFullscreen(bool value)
        {
            _graphicsManager.IsFullScreen = value;

            if (_graphicsManager.IsFullScreen)
            {
                ChangeToNativeResolution();
            }
            else
            {
                SettingsFile settings = DataFolder.GetSettingsFile();
                ChangeResolution(settings.ResolutionWidth, settings.ResolutionHeight);
            }

        }

        /// <summary>
        /// Returns the user's current monitor resolution.
        /// </summary>
        /// <returns></returns>
        public static Point GetMonitorResolution()
        {
            return new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        }

    }
}
