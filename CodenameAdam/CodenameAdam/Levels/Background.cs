using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CodenameAdam
{
    class Background
    {
        BackGroundImage background;
        BackGroundImage[] middlegrounds = new BackGroundImage[3];
        BackGroundImage[] foregrounds = new BackGroundImage[6];

        Vector2 fPos, mPos;

        public void Load(ContentManager Content, Level CurrentLevel, Vector2 resolution, Map map)
        {

            switch (CurrentLevel)
            {
                case Level.Level1:
                    for (int i = 0; i < middlegrounds.Length; i++)
                    {
                        middlegrounds[i].Texture = Content.Load<Texture2D>("Backgrounds/eden_middleground");
                    }

                    for (int i = 0; i < foregrounds.Length; i++)
                    {
                        foregrounds[i].Texture = Content.Load<Texture2D>("Backgrounds/eden_foreground");
                    }

                    background.Texture = Content.Load<Texture2D>("Backgrounds/eden_background");
                    break;
                case Level.Level2:                   
                    for (int i = 0; i < middlegrounds.Length; i++)
                    {
                        middlegrounds[i].Texture = Content.Load<Texture2D>("Backgrounds/mesa_middleground");
                    }

                    for (int i = 0; i < foregrounds.Length; i++)
                    {
                        foregrounds[i].Texture = Content.Load<Texture2D>("Backgrounds/mesa_foreground");
                    }

                    background.Texture = Content.Load<Texture2D>("Backgrounds/mesa_background");
                    break;
                case Level.Level3:
                    for (int i = 0; i < middlegrounds.Length; i++)
                    {
                        middlegrounds[i].Texture = Content.Load<Texture2D>("Backgrounds/mesa_middleground");
                    }

                    for (int i = 0; i < foregrounds.Length; i++)
                    {
                        foregrounds[i].Texture = Content.Load<Texture2D>("Backgrounds/mesa_foreground");
                    }

                    background.Texture = Content.Load<Texture2D>("Backgrounds/mesa_background");
                    break;
                case Level.Level4:
                    background.Texture = Content.Load<Texture2D>("backgroundTest");
                    break;

            }

            background.Rectangle = new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight);
            for (int i = 0; i < middlegrounds.Length; i++)
            {
                middlegrounds[i].Rectangle = new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight);
            }
            for (int i = 0; i < foregrounds.Length; i++)
            {
                foregrounds[i].Rectangle = new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight);
            }
        }


        public void Update(Camera camera)
        {
            middlegrounds[0].Rectangle = new Rectangle((int)(camera.lastCameraLeftCorner.X / 10), middlegrounds[0].Rectangle.Y, middlegrounds[0].Rectangle.Width, middlegrounds[0].Rectangle.Height);

            for (int i = 1; i < middlegrounds.Length; i++)
            {
                middlegrounds[i].Rectangle = new Rectangle(middlegrounds[i - 1].Rectangle.X + (middlegrounds[i - 1].Rectangle.Width * i), middlegrounds[1].Rectangle.Y, middlegrounds[1].Rectangle.Width, middlegrounds[1].Rectangle.Height);
            }

            foregrounds[0].Rectangle =  new Rectangle((int)(camera.lastCameraLeftCorner.X / 5), foregrounds[0].Rectangle.Y, foregrounds[0].Rectangle.Width, foregrounds[0].Rectangle.Height);

            for (int i = 1; i < foregrounds.Length; i++)
            {
                foregrounds[i].Rectangle = new Rectangle(foregrounds[i - 1].Rectangle.X + (foregrounds[i - 1].Rectangle.Width * i), foregrounds[1].Rectangle.Y, foregrounds[1].Rectangle.Width, foregrounds[1].Rectangle.Height);
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

    struct BackGroundImage
    {
        public Rectangle Rectangle { get; set; }
        public Texture2D Texture { get; set; }
    }
}
