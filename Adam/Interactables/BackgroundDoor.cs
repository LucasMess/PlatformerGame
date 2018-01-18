using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThereMustBeAnotherWay.UI.Elements;

namespace ThereMustBeAnotherWay.Interactables
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
            _collRectangle = new Rectangle(tile.DrawRectangle.X, tile.DrawRectangle.Y, TMBAW_Game.Tilesize * 3, TMBAW_Game.Tilesize * 4);
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
            foreach (Player player in GameWorld.GetPlayers())
                if (player.GetCollRectangle().Intersects(_collRectangle))
                {
                    KeyPopUp.Show("W", player.GetCollRectangle());
                    if (player.IsInteractPressed())
                    {
                        OnPlayerAction(tile, player);
                    }
                }
            base.Update(tile);
        }


        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (!IsConnectedToAnotherInteractable())
            {
                TMBAW_Game.Dialog.Say("It's locked.", null, null);
            }
            base.OnPlayerAction(tile, player);
        }
    }
}
