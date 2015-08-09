using Adam.Levels;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    class LightEngine
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

            width = GameWorld.Instance.worldData.mainMap.Width;
            height = GameWorld.Instance.worldData.mainMap.Height;

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
            foreach (Light l in newLights)
            {
                lights[l.index] = l;
            }
        }

        public void AddFixedLightSource(Tile tile, Light light)
        {
            light.index = tile.TileIndex;
            newLights.Add(light);
        }

        public void Update()
        {
            Camera camera = GameWorld.Instance.camera;
            WorldData worldData = GameWorld.Instance.worldData;
            int initial = camera.tileIndex - 17 * 2 * worldData.mainMap.Width - 25 * 2;
            int maxHoriz = 100;
            int maxVert = 60;
            int i = 0;
            for (int v = 0; v < maxVert; v++)
            {
                for (int h = 0; h < maxHoriz; h++)
                {
                    visibleLights[i] = initial + worldData.mainMap.Width * v + h;
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
