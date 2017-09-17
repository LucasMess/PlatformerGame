using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.PlayerCharacter;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.Interactables
{
    class PlayerDetector : Interactable
    {
        Rectangle collRectangle;
        Container container;
        Color color = Color.White;
        const int size = 5;

        public PlayerDetector(Tile tile)
        {
            int x = tile.DrawRectangle.X;
            int y = tile.DrawRectangle.Y;
            x -= size / 2 * TMBAW_Game.Tilesize;
            y -= size / 2 * TMBAW_Game.Tilesize;
            collRectangle = new Rectangle(x, y, size * TMBAW_Game.Tilesize, size * TMBAW_Game.Tilesize);
            container = new Container(collRectangle.Width, collRectangle.Height);
            container.SetPosition(new Vector2(collRectangle.X, collRectangle.Y));

            CanBeLinkedToOtherInteractables = true;
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
            {
                if (player.GetCollRectangle().Intersects(collRectangle))
                {
                    color = Color.Red;
                    OnPlayerAction(tile, player);
                }
                else color = Color.Green;
            }

            base.Update(tile);
        }

        public override void Draw(SpriteBatch spriteBatch, Tile tile)
        {
            container.SetColor(color * .5f);
            container.Draw(spriteBatch);

            base.Draw(spriteBatch, tile);
        }
    }
}
