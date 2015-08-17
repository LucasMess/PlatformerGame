using Adam.Levels;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    public class LightEngine
    {
        Light[] lights;
        int[] visibleLights = new int[60 * 100];
        List<Light> dynamicLights = new List<Light>();
        List<Light> newLights = new List<Light>();

        Tile[] tiles;
        Tile[] walls;

        int width;
        int height;

        public LightEngine()
        {

        }

        public void Load()
        {
            tiles = GameWorld.Instance.tileArray;
            walls = GameWorld.Instance.wallArray;

            width = GameWorld.Instance.worldData.LevelWidth;
            height = GameWorld.Instance.worldData.LevelHeight;

            CreateArray();
            GenerateSunLight();
            TransferNewLights();

            dynamicLights = new List<Light>();
            newLights = new List<Light>();
        }

        private void CreateArray()
        {
            //Creates light array that will be used in GameWorld.
            lights = new Light[width * height];
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i] = new Light();
            }
        }

        public void GenerateSunLight()
        {
            //Checks the arrays and looks for places where there is an opening to the sky.
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].sunlightPassesThrough && walls[i].sunlightPassesThrough)
                {
                    lights[i] = new SunLight(tiles[i].drawRectangle);
                }
            }

        }

        private void TransferNewLights()
        {
            for (int i = newLights.Count - 1; i >= 0; i--)
            {
                lights[newLights[i].index] = newLights[i];
                newLights.Remove(newLights[i]);
            }
        }

        public void UpdateSunLight(int index)
        {
            if (tiles[index].sunlightPassesThrough && walls[index].sunlightPassesThrough)
            {
                lights[index] = new SunLight(tiles[index].drawRectangle);
            }
            else lights[index] = new Light();
        }


        public void AddFixedLightSource(Tile tile, Light light)
        {
            light.index = tile.TileIndex;
            newLights.Add(light);
        }

        public void Update()
        {
            if (newLights.Count > 0)
            {
                TransferNewLights();
            }

            Camera camera = GameWorld.Instance.camera;
            WorldData worldData = GameWorld.Instance.worldData;
            int initial = camera.tileIndex - 17 * 2 * worldData.LevelWidth - 25 * 2;
            int maxHoriz = 100;
            int maxVert = 60;
            int i = 0;
            for (int v = 0; v < maxVert; v++)
            {
                for (int h = 0; h < maxHoriz; h++)
                {
                    visibleLights[i] = initial + worldData.LevelWidth * v + h;
                    i++;
                }
            }

            foreach (int index in visibleLights)
            {
                if (index >= 0 && index < lights.Length)
                {
                    lights[index].Update();
                }
            }
        }

        public void AddLuminousEntity()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (int index in visibleLights)
            {
                if (index >= 0 && index < lights.Length)
                {
                    lights[index].Draw(spriteBatch);
                }
            }
        }
    }
}
