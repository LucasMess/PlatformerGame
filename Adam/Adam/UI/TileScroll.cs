using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.UI
{
    public class TileScroll
    {
        protected bool IsActive;
        protected Rectangle Box;
        protected int MaxHeight = Main.UserResHeight;
        protected List<Tile> Tiles = new List<Tile>();
        protected List<TileName> Names = new List<TileName>();
        protected SpriteFont Font;
        protected SoundFx ScrollSound;
        protected int LastHitTileOnScroll;
        protected float VelocityY;
        protected float VelocityX;
        protected int LastScrollWheel;

        Rectangle _mouseRectangle;

        protected int ActiveX;
        protected int InactiveX;

        protected const int Width = 180;

        public delegate void TileSelectedHandler(TileSelectedArgs e);

        public event TileSelectedHandler TileSelected;

        public TileScroll()
        {
            ActiveX = 0;
            InactiveX = -400;

            Box = new Rectangle(0, 0, (int)(180 / Main.WidthRatio), (int)(Main.DefaultResHeight / Main.HeightRatio));
            Initialize();
        }

        protected void Initialize()
        {
            Font = ContentHelper.LoadFont("Fonts/x32");
            ScrollSound = new SoundFx("Sounds/Level Editor/scroll");
        }

        public void Load()
        {
            Tiles = new List<Tile>();
            Names = new List<TileName>();

            foreach (byte id in CurrentAvailableTileSet)
            {
                Tile tile = new Tile(true);
                tile.Id = id;
                tile.DefineTexture();

                tile.DrawRectangle = new Rectangle(ActiveX, (int)((Main.Tilesize / Main.HeightRatio) * Tiles.Count), (int)(Main.Tilesize/ Main.HeightRatio), (int)(Main.Tilesize / Main.HeightRatio));
                Tiles.Add(tile);

                TileName tileName = new TileName();
                Tile.Names.TryGetValue(tile.Id, out tileName.Name);
                if (tileName.Name == null) tileName.Name = "*";
                tileName.Position = new Vector2((float)(ActiveX) + 5, tile.DrawRectangle.Center.Y - Font.LineSpacing / 2);
                Names.Add(tileName);
            }

            Hide();
        }

        protected virtual void Hide()
        {
            //Start off screen
            foreach (Tile t in Tiles)
            {
                t.DrawRectangle.X -= 400;
            }
            Box.X -= 400;
            foreach (TileName s in Names)
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
            if (GameWorld.Instance.LevelEditor.OnInventory)
            {
                IsActive = true;
            }
            else IsActive = false;

            Box = new Rectangle(Tiles[0].DrawRectangle.X, 0, Box.Width, Box.Height);

            //Prevent super fast jumping
            if (Tiles[0].DrawRectangle.X > ActiveX)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].DrawRectangle.X = ActiveX;
                }
                VelocityX = 0;
            }


            if (IsActive)
            {
                if (Tiles[0].DrawRectangle.X < ActiveX)
                {
                    VelocityX = -Tiles[0].DrawRectangle.X / 5;
                }

                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].DrawRectangle.X += (int)VelocityX;
                    Names[i].Position.X = (float)(Tiles[i].DrawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
            else
            {
                if (Tiles[0].DrawRectangle.X < InactiveX)
                {
                    VelocityX = 0;
                    for (int i = 0; i < Tiles.Count; i++)
                    {
                        Tiles[i].DrawRectangle.X = InactiveX;
                    }
                }
                else
                {
                    VelocityX -= .6f;
                }

                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].DrawRectangle.X += (int)VelocityX;
                    Names[i].Position.X = (float)(Tiles[i].DrawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
        }

        private void CheckIfScrolling()
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;


            InputHelper.GetMouseRectRenderTarget(ref _mouseRectangle);
            if (_mouseRectangle.Intersects(Box))
            {
                if (LastScrollWheel != scrollWheel)
                {
                    VelocityY = (scrollWheel - LastScrollWheel) / 5;
                }

            }
            LastScrollWheel = scrollWheel;

            //Check Boundaries
            if (Tiles[0].DrawRectangle.Y > 0)
            {
                VelocityY = -1;
            }
            if (Tiles[Tiles.Count - 1].DrawRectangle.Y + Tiles[Tiles.Count - 1].DrawRectangle.Height< Main.UserResHeight)
            {
                VelocityY = 1;
            }

            if (Tiles.Count < 16)
                return;

            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].DrawRectangle.Y += (int)VelocityY;
                Names[i].Position.Y = Tiles[i].DrawRectangle.Center.Y - Font.LineSpacing / 2;
            }

            VelocityY = VelocityY * .92f;

            //scroll sound
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile t = Tiles[i];
                if (t.DrawRectangle.Intersects(new Rectangle(0, 0, 32, 32)))
                {
                    if (i != LastHitTileOnScroll)
                    {
                        // scrollSound.Play();
                        LastHitTileOnScroll = i;
                    }
                }
            }
        }

        private void CheckIfTileIsHovered()
        {

            foreach (Tile t in Tiles)
            {
                if (InputHelper.MouseRectangle.Intersects(t.DrawRectangle))
                {
                    t.Color = new Color(200, 200, 200);

                    if (InputHelper.IsLeftMousePressed())
                        TileSelected(new TileSelectedArgs(t.Id));

                }
                else t.Color = Color.White;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, Box, new Rectangle(0, 96, 180, Main.DefaultResHeight), Color.White * .5f);

            foreach (Tile t in Tiles)
                t.DrawByForce(spriteBatch);

            foreach (TileName t in Names)
            {
                FontHelper.DrawWithOutline(spriteBatch, Font, t.Name, t.Position, 2, Color.White, Color.Black);
            }
        }

        protected virtual byte[] CurrentAvailableTileSet
        {
            get
            {
                if (GameWorld.Instance.LevelEditor.OnWallMode)
                    return _availableWalls;
                else return AvailableTiles;
            }
        }

        protected virtual byte[] AvailableTiles
        {
            get
            {
                byte[] ds = new byte[]
                {
               1,2,5,6,4,39,40,38,20,10,41,57,8,21,3,18,29,30,14,15,16,23,24,25,26,37,11,12,22,31,32,34,33,19,42,43,45,46,47,48,49,50,51,52,53,54,58,59,60,61,62                };
                return ds;
            }
        }

        static byte[] _availableWalls = new byte[]
        {
            110,100,101,102,103,104,105,108,106,107,109
        };

    }

    public class TileName
    {
        public string Name;
        public Vector2 Position;
    }

    public class TileSelectedArgs : EventArgs
    {
        int _id;
        public TileSelectedArgs(int ID)
        {
            this._id = ID;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }
    }

}
