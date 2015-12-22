﻿using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class PlayerScript : Script
    {
        Player.Player _player;
        public static bool IsDoingAction = false;

        const float JumpAcc = -17f;
        const float WalkAcc = .13f;
        const float RunAcc = .20f;
        const float DashSpeed = 40f;

        Timer _idleTimer = new Timer();
        Timer _airTimer = new Timer();

        public static Timer TimeSinceLastPunch = new Timer();

        SoundFx _stepSound = new SoundFx("Sounds/Movement/walk1");

        public void Initialize(Player.Player player)
        {
            this._player = player;
            player.PlayerDamaged += OnPlayerDamaged;
            player.CollidedWithTileBelow += Player_CollidedWithTileBelow;
        }

        private void Player_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            entity.IsJumping = false;
        }

        protected override void OnGameTick()
        {
            _player = (Player.Player)_player.Get();
        }

        private void OnPlayerDamaged(Rectangle damageArea, int damage)
        {
            _player.Sounds.Get("hurt").Play();
        }

        public void OnStill(Player.Player player)
        {
            // Friction.
            if (player.IsJumping)
            {
                
            }

            // Toggle idle animations.
           
            if (_idleTimer.TimeElapsedInSeconds > 10)
            {
                player.AddAnimationToQueue("smellPoop");
                player.AnimationEnded += OnSmellPoopAnimationEnd;
                _idleTimer.Reset();
            }

            if (Math.Abs(player.GetVelocity().X) < 2f)
            {
                player.RemoveAnimationFromQueue("walk");
            }
            if (Math.Abs(player.GetVelocity().X) < 9)
            {
                player.RemoveAnimationFromQueue("run");
            }

            if (player.GetVelocity().Y > 2)
            {
                player.IsJumping = true;

                if (player.GetVelocity().Y > 10)
                    player.AddAnimationToQueue("fall");
            }

        }

        private void OnSmellPoopAnimationEnd(Player.Player player)
        {
            player.RemoveAnimationFromQueue("smellPoop");
            _idleTimer.Reset();
            player.AnimationEnded -= OnSmellPoopAnimationEnd;
        }

        internal void StopJumpAction(Player.Player player)
        {
            player.GravityStrength = Main.Gravity;
        }

        public void OnJumpAction(Player.Player player)
        {
            if (!player.IsJumping)
            {
                player.Sounds.Get("jump").Play();
                player.IsJumping = true;
                player.SetVelY(JumpAcc);
                player.ChangePosBy(0, -1);
                player.AddAnimationToQueue("jump");
                player.CollidedWithTileBelow += OnTouchGround;
            }

            if (_airTimer.TimeElapsedInMilliSeconds < 1000)
            {
                player.GravityStrength = Main.Gravity * .75f;
            }
            else
            {
                player.GravityStrength = Main.Gravity;
            }
        }

        private void OnTouchGround(Entity entity, Tile tile)
        {
            _airTimer.Reset();
            entity.IsJumping = false;
            entity.RemoveAnimationFromQueue("fall");
            entity.RemoveAnimationFromQueue("jump");
        }

        public void OnRightMove(Player.Player player)
        {
            float acc = WalkAcc;
            if (player.IsRunningFast)
            {
                acc = RunAcc;
                player.AddAnimationToQueue("run");
            }

            if (player.IsJumping)
            {
                acc /= 2;
            }

            player.IsFacingRight = true;
            player.SetVelX(player.GetVelocity().X + acc);
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

        public void OnLeftMove(Player.Player player)
        {
            float acc = WalkAcc;
            if (player.IsRunningFast)
            {
                acc = RunAcc;
                player.AddAnimationToQueue("run");
            }

            player.IsFacingRight = false;
            player.SetVelX(player.GetVelocity().X - acc);
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

        public void OnInteractAction(Player.Player player)
        {

        }

        public void OnClimbingAction(Player.Player player)
        {
            player.AddAnimationToQueue("climb");
            player.SetVelY(-10);
            player.ObeysGravity = false;
        }

        public void OnDuckAction(Player.Player player)
        {
            player.AddAnimationToQueue("standup");
            player.AnimationEnded += OnStandUpEnd;
            player.AddAnimationToQueue("fightIdle");
        }

        private void OnStandUpEnd(Player.Player player)
        {
            player.RemoveAnimationFromQueue("standup");
        }

        public void OnAttackAction(Player.Player player)
        {
            IsDoingAction = true;
            player.AttackSound.Play();
            player.AddAnimationToQueue("punch");
            player.AnimationEnded += OnPunchEnded;
            player.AnimationFrameChanged += OnPunchFrameChange;
            player.SetVelX(0);
        }

        private void OnPunchFrameChange(Player.Player player)
        {
            if(player.CurrentAnimationFrame == 2){
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

        private void OnPunchEnded(Player.Player player)
        {
            player.RemoveAnimationFromQueue("punch");
            player.AnimationEnded -= OnPunchEnded;
            IsDoingAction = false;

            if (TimeSinceLastPunch.TimeElapsedInMilliSeconds < 100)
            {
                player.AttackSound.Play();
                player.AddAnimationToQueue("punch2");
                player.AnimationFrameChanged += OnPunchFrameChange;
                player.AnimationEnded += OnPunch2Ended;
                IsDoingAction = true;
            }
        }

        private void OnPunch2Ended(Player.Player player)
        {
            player.RemoveAnimationFromQueue("punch2");
            player.AnimationEnded -= OnPunch2Ended;
            IsDoingAction = false;
        }

        public void OnDefendAction(Player.Player player)
        {

        }

        public void OnDashAction(Player.Player player)
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
                TestSmokeParticle.Generate(100, player);
            }
        }

        private void OnNinjaDashEnd(Player.Player player)
        {
            IsDoingAction = false;
            player.RemoveAnimationFromQueue("ninjaDash");
            player.AnimationEnded -= OnNinjaDashEnd;
            TestSmokeParticle.Generate(100, player);
        }

        public void OnUltimateAction(Player.Player player)
        {

        }

        public void OnFastRunTrigger(Player.Player player)
        {

        }

        public void OnDeath(Player.Player player)
        {

        }

        public void ResetIdleTimer()
        {
            _idleTimer.Reset();
        }

    }
}
