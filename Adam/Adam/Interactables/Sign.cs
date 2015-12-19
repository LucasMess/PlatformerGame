using Adam.Misc.Interfaces;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Interactables
{
    public class Sign : Entity
    {
        KeyPopUp _key;
        int _id;
        bool _playerIsOn;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public Sign(int xCoor, int yCoor, int id)
        {
            _key = new KeyPopUp();
            CollRectangle = new Rectangle(xCoor, yCoor, Main.Tilesize, Main.Tilesize);
            this._id = id;
        }

        public override void Update()
        {
            _key.Update(CollRectangle);
            if (GameWorld.Instance.Player.GetCollRectangle().Intersects(CollRectangle))
            {
                if (InputHelper.IsKeyDown(Keys.W))
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
            _key.Draw(spriteBatch);
        }

    }
}
