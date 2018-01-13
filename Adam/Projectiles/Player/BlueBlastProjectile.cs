using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Particles;

namespace ThereMustBeAnotherWay.Projectiles.Player
{
    class BlueBlastProjectile : Projectile
    {
        public override bool CanCollideWithPlayer => false;

        public override bool CanCollideWithEnemy => true;

        public override bool CanCollideWithNeutral => false;

        private bool _particleAlternatingVar = false;

        public BlueBlastProjectile(Entity owner)
        {
            ParticleSpawnIntervalMilliSeconds = 10;
            RibbonWidth = 6;

            Position = owner.Position;
            Velocity = new Vector2(25, 0);
            if (!owner.IsFacingRight)
            {
                Velocity *= -1;
                IsFacingRight = false;
            }

            Light = new Light(Center, Color.White, 50);
            Texture = GameWorld.SpriteSheet;
            SourceRectangle = new Rectangle(256, 176, 16, 16);
            CollRectangle = new Rectangle(0, 0, 32, 32);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void SpawnExplosion(Vector2 position)
        {
            for (int i = 0; i < 50; i++)
                ParticleSystem.Add(ParticleType.TilePiece, position, null, Color.White);

            position -= new Vector2(24, 24);
            ParticleSystem.Add(ParticleType.BlueFireExplosion, position, null, Color.White);

        }

        public override void SpawnInitialBurst()
        {
            throw new NotImplementedException();
        }

        public override void SpawnParticles()
        {
            if (_particleAlternatingVar)
            {
                ParticleSystem.Add(ParticleType.BlueFire, Center, null, Color.White);
            }
            else
            {
                ParticleSystem.Add(ParticleType.BlueFire, Center, null, new Color(0, 246, 255));
            }
            _particleAlternatingVar = !_particleAlternatingVar;
        }

        public override void SpawnRibbon(int x)
        {
            Vector2 position = new Vector2(x, Center.Y - 8);
            ParticleSystem.Add(ParticleType.BlueFireRibbon, position, null, Color.White);
        }
    }
}
