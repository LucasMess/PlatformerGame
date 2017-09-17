using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ThereMustBeAnotherWay
{
    /// <summary>
    /// Class responsible for drawing the three layered backgrounds for each level.
    /// </summary>
    public class Background
    {
        private Texture2D _backgroundTexture;
        private Texture2D _middlegroundTexture;
        private Texture2D _foregroundTexture;
        private Texture2D _sun = ContentHelper.LoadTexture("Backgrounds/sun");
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
                TMBAW_Game.MessageBox.Show("Could not find background with id: " + BackgroundId);
            }

            // Makes three of each (except background).
            List<Vector2> one = new List<Vector2>();
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(-TMBAW_Game.DefaultResWidth + (TMBAW_Game.DefaultResWidth * i), 0);
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
        public void Update()
        {
            // Checks if background changes.
            BackgroundId = GameWorld.WorldData.BackgroundId;

            if (_lastBackgroundId != BackgroundId)
            {
                Load();
            }


            middleCoords[0] = new Vector2((TMBAW_Game.Camera.LastCameraLeftCorner.X / 10) % TMBAW_Game.DefaultResWidth, (TMBAW_Game.Camera.LastCameraLeftCorner.Y / (TMBAW_Game.Tilesize * GameWorld.WorldData.LevelHeight) * TMBAW_Game.DefaultResHeight * 1 / 4) + TMBAW_Game.DefaultResHeight * 1 / 4);

            foreCoords[0] = new Vector2((TMBAW_Game.Camera.LastCameraLeftCorner.X / 5) % TMBAW_Game.DefaultResWidth, (TMBAW_Game.Camera.LastCameraLeftCorner.Y / (TMBAW_Game.Tilesize * GameWorld.WorldData.LevelHeight) * TMBAW_Game.DefaultResHeight * 1 / 3) + TMBAW_Game.DefaultResHeight * 1 / 3);

            middleCoords[1] = middleCoords[0] + new Vector2(TMBAW_Game.DefaultResWidth, 0);
            foreCoords[1] = foreCoords[0] + new Vector2(TMBAW_Game.DefaultResWidth, 0);

            middleCoords[2] = middleCoords[0] - new Vector2(TMBAW_Game.DefaultResWidth, 0);
            foreCoords[2] = foreCoords[0] - new Vector2(TMBAW_Game.DefaultResWidth, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.WorldData.IsTopDown)
            {
                // Draw a blue background to hide holes between ground and water in top down view.
                spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), new Rectangle(0, 0, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), new Color(65,96,228));
            }
            else
            {
                if (_backgroundTexture != null)
                {

                    spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), Color.White);

                }

                if (GameWorld.WorldData.HasSun)
                    spriteBatch.Draw(_sun, new Rectangle(0, 0, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), Color.White);

                if (_middlegroundTexture != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        spriteBatch.Draw(_middlegroundTexture, new Rectangle((int)middleCoords[i].X, (int)middleCoords[i].Y, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), Color.White);
                    }
                }

                if (_foregroundTexture != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        spriteBatch.Draw(_foregroundTexture, new Rectangle((int)foreCoords[i].X, (int)foreCoords[i].Y, TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), Color.White);
                    }
                }
            }
        }
    }

    struct Image
    {
        public Rectangle Rectangle { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Texture2D Texture { get; set; }
    }
}
