using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;

namespace ThereMustBeAnotherWay.Projectiles
{

    public abstract class Projectile : Entity
    {
        public abstract void SpawnInitialBurst();
        public abstract void SpawnExplosion(Vector2 position);
        public abstract void SpawnParticles();
        public abstract void SpawnRibbon(int x);

        public Projectile()
        {
            Weight = 0;
            ObeysGravity = false;

            CollidedWithTerrain += Projectile_CollidedWithTerrain;
        }

        private void Projectile_CollidedWithTerrain(Entity entity, Tile tile)
        {
            Vector2 pos;

            if (tile.GetDrawRectangle().Center.X > Position.X)
            {
                pos.X = tile.GetDrawRectangle().X;
            }
            else
            {
                pos.X = tile.GetDrawRectangle().Right;
            }

            pos.Y = Center.Y;

            SpawnExplosion(pos);

            // Self-destruct.
            ProjectileSystem.Remove(this);
        }

        public int DamageRadius { get; set; } = ProjectileBehavior.DEFAULT_RADIUS;
        public int ParticleSpawnIntervalMilliSeconds { get; set; } = ProjectileBehavior.DEFAULT_PARTICLE_SPAWN_INTERVAL;
        public int RibbonWidth { get; set; } = ProjectileBehavior.DEFAULT_RIBBON_WIDTH;
        public int ExplositionWidth { get; set; } = ProjectileBehavior.DEFAULT_EXPLOSION_WIDTH;

        public bool CanCollideWithOtherProjectiles { get; set; } = ProjectileBehavior.DEFAULT_CAN_COLLIDE_OTHER_PROJECTILES;
        public abstract bool CanCollideWithPlayer { get; }
        public abstract bool CanCollideWithEnemy { get; }
        public abstract bool CanCollideWithNeutral { get; }

        private GameTimer _particleSpawnTimer = new GameTimer();

        private int _lastRibbonX = -1;

        public override void Update()
        {
            _particleSpawnTimer.Increment();

            if (_particleSpawnTimer.TimeElapsedInMilliSeconds > ParticleSpawnIntervalMilliSeconds)
            {
                _particleSpawnTimer.Reset();
                SpawnParticles();
            }

            CalculateRibbonSpawns();

            base.Update();
        }

        private void CalculateRibbonSpawns()
        {
            if (_lastRibbonX == -1)
                _lastRibbonX = (int)Position.X;

            int ribbonsMissing = (int)Math.Abs(_lastRibbonX - Position.X) / RibbonWidth;
            for (int i = 0; i < ribbonsMissing; i++)
            {
                if (Velocity.X > 0)
                {
                    _lastRibbonX += RibbonWidth;
                }
                else
                {
                    _lastRibbonX -= RibbonWidth;
                }

                SpawnRibbon(_lastRibbonX);
            }

        }

        protected override Rectangle DrawRectangle => CollRectangle;
    }
}

