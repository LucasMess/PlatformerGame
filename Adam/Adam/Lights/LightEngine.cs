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
        SunLight[] _sunLights;
        int[] _visibleLights = new int[60 * 100];
        public List<Light> DynamicLights = new List<Light>();
        List<Light> _newLights = new List<Light>();

        Tile[] _tiles;
        Tile[] _walls;

        int _width;
        int _height;

        

        public LightEngine()
        {

        }

        public void Load()
        {
            _tiles = GameWorld.Instance.TileArray;
            _walls = GameWorld.Instance.WallArray;

            _width = GameWorld.Instance.WorldData.LevelWidth;
            _height = GameWorld.Instance.WorldData.LevelHeight;

            CreateArray();
            GenerateSunLight();
            TransferNewLights();

            DynamicLights = new List<Light>();
            _newLights = new List<Light>();
        }

        private void CreateArray()
        {
            //Creates light array that will be used in GameWorld.
            _sunLights = new SunLight[_width * _height];
            //for (int i = 0; i < sunLights.Length; i++)
            //{
            //    sunLights[i] = new SunLight();
            //}
        }

        public void GenerateSunLight()
        {
            //Checks the arrays and looks for places where there is an opening to the sky.
            for (int i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i].SunlightPassesThrough && _walls[i].SunlightPassesThrough)
                {
                    _sunLights[i] = new SunLight(_tiles[i].DrawRectangle);
                }
            }

        }

        private void TransferNewLights()
        {
            //for (int i = newLights.Count - 1; i >= 0; i--)
            //{
            //    sunLights[newLights[i].index] = newLights[i];
            //    newLights.Remove(newLights[i]);
            //}
        }

        public void UpdateSunLight(int index)
        {
            //if (tiles[index].sunlightPassesThrough && walls[index].sunlightPassesThrough)
            //{
            //    lights[index] = new SunLight(tiles[index].drawRectangle);
            //}
            //else lights[index] = new Light();
        }


        public void AddFixedLightSource(Tile tile, Light light)
        {
            light.Index = tile.TileIndex;
            _newLights.Add(light);
        }

        public void Update()
        {
            if (_newLights.Count > 0)
            {
                TransferNewLights();
            }

            _visibleLights = GameWorld.Instance.ChunkManager.GetVisibleIndexes();

            //Camera camera = GameWorld.Instance.camera;
            //WorldData worldData = GameWorld.Instance.worldData;
            //int initial = camera.tileIndex - 17 * 2 * worldData.LevelWidth - 25 * 2;
            //int maxHoriz = 100;
            //int maxVert = 60;
            //int i = 0;
            //for (int v = 0; v < maxVert; v++)
            //{
            //    for (int h = 0; h < maxHoriz; h++)
            //    {
            //        visibleLights[i] = initial + worldData.LevelWidth * v + h;
            //        i++;
            //    }
            //}

            //foreach (int index in visibleLights)
            //{
            //    if (index >= 0 && index < lights.Length)
            //    {
            //        lights[index].Update();
            //    }
            //}

            foreach (Light l in DynamicLights)
            {
                l.Update(l.source.Get());
            }


            // Limit the maximum amount of lights that can exist at the same time.
            if (DynamicLights.Count > 1000)
            {
                int overflow = DynamicLights.Count - 999;
                for (int c = 0; c < overflow; c++)
                {
                    DynamicLights.RemoveAt(0);
                }
            }
        }

        public void AddDynamicLight(Light light)
        {
            DynamicLights.Add(light);
        }

        public void RemoveDynamicLight(Light light)
        {
            DynamicLights.Remove(light);
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            foreach (int index in _visibleLights)
            {
                if (index >= 0 && index < _sunLights.Length)
                {
                    _sunLights[index]?.Draw(spriteBatch);
                }
            }

            foreach (Light l in DynamicLights)
            {
                l.Draw(spriteBatch);
            }
        }

        public void DrawGlows(SpriteBatch spriteBatch)
        {
            //foreach (int index in visibleLights)
            //{
            //    if (index >= 0 && index < lights.Length)
            //    {
            //        lights[index].DrawGlow(spriteBatch);
            //    }
            //}
            foreach (Light l in DynamicLights)
            {
                l.DrawGlow(spriteBatch);
            }

        }
    }
}
