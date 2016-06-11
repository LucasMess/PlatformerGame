using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Adam
{
    public class Cloud
    {
        public Texture2D Texture1, Texture2, Texture3, Texture4, CurrentTexture;
        Vector2 _velocity;
        Vector2 _position;
        public Rectangle Rectangle;
        int _maxClouds;
        int _distance;
        Random _randGen;

        Vector2 _prefRes;

        public Cloud(Vector2 monitorResolution, int maxClouds, int i)
        {
            Texture1 = ContentHelper.LoadTexture("Backgrounds/cloud_1");
            Texture2 = ContentHelper.LoadTexture("Backgrounds/cloud_2");
            Texture3 = ContentHelper.LoadTexture("Backgrounds/cloud_3");
            Texture4 = ContentHelper.LoadTexture("Backgrounds/cloud_4");
            this._maxClouds = maxClouds;
            _prefRes = monitorResolution;
            _randGen = new Random(i);
            _velocity = new Vector2(-.1f, 0);
            _distance = (int)monitorResolution.X * 2/ maxClouds;
            Create(i);
        }

        public void Create(int i)
        {
            Rectangle = new Rectangle(i * _distance, _randGen.Next(0, 200), Texture1.Width, Texture1.Height);
            _position = new Vector2(Rectangle.X, Rectangle.Y);

            switch (_randGen.Next(0, 3))
            {
                case 0:
                    CurrentTexture = Texture1;
                    break;
                case 1:
                    CurrentTexture = Texture2;
                    break;
                case 2:
                    CurrentTexture = Texture3;
                    break;
                case 3:
                    CurrentTexture = Texture4;
                    break;
            }

        }

        public void Update()
        {
            _position.X += _velocity.X;
            Rectangle.X = (int)_position.X;
        }

        public void CheckOutOfRange()
        {
            if (_position.X < -Texture1.Width)
            {
                _position.X = (int)_prefRes.X * 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(CurrentTexture, Rectangle, Color.White);
        }
    }
}
