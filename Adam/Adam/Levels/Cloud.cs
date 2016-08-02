using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam
{
    public class Cloud
    {
        private readonly int _distance;
        private Vector2 _position;
        private Vector2 _prefRes;
        private Vector2 _velocity;
        public Rectangle Rectangle;
        public Texture2D Texture1, Texture2, Texture3, Texture4, CurrentTexture;
        private bool _flipH, _flipV;

        public Cloud(Vector2 monitorResolution, int maxClouds, int i)
        {
            Texture1 = ContentHelper.LoadTexture("Backgrounds/cloud_1");
            Texture2 = ContentHelper.LoadTexture("Backgrounds/cloud_2");
            Texture3 = ContentHelper.LoadTexture("Backgrounds/cloud_3");
            Texture4 = ContentHelper.LoadTexture("Backgrounds/cloud_4");
            _prefRes = monitorResolution;
            _velocity = new Vector2(-.03f, 0);
            _distance = (int)(monitorResolution.X * 2 / maxClouds * Main.Random.NextDouble());
            _flipH = Main.Random.Next(0, 2) == 0;
            _flipV = Main.Random.Next(0, 2) == 0;

            Create(i);
        }

        public void Create(int i)
        {
            Rectangle = new Rectangle(i * _distance, Main.Random.Next(0, 50), Texture1.Width, Texture1.Height);
            _position = new Vector2(Rectangle.X, Rectangle.Y);

            switch (Main.Random.Next(0, 3))
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
            SpriteEffects h  =SpriteEffects.None , v = SpriteEffects.None;
            if (_flipH) h = SpriteEffects.FlipHorizontally;
            if (_flipV) v = SpriteEffects.FlipVertically;

            spriteBatch.Draw(CurrentTexture, Rectangle, null, Color.White * .5f, 0, Vector2.Zero, h | v, 0);
            
        }
    }
}