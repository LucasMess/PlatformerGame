using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Non_Playable
{
    public abstract class NonPlayableCharacter : Entity
    {
        bool destinationFound;
        bool toTheRight;
        double destinationTimer;
        int destinationX;
        GameTime gameTime;
        Player player;
        protected bool canTalk;
        protected bool isTalking;

        KeyPopUp key;
        public NonPlayableCharacter()
        {
            Texture = Main.DefaultTexture;

            key = new KeyPopUp();
        }

        public virtual void Update(GameTime gameTime, Player player)
        {
            key.Update(collRectangle);

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
            if (collRectangle.Intersects(player.GetCollRectangle()))
            {
                if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) && !isTalking)
                {
                    isTalking = true;
                    ShowMessage();
                }
            }
        }

        protected virtual void ShowMessage()
        {

        }

        protected void WalkAroundSpawnPoint(int spawnX)
        {
            int speed = 1;
            if (isTalking)
            {
                velocity.X = 0;
                return;
            }
            if (!destinationFound)
            {
                velocity.X = 0;
                destinationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (destinationTimer > 2)
                {
                    destinationTimer = 0;
                    destinationX = GameWorld.RandGen.Next(-3, 4);
                    destinationX *= Main.Tilesize;
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
            if (!isTalking)
                key.Draw(spriteBatch);
        }
    }
}
