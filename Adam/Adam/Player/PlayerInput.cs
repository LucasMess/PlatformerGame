using System;
using Adam.Characters;
using Microsoft.Xna.Framework.Input;

namespace Adam.Player
{
    public partial class Player : Character
    {
        public delegate void EventHandler();

        private static bool _hasInitialized;
        private readonly bool _isController = false;
        private bool _attackIsPressed;
        private bool _spaceIsPressed;
        public event EventHandler StillUpdate;
        public event EventHandler JumpAction;
        public event EventHandler StopJumpAction;
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
        public event EventHandler NotIdle;
        public event EventHandler ClimbingAction;

        private void InitializeInput()
        {
            if (!_hasInitialized)
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
                NotIdle += Player_NotIdle;
                ClimbingAction += Player_ClimbingAction;
            }
        }

        private void Player_ClimbingAction()
        {
            throw new NotImplementedException();
        }

        private void Player_NotIdle()
        {
            script.ResetIdleTimer();
        }

        private void Player_FastRunInactive()
        {
            IsRunningFast = false;
        }

        private void Player_StillUpdate()
        {
            script.OnStill(this);
        }

        private void Player_FastRunActive()
        {
            IsRunningFast = true;
        }

        private void Player_UltimateAction()
        {
        }

        private void Player_DashAction()
        {
            script.OnDashAction(this);
        }

        private void Player_DefendAction()
        {
        }

        private void Player_AttackAction()
        {
            script.OnAttackAction(this);
        }

        private void Player_DuckAction()
        {
            script.OnDuckAction(this);
        }

        private void Player_InteractAction()
        {
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
            script.OnJumpAction(this);
        }

        private void CheckInput()
        {
            if (_isController)
                UpdateWithController();
            else UpdateWithKeyboard();
        }

        /// <summary>
        ///     Check keyboard input and fire events according to what was pressed.
        /// </summary>
        private void UpdateWithKeyboard()
        {
            StillUpdate();

            if (InputHelper.IsKeyUp(Keys.H))
            {
                _attackIsPressed = false;
            }

            if (InputHelper.IsKeyDown(Keys.H) && !_attackIsPressed)
            {
                PlayerScript.TimeSinceLastPunch.Reset();
            }

            if (!PlayerScript.IsDoingAction)
            {
                if (InputHelper.IsKeyDown(Keys.A))
                    LeftMove();
                if (InputHelper.IsKeyDown(Keys.D))
                    RightMove();
                if (InputHelper.IsKeyDown(Keys.S))
                    DuckAction();
                if (InputHelper.IsKeyDown(Keys.W))
                    InteractAction();

                if (InputHelper.IsKeyDown(Keys.H))
                {
                    if (!_attackIsPressed)
                    {
                        AttackAction();
                        _attackIsPressed = true;
                    }
                }
                else
                {
                    _attackIsPressed = false;
                }


                if (InputHelper.IsKeyDown(Keys.J))
                    DefendAction();
                if (InputHelper.IsKeyDown(Keys.K))
                    DashAction();
                if (InputHelper.IsKeyDown(Keys.L))
                    UltimateAction();
                if (InputHelper.IsKeyDown(Keys.Space))
                {
                    if (!_spaceIsPressed)
                    {
                        JumpAction();
                        _spaceIsPressed = true;
                    }
                }
                else
                {
                    _spaceIsPressed = false;
                }
                if (InputHelper.IsKeyDown(Keys.LeftShift) || InputHelper.IsKeyDown(Keys.RightShift))
                    FastRunActive();
                else
                {
                    FastRunInactive();
                }
            }

            if (InputHelper.IsAnyInputPressed())
                NotIdle();
        }

        private void UpdateWithController()
        {
        }
    }
}