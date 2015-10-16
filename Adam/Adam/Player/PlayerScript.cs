using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Player
{
    public class PlayerScript : Script
    {
        public static bool IsDoingAction = false;

        const float JumpVelocity = -8f;
        const float WalkAcceleration = .5f;
        const float RunAcceleration = .8f;
        const float GroundFriction = .97f;
        const float AirFriction = .98f;


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
        }

        public void OnJumpAction(Player player)
        {
            player.IsJumping = true;
            player.SetVelY(JumpVelocity);
        }

        public void OnRightMove(Player player)
        {
            if (player.GetVelocity().X < 1)
                player.SetVelX(1f);

            float acc = WalkAcceleration;
            if (player.isRunningFast)
                acc = RunAcceleration;

            player.SetVelX(acc * player.GetVelocity().X);
        }

        public void OnLeftMove(Player player)
        {
            if (player.GetVelocity().X > -1)
                player.SetVelX(-1f);

            float acc = WalkAcceleration;
            if (player.isRunningFast)
                acc = RunAcceleration;

            player.SetVelX(acc * player.GetVelocity().X);
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


    }
}
