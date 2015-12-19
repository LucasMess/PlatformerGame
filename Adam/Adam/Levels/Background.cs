using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Adam
{


    public class Background
    {
        Image _background;
        Image[] _middlegrounds = new Image[6];
        Image[] _foregrounds = new Image[12];
        public int BackgroundId = 1;
        int _lastBackgroundId;

        public void Load()
        {
            BackgroundId = GameWorld.Instance.WorldData.BackgroundId;

            try {

                for (int i = 0; i < _middlegrounds.Length; i++)
                {
                    _middlegrounds[i].Texture = ContentHelper.LoadTexture("Backgrounds/" + BackgroundId + "_middleground");
                }

                for (int i = 0; i < _foregrounds.Length; i++)
                {
                    _foregrounds[i].Texture = ContentHelper.LoadTexture("Backgrounds/" + BackgroundId + "_foreground");
                }

                _background.Texture = ContentHelper.LoadTexture("Backgrounds/" + BackgroundId + "_background");
            }
            catch (ContentLoadException)
            {
                _lastBackgroundId = BackgroundId;
                return;
            }


            _background.Rectangle = new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight);
            for (int i = 0; i < _middlegrounds.Length; i++)
            {
                _middlegrounds[i].Rectangle = new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight);
            }
            for (int i = 0; i < _foregrounds.Length; i++)
            {
                _foregrounds[i].Rectangle = new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight);
            }

            _lastBackgroundId = BackgroundId;
        }


        public void Update(Camera camera)
        {
            BackgroundId = GameWorld.Instance.WorldData.BackgroundId;

            if (_lastBackgroundId != BackgroundId)
            {
                Load();
            }


            _middlegrounds[0].Rectangle = new Rectangle((int)(camera.LastCameraLeftCorner.X / 10), _middlegrounds[0].Rectangle.Y, _middlegrounds[0].Rectangle.Width, _middlegrounds[0].Rectangle.Height);

            for (int i = 1; i < _middlegrounds.Length; i++)
            {
                _middlegrounds[i].Rectangle = new Rectangle(_middlegrounds[i - 1].Rectangle.X + (_middlegrounds[i - 1].Rectangle.Width), _middlegrounds[i - 1].Rectangle.Y, _middlegrounds[i - 1].Rectangle.Width, _middlegrounds[i - 1].Rectangle.Height);
            }

            _foregrounds[0].Rectangle = new Rectangle((int)(camera.LastCameraLeftCorner.X / 5), _foregrounds[0].Rectangle.Y, _foregrounds[0].Rectangle.Width, _foregrounds[0].Rectangle.Height);

            for (int i = 1; i < _foregrounds.Length; i++)
            {
                _foregrounds[i].Rectangle = new Rectangle(_foregrounds[i - 1].Rectangle.X + (_foregrounds[i - 1].Rectangle.Width), _foregrounds[i - 1].Rectangle.Y, _foregrounds[i - 1].Rectangle.Width, _foregrounds[i - 1].Rectangle.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_background.Texture == null)
                return;

            spriteBatch.Draw(_background.Texture, _background.Rectangle, Color.White);

            for (int i = 0; i < _middlegrounds.Length; i++)
            {
                spriteBatch.Draw(_middlegrounds[i].Texture, _middlegrounds[i].Rectangle, Color.White);
            }

            for (int i = 0; i < _foregrounds.Length; i++)
            {
                spriteBatch.Draw(_foregrounds[i].Texture, _foregrounds[i].Rectangle, Color.White);
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
