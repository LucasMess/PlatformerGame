using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Adam;
using Adam.Interactables;
using Adam.Misc;
using Adam.Misc.Interfaces;

namespace Adam
{
    public class Gem : Item, ICollidable, INewtonian
    {
        byte gemID;

        public float GravityStrength
        {
            get
            {
                return Main.Gravity;
            }
            set
            {
                GravityStrength = value;
            }
        }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public bool IsAboveTile { get; set; }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public Gem(int centerX, int centerY)
        {
            gemID = GenerateID();
            Texture = GameWorld.SpriteSheet;
            collRectangle = new Rectangle(centerX, centerY, 16, 16);
            sourceRectangle = GetSourceRectangle();
            velocity = new Vector2(GameWorld.RandGen.Next(-3, 4), GameWorld.RandGen.Next(-10, -5));
            Light = new Lights.DynamicPointLight(this, .5f, false, GetGemColor(), .8f);
            GameWorld.Instance.lightEngine.AddDynamicLight(Light);

            pickUpSound = new Misc.SoundFx("Sounds/Items/gold" + GameWorld.RandGen.Next(0, 5));
        }

        private byte GenerateID()
        {
            int rand = GameWorld.RandGen.Next(0, 100);
            if (rand > 95) //5% - Diamond
            {
                return 5;
            }
            if (rand > 85) //10% - Ruby
            {
                return 4;
            }
            if (rand > 70) //15% - Sapphire
            {
                return 3;
            }
            if (rand > 50) //20% - Emerald
            {
                return 2;
            }
            if (rand > 25) //25% - Gold
            {
                return 1;
            }
            else return 0; //25% - Copper
        }

        private Rectangle GetSourceRectangle()
        {
            Rectangle source = new Rectangle();
            switch (gemID)
            {
                case 0:
                    source = new Rectangle(21 * 16, 9 * 16, 16, 16);
                    break;
                case 1:
                    source = new Rectangle(20 * 16, 9 * 16, 16, 16);
                    break;
                case 2:
                    source = new Rectangle(21 * 16, 8 * 16, 16, 16);
                    break;
                case 3:
                    source = new Rectangle(20 * 16, 8 * 16, 16, 16);
                    break;
                case 4:
                    source = new Rectangle(21 * 16, 8 * 16, 16, 16);
                    break;
                case 5:
                    source = new Rectangle(20 * 16, 10 * 16, 16, 16);
                    break;
            }
            return source;
        }

        public Color GetGemColor()
        {
            switch (gemID)
            {
                case 0:
                    return Color.Brown;
                case 1:
                    return Color.Gold;
                case 2:
                    return Color.Green;
                case 3:
                    return Color.Blue;
                case 4:
                    return Color.Red;
                case 5:
                    return Color.Cyan;
            }
            return Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, sourceRectangle, Color.White);        

        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = -3f;
            velocity.X *= .5f;
            bounceSound?.PlayNewInstanceOnce();
            bounceSound?.Reset();
            for (int i = 0; i < 5; i++)
            {
                GameWorld.Instance.particles.Add(new StompSmokeParticle(this));
            }
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {

        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {

        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {

        }
    }
}
