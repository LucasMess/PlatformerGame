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
    public class SplashNumber : Particle
    {
        SpriteFont font;
        string text;
        bool isNegative;
        bool hasExpanded;
        int number;
        float scale, normScale;

        public SplashNumber(Entity entity, int number, Color color)
        {
            this.number = number;
            text = this.number.ToString();
            position = new Vector2(entity.GetCollRectangle().Right + 20, entity.GetCollRectangle().Y - 20);
            collRectangle = new Rectangle((int)position.X, (int)position.Y, 50, 50);
            Color = color;

            if (this.number < 0)
                isNegative = true;

            int absDamage = Math.Abs(this.number);
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
            Opacity = 2;
        }


        public override void Update(GameTime gameTime)
        {
            Opacity -= .05f;
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

            if (Opacity <= 0)
                ToDelete = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isNegative)
            {
                FontHelper.DrawWithOutline(spriteBatch, font, number.ToString(), position, 2, Color * Opacity, Color.Black * Opacity);
            }
            else FontHelper.DrawWithOutline(spriteBatch, font, "+" + number, position, 2, Color * Opacity, Color.Black * Opacity);
        }
    }
}
