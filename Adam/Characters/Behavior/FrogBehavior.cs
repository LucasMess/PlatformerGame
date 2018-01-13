using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Projectiles;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System.Collections;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    /// <summary>
    /// Defines what the frog does along the game.
    /// </summary>
    public class FrogBehavior : Behavior
    {
        const int TimeBetweenJumps_ACTIVE = 200;
        const int TimeBetweenJumps_IDLE = 2000;
        private Misc.GameTimer _jumpTimer;
        const float JumpVel = -14f;
        const float MoveVel = 5;

        private bool _canSeePlayer;

        public override void Initialize(Entity entity)
        {
            _jumpTimer = CreateTimedAction(TimeBetweenJumps_IDLE, Jump);

            entity.CurrentCollisionType = CollisionType.None;
            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;
            entity.AddAnimationToQueue("still");

            base.Initialize(entity);
        }

        public override void Update(Entity entity)
        {
            if (CollisionRay.IsEntityInSight(entity as Enemy, GameWorld.GetPlayers()[0]))
            {
                _jumpTimer.ChangeWaitTime(TimeBetweenJumps_ACTIVE);
                _canSeePlayer = true;
            }
            else
            {
                _jumpTimer.ChangeWaitTime(TimeBetweenJumps_IDLE);
                _canSeePlayer = false;
            }
            base.Update(entity);
        }

        private void Entity_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            if (entity.IsJumping)
            {
                Enemy enemy = (Enemy)entity;
                enemy.IsCollidableWithEnemies = true;
                entity.SetVelY(0);
                entity.RemoveAnimationFromQueue("jump");
                entity.IsJumping = false;
            }
        }

        private void Jump()
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
                    if (!enemy.IsPlayerToTheRight())
                        speed *= -1;
                }
                else if (TMBAW_Game.Random.Next(0, 2) == 0)
                {
                    speed *= -1;
                }

                Entity.SetVelX(speed);
                Entity.AddAnimationToQueue("jump");
                Entity.Sounds.GetSoundRef("jump").Play();
            }
        }









    }
}
