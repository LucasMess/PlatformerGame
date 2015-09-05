using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Adam.UI
{
    /// <summary>
    /// This creates a minimap of the level for the level editor.
    /// </summary>
    class Minimap
    {
        Color[] pixels;
        Texture2D texture, antiTexture, temp;
        Thread thread;
        Rectangle rectangle;
        bool isAnti;

        public Minimap()
        {
            temp = Main.DefaultTexture;
            texture = new Texture2D(Main.GraphicsDeviceInstance, GameWorld.Instance.worldData.LevelWidth, GameWorld.Instance.worldData.LevelHeight);
            antiTexture = new Texture2D(Main.GraphicsDeviceInstance, GameWorld.Instance.worldData.LevelWidth, GameWorld.Instance.worldData.LevelHeight);
            pixels = new Color[texture.Width * texture.Height];
            rectangle = new Rectangle(Main.UserResWidth - texture.Width, Main.UserResHeight - texture.Height, texture.Width, texture.Height);
        }

        /// <summary>
        /// Initializes the thread that will be updating the minimap in the background.
        /// </summary>
        public void StartUpdating()
        {
            thread = new Thread(new ThreadStart(Update));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Updates the minimap. This is a background thread method.
        /// </summary>
        private void Update()
        {
            if (Main.IsLoadingContent)
                return;

            bool isGoing = true;
            while (isGoing)
            {
                Tile[] tileArray = GameWorld.Instance.tileArray;
                Tile[] wallArray = GameWorld.Instance.wallArray;

                for (int i = 0; i < tileArray.Length; i++)
                {
                    pixels[i] = new Color(0, 0, 0, 50);
                    if (tileArray[i].ID != 0 && tileArray[i].ID < 200)
                    {
                        pixels[i] = Color.ForestGreen;
                    }
                    else if (wallArray[i].ID != 0)
                    {
                        pixels[i] = Color.DarkGreen;
                    }
                }

                if (!isAnti)
                {
                    try
                    {
                        texture?.SetData(pixels);
                    }
                    catch
                    {
                        isGoing = false;
                    }
                }
                else
                {
                    try
                    {
                        antiTexture?.SetData(pixels);
                    }
                    catch
                    {
                        isGoing = false;
                    }
                }
                isAnti = !isAnti;

                Thread.Sleep(1000);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAnti)
                spriteBatch.Draw(texture, rectangle, Color.White);
            else
            {
                spriteBatch.Draw(antiTexture, rectangle, Color.White);
            }
        }
    }
}
