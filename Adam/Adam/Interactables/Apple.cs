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
        Game1 game1;

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

        public void Update(Player player, GameTime gameTime, GameWorld map, Game1 game1)
        {
            this.game1 = game1;

            if (player.collRectangle.Intersects(rectangle))
            {
                PlaySound();
                wasPicked = true;
                player.manual_hasControl = false;
            }
            if (wasPicked)
            {
                winTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (winTimer > 2000)
                {
                    game1.ChangeState(GameState.MainMenu, Level.Level0);
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

        private Level GetNextLevel()
        {
            switch (game1.CurrentLevel)
            {
                case Level.Level0:
                    return Level.Level0;

                case Level.Level1and1:
                    return Level.Level2and1;
                case Level.Level1and2:
                    return Level.Level1and3;
                case Level.Level1and3:
                    return Level.Level2and1;
                    
                case Level.Level2and1:
                    break;
                case Level.Level3and1:
                    break;
                case Level.Level4and1:
                    break;
                default:
                    break;
            }

            return Level.Level0;
        }
    }
}
