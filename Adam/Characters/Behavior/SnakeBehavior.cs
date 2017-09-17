using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    class SnakeBehavior : Behavior
    {
        Timer attackTimer = new Timer();
        const int TimeBetweenAttacks = 3000;

        public override void Initialize(Entity entity)
        {
            entity.AddAnimationToQueue("still");


            base.Initialize(entity);
        }

        public override void Update(Entity entity)
        {
            attackTimer.Increment();
            if (attackTimer.TimeElapsedInMilliSeconds > TimeBetweenAttacks)
            {
                attackTimer.Reset();
                entity.AddAnimationToQueue("attack");
                entity.ComplexAnimation.AnimationEnded += ComplexAnimation_AnimationEnded;

                float velocityY = TMBAW_Game.Random.Next(-200, -180) / 10f;
                float velocityX = (GameWorld.GetPlayers()[0].CollRectangle.X - entity.GetCollRectangle().X) / 30f;

                Projectile projectile = new Projectile(Projectile.Type.SnakeVenom, entity.Position + new Vector2(16, 13) * 2, new Vector2(velocityX, velocityY), entity);
                GameWorld.EnemyProjectiles.Add(projectile);
            }

            base.Update(entity);
        }

        private void ComplexAnimation_AnimationEnded()
        {
            Entity.ComplexAnimation.AnimationEnded -= ComplexAnimation_AnimationEnded;
            Entity.RemoveAnimationFromQueue("attack");
        }
    }
}
