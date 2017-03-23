using Adam.Levels;
using Adam.PlayerCharacter;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Interactables
{
    /// <summary>
    /// Used to teleport the player to different areas of the map or different levels.
    /// </summary>
    public class BackgroundDoor : Interactable
    {
        Rectangle _collRectangle;

        public BackgroundDoor(Tile tile)
        {
            CanBeLinkedToOtherInteractables = false;
            _collRectangle = new Rectangle(tile.DrawRectangle.X, tile.DrawRectangle.Y, AdamGame.Tilesize * 3, AdamGame.Tilesize * 4);
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
        }

        public override void Update(Tile tile)
        {
            if (GameWorld.GetPlayer().GetCollRectangle().Intersects(_collRectangle))
            {
                KeyPopUp.Show("W", GameWorld.GetPlayer().GetCollRectangle());
                if (GameWorld.GetPlayer().IsInteractPressed())
                {
                    OnPlayerAction(tile, GameWorld.GetPlayer());
                }
            }
            base.Update(tile);
        }


        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (!IsConnectedToAnotherInteractable())
            {
                AdamGame.Dialog.Say("It's locked.", null, null);
            }
            base.OnPlayerAction(tile, player);
        }
    }
}
