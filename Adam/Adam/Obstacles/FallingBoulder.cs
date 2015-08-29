using Adam;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    public class FallingBoulder : Obstacle, ICollidable, INewtonian
    {
        bool hasFallen;
        Vector2 original;

        SoundEffect fallingSound;
        SoundEffectInstance fallingSoundInstance;

        public float GravityStrength { get; set; }

        public bool IsFlying { get; set; }

        public bool IsJumping
        {
            get; set;
        }

        public bool IsAboveTile
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public FallingBoulder(int x, int y)
        {
            GravityStrength = Main.Gravity ;
            Texture = GameWorld.SpriteSheet;
            fallingSound = ContentHelper.LoadSound("Sounds/Obstacles/boulder_smash");
            fallingSoundInstance = fallingSound.CreateInstance();

            collRectangle = new Rectangle(x, y, Main.Tilesize * 2, Main.Tilesize * 2);
            sourceRectangle = new Rectangle(12 * 16, 26 * 16, 32, 32);
            CurrentDamageType = DamageType.Bottom;
            IsCollidable = true;
            original = new Vector2(DrawRectangle.X, DrawRectangle.Y);
        }

        public override void Update(GameTime gameTime, Player player, GameWorld map)
        {
            base.Update(gameTime, player, map);

            if (IsTouching)
            {
                player.PlayGoreSound();
                player.KillAndRespawn();
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, sourceRectangle, Color.White);

            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), attackBox, Color.Red);
        }


        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
            GravityStrength = Main.Gravity;
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y =-0;
            GravityStrength = -Main.Gravity;
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
