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
        protected bool isActive;
        protected Rectangle box;
        protected int maxHeight = Main.UserResHeight;
        protected List<Tile> tiles = new List<Tile>();
        protected List<TileName> names = new List<TileName>();
        protected SpriteFont font;
        protected SoundFx scrollSound;
        protected int lastHitTileOnScroll;
        protected float velocityY;
        protected float velocityX;
        protected int lastScrollWheel;

        Rectangle mouseRectangle;

        protected int activeX;
        protected int inactiveX;

        protected const int Width = 180;

        public delegate void TileSelectedHandler(TileSelectedArgs e);

        public event TileSelectedHandler TileSelected;

        public TileScroll()
        {
            activeX = 0;
            inactiveX = -400;

            box = new Rectangle(0, 0, (int)(180 / Main.WidthRatio), (int)(Main.DefaultResHeight / Main.HeightRatio));
            Initialize();
        }

        protected void Initialize()
        {
            font = ContentHelper.LoadFont("Fonts/objectiveText");
            scrollSound = new SoundFx("Sounds/Level Editor/scroll");
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

                tile.drawRectangle = new Rectangle(activeX, (int)((Main.Tilesize / Main.HeightRatio) * tiles.Count), (int)(Main.Tilesize / Main.HeightRatio), (int)(Main.Tilesize / Main.HeightRatio));
                tiles.Add(tile);

                TileName tileName = new TileName();
                Tile.Names.TryGetValue(tile.ID, out tileName.Name);
                if (tileName.Name == null) tileName.Name = "*";
                tileName.Position = new Vector2((float)(activeX) + 5, tile.drawRectangle.Center.Y - font.LineSpacing / 2);
                names.Add(tileName);
            }

            Hide();
        }

        protected virtual void Hide()
        {
            //Start off screen
            foreach (Tile t in tiles)
            {
                t.drawRectangle.X -= 400;
            }
            box.X -= 400;
            foreach (TileName s in names)
            {
                s.Position.X -= 400;
            }
        }

        public void Update()
        {
            CheckIfActive();
            CheckIfScrolling();
            CheckIfTileIsHovered();
        }

        protected virtual void CheckIfActive()
        {
            if (GameWorld.Instance.levelEditor.onInventory)
            {
                isActive = true;
            }
            else isActive = false;

            box = new Rectangle(tiles[0].drawRectangle.X, 0, box.Width, box.Height);

            //Prevent super fast jumping
            if (tiles[0].drawRectangle.X > activeX)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X = activeX;
                }
                velocityX = 0;
            }


            if (isActive)
            {
                if (tiles[0].drawRectangle.X < activeX)
                {
                    velocityX = -tiles[0].drawRectangle.X / 5;
                }

                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X += (int)velocityX;
                    names[i].Position.X = (float)(tiles[i].drawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
            else
            {
                if (tiles[0].drawRectangle.X < inactiveX)
                {
                    velocityX = 0;
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].drawRectangle.X = inactiveX;
                    }
                }
                else
                {
                    velocityX -= .6f;
                }

                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X += (int)velocityX;
                    names[i].Position.X = (float)(tiles[i].drawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
        }

        private void CheckIfScrolling()
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;


            InputHelper.GetMouseRectRenderTarget(ref mouseRectangle);
            if (mouseRectangle.Intersects(box))
            {
                if (lastScrollWheel != scrollWheel)
                {
                    velocityY = (scrollWheel - lastScrollWheel) / 5;
                }

            }
            lastScrollWheel = scrollWheel;

            //Check Boundaries
            if (tiles[0].drawRectangle.Y > 0)
            {
                velocityY = -1;
            }
            if (tiles[tiles.Count - 1].drawRectangle.Y + tiles[tiles.Count - 1].drawRectangle.Height< Main.UserResHeight)
            {
                velocityY = 1;
            }

            if (tiles.Count < 16)
                return;

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
            spriteBatch.Draw(GameWorld.UI_SpriteSheet, box, new Rectangle(0, 96, 180, Main.DefaultResHeight), Color.White * .5f);

            foreach (Tile t in tiles)
                t.DrawByForce(spriteBatch);

            foreach (TileName t in names)
            {
                FontHelper.DrawWithOutline(spriteBatch, font, t.Name, t.Position, 2, Color.White, Color.Black);
            }
        }

        protected virtual byte[] CurrentAvailableTileSet
        {
            get
            {
                if (GameWorld.Instance.levelEditor.onWallMode)
                    return AvailableWalls;
                else return AvailableTiles;
            }
        }

        protected virtual byte[] AvailableTiles
        {
            get
            {
                byte[] IDs = new byte[]
                {
               1,2,5,6,4,39,40,38,10,41,57,8,21,3,18,29,30,14,15,16,23,24,25,26,37,11,12,22,31,32,34,33,19,42,43,45,46,47,48,49,50,51,52,53,54,                };
                return IDs;
            }
        }

        static byte[] AvailableWalls = new byte[]
        {
            100,101,102,103,104,105,108,106,107
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
