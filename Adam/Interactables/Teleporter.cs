using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace Adam.Interactables
{
    class Teleporter : Interactable
    {
        private Vector2 teleportPosition;

        public Teleporter(Tile tile)
        {
            teleportPosition = new Vector2(tile.DrawRectangle.X, tile.DrawRectangle.Y);
            CanBeLinkedByOtherInteractables = true;
            CanBeLinkedToOtherInteractables = false;
        }

        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (AdamGame.CurrentGameMode == GameMode.Play)
            {
                player.SetPosition(teleportPosition);
                player.SetVelX(0);
                player.SetVelY(0);
            }
            base.OnPlayerAction(tile, player);
        }
    }
}
