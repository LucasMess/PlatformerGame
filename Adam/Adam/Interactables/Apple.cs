using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Adam
{
    class Apple
    {
        Texture2D texture;
        Rectangle rectangle;
        SoundEffect levelFinishedSound;
        bool wasPicked;
        double winTimer;
        Animation animation;

        public Apple(int x, int y)
        {
            rectangle = new Rectangle(x, y, 0, 0);
            Load(Game1.Content);
        }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Objects/gold_apple");
            levelFinishedSound = Content.Load<SoundEffect>("Sounds/LevelFinished");
            rectangle.Width = 32;
            rectangle.Height = 32;
            animation = new Animation(texture, rectangle, 200, 100, AnimationType.PlayInIntervals);
        }

        public void Update(Player player, GameTime gameTime, Map map)
        {
            if (player.collRectangle.Intersects(rectangle))
            {
                PlaySound();
                wasPicked = true;
                player.hasControl = false;
            }
            if (wasPicked)
            {
                winTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (winTimer > 2000)
                {
                    player.returnToMainMenu = true;
                }
            }

            animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }

        public void PlaySound()
        {
            if (wasPicked == false)
            {
                levelFinishedSound.Play();
            }
        }
    }
}
