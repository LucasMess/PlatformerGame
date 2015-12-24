using Adam.Misc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    class BeingOfSight : Enemy
    {
        const int ProjSpeed = 5;
        float _rotation;

        public override byte Id
        {
            get
            {
                return 209;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDb.BeingOfSightMaxHealth;
            }
        }

        SoundFx _meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                return null;
            }
        }

        SoundFx _attackSound;
        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        SoundFx _deathSound;
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
                return CollRectangle;
            }
        }

        public BeingOfSight(int x, int y)
        {
            CollRectangle = new Rectangle(x, y, 32, 32);
            SourceRectangle = new Rectangle(0, 0, 16, 16);
            Texture = ContentHelper.LoadTexture("Void Shurinken/voisshurinken");
            Velocity = new Vector2(1, 1);

            CollidedWithTileAbove += OnCollisionWithTerrainAbove;
            CollidedWithTileBelow += OnCollisionWithTerrainBelow;
            CollidedWithTileToLeft += OnCollisionWithTerrainLeft;
            CollidedWithTileToRight += OnCollisionWithTerrainRight;

        }

        public override void Update()
        {
            _rotation += .05f;

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CollRectangle, SourceRectangle, Color.White, _rotation, new Vector2(8, 8), SpriteEffects.None, 0);
        }

        public void OnCollisionWithTerrainAbove(Entity entity, Tile tile)
        {
            Velocity.Y = -Velocity.Y;
            LinearProjectile proj = new FlyingWheelProjectile(DrawRectangle.X, DrawRectangle.Y, 0, ProjSpeed);
            GameWorld.Instance.Entities.Add(proj);
        }

        public void OnCollisionWithTerrainBelow(Entity entity, Tile tile)
        {
            Velocity.Y = -Velocity.Y;
            LinearProjectile proj = new FlyingWheelProjectile(DrawRectangle.X, DrawRectangle.Y, 0, -ProjSpeed);
            GameWorld.Instance.Entities.Add(proj);
        }

        public void OnCollisionWithTerrainLeft(Entity entity, Tile tile)
        {
            Velocity.X = -Velocity.X;
            LinearProjectile proj = new FlyingWheelProjectile(DrawRectangle.X, DrawRectangle.Y, ProjSpeed, 0);
            GameWorld.Instance.Entities.Add(proj);
        }

        public void OnCollisionWithTerrainRight(Entity entity, Tile tile)
        {
            Velocity.X = -Velocity.X;
            LinearProjectile proj = new FlyingWheelProjectile(DrawRectangle.X, DrawRectangle.Y, -ProjSpeed, 0);
            GameWorld.Instance.Entities.Add(proj);
        }
    }
}
