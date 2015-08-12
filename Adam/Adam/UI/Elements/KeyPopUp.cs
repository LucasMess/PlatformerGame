using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Elements
{
    public class KeyPopUp : Entity
    {
        /// <summary>
        /// To display the 'W' key above the object.
        /// </summary>
        /// 
        bool playerOn;
        public KeyPopUp()
        {
            texture = GameWorld.SpriteSheet;
            sourceRectangle = new Rectangle(16 * 20, 16 * 7, 16, 16);
        }

        public void Update(Rectangle collRectangle)
        {
            drawRectangle = new Rectangle(collRectangle.Center.X, GameWorld.Instance.player.collRectangle.Y - 16, 32, 32);
            origin = new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2);

            if (GameWorld.Instance.player.collRectangle.Intersects(collRectangle))
                playerOn = true;
            else playerOn = false;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (playerOn)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }

}
