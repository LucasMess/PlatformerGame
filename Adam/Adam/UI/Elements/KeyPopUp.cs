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
            drawRectangle = new Rectangle(collRectangle.Center.X, GameWorld.Instance.player.GetCollRectangle().Y - 16, 32, 32);
            origin = new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2);

            if (GameWorld.Instance.player.GetCollRectangle().Intersects(collRectangle))
                playerOn = true;
            else playerOn = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (playerOn)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }

}
