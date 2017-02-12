using Adam.Interactables;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace Adam
{
    public class Door : Interactable
    {
        bool isOpen = false;
        bool isLocked = false;
        Rectangle rectangle;

        public override void Update(Tile tile)
        {

            base.Update(tile);
        }

        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (isLocked)
            {

            }
            isOpen = true;

            base.OnPlayerAction(tile, player);
        }
    }
}
