using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.PlayerCharacter;

namespace Adam
{
    public class Tech
    {
        public Rectangle Rectangle;
        Rectangle _radius;
        private GameTime _gameTime;
        Animation _animation;
        SoundEffect _glow;
        double _glowTimer;

        public Texture2D Texture; 
        public int RectangleX
        {
            get { return Rectangle.X; }
            set { Rectangle.X = value; }
        }
        public int RectangleY
        {
            get { return Rectangle.Y; }
            set { Rectangle.Y = value; }
        }
        public bool ToDelete { get; set; }

        int _tileSize;

        public Tech(int x, int y, ContentManager content)
        {
            Texture = ContentHelper.LoadTexture("Objects/gold_apple");
            _glow = content.Load<SoundEffect>("Sounds/apple_glow");
            _tileSize = Main.Tilesize;
            Rectangle = new Rectangle(x, y, _tileSize, _tileSize);
            _animation = new Animation(Texture, Rectangle, 50, 3000, AnimationType.PlayInIntervals);
            _radius = new Rectangle(x - 1000, y - 1000, 2000, 2000);
        }

        public void Update(GameTime gameTime, Player player, PopUp popUp)
        {
            this._gameTime = gameTime;
            _animation.Update(gameTime);
            PickedUp(player, popUp);

            _glowTimer+= gameTime.ElapsedGameTime.Milliseconds;
            if (_radius.Intersects(player.GetCollRectangle()))
            {
                if (_glowTimer > 4000)
                {
                    _glow.Play();
                    _glowTimer = 0;
                }
            }

        }

        public void PickedUp(Player player, PopUp popUp)
        {
            if (player.GetCollRectangle().Intersects(Rectangle) && !ToDelete)
            {
                ToDelete = true;
                popUp.IsVisible = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch);
        }
    }
}
