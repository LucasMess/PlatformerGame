using Adam.Misc;
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
    public class TileScroll
    {
        bool isActive;
        Image blackSelectionBox;
        int maxHeight = Game1.UserResHeight;
        List<Tile> tiles = new List<Tile>();
        List<TileName> names = new List<TileName>();
        SpriteFont font;
        SoundFx scrollSound;
        int lastHitTileOnScroll;
        float velocityY;
        float velocityX;
        int lastScrollWheel;

        public delegate void TileSelectedHandler(TileSelectedArgs e);

        public event TileSelectedHandler TileSelected;

        public TileScroll()
        {
            font = ContentHelper.LoadFont("Fonts/objectiveText");
            scrollSound = new SoundFx("Sounds/Level Editor/scroll");
            blackSelectionBox.Rectangle = new Rectangle(0, 0, (int)(180 / Game1.WidthRatio), (int)(Game1.DefaultResHeight / Game1.HeightRatio));
        }

        public void Load()
        {
            tiles = new List<Tile>();
            names = new List<TileName>();

            foreach (byte ID in CurrentAvailableTileSet)
            {
                Tile tile = new Tile();
                tile.ID = ID;
                tile.DefineTexture();

                tile.drawRectangle = new Rectangle(0, (int)((Game1.Tilesize / Game1.HeightRatio) * tiles.Count), (int)(Game1.Tilesize / Game1.HeightRatio), (int)(Game1.Tilesize / Game1.HeightRatio));
                tiles.Add(tile);

                TileName tileName = new TileName();
                Tile.TileNames.TryGetValue(tile.ID, out tileName.Name);
                if (tileName.Name == null) tileName.Name = "*";
                tileName.Position = new Vector2((float)(Game1.Tilesize / Game1.WidthRatio) + 5, tile.drawRectangle.Center.Y - font.LineSpacing / 2);
                names.Add(tileName);
            }
        }

        public void Update()
        {
            CheckIfActive();
            CheckIfScrolling();
            CheckIfTileIsHovered();
        }

        private void CheckIfActive()
        {
            if (InputHelper.IsKeyDown(Keys.Tab))
            {
                isActive = true;
            }
            else isActive = false;

            blackSelectionBox.Rectangle = new Rectangle(tiles[0].drawRectangle.X, 0, blackSelectionBox.Rectangle.Width, blackSelectionBox.Rectangle.Height);

            if (isActive)
            {
                if (tiles[0].drawRectangle.X < 0)
                {
                    velocityX = -tiles[0].drawRectangle.X / 5;
                }
                if (tiles[0].drawRectangle.X > 0)
                {
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].drawRectangle.X = 0;
                    }
                    velocityX = 0;
                }

                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X += (int)velocityX;
                    names[i].Position.X = (float)(tiles[i].drawRectangle.X + Game1.Tilesize / Game1.WidthRatio) + 5;
                }
            }
            else
            {
                if (tiles[0].drawRectangle.X < -400)
                {
                    velocityX = 0;
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].drawRectangle.X = -400;
                    }
                }
                else
                {
                    velocityX -= .6f;
                }

                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X += (int)velocityX;
                    names[i].Position.X = (float)(tiles[i].drawRectangle.X + Game1.Tilesize / Game1.WidthRatio) + 5;
                }
            }
        }

        private void CheckIfScrolling()
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            if (InputHelper.MouseRectangleRenderTarget.Intersects(blackSelectionBox.Rectangle))
            {
                if (lastScrollWheel != scrollWheel)
                {
                    velocityY = (scrollWheel - lastScrollWheel) / 5;
                }
                lastScrollWheel = scrollWheel;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].drawRectangle.Y += (int)velocityY;
                names[i].Position.Y = tiles[i].drawRectangle.Center.Y - font.LineSpacing / 2;
            }

            velocityY = velocityY * .92f;

            //scroll sound
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile t = tiles[i];
                if (t.drawRectangle.Intersects(new Rectangle(0, 0, 32, 32)))
                {
                    if (i != lastHitTileOnScroll)
                    {
                        // scrollSound.Play();
                        lastHitTileOnScroll = i;
                    }
                }
            }
        }

        private void CheckIfTileIsHovered()
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UI_SpriteSheet, blackSelectionBox.Rectangle, new Rectangle(0, 96, 180, Game1.DefaultResHeight), Color.White * .5f);

            foreach (Tile t in tiles)
                t.DrawByForce(spriteBatch);

            foreach (TileName t in names)
            {
                FontHelper.DrawWithOutline(spriteBatch, font, t.Name, t.Position, 2, Color.White, Color.Black);
            }
        }

        private byte[] CurrentAvailableTileSet
        {
            get
            {
                if (GameWorld.Instance.levelEditor.onWallMode)
                    return AvailableWalls;
                else return AvailableTiles;
            }
        }

        static byte[] AvailableTiles = new byte[]
        {
            1,2,5,6,4,39,40,38,10,8,21,3,18,29,30,14,15,16,23,24,25,11,12,22,31,32,34,33,19,27
        };

        static byte[] AvailableWalls = new byte[]
        {
            100,101,102,103,104,105,106
        };
            
    }

    public class TileName
    {
        public string Name;
        public Vector2 Position;
    }

    public class TileSelectedArgs : EventArgs
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
