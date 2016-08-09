using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Adam.UI.Elements
{
    /// <summary>
    /// Makes a line from two points.
    /// </summary>
    [Serializable]
    public class Line
    {
        Rectangle _rect, _sourceRect;
        float _rotation;

        public Line(Vector2 a, Vector2 b)
        {
            float distance = Vector2.Distance(a, b);

            float yDiff = b.Y - a.Y;
            float xDiff = b.X - a.X;

            _rotation = (float)Math.Atan2((double)yDiff , (double) xDiff);


            _rect = new Rectangle((int)a.X, (int)a.Y, (int)distance, 4);
            //rect = new Rectangle((int)a.X, (int)a.Y, (int)(Math.Abs(xDiff)), (int)(Math.Abs(yDiff)));
            _sourceRect = new Rectangle(312, 224, 1, 1);
            //Console.WriteLine("Rotation:{0}, Distance:{1}", rotation, distance);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.SpriteSheet, _rect, _sourceRect, Color.Red, _rotation, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
