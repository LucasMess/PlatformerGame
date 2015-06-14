using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CodenameAdam
{
    class Cloud
    {
        public Texture2D texture1, texture2, texture3, texture4, currentTexture;
        Vector2 velocity;
        Vector2 position;
        public Rectangle rectangle;
        int maxClouds;
        int distance;
        Random randGen;

        Vector2 PrefRes;

        public Cloud(ContentManager Content, Vector2 monitorResolution,int maxClouds, int i)
        {
            texture1 = Content.Load<Texture2D>("Backgrounds/cloud_1");
            texture2 = Content.Load<Texture2D>("Backgrounds/cloud_2");
            texture3 = Content.Load<Texture2D>("Backgrounds/cloud_3");
            texture4 = Content.Load<Texture2D>("Backgrounds/cloud_4");
            this.maxClouds = maxClouds;
            PrefRes = monitorResolution;
            randGen = new Random(i);
            velocity = new Vector2(-.1f, 0);
            distance = (int)monitorResolution.X * 2/ maxClouds;
            Create(i);
        }

        public void Create(int i)
        {
            rectangle = new Rectangle(i * distance, randGen.Next(0, 200), texture1.Width, texture1.Height);
            position = new Vector2(rectangle.X, rectangle.Y);

            switch (randGen.Next(0, 3))
            {
                case 0:
                    currentTexture = texture1;
                    break;
                case 1:
                    currentTexture = texture2;
                    break;
                case 2:
                    currentTexture = texture3;
                    break;
                case 3:
                    currentTexture = texture4;
                    break;
            }

        }

        public void Update(GameTime gameTime)
        {
            position.X += velocity.X;
            rectangle.X = (int)position.X;
        }

        public void CheckOutOfRange()
        {
            if (position.X < -texture1.Width)
            {
                position.X = (int)PrefRes.X * 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentTexture, rectangle, Color.White);
        }
    }
}
