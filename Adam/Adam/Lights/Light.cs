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
            Texture = GameWorld.SpriteSheet;
            SourceRectangle = new Rectangle(256, 240, 64, 64);
            Origin = new Vector2(32, 32);
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
            spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White * Opacity);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            Glow?.Draw(spriteBatch);
        }

    }
}
