using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    class Duck : Enemy, ICollidable, INewtonian
    {

        public Duck(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Enemies/duck");
            collRectangle = new Rectangle(x, y, 32, 32);
            sourceRectangle = new Rectangle(0, 0, 16, 16);
            velocity.X = 1;
        }

        public override void Update()
        {
            if (velocity.X > 0) IsFacingRight = true;
            else IsFacingRight = false;

            base.Update();
        }

        public float GravityStrength { get; set; } = Main.Gravity;

        public bool IsAboveTile { get; set; }

        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public override byte ID
        {
            get
            {
                return 208;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDB.Duck_MaxHealth;
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                return null;
            }
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

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
