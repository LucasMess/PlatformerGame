using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class SplashDamage
    {
        SpriteFont font;
        Vector2 position;
        string text;
        bool isNegative;
        public bool toDelete;
        bool hasExpanded;
        float opacity = 2;
        int damage;
        float scale, normScale;
        Vector2 velocity, origin;

        public SplashDamage(int damage)
        {
            this.damage = damage;
            text = damage.ToString();
            position = new Vector2(Main.UserResWidth / 2, Main.UserResHeight / 2);

            if (damage < 0)
                isNegative = true;

            int absDamage = Math.Abs(damage);
            scale = .1f;
            if (absDamage > 10)
                scale = .15f;
            if (absDamage > 20)
                scale = .2f;
            if (absDamage > 40)
                scale = .3f;
            if (absDamage > 80)
                scale = .5f;

            font = ContentHelper.LoadFont("Fonts/splash_damage");
            velocity = new Vector2(0, -5);

            origin = font.MeasureString(text) / 2;
            normScale = scale;
        }


        public void Update(GameTime gameTime)
        {
            opacity -= .05f;
            position += velocity;

            velocity.Y = velocity.Y * 0.95f;

            if (scale > normScale * 2)
            {
                hasExpanded = true;
            }

            if (!hasExpanded)
            {
                scale += .00005f;
            }
            else
            {
                scale -= .01f;
            }

            if (opacity < 0)
                toDelete = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isNegative)
            {
                FontHelper.DrawWithOutline(spriteBatch, font, damage.ToString(), position, 2, Color.Red* opacity, Color.Black * opacity);
            }
            else FontHelper.DrawWithOutline(spriteBatch, font, "+" + damage, position, 2, Color.Green * opacity, Color.Black * opacity);
        }
    }
}
