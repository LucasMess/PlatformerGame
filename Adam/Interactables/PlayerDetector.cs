using Adam.Levels;
using Adam.PlayerCharacter;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Interactables
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
            x -= size / 2 * AdamGame.Tilesize;
            y -= size / 2 * AdamGame.Tilesize;
            collRectangle = new Rectangle(x, y, size * AdamGame.Tilesize, size * AdamGame.Tilesize);
            container = new Container(collRectangle.Width, collRectangle.Height);
            container.SetPosition(new Vector2(collRectangle.X, collRectangle.Y));

            CanBeLinkedToOtherInteractables = true;
        }

        protected override void OnConnectionToInteractable(Tile source, Tile other)
        {
            GameWorld.WorldData.MetaData[source.TileIndex] = "activate:" + other.TileIndex;

            base.OnConnectionToInteractable(source, other);
        }

        public override void Update(Tile tile)
        {
            Player player = GameWorld.GetPlayer();
            if (player.GetCollRectangle().Intersects(collRectangle))
            {
                color = Color.Red;
                OnPlayerAction(tile);
            }
            else color = Color.Green;

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
