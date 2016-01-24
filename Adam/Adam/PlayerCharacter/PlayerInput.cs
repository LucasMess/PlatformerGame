using System;
using Adam.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adam.Player
{
    public partial class Player : Character
    {
        private static bool _hasInitialized;
        public bool IsController { get; set; }
        private bool _attackIsPressed;
        private bool _jumpButtonIsPressed;
        public event EventHandler StillUpdate;
        public event EventHandler JumpAction;
        public event EventHandler StopJumpAction;
        public event EventHandler RightMove;
        public event EventHandler LeftMove;
        public event EventHandler InteractAction;
        public event EventHandler DuckAction;
        public event EventHandler DuckActionStop;
        public event EventHandler AttackAction;
        public event EventHandler DefendAction;
        public event EventHandler DashAction;
        public event EventHandler UltimateAction;
        public event EventHandler FastRunActive;
        public event EventHandler FastRunInactive;
        public event EventHandler NotIdle;

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
                DuckActionStop += Player_DuckActionStop; ;
                AttackAction += Player_AttackAction;
                DefendAction += Player_DefendAction;
                DashAction += Player_DashAction;
                UltimateAction += Player_UltimateAction;
                FastRunActive += Player_FastRunActive;
                FastRunInactive += Player_FastRunInactive;
                NotIdle += Player_NotIdle;
                _hasInitialized = true;
            }
        }

        private void Player_DuckActionStop()
        {
            script.OnDuckActionStop(this);
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
            script.OnInteractAction(this);
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

        /// <summary>
        ///     Check keyboard input and fire events according to what was pressed.
        /// </summary>
        private void CheckInput()
        {
            if (Main.Dialog.IsActive)
                return;

            StillUpdate?.Invoke();

            if (!IsPunchPressed())
            {
                _attackIsPressed = false;
            }

            if (IsPunchPressed() && !_attackIsPressed)
            {
                PlayerScript.TimeSinceLastPunch.Reset();
            }

            if (!PlayerScript.IsDoingAction)
            {
                if (IsMoveLeftPressed())
                    LeftMove?.Invoke();
                if (IsMoveRightPressed())
                    RightMove?.Invoke();
                if (IsMoveDownPressed())
                    DuckAction?.Invoke();
                else DuckActionStop?.Invoke();
                if (IsInteractPressed())
                    InteractAction?.Invoke();

                if (IsPunchPressed())
                {
                    if (!_attackIsPressed)
                    {
                        AttackAction?.Invoke();
                        _attackIsPressed = true;
                    }
                }
                else
                {
                    _attackIsPressed = false;
                }


                if (InputHelper.IsKeyDown(Keys.J))
                    DefendAction?.Invoke();
                if (InputHelper.IsKeyDown(Keys.K))
                    DashAction?.Invoke();
                if (InputHelper.IsKeyDown(Keys.L))
                    UltimateAction?.Invoke();
                if (IsJumpButtonPressed())
                {
                    if (!_jumpButtonIsPressed)
                    {
                        JumpAction?.Invoke();
                        _jumpButtonIsPressed = true;
                    }
                }
                else
                {
                    _jumpButtonIsPressed = false;
                }
                if (IsSprinting())
                    FastRunActive?.Invoke();
                else
                {
                    FastRunInactive?.Invoke();
                }
            }

            if (InputHelper.IsAnyInputPressed())
                NotIdle?.Invoke();
        }

        private bool IsJumpButtonPressed()
        {
            return InputHelper.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A);
        }

        private bool IsMoveRightPressed()
        {
            return InputHelper.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight);
        }

        private bool IsMoveLeftPressed()
        {
            return InputHelper.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft);
        }

        private bool IsMoveDownPressed()
        {
            return InputHelper.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown);
        }

        private bool IsInteractPressed()
        {
            return InputHelper.IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y);
        }

        private bool IsPunchPressed()
        {
            return InputHelper.IsKeyDown(Keys.H) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X);
        }

        private bool IsSprinting()
        {
            return InputHelper.IsKeyDown(Keys.LeftShift) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder);
        }

    }
}