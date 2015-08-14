using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class Tech
    {
        public Rectangle rectangle;
        Rectangle radius;
        private GameTime gameTime;
        Animation animation;
        SoundEffect glow;
        double glowTimer;

        public Texture2D texture; 
        public int RectangleX
        {
            get { return rectangle.X; }
            set { rectangle.X = value; }
        }
        public int RectangleY
        {
            get { return rectangle.Y; }
            set { rectangle.Y = value; }
        }
        public bool ToDelete { get; set; }

        int tileSize;

        public Tech(int x, int y, ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Objects/gold_apple");
            glow = Content.Load<SoundEffect>("Sounds/apple_glow");
            tileSize = Main.Tilesize;
            rectangle = new Rectangle(x, y, tileSize, tileSize);
            animation = new Animation(texture, rectangle, 50, 3000, AnimationType.PlayInIntervals);
            radius = new Rectangle(x - 1000, y - 1000, 2000, 2000);
        }

        public void Update(GameTime gameTime, Player player, PopUp popUp)
        {
            this.gameTime = gameTime;
            animation.Update(gameTime);
            PickedUp(player, popUp);

            glowTimer+= gameTime.ElapsedGameTime.Milliseconds;
            if (radius.Intersects(player.collRectangle))
            {
                if (glowTimer > 4000)
                {
                    glow.Play();
                    glowTimer = 0;
                }
            }

        }

        public void PickedUp(Player player, PopUp popUp)
        {
            if (player.collRectangle.Intersects(rectangle) && !ToDelete)
            {
                ToDelete = true;
                popUp.isVisible = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }
}
