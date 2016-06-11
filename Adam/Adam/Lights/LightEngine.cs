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
            _tiles = GameWorld.TileArray;
            _walls = GameWorld.WallArray;

            _width = GameWorld.WorldData.LevelWidth;
            _height = GameWorld.WorldData.LevelHeight;

            CreateArray();
            GenerateSunLight();

            DynamicLights = new List<Light>();
            _newLights = new List<Light>();
        }

        private void CreateArray()
        {
            _sunLights = new SunLight[_width * _height];
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

        public void AddFixedLightSource(Tile tile, Light light)
        {
            DynamicLights.Add(light);
        }

        public void Update()
        {
            _visibleLights = GameWorld.ChunkManager.GetVisibleIndexes();

            foreach (Light l in DynamicLights)
            {
                l.Update();
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
