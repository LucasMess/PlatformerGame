﻿using Adam.Misc.Interfaces;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    public class Sign : Entity
    {
        KeyPopUp key;
        int ID;
        bool playerIsOn;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public Sign(int xCoor, int yCoor, int ID)
        {
            key = new KeyPopUp();
            collRectangle = new Rectangle(xCoor, yCoor, Main.Tilesize, Main.Tilesize);
            this.ID = ID;
        }

        public override void Update()
        {
            key.Update(collRectangle);
            if (GameWorld.Instance.player.GetCollRectangle().Intersects(collRectangle))
            {
                if (InputHelper.IsKeyDown(Keys.W) && GameWorld.Instance.player.manual_hasControl)
                {
                    ShowMessage();
                }
            }
        }

        private void ShowMessage()
        {
           // Main.Dialog.Show(GameWorld.Instance.worldData.GetSignMessage(ID));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            key.Draw(spriteBatch);
        }

    }
}
