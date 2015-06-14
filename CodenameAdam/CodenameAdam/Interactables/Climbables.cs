using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodenameAdam
{
    class Climbables
    {
        Rectangle rectangle;
        int tileSize;       

        public Climbables(int x, int y)
        {
            tileSize = Game1.TILESIZE;
            rectangle = new Rectangle(x, y, tileSize, tileSize);
        }

        public bool IsOnPlayer(Player player)
        {
            if (player.collRectangle.Intersects(rectangle))
            {
                return true;
            }
            else return false;
        }
    }
}
