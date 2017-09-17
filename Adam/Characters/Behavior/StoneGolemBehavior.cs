using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Projectiles;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    class StoneGolemBehavior : Behavior
    {
        public override void Initialize(Entity entity)
        {
            entity.AddAnimationToQueue("idle");
            base.Initialize(entity);
        }

        public override void Update(Entity entity)
        {
            StoneGolem golem = entity as StoneGolem;
            if (CollisionRay.IsEntityInSight(entity, GameWorld.GetPlayers()[0]))
            {
                float velX = 1.0f;
                golem.IsFacingRight =false;
                if (!golem.IsPlayerToTheRight())
                {
                    velX *= -1;
                    golem.IsFacingRight = true;
                }
                golem.SetVelX(velX);
                entity.AddAnimationToQueue("walk");
            }
            else
            {
                entity.RemoveAnimationFromQueue("walk");
            }
            base.Update(entity);
        }
    }
}
