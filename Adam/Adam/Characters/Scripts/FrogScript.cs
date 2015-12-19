using Adam.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Scripts
{
    public class FrogScript : Script
    {
        Timer jumpTimer = new Timer();

        double timeBetweenJumps = 2000 * GameWorld.RandGen.NextDouble();
        const float JumpVel = -15f;
        const float MoveVel = 5f;

        public override void Initialize(Entity entity)
        {
            jumpTimer.ResetAndWaitFor(timeBetweenJumps);
            jumpTimer.SetTimeReached += JumpTimer_SetTimeReached;

            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;
            entity.AddAnimationToQueue("still");

            base.Initialize(entity);
        }

        private void Entity_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            if (entity.IsJumping)
            {
                entity.SetVelY(0);
                jumpTimer.Reset();
                entity.RemoveAnimationFromQueue("jump");
                entity.IsJumping = false;
            }
        }

        private void JumpTimer_SetTimeReached()
        {
            if (!entity.IsJumping)
            {
                entity.ChangePosBy(0, -1);
                entity.SetVelY(JumpVel);
                entity.IsJumping = true;

                float speed = MoveVel;
                if (GameWorld.RandGen.Next(0, 2) == 0)
                {
                    speed *= -1;
                }
                entity.SetVelX(speed);
                entity.AddAnimationToQueue("jump");
                entity.Sounds.Get("jump").Play();
            }
        }

        protected override void OnGameTick()
        {
            jumpTimer.Increment();

            base.OnGameTick();
        }









    }
}
