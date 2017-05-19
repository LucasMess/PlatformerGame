using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    class HellboarBehavior : Behavior
    {
        bool isAngry;
        bool isRunningToPlayer;
        bool firingUp;
        bool playerToRightWhenAngry;
        Timer chargeUpTimer = new Timer();

        public override void Initialize(Entity entity)
        {

            chargeUpTimer.SetTimeReached += ChargeUpTimer_SetTimeReached;
            entity.CollidedWithTileToLeft += Entity_CollidedWithTileToLeft;
            entity.CollidedWithTileToRight += Entity_CollidedWithTileToRight;
            entity.AddAnimationToQueue("still");

            base.Initialize(entity);

        }

        private void Entity_CollidedWithTileToRight(Entity entity, Tile tile)
        {
            Stun(entity);
        }

        private void Entity_CollidedWithTileToLeft(Entity entity, Tile tile)
        {
            Stun(entity);
        }

        private void ChargeUpTimer_SetTimeReached()
        {
            isRunningToPlayer = true;
            playerToRightWhenAngry = Entity.IsPlayerToRight();
        }

        private void Stun(Entity entity)
        {
            isRunningToPlayer = false;
            isAngry = false;
            firingUp = false;
            entity.SetVelX(0);
            entity.SetVelY(0);
            entity.ComplexAnimation.RemoveAllFromQueue();
            entity.AddAnimationToQueue("still");
        }

        public override void Update(Entity entity)
        {
            if (CollisionRay.IsEntityInSight(entity, GameWorld.GetPlayer()) && !isAngry)
            {
                isAngry = true;
                chargeUpTimer.ResetAndWaitFor(1000);
                entity.IsFacingRight = !entity.IsPlayerToRight();
            }

            if (isAngry)
            {
                if (!firingUp)
                {
                    firingUp = true;
                    entity.AddAnimationToQueue("angry");
                    entity.Sounds.GetSoundRef("scream").PlayIfStopped();
                    entity.Sounds.GetSoundRef("fire").PlayIfStopped();
                }
            }
            if (firingUp)
            {
                chargeUpTimer.Increment();
                entity.Sounds.GetSoundRef("fire").PlayIfStopped();
            }

            if (isRunningToPlayer)
            {
                chargeUpTimer.Reset();
                entity.AddAnimationToQueue("charge");
                entity.Sounds.GetSoundRef("fire").PlayIfStopped();
                if (playerToRightWhenAngry)
                {
                    entity.SetVelX(5);
                }
                else
                {
                    entity.SetVelX(-5);
                }
                entity.IsFacingRight = !playerToRightWhenAngry;
            }


            base.Update(entity);
        }
    }
}
