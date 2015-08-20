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

        public FallingBoulder(int x, int y)
        {
            GravityStrength = Main.Gravity ;
            texture = GameWorld.SpriteSheet;
            fallingSound = ContentHelper.LoadSound("Sounds/Obstacles/boulder_smash");
            fallingSoundInstance = fallingSound.CreateInstance();

            drawRectangle = new Rectangle(x, y, Main.Tilesize * 2, Main.Tilesize * 2);
            collRectangle = drawRectangle;
            sourceRectangle = new Rectangle(12 * 16, 26 * 16, 32, 32);
            CurrentDamageType = DamageType.Bottom;
            IsCollidable = true;
            original = new Vector2(drawRectangle.X, drawRectangle.Y);
        }

        public override void Update(GameTime gameTime, Player player, GameWorld map)
        {
            base.Update(gameTime, player, map);

            drawRectangle = collRectangle;


            if (IsTouching)
            {
                player.PlayGoreSound();
                player.KillAndRespawn();
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White);

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
