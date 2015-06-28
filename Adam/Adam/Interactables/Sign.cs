using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    class Sign : Entity
    {
        Texture2D keyPopUp;
        int ID;
        bool playerIsOn;

        public Sign(int xCoor, int yCoor, int ID)
        {
            keyPopUp = ContentHelper.LoadTexture("Menu/Keys/'W' Key");
            collRectangle = new Rectangle(xCoor, yCoor, Game1.Tilesize, Game1.Tilesize);
            this.ID = ID;
        }

        public override void Update()
        {
            if (GameWorld.Instance.player.collRectangle.Intersects(collRectangle))
            {
                playerIsOn = true;
                if (InputHelper.IsKeyDown(Keys.W))
                {
                    ShowMessage();
                }
            }
            else playerIsOn = false;
        }

        private void ShowMessage()
        {
            Game1.Dialog.Say(GameWorld.Instance.worldData.GetSignMessage(ID));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (playerIsOn)
                spriteBatch.Draw(keyPopUp, new Rectangle(collRectangle.X, collRectangle.Y - 70, 32, 32), Color.White);
        }

    }
}
