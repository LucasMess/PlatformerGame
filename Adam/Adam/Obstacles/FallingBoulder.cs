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
    class FallingBoulder : Obstacle, ICollidable
    {
        bool hasFallen;
        Vector2 velocity;
        Vector2 original;

        SoundEffect fallingSound;
        SoundEffectInstance fallingSoundInstance;

        /// <summary>
        /// This will add a boulder that has spikes that falls from its original heights until it reaches the floor. The boulder will then return to its
        /// starting position slowly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FallingBoulder(int x, int y)
        {
            texture = ContentHelper.LoadTexture("Tiles/Obstacles/mesa_boulder");
            fallingSound = ContentHelper.LoadSound("Sounds/Obstacles/boulder_smash");
            fallingSoundInstance = fallingSound.CreateInstance();

            drawRectangle = new Rectangle(x, y, Game1.Tilesize * 2, Game1.Tilesize * 2);
            collRectangle = drawRectangle;
            CurrentDamageType = DamageType.Bottom;
            IsCollidable = true;
            original = new Vector2(drawRectangle.X, drawRectangle.Y);

            // CollidedWithTerrainBelow += FallingBoulderObstacle_CollidedWithTerrainBelow;

        }

        void FallingBoulderObstacle_CollidedWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
            drawRectangle.Y = e.Tile.rectangle.Y - drawRectangle.Height - 1;
            hasFallen = true;
            fallingSoundInstance.Play();

            float xDist = player.collRectangle.Center.X - drawRectangle.Center.X;
            float yDist = player.collRectangle.Center.Y - drawRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);

            if (distanceTo > 1000)
                fallingSoundInstance.Volume = 0;
            else fallingSoundInstance.Volume = .5f - (distanceTo / 1000) / 2;
        }

        public override void Update(GameTime gameTime, Player player, GameWorld map)
        {
            base.Update(gameTime, player, map);

            drawRectangle.X += (int)velocity.X;
            drawRectangle.Y += (int)velocity.Y;

            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 15, collRectangle.Width, collRectangle.Height - 20);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, collRectangle.Width - 20, collRectangle.Height);

            collRectangle = drawRectangle;

            if (!hasFallen)
            {
                velocity.Y += .3f;
            }
            else
            {
                velocity.Y = -1f;
                if (drawRectangle.Y <= original.Y)
                {
                    hasFallen = false;
                    drawRectangle.Y = (int)original.Y + 1;
                    velocity.Y = 0;
                }
            }

            if (IsTouching)
            {
                player.PlayGoreSound();
                player.KillAndRespawn();
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), attackBox, Color.Red);
        }


        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
            drawRectangle.Y = e.Tile.rectangle.Y - drawRectangle.Height - 1;
            hasFallen = true;
            fallingSoundInstance.Play();

            float xDist = player.collRectangle.Center.X - drawRectangle.Center.X;
            float yDist = player.collRectangle.Center.Y - drawRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);

            if (distanceTo > 1000)
                fallingSoundInstance.Volume = 0;
            else fallingSoundInstance.Volume = .5f - (distanceTo / 1000) / 2;
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
