using Adam.Levels;
using Adam.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Graphics
{
    /// <summary>
    /// Responsible for rendering everything the screen. Keeps track of all the render targets and does special effects.
    /// </summary>
    public static class GraphicsRenderer
    {
        public static bool ShadowsEnabled { get; set; } = true;
        public static bool ComplexLightingEnabled { get; set; } = true;
        public static bool IsFullScreen { get; set; } = true;

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

        private static RenderTarget2D _lightingRenderTarget;
        private static RenderTarget2D _userInterfaceRenderTarget;
        private static RenderTarget2D _mainRenderTarget;

        /// <summary>
        /// Initializes all of the components used for rendering, such as rendertargets and spritebatches.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _graphicsDevice = graphicsDevice;

            _mainRenderTarget = new RenderTarget2D(graphicsDevice, AdamGame.DefaultResWidth, AdamGame.DefaultResHeight, false,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                    _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _lightingRenderTarget = new RenderTarget2D(graphicsDevice, AdamGame.DefaultResWidth, AdamGame.DefaultResHeight, false,
               graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                _graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            _userInterfaceRenderTarget = new RenderTarget2D(graphicsDevice, AdamGame.DefaultResWidth, AdamGame.DefaultResHeight, false,
                 graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat,
                    graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Draws everything to the screen.
        /// </summary>
        public static void Draw()
        {
            _graphicsDevice.SetRenderTarget(_mainRenderTarget);
            _graphicsDevice.Clear(Color.CornflowerBlue);
            DrawGameWorld();

            _graphicsDevice.SetRenderTarget(_userInterfaceRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawUserInterface();

            _graphicsDevice.SetRenderTarget(_lightingRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            DrawLights();

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Transparent);
            CombineRenderTargets();

        }

        /// <summary>
        /// Everything drawn here will not be translated by a camera.
        /// </summary>
        private static void DrawUserInterface()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);
            switch (AdamGame.CurrentGameState)
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
            AdamGame.MessageBox.Draw(_spriteBatch);
            AdamGame.TextInputBox.Draw(_spriteBatch);
            AdamGame.Dialog.Draw(_spriteBatch);
            GameDebug.Draw(_spriteBatch);

            _spriteBatch.End();
        }


        /// <summary>
        /// Draws things that are only seen by the camera in the gameworld.
        /// </summary>
        private static void DrawGameWorld()
        {
            if (AdamGame.CurrentGameState == GameState.LoadingScreen)
                return;

            _spriteBatch.Begin();
            GameWorld.DrawBackground(_spriteBatch);
            _spriteBatch.End();


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, AdamGame.Camera.Translate);

            GameWorld.DrawWalls(_spriteBatch);
            GameWorld.Draw(_spriteBatch);


            _spriteBatch.End();
        }

        /// <summary>
        /// Draws all lights from the lighting engine.
        /// </summary>
        private static void DrawLights()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, AdamGame.Camera.Translate);
            GameWorld.DrawLights(_spriteBatch);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draws all the rendertargets in the correct order.
        /// </summary>
        private static void CombineRenderTargets()
        {
            int width = AdamGame.UserResWidth;
            int height = AdamGame.UserResHeight;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
            _spriteBatch.Draw(_mainRenderTarget, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.End();

            if (AdamGame.CurrentGameState == GameState.GameWorld)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, LightingBlend);
                _spriteBatch.Draw(_lightingRenderTarget, new Rectangle(0, 0, width, height), Color.White);
                _spriteBatch.End();
            }

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

    }
}
