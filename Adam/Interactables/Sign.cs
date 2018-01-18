using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ThereMustBeAnotherWay.PlayerCharacter;

namespace ThereMustBeAnotherWay.Interactables
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
            CollRectangle = new Rectangle(xCoor, yCoor, TMBAW_Game.Tilesize, TMBAW_Game.Tilesize);
            this._id = id;
        }

        public override void Update()
        {
            foreach (Player player in GameWorld.GetPlayers())
                if (player.GetCollRectangle().Intersects(CollRectangle))
                {
                    if (InputSystem.IsKeyDown(Keys.W))
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
