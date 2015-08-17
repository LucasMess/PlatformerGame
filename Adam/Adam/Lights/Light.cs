using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class Light
    {
        public Rectangle drawRectangle;
        protected Rectangle sourceRectangle;
        protected Rectangle original;
        public Texture2D texture;
        public int pos;
        public int index;
        int tileSize;
        public bool lightHere;
        bool shakyLight;
        protected Vector2 origin;
        public  Color color = Color.White;
        protected ContentManager Content;
        protected Glow glow;
        protected const int DefaultSize = 256;
        protected int size = DefaultSize;
        Random randGen;
        public float intensity = 1f;
        protected bool isShaky;

        protected float opacity = 1f;

        public Light()
        {
            this.tileSize = Main.Tilesize;
            this.Content = Main.Content;
            color = Color.White;
            opacity = 1;
            sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);
            texture = GameWorld.SpriteSheet;
        }

        public void Load(ContentManager Content)
        {
            this.Content = Content;
            texture = GameWorld.SpriteSheet;
            sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);
            origin = new Vector2(128, 128);
        }

        public void CalculateLighting(Tile[] tile, Tile[] wall, Texture2D map)
        {
            int w = map.Width;

            if (tile[pos].sunlightPassesThrough == true && wall[pos].sunlightPassesThrough == true)
            {
                //sky light
                lightHere = true;
                drawRectangle = new Rectangle(tile[pos].drawRectangle.Center.X, tile[pos].drawRectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
                origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);
                sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                drawRectangle.X = drawRectangle.X - (int)origin.X;
                drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
            }

            if (tile[pos].sunlightPassesThrough == true && tile[pos].ID != 0)
            {
                //light sauce
                lightHere = true;
                randGen = new Random(tile[pos].drawRectangle.X);

                switch (tile[pos].ID)
                {
                    case 11:
                        intensity = 3;
                        texture = GameWorld.SpriteSheet;
                        shakyLight = true;
                        drawRectangle = new Rectangle(tile[pos].drawRectangle.Center.X, tile[pos].drawRectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
                        origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);
                        sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                        drawRectangle.X = drawRectangle.X - (int)origin.X;
                        drawRectangle.Y = drawRectangle.Y - (int)origin.Y;

                        original = drawRectangle;
                        break;
                    case 12:
                        intensity = 4;
                        texture = GameWorld.SpriteSheet;
                        drawRectangle = new Rectangle(tile[pos].drawRectangle.Center.X, tile[pos].drawRectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
                        origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);
                        sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                        drawRectangle.X = drawRectangle.X - (int)origin.X;
                        drawRectangle.Y = drawRectangle.Y - (int)origin.Y;

                        original = drawRectangle;
                        break;
                }
            }
        }

        public virtual void Update(Entity source)
        {

        }

        protected void SetPosition(Rectangle parentRectangle)
        {
            drawRectangle = new Rectangle(parentRectangle.Center.X, parentRectangle.Center.Y, size, size);
            origin = new Vector2(size / 2, size / 2);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;

            original = drawRectangle;

        }

        public virtual void Update()
        {
            glow?.Update(this);
        }

        public void Shake()
        {
            Random randGen = GameWorld.RandGen;
            drawRectangle = original;

            drawRectangle.X += randGen.Next(-6, 7);
            drawRectangle.Y += randGen.Next(-6, 7);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (lightHere)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            glow?.Draw(spriteBatch);
        }

    }
}
