using ThereMustBeAnotherWay.GameData;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.Graphics
{
    /// <summary>
    /// Responsible for rendering everything the screen. Keeps track of all the render targets and does special effects.
    /// </summary>
    public static class GraphicsRenderer
    {
        public static bool ShadowsEnabled { get; set; } = true;
        public static bool ComplexLightingEnabled { get; set; } = true;

        private static bool IsDarkOutline => GameWorld.WorldData.IsDarkOutline;

        /// <summary>
        /// When drawing the lights on top of the world, any opaque area becomes transparent.
        /// </summary>
        private static BlendState LightingBlend = new BlendState
        {
            AlphaSourceBlend = Blend.DestinationColor,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero
        };

        private static SpriteBatch _spriteBatch;
        private static GraphicsDevice _graphicsDevice;
        private static GraphicsDeviceManager _graphicsManager;

        private static RenderTarget2D _lightingRenderTarget;
        private static RenderTarget2D _userInterfaceRenderTarget;
        private static RenderTarget2D _mainRenderTarget;
        private static RenderTarget2D _rippleRenderTarget;

        private static Effect testEffect;

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

            _lightingRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
               graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _userInterfaceRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
                 graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                    graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _rippleRenderTarget = new RenderTarget2D(graphicsDevice, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight, false,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                   graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            testEffect = ContentHelper.LoadEffect("Effects/testEffect");

        }

        /// <summary>
        /// Draws everything to the screen.
        /// </summary>
        public static void Draw()
        {
            _graphicsDevice.SetRenderTarget(_mainRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawGameWorld();

            _graphicsDevice.SetRenderTarget(_userInterfaceRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawUserInterface();

            _graphicsDevice.SetRenderTarget(_lightingRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawLights();

            _graphicsDevice.SetRenderTarget(_rippleRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawRipples();

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Green);
            CombineRenderTargets();

        }

        private static void DrawRipples()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawRipples(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
            Overlay.DrawRipples(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Everything drawn here will not be translated by a camera.
        /// </summary>
        private static void DrawUserInterface()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);
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

            Overlay.Draw(_spriteBatch);
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
        /// Draws things that are only seen by the camera in the gameworld.
        /// </summary>
        private static void DrawGameWorld()
        {
            if (TMBAW_Game.CurrentGameState == GameState.LoadingScreen)
                return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
            GameWorld.DrawBackground(_spriteBatch);
            _spriteBatch.End();


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, TMBAW_Game.Camera.Translate);
            //GameWorld.DrawBackground(_spriteBatch);
            GameWorld.DrawWalls(_spriteBatch);
            GameWorld.Draw(_spriteBatch);
            KeyPopUp.Draw(_spriteBatch);
            //GameWorld.DrawRipples(_spriteBatch);



            _spriteBatch.End();
        }

        /// <summary>
        /// Draws all lights from the lighting engine.
        /// </summary>
        private static void DrawLights()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, TMBAW_Game.Camera.Translate);
            GameWorld.DrawLights(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draws all the rendertargets in the correct order.
        /// </summary>
        private static void CombineRenderTargets()
        {
            int width = TMBAW_Game.UserResWidth;
            int height = TMBAW_Game.UserResHeight;


            testEffect.Parameters["InputTexture"].SetValue(_rippleRenderTarget);
            testEffect.Parameters["LastTexture"].SetValue(_mainRenderTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, testEffect);
            _spriteBatch.Draw(_mainRenderTarget, new Rectangle(0, 0, width, height), GetMainRenderTargetColor());
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, LightingBlend, SamplerState.AnisotropicClamp);
            _spriteBatch.Draw(_lightingRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.End();


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
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

        private static Color GetMainRenderTargetColor()
        {
            if (IsDarkOutline) return Color.Black;

            //if (GameWorld.GetPlayer().rewindTracker.IsRewinding)
            //{
            //    return new Color(12, 76, 79);
            //}

            return Color.White;
        }

        /// <summary>
        /// Changes the window resolution and applies settings.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ChangeResolution(int width, int height)
        {
            _graphicsManager.PreferredBackBufferWidth = width;
            _graphicsManager.PreferredBackBufferHeight = height;
            TMBAW_Game.UserResWidth = width;
            TMBAW_Game.UserResHeight = height;

            TMBAW_Game.WidthRatio = (TMBAW_Game.DefaultResWidth / (double)TMBAW_Game.UserResWidth);
            TMBAW_Game.HeightRatio = (TMBAW_Game.DefaultResHeight / (double)TMBAW_Game.UserResHeight);

            TMBAW_Game.UiWidthRatio = (TMBAW_Game.DefaultUiWidth / (double)TMBAW_Game.UserResWidth);
            TMBAW_Game.UiHeightRatio = (TMBAW_Game.DefaultUiHeight / (double)TMBAW_Game.UserResHeight);


            _graphicsManager.ApplyChanges();
        }

        public static void ChangeToNativeResolution()
        {
            ChangeResolution(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        }

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

    }
}
