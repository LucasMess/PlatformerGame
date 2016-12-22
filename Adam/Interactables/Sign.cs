using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            CollRectangle = new Rectangle(xCoor, yCoor, AdamGame.Tilesize, AdamGame.Tilesize);
            this._id = id;
        }

        public override void Update()
        {
            _key.Update(CollRectangle);
            if (GameWorld.Player.GetCollRectangle().Intersects(CollRectangle))
            {
                if (InputHelper.IsKeyDown(Keys.W))
                {
                    ShowMessage();
                }
            }
        }

        private void ShowMessage()
        {
           // Main.Dialog.Show(GameWorld.worldData.GetSignMessage(ID));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _key.Draw(spriteBatch);
        }

    }
}
