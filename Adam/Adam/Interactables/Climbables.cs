using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class Climbables : Entity
    {   

        public Climbables(int x, int y)
        {
            int tileSize = Game1.Tilesize;
            collRectangle = new Rectangle(x, y, tileSize, tileSize);
        }

        public bool IsOnPlayer(Player player)
        {
            if (player.collRectangle.Intersects(collRectangle))
            {
                return true;
            }
            else return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //DO NOTHING
        }
    }
}
