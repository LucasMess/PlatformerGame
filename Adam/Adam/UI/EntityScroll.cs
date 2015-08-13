using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class EntityScroll
    {
        Rectangle box;

        public EntityScroll()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UI_SpriteSheet, box, new Rectangle(0,0,180,Game1.DefaultResHeight), Color.White * .5f);
        }
    }
}
