using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class TriggerZone
    {
        Rectangle rectangle;
        GameWorld map;
        int ID;
        int incrementingWidth;
        int incrementingHeight;

        public TriggerZone(int x, int y, int ID)
        {
            this.ID = ID;
            rectangle = new Rectangle(x, y, 1, 1);
        }

        public void IncreaseDimensions(int x, int y)
        {

        }

        public void Update(Player player, GameMode CurrentLevel)
        {
        }


    }
}
