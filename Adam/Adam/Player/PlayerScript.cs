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

        const float JumpVelocity = -10f;
        const float WalkSpeed = 5f;
        const float RunSpeed = 7f;
        const float GroundFriction = .97f;
        const float AirFriction = .99f;
        const float DashSpeed = 20f;

        Timer idleTimer = new Timer();

        SoundFx stepSound = new SoundFx("Sounds/Movement/walk1");

    
        public void Initialize()
        {
        }

        public void OnStill(Player player)
        {
            // Friction.
            if (player.IsJumping)
            {
                player.SetVelX(player.GetVelocity().X * AirFriction);
                player.SetVelY(player.GetVelocity().Y * AirFriction);
            }
            else
            {
                player.SetVelX(player.GetVelocity().X * GroundFriction);
            }

            // Toggle idle animations.
            idleTimer.Increment();
            if (idleTimer.TimeElapsedInSeconds > 10)
            {
                player.AddAnimationToQueue("smellPoop");
                player.AnimationEnded += OnSmellPoopAnimationEnd;
                idleTimer.Reset();
            }

            if(Math.Abs(player.GetVelocity().X) < .5f)
            {
                player.RemoveAnimationFromQueue("walk");
            }
            if(Math.Abs(player.GetVelocity().X) < WalkSpeed)
            {
                player.RemoveAnimationFromQueue("run");
            }

        }

        private void OnSmellPoopAnimationEnd(Player player)
        {
            player.RemoveAnimationFromQueue("smellPoop");
            idleTimer.Reset();
            player.AnimationEnded -= OnSmellPoopAnimationEnd;
        }

        public void OnJumpAction(Player player)
        {
            if (!player.IsJumping)
            {
                player.IsJumping = true;
                player.SetVelY(JumpVelocity);
                player.ChangePosBy(0, -1);
                player.AddAnimationToQueue("jump");
                player.AnimationEnded += OnJumpEnded;
                player.CollidedWithTileBelow += OnTouchGround;
            }
        }

        private void OnTouchGround(Entity entity, Tile tile)
        {
            entity.IsJumping = false;
            entity.RemoveAnimationFromQueue("fall");
            entity.RemoveAnimationFromQueue("jump");
        }

        private void OnJumpEnded(Player player)
        {
            player.RemoveAnimationFromQueue("jump");
            if (player.IsJumping)
            {
                player.AddAnimationToQueue("fall");
            }
        }

        public void OnRightMove(Player player)
        {
            if (player.GetVelocity().X < 1)
                player.SetVelX(1f);

            float speed = WalkSpeed;
            if (player.isRunningFast)
            {
                speed = RunSpeed;
                player.AddAnimationToQueue("run");
            }

            player.IsFacingRight = false;
            player.SetVelX(speed);

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
            if (player.GetVelocity().X > -1)
                player.SetVelX(-1f);

            float speed = WalkSpeed;
            if (player.isRunningFast)
            {
                speed = RunSpeed;
                player.AddAnimationToQueue("run");
            }

            player.IsFacingRight = true;
            player.SetVelX(-speed);

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
