using Adam.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Characters.Scripts
{
    public class FrogScript : Script
    {
        Timer _jumpTimer = new Timer(true);

        int _timeBetweenJumps = 2000;
        const float JumpVel = -14f;
        const float MoveVel = 5f;

        public override void Initialize(Entity entity)
        {
            _jumpTimer.ResetAndWaitFor(_timeBetweenJumps);
            _jumpTimer.SetTimeReached += JumpTimer_SetTimeReached;

            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;
            entity.AddAnimationToQueue("still");

            base.Initialize(entity);
        }

        private void Entity_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            if (entity.IsJumping)
            {
                entity.SetVelY(0);
                _jumpTimer.Reset();
                entity.RemoveAnimationFromQueue("jump");
                entity.IsJumping = false;
            }
        }

        private void JumpTimer_SetTimeReached()
        {
            if (!Entity.IsJumping)
            {
                Entity.ChangePosBy(0, -1);
                Entity.SetVelY(JumpVel);
                Entity.IsJumping = true;

                float speed = MoveVel;
                if (GameWorld.RandGen.Next(0, 2) == 0)
                {
                    speed *= -1;
                }
                Entity.SetVelX(speed);
                Entity.AddAnimationToQueue("jump");
                Entity.Sounds.Get("jump").Play();
            }
        }









    }
}
