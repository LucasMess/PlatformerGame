using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.Projectiles
{
    public static class ProjectileSystem
    {
        private static List<Projectile> projectiles;
        private static List<Projectile> proj_canCollideOtherProjectiles;
        private static List<Projectile> proj_canCollideEnemy;
        private static List<Projectile> proj_canCollidePlayer;
        private static List<Projectile> proj_canCollideNeutral;

        private static List<Projectile> deadProjectiles;

        public static void Initialize()
        {
            projectiles = new List<Projectile>();
            proj_canCollideOtherProjectiles = new List<Projectile>();
            proj_canCollideEnemy = new List<Projectile>();
            proj_canCollidePlayer = new List<Projectile>();
            proj_canCollideNeutral = new List<Projectile>();
            deadProjectiles = new List<Projectile>();
        }

        public static void Add(Projectile projectile)
        {
            projectiles.Add(projectile);

            if (projectile.CanCollideWithOtherProjectiles)
                proj_canCollideOtherProjectiles.Add(projectile);

            if (projectile.CanCollideWithEnemy)
                proj_canCollideEnemy.Add(projectile);

            if (projectile.CanCollideWithPlayer)
                proj_canCollidePlayer.Add(projectile);

            if (projectile.CanCollideWithNeutral)
                proj_canCollideNeutral.Add(projectile);
        }

        public static void Remove(Projectile projectile)
        {
            deadProjectiles.Add(projectile);
        }

        public static void Update()
        {
            foreach (var proj in projectiles)
            {
                proj.Update();
            }

            CollectDeadProjectiles();

        }

        private static void CollectDeadProjectiles()
        {
            foreach (var projectile in deadProjectiles)
            {
                projectiles.Remove(projectile);

                if (projectile.CanCollideWithOtherProjectiles)
                    proj_canCollideOtherProjectiles.Remove(projectile);

                if (projectile.CanCollideWithEnemy)
                    proj_canCollideEnemy.Remove(projectile);

                if (projectile.CanCollideWithPlayer)
                    proj_canCollidePlayer.Remove(projectile);

                if (projectile.CanCollideWithNeutral)
                    proj_canCollideNeutral.Remove(projectile);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var proj in projectiles)
            {
                proj.Draw(spriteBatch);
            }
        }

        public static void DrawLights(SpriteBatch spriteBatch)
        {
            foreach (var proj in projectiles)
            {
                proj.DrawLight(spriteBatch);
            }
        }

        public static void DrawGlow(SpriteBatch spriteBatch)
        {
            foreach (var proj in projectiles)
            {
                proj.DrawGlow(spriteBatch);
            }
        }
    }
}
