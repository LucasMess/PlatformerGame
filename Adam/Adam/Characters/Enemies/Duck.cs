using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Characters.Enemies
{
    class Duck : Enemy, ICollidable, INewtonian
    {

        public Duck(int x, int y)
        {
            texture = ContentHelper.LoadTexture("Enemies/duck");
            collRectangle = new Rectangle(x, y, 32, 32);
            drawRectangle = collRectangle;
            sourceRectangle = new Rectangle(0, 0, 16, 16);
            velocity.X = 1;
            health = 100;

            Initialize();
        }

        public override void Update(Player player, GameTime gameTime)
        {
            drawRectangle = collRectangle;

            if (velocity.X > 0) isFacingRight = true;
            else isFacingRight = false;

            base.Update(player, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }


        public float GravityStrength { get; set; } = Main.Gravity;

        public bool IsAboveTile { get; set; }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
           
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = -velocity.X;
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            velocity.X = -velocity.X;  
        }
    }
}
