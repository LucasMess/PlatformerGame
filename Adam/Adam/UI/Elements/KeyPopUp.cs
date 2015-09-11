using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Elements
{
    public class KeyPopUp
    {
        bool playerOn;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;
        Texture2D texture;
        Vector2 origin;

        /// <summary>
        /// To display the 'W' key above the object.
        /// </summary>
        public KeyPopUp()
        {
            texture = GameWorld.SpriteSheet;
            sourceRectangle = new Rectangle(16 * 20, 16 * 7, 16, 16);
        }

        public void Update(Rectangle collRectangle)
        {
            drawRectangle = new Rectangle(collRectangle.X , collRectangle.Y - 48, 32, 32);

            if (GameWorld.Instance.player.GetCollRectangle().Intersects(collRectangle))
                playerOn = true;
            else playerOn = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (playerOn)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, new Vector2(0,0), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }

}
