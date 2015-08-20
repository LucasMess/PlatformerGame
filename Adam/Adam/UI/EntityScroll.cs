using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class EntityScroll : TileScroll
    {

        public EntityScroll()
        {
            activeX = (int)((Main.DefaultResWidth - Width) / Main.WidthRatio);
            // inactiveX = (int)((Game1.DefaultResWidth + 400) / Game1.WidthRatio);
            inactiveX = Main.UserResWidth + 400;

            box = new Rectangle(activeX, 0, (int)(Width / Main.WidthRatio), (int)(Main.DefaultResHeight / Main.HeightRatio));
            Initialize();
        }

        protected override void Hide()
        {
            foreach (Tile t in tiles)
            {
                t.drawRectangle.X += 400;
            }
            box.X += 400;
            foreach (TileName s in names)
            {
                s.Position.X += 400;
            }
        }

        protected override void CheckIfActive()
        {
            if (GameWorld.Instance.levelEditor.onInventory)
            {
                isActive = true;
            }
            else isActive = false;

            box = new Rectangle(tiles[0].drawRectangle.X, 0, box.Width, box.Height);

            //Prevent super fast jumping
            if (tiles[0].drawRectangle.X < activeX)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X = activeX;
                }
                velocityX = 0;
            }


            if (isActive)
            {
                if (tiles[0].drawRectangle.X > activeX)
                {
                    velocityX = -(tiles[0].drawRectangle.X - activeX) / 5;
                }

                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X += (int)velocityX;
                    names[i].Position.X = (float)(tiles[i].drawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
                }
            }
            else
            {
                if (tiles[0].drawRectangle.X > inactiveX)
                {
                    velocityX = 0;
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].drawRectangle.X = inactiveX;
                    }
                }
                else
                {
                    velocityX += .6f;
                }

                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].drawRectangle.X += (int)velocityX;
                    names[i].Position.X = (float)(tiles[i].drawRectangle.X + Main.Tilesize / Main.WidthRatio) + 5;
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
                byte[] IDs = new byte[]
                {
               200,201,202,203,204,205,206
                };
                return IDs;
            }
        }

    }
}
