using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Non_Playable
{
    class NonPlayableCharacter : Entity
    {
        bool destinationFound;
        bool toTheRight;
        double destinationTimer;
        int destinationX;
        GameTime gameTime;
        Player player;
        protected bool canTalk;
        protected bool isTalking;

        Image keyPopUp;
        bool keyIsVisible;

        public NonPlayableCharacter()
        {
            texture = Game1.DefaultTexture;

            keyPopUp.Texture = ContentHelper.LoadTexture("Menu/Keys/'W' Key");
        }

        public virtual void Update(GameTime gameTime, Player player)
        {
            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            this.player = player;
            this.gameTime = gameTime;
            base.Update();

            if (canTalk)
                CheckForPlayer();
        }

        private void CheckForPlayer()
        {
            if (collRectangle.Intersects(player.collRectangle))
            {
                keyIsVisible = true;
                if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) && !isTalking)
                {
                    isTalking = true;
                    ShowMessage();
                }
            }
            else keyIsVisible = false;
        }

        protected virtual void ShowMessage()
        {

        }

        protected void WalkAroundSpawnPoint(int spawnX)
        {
            int speed = 1;

            if (!destinationFound)
            {
                velocity.X = 0;
                destinationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (destinationTimer > 2)
                {
                    destinationTimer = 0;
                    destinationX = GameWorld.RandGen.Next(-3, 4);
                    destinationX *= Game1.Tilesize;
                    destinationX += spawnX;
                    destinationFound = true;

                    if (destinationX > collRectangle.X)
                    {
                        toTheRight = true;
                    }
                    else toTheRight = false;
                }
            }

            if (destinationFound)
            {
                if (toTheRight)
                {
                    velocity.X = speed;
                    if (collRectangle.X > destinationX)
                    {
                        destinationFound = false;
                    }
                }
                else
                {
                    velocity.X = -speed;
                    if (collRectangle.X < destinationX)
                    {
                        destinationFound = false;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (keyIsVisible)
                spriteBatch.Draw(keyPopUp.Texture, new Rectangle(collRectangle.X, collRectangle.Y - 40, 32, 32), Color.White);
        }
    }
}
