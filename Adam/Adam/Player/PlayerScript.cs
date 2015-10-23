using Adam.Misc;
using Adam.Misc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class PlayerScript : Script
    {
        public static bool IsDoingAction = false;

        const float JumpAcc = -10f;
        const float WalkAcc = .2f;
        const float RunAcc = .45f;
        const float GroundFriction = .96f;
        const float DashSpeed = 40f;

        Timer idleTimer = new Timer();
        Timer airTimer = new Timer();

        SoundFx stepSound = new SoundFx("Sounds/Movement/walk1");


        public void Initialize()
        {
        }

        public void OnStill(Player player)
        {
            // Friction.
            if (player.IsJumping)
            {
                airTimer.Increment();
            }

            player.SetVelX(player.GetVelocity().X * GroundFriction);

            // Toggle idle animations.
            idleTimer.Increment();
            if (idleTimer.TimeElapsedInSeconds > 10)
            {
                player.AddAnimationToQueue("smellPoop");
                player.AnimationEnded += OnSmellPoopAnimationEnd;
                idleTimer.Reset();
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

        private void OnSmellPoopAnimationEnd(Player player)
        {
            player.RemoveAnimationFromQueue("smellPoop");
            idleTimer.Reset();
            player.AnimationEnded -= OnSmellPoopAnimationEnd;
        }

        internal void StopJumpAction(Player player)
        {
            player.GravityStrength = Main.Gravity;
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
            }

            if (airTimer.TimeElapsedInMilliSeconds < 1000)
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
            airTimer.Reset();
            entity.IsJumping = false;
            entity.RemoveAnimationFromQueue("fall");
            entity.RemoveAnimationFromQueue("jump");
        }

        public void OnRightMove(Player player)
        {
            float acc = WalkAcc;
            if (player.IsRunningFast)
            {
                acc = RunAcc;
                player.AddAnimationToQueue("run");
            }

            player.IsFacingRight = false;
            player.SetVelX(player.GetVelocity().X + acc);

            if (player.CurrentAnimationFrame == 1 || player.CurrentAnimationFrame == 3)
            {
                stepSound.PlayNewInstanceOnce();
            }
            else
            {
                stepSound.Reset();
            }

            player.AddAnimationToQueue("walk");
        }

        public void OnLeftMove(Player player)
        {
            float acc = WalkAcc;
            if (player.IsRunningFast)
            {
                acc = RunAcc;
                player.AddAnimationToQueue("run");
            }

            player.IsFacingRight = true;
            player.SetVelX(player.GetVelocity().X - acc);

            if (player.CurrentAnimationFrame == 1 || player.CurrentAnimationFrame == 3)
            {
                stepSound.PlayNewInstanceOnce();
            }
            else
            {
                stepSound.Reset();
            }

            player.AddAnimationToQueue("walk");
        }

        public void OnInteractAction(Player player)
        {

        }

        public void OnDuckAction(Player player)
        {

        }

        public void OnAttackAction(Player player)
        {

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
                TestSmokeParticle.Generate(100, player);
            }
        }

        private void OnNinjaDashEnd(Player player)
        {
            IsDoingAction = false;
            player.RemoveAnimationFromQueue("ninjaDash");
            player.AnimationEnded -= OnNinjaDashEnd;
            TestSmokeParticle.Generate(100, player);
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
            idleTimer.Reset();
        }


    }
}
