using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam.Interactables
{
    public class Sign : Entity
    {
        int _id;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public Sign(int xCoor, int yCoor, int id)
        {
            CollRectangle = new Rectangle(xCoor, yCoor, AdamGame.Tilesize, AdamGame.Tilesize);
            this._id = id;
        }

        public override void Update()
        {
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

    }
}
