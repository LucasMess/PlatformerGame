using Adam;
using Adam.Misc;
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
        int originalY;

        SoundFx fallingSound;

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
            fallingSound = new SoundFx("Sounds/Boulder/boulder_fall", this);   

            collRectangle = new Rectangle(x, y, Main.Tilesize * 2, Main.Tilesize * 2);
            sourceRectangle = new Rectangle(12 * 16, 26 * 16, 32, 32);
            CurrentDamageType = DamageType.Bottom;
            IsCollidable = true;
            originalY = DrawRectangle.Y;
        }

        public override void Update()
        {
            base.Update();

            if (IsTouchingPlayer)
            {
                Player player = GameWorld.Instance.player;
                player.PlayGoreSound();
                player.KillAndRespawn();
            }

            // If hit ground go back up slowly.
            if (hasFallen)
            {
                velocity.Y = -1f;
            }
            else
            {
                GravityStrength = Main.Gravity;
            }

            if (collRectangle.Y < originalY && hasFallen)
            {
                fallingSound.Reset();
                hasFallen = false;
                velocity.Y = 0;
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
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
            GravityStrength = 0;
            hasFallen = true;

            StompSmokeParticle.Generate(10, this);
            fallingSound.PlayNewInstanceOnce();
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
