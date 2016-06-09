using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Elements
{
    /// <summary>
    /// Containers are used to place other UI objects relative to the container.
    /// </summary>
    public class Container
    {
        private Rectangle _box;

        public Container(int x, int y, int width, int height)
        {
            _box = new Rectangle(x,y,width,height);
        }
    }
}
