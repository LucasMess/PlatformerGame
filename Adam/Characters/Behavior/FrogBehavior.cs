﻿using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Projectiles;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    /// <summary>
    /// Defines what the frog does along the game.
    /// </summary>
    public class FrogBehavior : Behavior
    {
        Timer _jumpTimer = new Timer(true);

        const int TimeBetweenJumps_ACTIVE = 200;
        const int TimeBetweenJumps_IDLE = 2000;
        const float JumpVel = -14f;
        const float MoveVel = 5;

        private bool _canSeePlayer;

        public override void Initialize(Entity entity)
        {
            _jumpTimer.ResetAndWaitFor(TimeBetweenJumps_IDLE);
            _jumpTimer.SetTimeReached += JumpTimer_SetTimeReached;

            entity.CurrentCollisionType = CollisionType.None;
            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;
            entity.AddAnimationToQueue("still");

            base.Initialize(entity);
        }

        public override void Update(Entity entity)
        {
            if (CollisionRay.IsEntityInSight(entity as Enemy, GameWorld.GetPlayer()))
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