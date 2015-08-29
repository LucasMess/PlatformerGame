using Adam.Misc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    class BeingOfSight : Enemy, ICollidable
    {
        const int projSpeed = 5;
        float rotation;

        public override byte ID
        {
            get
            {
                return 209;
            }
        }

        protected override int MaxHealth
        {
            get
            {
                return EnemyDB.BeingOfSight_MaxHealth;
            }
        }

        SoundFx meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                return null;
            }
        }

        SoundFx attackSound;
        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        SoundFx deathSound;
        protected override SoundFx DeathSound
        {
            get
            {
                return null;
            }
        }

        public BeingOfSight(int x, int y)
        {
            collRectangle = new Rectangle(x, y, 32, 32);
            drawRectangle = collRectangle;
            sourceRectangle = new Rectangle(0, 0,16, 16);
            Texture = ContentHelper.LoadTexture("Void Shurinken/voisshurinken");
            velocity = new Vector2(1,1);
            light = new Lights.DynamicPointLight(this, 1, false, Color.Red, 1);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);

        }

        public override void Update()
        {
            drawRectangle = collRectangle;
            rotation += .05f;

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, collRectangle, sourceRectangle, Color.White, rotation, new Vector2(8, 8), SpriteEffects.None, 0);
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            velocity.Y = -velocity.Y;
            LinearProjectile proj = new FlyingWheelProjectile(drawRectangle.X,drawRectangle.Y,0,projSpeed);
            GameWorld.Instance.entities.Add(proj);
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
            
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = -velocity.Y;
            LinearProjectile proj = new FlyingWheelProjectile(drawRectangle.X, drawRectangle.Y, 0, -projSpeed);
            GameWorld.Instance.entities.Add(proj);
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = -velocity.X;
            LinearProjectile proj = new FlyingWheelProjectile(drawRectangle.X, drawRectangle.Y, projSpeed, 0);
            GameWorld.Instance.entities.Add(proj);
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            velocity.X = -velocity.X;
            LinearProjectile proj = new FlyingWheelProjectile(drawRectangle.X, drawRectangle.Y, -projSpeed, 0);
            GameWorld.Instance.entities.Add(proj);
        }
    }
}
