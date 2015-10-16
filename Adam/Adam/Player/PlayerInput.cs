using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Player
{
    public partial class Player
    {
        bool isController = false;

        PlayerScript script = new PlayerScript();

        public delegate void EventHandler();

        public event EventHandler StillUpdate;
        public event EventHandler JumpAction;
        public event EventHandler RightMove;
        public event EventHandler LeftMove;
        public event EventHandler InteractAction;
        public event EventHandler DuckAction;
        public event EventHandler AttackAction;
        public event EventHandler DefendAction;
        public event EventHandler DashAction;
        public event EventHandler UltimateAction;
        public event EventHandler FastRunActive;
        public event EventHandler FastRunInactive;

        private void InitializeInput()
        {
            StillUpdate += Player_StillUpdate;
            JumpAction += Player_JumpAction;
            RightMove += Player_RightMove;
            LeftMove += Player_LeftMove;
            InteractAction += Player_InteractAction;
            DuckAction += Player_DuckAction;
            AttackAction += Player_AttackAction;
            DefendAction += Player_DefendAction;
            DashAction += Player_DashAction;
            UltimateAction += Player_UltimateAction;
            FastRunActive += Player_FastRunActive;
            FastRunInactive += Player_FastRunInactive;
        }

        private void Player_FastRunInactive()
        {
            isRunningFast = false;
        }

        private void Player_StillUpdate()
        {
            script.OnStill(this);
        }

        private void Player_FastRunActive()
        {
            isRunningFast = true;
        }

        private void Player_UltimateAction()
        {
            throw new NotImplementedException();
        }

        private void Player_DashAction()
        {
            throw new NotImplementedException();
        }

        private void Player_DefendAction()
        {
            throw new NotImplementedException();
        }

        private void Player_AttackAction()
        {
            throw new NotImplementedException();
        }

        private void Player_DuckAction()
        {
            throw new NotImplementedException();
        }

        private void Player_InteractAction()
        {
            throw new NotImplementedException();
        }

        private void Player_LeftMove()
        {
            script.OnLeftMove(this);
        }

        private void Player_RightMove()
        {
            script.OnRightMove(this);
        }

        private void Player_JumpAction()
        {
            throw new NotImplementedException();
        }

        private void CheckInput()
        {
            if (isController)
                UpdateWithController();
            else UpdateWithKeyboard();
        }

        /// <summary>
        /// Check keyboard input and fire events according to what was pressed.
        /// </summary>
        private void UpdateWithKeyboard()
        {
            StillUpdate();

            if (InputHelper.IsKeyDown(Keys.A))
                LeftMove();
            if (InputHelper.IsKeyDown(Keys.D))
                RightMove();
            if (InputHelper.IsKeyDown(Keys.S))
                DuckAction();
            if (InputHelper.IsKeyDown(Keys.W))
                InteractAction();
            if (InputHelper.IsKeyDown(Keys.H))
                AttackAction();
            if (InputHelper.IsKeyDown(Keys.J))
                DefendAction();
            if (InputHelper.IsKeyDown(Keys.K))
                DashAction();
            if (InputHelper.IsKeyDown(Keys.L))
                UltimateAction();
            if (InputHelper.IsKeyDown(Keys.LeftShift) || InputHelper.IsKeyDown(Keys.RightShift))
                FastRunActive();
            else
            {
                FastRunInactive();
            }
        }

        private void UpdateWithController()
        {

        }


    }
}
