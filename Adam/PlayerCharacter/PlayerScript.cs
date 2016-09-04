﻿using Adam.Levels;
using Adam.Misc;
using Adam.Particles;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;

namespace Adam
{
    public class PlayerScript : Behavior
    {
        Player _player;
        public static bool IsDoingAction = false;
        private bool _isDucking;

        private const float MaxWalkVelX = 300f;
        private const float MaxRunVelX = 400f;
        private const float MoveJumpAcc = 3f;
        const float JumpAcc = -834f;
        const float WalkAcc = 30f;
        const float RunAcc = 42f;
        const float DashSpeed = 24000f;

        Timer _idleTimer = new Timer(true);
        Timer _airTimer = new Timer(true);
        Timer _weaponFireRateTimer = new Timer(true);

        public static Timer TimeSinceLastPunch = new Timer(true);

        SoundFx _stepSound = new SoundFx("Sounds/Movement/walk1");

        public void Initialize(Player player)
        {
            this._player = player;
            player.PlayerDamaged += OnPlayerDamaged;
            player.CollidedWithTileBelow += Player_CollidedWithTileBelow;
        }

        private void Player_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            entity.IsJumping = false;
            entity.RemoveAnimationFromQueue("fall");
            entity.RemoveAnimationFromQueue("jump");
        }

        protected override void OnGameTick()
        {
            _player = (Player)_player.Get();
        }

        private void OnPlayerDamaged(Rectangle damageArea, int damage)
        {
            _player.Sounds.Get("hurt").Play();
        }

        public void OnStill(Player player)
        {
            if (!player.IsOnVines)
            {
                player.RemoveAnimationFromQueue("climb");
            }
            else
            {
                player.RemoveAnimationFromQueue("fall");
            }

            // Toggle idle animations.

            if (_idleTimer.TimeElapsedInSeconds > 10)
            {
                player.AddAnimationToQueue("smellPoop");
                player.AnimationEnded += OnSmellPoopAnimationEnd;
                _idleTimer.Reset();
            }

            if (Math.Abs(player.GetVelocity().X) < 150f)
            {
                player.RemoveAnimationFromQueue("walk");
            }
            if (Math.Abs(player.GetVelocity().X) < 540)
            {
                player.RemoveAnimationFromQueue("run");
            }
            if (Math.Abs(player.GetVelocity().X) < 60)
            {
                player.RemoveAnimationFromQueue("slide");
            }

            if (player.GetVelocity().Y > 120)
            {
                player.IsJumping = true;

                if (player.GetVelocity().Y > 600)
                    player.AddAnimationToQueue("fall");
            }

        }

        private void OnSmellPoopAnimationEnd(Player player)
        {
            player.RemoveAnimationFromQueue("smellPoop");
            _idleTimer.Reset();
            player.AnimationEnded -= OnSmellPoopAnimationEnd;
        }

        public void OnJumpAction(Player player)
        {
            if (!player.IsJumping)
            {
                player.Sounds.Get("jump").Play();
                player.IsJumping = true;
                player.SetVelY(JumpAcc);
                player.ChangePosBy(0, -1);
                player.AddAnimationToQueue("jump");
                player.CollidedWithTileBelow += OnTouchGround;

                for (int i = 0; i < 10; i++)
                {
                    SmokeParticle par = new SmokeParticle(CalcHelper.GetRandomX(player.GetCollRectangle()), player.GetCollRectangle().Bottom, new Vector2(Main.Random.Next((int)player.GetVelocity().X - 1, (int)player.GetVelocity().X + 1) / 10f, -Main.Random.Next(60, 600) / 10f), Color.White);
                    GameWorld.ParticleSystem.Add(par);
                }


            }

            if (_airTimer.TimeElapsedInMilliSeconds < 1000)
            {
               // player.GravityStrength = Main.Gravity * .75f;
            }
            else
            {
                player.GravityStrength = Main.Gravity;
                player.GravityStrength = Main.Gravity * .75f;
            }
        }

        private void OnTouchGround(Entity entity, Tile tile)
        {
            _airTimer.Reset();
            entity.CollidedWithTileBelow -= OnTouchGround;
        }

        public void OnRightMove(Player player)
        {
            if (_isDucking)
                return;

            float acc = WalkAcc;
            if (player.IsRunningFast)
            {
                acc = RunAcc;
                player.AddAnimationToQueue("run");
            }

            if (!player.IsTouchingGround)
            {
                acc = MoveJumpAcc;
            }

            if (player.GetVelocity().X < -3 && player.IsRunningFast)
            {
                player.AddAnimationToQueue("slide");
            }

            player.IsFacingRight = true;
            player.SetVelX(player.GetVelocity().X + acc);

            if (player.IsRunningFast)
            {
                if (player.GetVelocity().X > MaxRunVelX)
                    player.SetVelX(MaxRunVelX);
            }
            else if (player.GetVelocity().X > MaxWalkVelX)
                player.SetVelX(MaxWalkVelX);

            player.CreateMovingParticles();

            if (player.CurrentAnimationFrame == 1 || player.CurrentAnimationFrame == 3)
            {
                _stepSound.PlayNewInstanceOnce();
            }
            else
            {
                _stepSound.Reset();
            }

            player.AddAnimationToQueue("walk");
        }

