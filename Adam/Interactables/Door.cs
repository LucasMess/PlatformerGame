using ThereMustBeAnotherWay.Interactables;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay
{
    public class Door : Interactable
    {
        public override void Update(Tile tile)
        {

            base.Update(tile);
        }

        public override void OnPlayerAction(Tile tile, Player player)
        {

            base.OnPlayerAction(tile, player);
        }
    }
}
