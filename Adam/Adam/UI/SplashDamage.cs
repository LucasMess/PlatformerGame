﻿using Microsoft.Xna.Framework;
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
            position = new Vector2(Game1.UserResWidth / 2, Game1.UserResHeight / 2);

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
                spriteBatch.DrawString(font, text, position, Color.OrangeRed * opacity, 0, origin, scale, SpriteEffects.None, 0);
            }
            else spriteBatch.DrawString(font, "+" + text, position, Color.ForestGreen * opacity, 0, origin, scale, SpriteEffects.None, 0);
        }
    }
}