        public void OnLeftMove(Player player)
        {
            if (_isDucking)
                return;

            float acc = WalkAcc;
            if (player.IsRunningFast)
            {
                acc = RunAcc;
                player.AddAnimationToQueue("run");
            }

            if (!player.IsTouchingGround)
            {
                acc = MoveJumpAcc;
            }

            if (player.GetVelocity().X > 3 && player.IsRunningFast)
            {
                player.AddAnimationToQueue("slide");
            }

            player.IsFacingRight = false;
            player.SetVelX(player.GetVelocity().X - acc);
            player.CreateMovingParticles();

            if (player.IsRunningFast)
            {
                if (player.GetVelocity().X < -MaxRunVelX)
                    player.SetVelX(-MaxRunVelX);
            }
            else if (player.GetVelocity().X < -MaxWalkVelX)
                player.SetVelX(-MaxWalkVelX);

            if (player.CurrentAnimationFrame == 1 || player.CurrentAnimationFrame == 3)
            {
                _stepSound.PlayNewInstanceOnce();
            }
            else
            {
                _stepSound.Reset();
            }

            player.AddAnimationToQueue("walk");
        }

        public void OnInteractAction(Player player)
        {
            if (player.IsOnVines)
            {
                OnClimbingUpAction(player);
            }
        }

        public void OnClimbingUpAction(Player player)
        {
            player.AddAnimationToQueue("climb");
            player.SetVelY(-5);
            player.ObeysGravity = false;
        }

        public void OnClimbingDownAction(Player player)
        {
            player.AddAnimationToQueue("climb");
            player.SetVelY(5);
            player.ObeysGravity = false;
        }

        public void OnDuckAction(Player player)
        {
            if (player.IsOnVines)
            {
                OnClimbingDownAction(player);
            }
            player.AddAnimationToQueue("duck");
            _isDucking = true;
        }

        public void OnDuckActionStop(Player player)
        {
            player.RemoveAnimationFromQueue("duck");
            _isDucking = false;
        }

        public void OnWeaponFire(Player player)
        {
            if (_weaponFireRateTimer.TimeElapsedInMilliSeconds > 200)
            {
                new PlayerWeaponProjectile();
                _weaponFireRateTimer.Reset();
            }
        }

        public void OnAttackAction(Player player)
        {
            IsDoingAction = true;
            player.AttackSound.Play();
            player.AddAnimationToQueue("punch");
            player.AnimationEnded += OnPunchEnded;
            player.AnimationFrameChanged += OnPunchFrameChange;
            player.SetVelX(0);
        }

        private void OnPunchFrameChange(Player player)
        {
            if (player.CurrentAnimationFrame == 2)
            {
                int speed = 2;
                if (!player.IsFacingRight)
                    speed *= -1;
                player.SetVelX(speed);

                Rectangle punchHitBox = player.GetCollRectangle();
                punchHitBox.Width += 50;
                if (player.IsFacingRight)
                    punchHitBox.X += player.GetCollRectangle().Width / 2;
                else punchHitBox.X -= (punchHitBox.Width + player.GetCollRectangle().Width / 2);

                player.DealDamage(punchHitBox, 20);
                player.AnimationFrameChanged -= OnPunchFrameChange;
            }
        }

        private void OnPunchEnded(Player player)
        {
            player.RemoveAnimationFromQueue("punch");
            player.AnimationEnded -= OnPunchEnded;
            IsDoingAction = false;

            if (TimeSinceLastPunch.TimeElapsedInMilliSeconds < 325)
            {
                player.AttackSound.Play();
                player.AddAnimationToQueue("punch2");
                player.AnimationFrameChanged += OnPunchFrameChange;
                player.AnimationEnded += OnPunch2Ended;
                IsDoingAction = true;
            }
        }

        private void OnPunch2Ended(Player player)
        {
            player.RemoveAnimationFromQueue("punch2");
            player.AnimationEnded -= OnPunch2Ended;
            IsDoingAction = false;
        }

        public void OnDefendAction(Player player)
        {

        }

        public void OnDashAction(Player player)
        {
            if (!IsDoingAction)
            {
                IsDoingAction = true;
                float speed = DashSpeed;
                if (player.IsFacingRight)
                    speed = -DashSpeed;
                player.SetVelX(speed);
                player.AddAnimationToQueue("ninjaDash");
                player.AnimationEnded += OnNinjaDashEnd;
            }
        }

        private void OnNinjaDashEnd(Player player)
        {
            IsDoingAction = false;
            player.RemoveAnimationFromQueue("ninjaDash");
            player.AnimationEnded -= OnNinjaDashEnd;
        }

        public void OnUltimateAction(Player player)
        {

        }

        public void OnFastRunTrigger(Player player)
        {

        }

        public void OnDeath(Player player)
        {

        }

        public void ResetIdleTimer()
        {
            _idleTimer.Reset();
        }

    }
}
