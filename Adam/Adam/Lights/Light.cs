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
    class Light
    {
        public Rectangle drawRectangle;
        protected Rectangle sourceRectangle;
        Rectangle original;
        public Texture2D texture;
        public int pos;
        int tileSize;
        protected bool lightHere;
        bool shakyLight;
        protected Vector2 origin;
        protected Color color = Color.White;
        protected ContentManager Content;
        protected Glow glow;
        Random randGen;
        public float intensity = 1f;
        public const int DefaultSize = 256;
        protected float opacity = 1f;

        public Light()
        {
            this.tileSize = Game1.Tilesize;
            this.Content = Game1.Content;
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

        public void CreateProjectileLight(int intensity, Projectile proj, ContentManager Content)
        {
            this.Content = Content;
            this.intensity = intensity;
            texture = GameWorld.SpriteSheet;
            lightHere = true;

            drawRectangle = new Rectangle(proj.collRectangle.Center.X, proj.collRectangle.Center.Y, 256 * intensity, 256 * intensity);
            origin = new Vector2(256 * intensity / 2,256 * intensity / 2);
            sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public void EffectLight(float intensity, Adam.Particle effect, ContentManager Content)
        {
            this.Content = Content;
            this.intensity = intensity;
            switch (effect.CurrentParticle)
            {
                case Adam.Particle.ParticleType.WeaponBurst:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Red;
                    break;
                case Adam.Particle.ParticleType.ChestSparkles:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Yellow;
                    break;
            }

            lightHere = true;

            drawRectangle = new Rectangle(effect.drawRectangle.Center.X, effect.drawRectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
            origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);
            sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public void GemLight(int intensity, Gem gem, ContentManager Content, Gem.Type type)
        {
            this.Content = Content;
            this.intensity = intensity;
            lightHere = true;
            switch (type)
            {
                case Gem.Type.sapphire:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Blue;
                    break;
                case Gem.Type.emerald:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Green;
                    break;
                case Gem.Type.diamond:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Cyan;
                    break;
                case Gem.Type.goldOre:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Yellow;
                    lightHere = false;
                    break;
                case Gem.Type.copperOre:
                    texture = GameWorld.SpriteSheet;
                    color = Color.Brown;
                    lightHere = false;
                    break;
            }
            drawRectangle = new Rectangle(gem.rectangle.Center.X, gem.rectangle.Center.Y, 256 * intensity, 256 * intensity);
            origin = new Vector2(256* intensity / 2, 256 * intensity / 2);
            sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public void CalculateLighting(Tile[] tile, Tile[] wall, Texture2D map)
        {
            int w = map.Width;

            if (tile[pos].emitsLight == true && wall[pos].emitsLight == true)
            {
                //sky light
                lightHere = true;
                drawRectangle = new Rectangle(tile[pos].drawRectangle.Center.X, tile[pos].drawRectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
                origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);
                sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                drawRectangle.X = drawRectangle.X - (int)origin.X;
                drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
            }

            if (tile[pos].emitsLight == true && tile[pos].ID != 0)
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
                        drawRectangle = new Rectangle(tile[pos].drawRectangle.Center.X, tile[pos].drawRectangle.Center.Y, (int)(256* intensity), (int)(256 * intensity));
                        origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);
                        sourceRectangle = new Rectangle(16 * 16, 15 * 16, 64, 64);

                        drawRectangle.X = drawRectangle.X - (int)origin.X;
                        drawRectangle.Y = drawRectangle.Y - (int)origin.Y;

                        original = drawRectangle;
                        break;
                }
            }
        }

        public void Update(Vector2 source)
        {
            drawRectangle.X = (int)source.X;
            drawRectangle.Y = (int)source.Y;
            glow.Update(drawRectangle);
        }

        public void Update(Player player)
        {
            lightHere = true;
            drawRectangle = new Rectangle(player.collRectangle.Center.X, player.collRectangle.Center.Y, 256 * 3, 256 * 3);
            origin = new Vector2(256* 3 / 2, 256* 3 / 2);
            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public void Update(Projectile proj)
        {
            lightHere = true;
            drawRectangle = new Rectangle(proj.collRectangle.Center.X, proj.collRectangle.Center.Y, (int)(256* intensity), (int)(256 * intensity));
            origin = new Vector2(256 * intensity / 2,256 * intensity / 2);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public void Update(Adam.Particle effect)
        {
            lightHere = true;
            drawRectangle = new Rectangle(effect.drawRectangle.Center.X, effect.drawRectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
            origin = new Vector2(256 * intensity / 2, 256 * intensity / 2);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;

            opacity = effect.Opacity;
        }

        public void Update(Gem gem)
        {
            lightHere = true;
            drawRectangle = new Rectangle(gem.rectangle.Center.X, gem.rectangle.Center.Y, (int)(256 * intensity), (int)(256 * intensity));
            origin = new Vector2(256* intensity / 2, 256* intensity / 2);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }

        public void Shake()
        {
            if (shakyLight)
            {
                drawRectangle = original;

                drawRectangle.X += randGen.Next(-6, 7);
                drawRectangle.Y += randGen.Next(-6, 7);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (lightHere)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            glow.Draw(spriteBatch);
        }

    }
}
