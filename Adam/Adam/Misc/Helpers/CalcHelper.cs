using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Adam
{
    public static class CalcHelper
    {
        /// <summary>
        /// Returns the hypotenuse of two sides or "pythagorates" a vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float GetPythagoras(float a, float b)
        {
            double x = (double)a;
            double y = (double)b;

            double x2 = Math.Pow(x, 2);
            double y2 = Math.Pow(y, 2);

            double sqroot = Math.Sqrt(x2 + y2);
            return (float)sqroot;
        }

        /// <summary>
        /// Returns the unit vector as a Vector2 of another Vector2
        /// </summary>
        /// <param name="vector2">The Vector2 that the unit vector will be extraced from</param>
        /// <returns></returns>
        public static Vector2 GetUnitVector(Vector2 vector2)
        {
            float magnitude = GetPythagoras(vector2.X, vector2.Y);
            Vector2 unitVector = new Vector2(vector2.X / magnitude, vector2.Y / magnitude);

            return unitVector;
        }

        /// <summary>
        /// Returns an instance of a public class as a byte array.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(object source)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        public static object ConvertToObject(byte[] array)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (var stream = new MemoryStream(array))
            {
                var obj = formatter.Deserialize(stream);
                return obj;
            }

            

        }

    }
}
