using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Adam;

namespace Adam
{
    class Chest
    {
        Texture2D texture;
        public Rectangle rectangle, sourceRectangle;
        bool isOpen;
        SoundEffect chestOpen;

        Vector2 frameCounter;
        int switchframe, currentFrame;
        double frameTimer;
        bool animationStopped = true;
        public bool IsGolden { get; set; }

        public Chest(Vector2 position, ContentManager Content, bool golden)
        {
            texture = Content.Load<Texture2D>("Objects/chest_updated");
            rectangle = new Rectangle((int)position.X + 8, (int)position.Y, 48, 32);
            sourceRectangle = new Rectangle(0, 0, 48, 32);
            frameCounter = new Vector2(5, 0);
            chestOpen = Content.Load<SoundEffect>("Sounds/ChestOpenDB");

            if (golden)
            {
                IsGolden = true;
                texture = ContentHelper.LoadTexture("Objects/golden_chest");
            }
        }

        public void Animate(GameTime gameTime)
        {
            if (!animationStopped && currentFrame <frameCounter.X)
            {
                switchframe = 20;
                frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (frameTimer > switchframe)
                {
                    sourceRectangle.X += sourceRectangle.Width;
                    currentFrame++;
                    frameTimer = 0;
                }

                
            }
        }

        public bool CheckOpened(GameTime gameTime, Player player)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && rectangle.Intersects(player.collRectangle) && isOpen == false)
            {
                Open();
                return true;
            }
            else return false;
        }

        void Open()
        {
            isOpen = true;
            animationStopped = false;
            chestOpen.Play();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle,sourceRectangle, Color.White);
        }
    }
}
