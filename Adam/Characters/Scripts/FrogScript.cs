using Adam.Characters.Enemies;
using Adam.Levels;
using Adam.Misc;
using Adam.Projectiles;
using Microsoft.Xna.Framework;

namespace Adam.Characters.Scripts
{
    /// <summary>
    /// Defines what the frog does along the game.
    /// </summary>
    public class FrogBehavior : Behavior
    {
        Timer _jumpTimer = new Timer(true);

        const int TimeBetweenJumps_ACTIVE = 200;
        const int TimeBetweenJumps_IDLE = 2000;
        const float JumpVel = -840f;
        const float MoveVel = 300f;

        private bool _canSeePlayer;

        public override void Initialize(Entity entity)
        {
            _jumpTimer.ResetAndWaitFor(TimeBetweenJumps_IDLE);
            _jumpTimer.SetTimeReached += JumpTimer_SetTimeReached;

            entity.CurrentCollisionType = CollisionType.None;
            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;
            entity.CollidedWithEntityAbove += Entity_CollidedWithEntityAbove;
            entity.UpdateCall += Entity_UpdateCall;
            entity.AddAnimationToQueue("still");

            base.Initialize(entity);
        }

        private void Entity_CollidedWithEntityAbove(Entity entity)
        {
            entity.SetVelY(0);
        }

        private void Entity_UpdateCall(Entity entity)
        {
            if (CollisionRay.IsPlayerInSight(entity as Enemy, GameWorld.GetPlayer()))
            {
                _jumpTimer.ChangeWaitTime(TimeBetweenJumps_ACTIVE);
                _canSeePlayer = true;
                entity.Color = Color.Red;
            }
            else
            {
                _jumpTimer.ChangeWaitTime(TimeBetweenJumps_IDLE);
                _canSeePlayer = false;
                entity.Color = Color.White;
            }

        }

        private void Entity_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            if (entity.IsJumping)
            {
                Enemy enemy = (Enemy)entity;
                enemy.IsCollidableWithEnemies = true;
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
                Enemy enemy = (Enemy)Entity;
                enemy.IsCollidableWithEnemies = false;

                Entity.ChangePosBy(0, -1);
                Entity.SetVelY(JumpVel);
                Entity.IsJumping = true;

                float speed = MoveVel;

                if (_canSeePlayer)
                {
                    if (!Entity.IsPlayerToRight())
                        speed *= -1;
                }
                else if (AdamGame.Random.Next(0, 2) == 0)
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
