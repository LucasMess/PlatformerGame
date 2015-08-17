using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Adam
{


    public class Background
    {
        Image background;
        Image[] middlegrounds = new Image[6];
        Image[] foregrounds = new Image[12];
        public int backgroundID = 1;
        int lastBackgroundID;

        public void Load()
        {
            backgroundID = GameWorld.Instance.worldData.BackgroundID;

            for (int i = 0; i < middlegrounds.Length; i++)
            {
                middlegrounds[i].Texture = ContentHelper.LoadTexture("Backgrounds/" + backgroundID + "_middleground");
            }

            for (int i = 0; i < foregrounds.Length; i++)
            {
                foregrounds[i].Texture = ContentHelper.LoadTexture("Backgrounds/" + backgroundID + "_foreground");
            }

            background.Texture = ContentHelper.LoadTexture("Backgrounds/" + backgroundID + "_background");


            background.Rectangle = new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight);
            for (int i = 0; i < middlegrounds.Length; i++)
            {
                middlegrounds[i].Rectangle = new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight);
            }
            for (int i = 0; i < foregrounds.Length; i++)
            {
                foregrounds[i].Rectangle = new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight);
            }

            lastBackgroundID = backgroundID;
        }


        public void Update(Camera camera)
        {
            backgroundID = GameWorld.Instance.worldData.BackgroundID;

            if (lastBackgroundID != backgroundID)
            {
                Load();
            }


            middlegrounds[0].Rectangle = new Rectangle((int)(camera.lastCameraLeftCorner.X / 10), middlegrounds[0].Rectangle.Y, middlegrounds[0].Rectangle.Width, middlegrounds[0].Rectangle.Height);

            for (int i = 1; i < middlegrounds.Length; i++)
            {
                middlegrounds[i].Rectangle = new Rectangle(middlegrounds[i - 1].Rectangle.X + (middlegrounds[i - 1].Rectangle.Width), middlegrounds[i - 1].Rectangle.Y, middlegrounds[i - 1].Rectangle.Width, middlegrounds[i - 1].Rectangle.Height);
            }

            foregrounds[0].Rectangle = new Rectangle((int)(camera.lastCameraLeftCorner.X / 5), foregrounds[0].Rectangle.Y, foregrounds[0].Rectangle.Width, foregrounds[0].Rectangle.Height);

            for (int i = 1; i < foregrounds.Length; i++)
            {
                foregrounds[i].Rectangle = new Rectangle(foregrounds[i - 1].Rectangle.X + (foregrounds[i - 1].Rectangle.Width), foregrounds[i - 1].Rectangle.Y, foregrounds[i - 1].Rectangle.Width, foregrounds[i - 1].Rectangle.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background.Texture, background.Rectangle, Color.White);

            for (int i = 0; i < middlegrounds.Length; i++)
            {
                spriteBatch.Draw(middlegrounds[i].Texture, middlegrounds[i].Rectangle, Color.White);
            }

            for (int i = 0; i < foregrounds.Length; i++)
            {
                spriteBatch.Draw(foregrounds[i].Texture, foregrounds[i].Rectangle, Color.White);
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
