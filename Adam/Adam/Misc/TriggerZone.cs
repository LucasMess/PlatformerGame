using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam
{
    public class TriggerZone
    {
        Rectangle _rectangle;
        GameWorld _map;
        int _id;
        int _incrementingWidth;
        int _incrementingHeight;

        public TriggerZone(int x, int y, int id)
        {
            this._id = id;
            _rectangle = new Rectangle(x, y, 1, 1);
        }

        public void IncreaseDimensions(int x, int y)
        {

        }

        public void Update(Player.Player player, GameMode currentLevel)
        {
        }


    }
}
