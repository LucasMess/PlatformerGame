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
    public class EntityScroll : TileScroll
    {

        public EntityScroll()
        {
            ActiveX = (int)((Main.DefaultResWidth - Width) / Main.WidthRatio);
            // inactiveX = (int)((Game1.DefaultResWidth + 400) / Game1.WidthRatio);
            InactiveX = Main.UserResWidth + 400;

            Box = new Rectangle(ActiveX, 0, (int)(Width / Main.WidthRatio), (int)(Main.DefaultResHeight / Main.HeightRatio));
            Initialize();
        }

        protected override void Hide()
        {
            foreach (Tile t in Tiles)
            {
                t.DrawRectangle.X += 400;
            }
            Box.X += 400;
            foreach (TileName s in Names)
            {
                s.Position.X += 400;
            }
        }

        protected override void CheckIfActive()
        {
            if (GameWorld.Instance.LevelEditor.OnInventory)
            {
                IsActive = true;
            }
            else IsActive = false;

            Box = new Rectangle(Tiles[0].DrawRectangle.X, 0, Box.Width, Box.Height);

            //Prevent super fast jumping
            if (Tiles[0].DrawRectangle.X < ActiveX)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].DrawRectangle.X = ActiveX;
                }
                VelocityX = 0;
            }


            if (IsActive)
            {
                if (Tiles[0].DrawRectangle.X > ActiveX)
                {
                    VelocityX = -(Tiles[0].DrawRectangle.X - ActiveX) / 5;
                }

                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].DrawRectangle.X += (int)VelocityX;
                    Names[i].Position.X = (float)(Tiles[i].DrawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
            else
            {
                if (Tiles[0].DrawRectangle.X > InactiveX)
                {
                    VelocityX = 0;
                    for (int i = 0; i < Tiles.Count; i++)
                    {
                        Tiles[i].DrawRectangle.X = InactiveX;
                    }
                }
                else
                {
                    VelocityX += .6f;
                }

                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].DrawRectangle.X += (int)VelocityX;
                    Names[i].Position.X = (float)(Tiles[i].DrawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
        }

        protected override byte[] CurrentAvailableTileSet
        {
            get
            {
                return AvailableTiles;
            }
        }

        protected override byte[] AvailableTiles
        {
            get
            {
                byte[] ds = new byte[]
                {
               200,201,202,203,207,208,209,204,205,206
                };
                return ds;
            }
        }

    }
}
