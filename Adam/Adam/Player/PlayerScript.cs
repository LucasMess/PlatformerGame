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

        const float JumpVelocity = -8f;
        const float WalkSpeed = 5f;
        const float RunSpeed = 7f;
        const float GroundFriction = .97f;
        const float AirFriction = .98f;
        const float DashSpeed = 20f;

        Timer idleTimer = new Timer();

        SoundFx stepSound = new SoundFx("Sounds/Movement/walk1");


        bool isDashing = false;
    
        public void Initialize()
        {
            stepSound.MaxVolume = 1f;
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

            idleTimer.Increment();
            if (idleTimer.TimeElapsedInSeconds > 10)
            {
                player.AddAnimationToQueue("smellPoop");
                player.AnimationEnded += OnSmellPoopAnimationEnd;
            }
            else
            {
                player.AddAnimationToQueue("idle");
            }

            if (isDashing)
            {
                player.AddAnimationToQueue("ninjaDash");
            }

        }

        private void OnSmellPoopAnimationEnd(Player player)
        {
            player.AddAnimationToQueue("idle");
            idleTimer.Reset();
            player.AnimationEnded -= OnSmellPoopAnimationEnd;
        }

        public void OnJumpAction(Player player)
        {
            player.IsJumping = true;
            player.SetVelY(JumpVelocity);
            player.ChangePosBy(0, -1);
            player.AddAnimationToQueue("jump");
        }

        public void OnRightMove(Player player)
        {
            if (player.GetVelocity().X < 1)
                player.SetVelX(1f);

            float speed = WalkSpeed;
            if (player.isRunningFast)
                speed = RunSpeed;

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
                speed = RunSpeed;

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
                isDashing = true;
                player.SetVelX(DashSpeed);
                player.AnimationEnded += OnNinjaDashEnd;
            }
        }

        private void OnNinjaDashEnd(Player player)
        {
            isDashing = false;
            IsDoingAction = false;
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
            idleTimer.Reset();
        }


    }
}
