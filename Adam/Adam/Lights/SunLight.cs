using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    class SunLight : Light
    {

        public SunLight(Rectangle tileDrawRectangle)
        {
            lightHere = true;
            drawRectangle = new Rectangle(tileDrawRectangle.Center.X, tileDrawRectangle.Center.Y, DefaultSize, DefaultSize);
            origin = new Vector2(DefaultSize / 2, DefaultSize / 2);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public static Light[] CalculateFromGameWorld()
        {
            Tile[] tiles = GameWorld.Instance.tileArray;
            Tile[] walls = GameWorld.Instance.wallArray;
           
            int width = GameWorld.Instance.worldData.mainMap.Width;
            int height = GameWorld.Instance.worldData.mainMap.Height;

            //Creates light array that will be used in GameWorld.
            Light[] lights = new Light[width * height];
            for (int i = 0; i < lights.Length; i ++)
            {
                lights[i] = new Light();
            }

            //Checks the arrays and looks for places where there is an opening to the sky.
            for (int i = 0; i < tiles.Length;i++)
            {
                if (tiles[i].emitsLight && walls[i].emitsLight)
                {
                    lights[i] = new SunLight(tiles[i].drawRectangle);
                   
                }
            }

            return lights;
        }

    }
}
