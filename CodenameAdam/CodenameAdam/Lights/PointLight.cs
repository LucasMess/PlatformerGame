using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    class PointLight : Light
    {
        public void Create(Vector2 source)
        {
            texture = Content.Load<Texture2D>("Lighting/shadow10");
            rectangle = new Rectangle((int)source.X, (int)source.Y, texture.Width, texture.Height);
            this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
            sourceRectangle = new Rectangle(0, 0, rectangle.Width, rectangle.Height);
            glow = new Glow(this);
        }

        public void SetColor(Color color)
        {
            glow.Color = color;
        }

        public void SetSize(Vector2 size)
        {
            rectangle.Width = (int)size.X;
            rectangle.Height = (int)size.Y;
        }

        public void SetOpacity(float opacity)
        {
            glow.Opacity = opacity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White, 0, origin, SpriteEffects.None, 0);
        }




    }
}
