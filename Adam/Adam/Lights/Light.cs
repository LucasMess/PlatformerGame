using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam
{
    public class Light
    {
        public Entity source;
        public Rectangle DrawRectangle;
        protected Rectangle SourceRectangle;
        protected Rectangle Original;
        public Texture2D Texture;
        public float GlowIntensity;
        public int Pos;
        public int Index;
        int _tileSize;
        public bool LightHere;
        bool _shakyLight;
        protected Vector2 Origin;
        public  Color Color = Color.White;
        protected ContentManager Content;
        protected Glow Glow;
        protected const int DefaultSize = 256;
        protected int Size = DefaultSize;
        Random _randGen;
        public float Intensity = 1f;
        protected bool IsShaky;

        protected float Opacity = 1f;

        public Light()
        {
            this._tileSize = Main.Tilesize;
            this.Content = Main.Content;
            Color = Color.White;
            Opacity = 1;
            SourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);
            Texture = GameWorld.SpriteSheet;
        }

        public void Load()
        {
            this.Content = Content;
            Texture = GameWorld.SpriteSheet;
            SourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);
            Origin = new Vector2(128, 128);
        }

        public void CalculateLighting(Tile[] tile, Tile[] wall, Texture2D map)
        {
            int w = map.Width;

            if (tile[Pos].SunlightPassesThrough == true && wall[Pos].SunlightPassesThrough == true)
            {
                //sky light
                LightHere = true;
                DrawRectangle = new Rectangle(tile[Pos].DrawRectangle.Center.X, tile[Pos].DrawRectangle.Center.Y, (int)(256 * Intensity), (int)(256 * Intensity));
                Origin = new Vector2(256 * Intensity / 2, 256 * Intensity / 2);
                SourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                DrawRectangle.X = DrawRectangle.X - (int)Origin.X;
                DrawRectangle.Y = DrawRectangle.Y - (int)Origin.Y;
            }

            if (tile[Pos].SunlightPassesThrough == true && tile[Pos].Id != 0)
            {
                //light sauce
                LightHere = true;
                _randGen = new Random(tile[Pos].DrawRectangle.X);

                switch (tile[Pos].Id)
                {
                    case 11:
                        Intensity = 3;
                        Texture = GameWorld.SpriteSheet;
                        _shakyLight = true;
                        DrawRectangle = new Rectangle(tile[Pos].DrawRectangle.Center.X, tile[Pos].DrawRectangle.Center.Y, (int)(256 * Intensity), (int)(256 * Intensity));
                        Origin = new Vector2(256 * Intensity / 2, 256 * Intensity / 2);
                        SourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                        DrawRectangle.X = DrawRectangle.X - (int)Origin.X;
                        DrawRectangle.Y = DrawRectangle.Y - (int)Origin.Y;

                        Original = DrawRectangle;
                        break;
                    case 12:
                        Intensity = 4;
                        Texture = GameWorld.SpriteSheet;
                        DrawRectangle = new Rectangle(tile[Pos].DrawRectangle.Center.X, tile[Pos].DrawRectangle.Center.Y, (int)(256 * Intensity), (int)(256 * Intensity));
                        Origin = new Vector2(256 * Intensity / 2, 256 * Intensity / 2);
                        SourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                        DrawRectangle.X = DrawRectangle.X - (int)Origin.X;
                        DrawRectangle.Y = DrawRectangle.Y - (int)Origin.Y;

                        Original = DrawRectangle;
                        break;
                }
            }
        }

        public virtual void Update(Entity source)
        {
            Glow?.Update(this);
        }

        protected void SetPosition(Rectangle parentRectangle)
        {
            DrawRectangle = new Rectangle(parentRectangle.Center.X, parentRectangle.Center.Y, Size, Size);
            Origin = new Vector2(Size / 2, Size / 2);

            DrawRectangle.X = DrawRectangle.X - (int)Origin.X;
            DrawRectangle.Y = DrawRectangle.Y - (int)Origin.Y;

            Original = DrawRectangle;

        }

        public virtual void Update()
        {
            Glow?.Update(this);
        }

        public void Shake()
        {
            Random randGen = GameWorld.RandGen;
            DrawRectangle = Original;

            DrawRectangle.X += randGen.Next(-6, 7);
            DrawRectangle.Y += randGen.Next(-6, 7);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (LightHere)
                spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White * Opacity);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            Glow?.Draw(spriteBatch);
        }

    }
}
