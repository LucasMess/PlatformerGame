using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    class TileScroll
    {
        int maxHeight = Game1.UserResHeight;
        byte[] availableIDs;
        List<Tile> tiles = new List<Tile>();
        List<TileName> names = new List<TileName>();
        SpriteFont font;
        int lastScrollWheel;

        public delegate void TileSelectedHandler(TileSelectedArgs e);

        public event TileSelectedHandler TileSelected;

        public TileScroll()
        {
            font = ContentHelper.LoadFont("Fonts/objectiveText");
        }

        public void Load()
        {
            availableIDs = new byte[50];
            for (byte i = 1; i < availableIDs.Length; i++)
            {
                availableIDs[i - 1] = i;
            }

            foreach (byte ID in availableIDs)
            {
                Tile tile = new Tile();
                tile.ID = ID;
                tile.DefineTexture();

                tile.drawRectangle = new Rectangle(0, (int)((Game1.Tilesize / Game1.HeightRatio) * tiles.Count), (int)(Game1.Tilesize / Game1.HeightRatio), (int)(Game1.Tilesize / Game1.HeightRatio));
                tiles.Add(tile);

                TileName tileName = new TileName();
                tileName.Name = tile.name;
                tileName.Position = new Vector2((float)(Game1.Tilesize / Game1.WidthRatio) + 5, tile.drawRectangle.Center.Y - font.LineSpacing / 2);
                names.Add(tileName);
            }
        }

        public void Update()
        {
            foreach (Tile t in tiles)
            {
                if (InputHelper.MouseRectangle.Intersects(t.drawRectangle))
                {
                    t.color = new Color(200, 200, 200);

                    if (InputHelper.IsLeftMousePressed())
                        TileSelected(new TileSelectedArgs(t.ID));

                }
                else t.color = Color.White;
            }

            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            if (scrollWheel < lastScrollWheel)
            {
                foreach (Tile t in tiles)
                    t.drawRectangle.Y += 50;

                foreach (TileName t in names)
                {
                    t.Position.Y += 50;
                }
            }
            if (scrollWheel > lastScrollWheel)
            {
                foreach (Tile t in tiles)
                    t.drawRectangle.Y -= 50;

                foreach (TileName t in names)
                {
                    t.Position.Y -= 50;
                }
            }
            lastScrollWheel = scrollWheel;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile t in tiles)
                t.Draw(spriteBatch);

            foreach (TileName t in names)
            {
                FontHelper.DrawWithOutline(spriteBatch, font, t.Name, t.Position, 2, Color.White, Color.Black);
            }
        }
    }

    class TileName
    {
        public string Name;
        public Vector2 Position;
    }

    class TileSelectedArgs : EventArgs
    {
        int id;
        public TileSelectedArgs(int ID)
        {
            this.id = ID;
        }

        public int ID
        {
            get
            {
                return id;
            }
        }
    }
}
