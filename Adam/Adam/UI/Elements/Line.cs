using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Elements
{
    /// <summary>
    /// Makes a line from two points.
    /// </summary>
    public class Line
    {
        Rectangle rect, sourceRect;
        float rotation;

        public Line(Vector2 a, Vector2 b)
        {
            float distance = Vector2.Distance(a, b);

            float yDiff = b.Y - a.Y;
            float xDiff = b.X - b.X;

            rotation = (float)Math.Atan((double)(yDiff / xDiff));

            //rect = new Rectangle((int)a.X, (int)a.Y, (int)distance, 4);
            rect = new Rectangle((int)a.X, (int)a.Y, (int)(Math.Abs(xDiff)), (int)(Math.Abs(yDiff)));
            sourceRect = new Rectangle(312, 224, 1, 1);
            Console.WriteLine("Rotation:{0}, Distance:{1}", rotation, distance);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.SpriteSheet, rect, sourceRect, Color.Red, 0, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
