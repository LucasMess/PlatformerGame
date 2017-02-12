using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam.Interactables
{
    /// <summary>
    /// Used to teleport the player to different areas of the map or different levels.
    /// </summary>
    public class BackgroundDoor : Interactable
    {
        public BackgroundDoor()
        {

            CanBeLinkedByOtherInteractables = true;
            CanBeLinkedToOtherInteractables = false;
        }

        protected override void OnConnectionToInteractable(Tile source, Tile other)
        {
            string value = "activate:" + other.TileIndex;
            if (GameWorld.WorldData.MetaData.ContainsKey(source.TileIndex))
            {
                GameWorld.WorldData.MetaData[source.TileIndex] = value;
            }
            else
            {
                GameWorld.WorldData.MetaData.Add(source.TileIndex, value);
            }

            base.OnConnectionToInteractable(source, other);

            base.OnConnectionToInteractable(source, other);
        }



        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (!IsConnectedToAnotherInteractable())
            {
                AdamGame.MessageBox.Show("It's locked.");
            }
            base.OnPlayerAction(tile, player);
        }
    }
}
