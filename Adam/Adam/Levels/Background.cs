using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Documents;
using Adam.Levels;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Adam
{
    /// <summary>
    /// Class responsible for drawing the three layered backgrounds for each level.
    /// </summary>
    public class Background
    {
        private Texture2D _backgroundTexture;
        private Texture2D _middlegroundTexture;
        private Texture2D _foregroundTexture;
        private Texture2D _skyFog = ContentHelper.LoadTexture("Backgrounds/sky_fog");

        private Vector2[] middleCoords = new Vector2[3];
        private Vector2[] foreCoords = new Vector2[3];
        private const float FogGradient = .25f;

        /// <summary>
        /// The current background Id.
        /// </summary>
        public int BackgroundId { get; set; } = 1;
        int _lastBackgroundId;

        /// <summary>
        /// Loads the textures based on the level settings.
        /// </summary>
        public void Load()
        {
            BackgroundId = GameWorld.WorldData.BackgroundId;
            if (BackgroundId == 0)
            {
                GameWorld.WorldData.BackgroundId = 1;
                BackgroundId = 1;
            }

            // Sets the textures to a default texture to avoid problems.
            _backgroundTexture = Main.DefaultTexture;
            _middlegroundTexture = Main.DefaultTexture;
            _foregroundTexture = Main.DefaultTexture;

            // Tries to set the textures to what the level settings specify. If none are found, the default are kept.
            try
            {
                Console.WriteLine("Reloading background textures.");
                _backgroundTexture = ContentHelper.LoadTexture("Backgrounds/" + BackgroundId + "_background");
                _middlegroundTexture = ContentHelper.LoadTexture("Backgrounds/" + BackgroundId + "_middleground");
                _foregroundTexture = ContentHelper.LoadTexture("Backgrounds/" + BackgroundId + "_foreground");
            }
            catch (ContentLoadException)
            {

            }

            // Makes three of each (except background).
            List<Vector2> one = new List<Vector2>();
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(-Main.DefaultResWidth + (Main.DefaultResWidth * i), 0);
                one.Add(pos);
            }
            middleCoords = one.ToArray();
            foreCoords = one.ToArray();

            // Changes the last Id so that it does not continue loading.
            _lastBackgroundId = BackgroundId;
        }

        /// <summary>
        /// Moves the backgrounds and sets their position, and also checks if the background changed.
        /// </summary>
        /// <param name="camera"></param>
        public void Update(Camera camera)
        {
            // Checks if background changes.
            BackgroundId = GameWorld.WorldData.BackgroundId;

            if (_lastBackgroundId != BackgroundId)
            {
                Load();
            }


            middleCoords[0] = new Vector2((camera.LastCameraLeftCorner.X / 10) % Main.DefaultResWidth, 0);
            foreCoords[0] = new Vector2((camera.LastCameraLeftCorner.X / 5) % Main.DefaultResWidth, 0);

            middleCoords[1] = middleCoords[0] + new Vector2(Main.DefaultResWidth, 0);
            foreCoords[1] = foreCoords[0] + new Vector2(Main.DefaultResWidth, 0);

            middleCoords[2] = middleCoords[0] - new Vector2(Main.DefaultResWidth, 0);
            foreCoords[2] = foreCoords[0] - new Vector2(Main.DefaultResWidth, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);

            spriteBatch.Draw(_skyFog, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White * FogGradient);

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(_middlegroundTexture, new Rectangle((int)middleCoords[i].X, (int)middleCoords[i].Y, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);
            }

            spriteBatch.Draw(_skyFog, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White * FogGradient);

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(_foregroundTexture, new Rectangle((int)foreCoords[i].X, (int)foreCoords[i].Y, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);
            }

            spriteBatch.Draw(_skyFog, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White * FogGradient);
        }
    }

    struct Image
    {
        public Rectangle Rectangle { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Texture2D Texture { get; set; }
    }
}
