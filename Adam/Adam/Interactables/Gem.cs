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
using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.UI;

namespace Adam
{
    public class Gem : Item, INewtonian
    {
        byte _gemId;

        public bool IsFlying { get; set; }

        public bool IsAboveTile { get; set; }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public Gem(int centerX, int centerY)
        {
            _gemId = GenerateId();
            Texture = GameWorld.SpriteSheet;
            CollRectangle = new Rectangle(centerX, centerY, 16, 16);
            SourceRectangle = GetSourceRectangle();
            Velocity = new Vector2(GameWorld.RandGen.Next(-100, 100) / 10f, -GameWorld.RandGen.Next(100, 180) / 10f);
            Light = new Lights.DynamicPointLight(this, .5f, false, GetGemColor(), .8f);
            GameWorld.Instance.LightEngine.AddDynamicLight(Light);

            //pickUpSound = new Misc.SoundFx("Sounds/Items/gold" + GameWorld.RandGen.Next(0, 5));

            OnPlayerPickUp += Gem_OnPlayerPickUp;
            CurrentCollisionType = CollisionType.Bouncy;
        }

        public Gem(int centerX, int centerY, byte id)
        {
            _gemId = id;
            Texture = GameWorld.SpriteSheet;
            CollRectangle = new Rectangle(centerX, centerY, 16, 16);
            SourceRectangle = GetSourceRectangle();
            Velocity = new Vector2(GameWorld.RandGen.Next(-100, 100) / 10f, -GameWorld.RandGen.Next(100, 180) / 10f);
            Light = new Lights.DynamicPointLight(this, .5f, false, GetGemColor(), .8f);
            GameWorld.Instance.LightEngine.AddDynamicLight(Light);

            PickUpSound = new Misc.SoundFx("Sounds/Items/gold" + GameWorld.RandGen.Next(0, 5));

            OnPlayerPickUp += Gem_OnPlayerPickUp;
            CurrentCollisionType = CollisionType.Bouncy;
        }

        private void Gem_OnPlayerPickUp(PickedUpArgs e)
        {
            e.Player.Score += GetValue();

            GameWorld.Instance.Particles.Add(new SplashNumber(this, GetValue(), Color.Yellow));
        }

        private int GetValue()
        {
            switch (_gemId)
            {
                case 0: // Copper
                    return 1;
                case 1: // Gold
                    return 2;
                case 2: // Emerald
                    return 4;
                case 3: // Sapphire
                    return 5;
                case 4: // Ruby
                    return 6;
                case 5: // Diamond
                    return 10;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Generates an ID for this gem depending on their rarity.
        /// </summary>
        /// <returns>ID</returns>
        private byte GenerateId()
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
            switch (_gemId)
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
                    source = new Rectangle(21 * 16, 10 * 16, 16, 16);
                    break;
                case 5:
                    source = new Rectangle(20 * 16, 10 * 16, 16, 16);
                    break;
            }
            return source;
        }

        public Color GetGemColor()
        {
            switch (_gemId)
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
            spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White);

        }

        public void OnCollisionWithTerrainBelow(Entity entity, Tile tile)
        {
            BounceSound?.PlayNewInstanceOnce();
            BounceSound?.Reset();
            for (int i = 0; i < 5; i++)
            {
                GameWorld.Instance.Particles.Add(new StompSmokeParticle(this));
            }
        }

        /// <summary>
        /// Generates specified number of gems in gameworld.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="entity"></param>
        public static void Generate(int count, Entity entity)
        {
            for (int i = 0; i < count; i++)
            {
                Gem gem = new Gem(entity.GetCollRectangle().Center.X, entity.GetCollRectangle().Center.Y);
                GameWorld.Instance.Entities.Add(gem);
            }
        }

        /// <summary>
        /// Generates specified number of a particular type of gem in gameworld.
        /// </summary>
        /// <param name="gemId"></param>
        /// <param name="tile"></param>
        /// <param name="count"></param>
        public static void GenerateIdentical(byte gemId, Tile tile, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Gem gem = new Gem(tile.DrawRectangle.Center.X, tile.DrawRectangle.Y - Main.Tilesize / 2, gemId);
                GameWorld.Instance.Entities.Add(gem);
            }
        }
    }
}
