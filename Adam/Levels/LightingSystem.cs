using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Misc.Helpers;

namespace ThereMustBeAnotherWay.Levels
{
    public static class LightingSystem
    {
        public static Color DEFAULT_AMBIENT_COLOR = Color.White;

        public static Color AmbientColor { get; set; } = DEFAULT_AMBIENT_COLOR;

        public static void Draw(SpriteBatch spriteBatch)
        {
            GameWorld.DrawLights(spriteBatch);
        }
    }
}
