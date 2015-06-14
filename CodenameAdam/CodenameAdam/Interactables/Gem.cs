using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Adam;

namespace CodenameAdam
{
    class Gem : Entity
    {
        public Rectangle rectangle, topMidBound;
        Random randGen;
        public Vector2 velocity;
        SoundEffect goldHit, addGold;
        double elapsedTime;
        Light light;

        public enum Type { goldOre, copperOre, diamond, sapphire, emerald }
        Type type = Type.copperOre;

        public Gem(Chest chest, int randomSeed, ContentManager Content)
        {
            randGen = new Random(randomSeed);
            velocity = new Vector2(randGen.Next(-3, 3), randGen.Next(-10, -5));
            goldHit = Content.Load<SoundEffect>("Sounds/PickUpGem");
            addGold = Content.Load<SoundEffect>("Sounds/PickUpGem");
            DefineGem(Content);
            rectangle = new Rectangle(chest.rectangle.Center.X, chest.rectangle.Center.Y, texture.Width, texture.Height);
            light = new Light();
            light.GemLight(1, this, Content, type);
        }

        void DefineGem(ContentManager Content)
        {
            int prob = randGen.Next(1, 100);

            if (prob <= 30)//gold
            {
                type = Type.goldOre;
                texture = Content.Load<Texture2D>("Objects/Gold Ore Chunk");
            }

            if (prob > 30 && prob <= 60)//copper
            {
                type = Type.copperOre;
                texture = Content.Load<Texture2D>("Objects/Copper Ore Chunk");
            }

            if (prob > 60 && prob <= 80)//emerald
            {
                type = Type.emerald;
                texture = Content.Load<Texture2D>("Objects/emerald");
            }

            if (prob > 80 && prob <= 90)//sapphire
            {
                type = Type.sapphire;
                texture = Content.Load<Texture2D>("Objects/Sapphire Gem");
            }
            if (prob > 90 && prob <= 100)//diamond
            {
                type = Type.diamond;
                texture = Content.Load<Texture2D>("Objects/Diamond gem Cut");
            }


        }

        public void Update(GameTime gameTime)
        {
            rectangle.X += (int)velocity.X;
            rectangle.Y += (int)velocity.Y;

            velocity.Y += .3f;
            if (velocity.Y > 5f)
                velocity.Y = 5f;

            topMidBound = new Rectangle(rectangle.X + texture.Width / 2, rectangle.Y + texture.Height / 2, 1, 1);
            xRect = new Rectangle(rectangle.X, rectangle.Y + 5, texture.Width, texture.Height - 10);
            yRect = new Rectangle(rectangle.X + 10, rectangle.Y, texture.Width - 20, texture.Height);
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            light.Update(this);

        }

        public bool WasPickedUp(Player player)
        {

            if (elapsedTime > 1 && player.collRectangle.Intersects(rectangle))
                return true;
            else return false;
        }

        public void AddScore(Player player)
        {
            switch (type)
            {
                case Type.copperOre:
                    player.score += 1;
                    break;
                case Type.goldOre:
                    player.score += 5;
                    break;
                case Type.emerald:
                    player.score += 10;
                    break;
                case Type.sapphire:
                    player.score += 20;
                    break;
                case Type.diamond:
                    player.score += 30;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            light.Draw(spriteBatch);
        }

        public void SoundHit()
        {
            if (Math.Abs(velocity.Y) > 2)
                goldHit.Play();
        }

        public void SoundAdd()
        {
            addGold.Play();
        }


    }
}
