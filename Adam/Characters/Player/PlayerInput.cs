﻿using ThereMustBeAnotherWay.Characters;
using ThereMustBeAnotherWay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ThereMustBeAnotherWay.PlayerCharacter
{
    public partial class Player : Character
    {
        private static bool _hasInitialized;
        public bool IsController { get; set; }
        private bool _attackIsPressed;
        private bool _jumpButtonIsPressed;
        public event EventHandler StillUpdate;
        public event EventHandler JumpAction;
        public event EventHandler RightMove;
        public event EventHandler LeftMove;
        public event EventHandler InteractAction;
        public event EventHandler DuckAction;
        public event EventHandler DuckActionStop;
        public event EventHandler FireWeaponAction;
        public event EventHandler RewindAction;
        public event EventHandler FastRunActive;
        public event EventHandler FastRunInactive;
        public event EventHandler NotIdle;
        public event EventHandler ReleaseUpAndDownAction;

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
                FireWeaponAction += Player_FireWeaponAction;
                ReleaseUpAndDownAction += Player_ReleaseUpAndDownAction;
                //AttackAction += Player_AttackAction;
                //DefendAction += Player_DefendAction;
                //DashAction += Player_DashAction;
                //UltimateAction += Player_UltimateAction;
                RewindAction += Player_RewindAction;
                FastRunActive += Player_FastRunActive;
                FastRunInactive += Player_FastRunInactive;
                NotIdle += Player_NotIdle;
                _hasInitialized = true;
            }
        }

        private void Player_RewindAction()
        {
            script.OnRewindAction(this);
        }

        private void Player_ReleaseUpAndDownAction()
        {
            script.OnUpAndDownReleased(this);
        }

        private void Player_FireWeaponAction()
        {
            script.OnWeaponFire(this);
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
            if (TMBAW_Game.CurrentGameMode == GameMode.None) return;

            if (TMBAW_Game.Dialog.IsActive)
                return;

            if (StoryTracker.InCutscene)
                return;

            if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
                return;

            if (IsPunchPressed() && !_attackIsPressed)
            {
                PlayerScript.TimeSinceLastPunch.Reset();
            }

            if (!PlayerScript.IsDoingAction)
            {
                if (TMBAW_Game.CurrentGameMode == GameMode.Play)
                    if (IsRewindPressed())
                    {
                        RewindAction?.Invoke();
                    }
                if (IsMoveLeftPressed())
                    LeftMove?.Invoke();
                if (IsMoveRightPressed())
                    RightMove?.Invoke();
                if (IsMoveDownPressed())
                    DuckAction?.Invoke();
                else DuckActionStop?.Invoke();
                if (IsInteractPressed())
                    InteractAction?.Invoke();

                if (!IsMoveDownPressed() && !IsInteractPressed())
                    ReleaseUpAndDownAction?.Invoke();

                if (IsPunchPressed())
                {
                    if (!_attackIsPressed)
                    {
                        //FireWeaponAction?.Invoke();
                        _attackIsPressed = true;
                    }
                }
                else
                {
                    _attackIsPressed = false;
                }

                if (IsWeaponFirePressed())
                    FireWeaponAction?.Invoke();

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
                if (IsSprintButtonPressed())
                    FastRunActive?.Invoke();
                else
                {
                    FastRunInactive?.Invoke();
                }
            }


            StillUpdate?.Invoke();

            if (InputHelper.IsAnyInputPressed())
                NotIdle?.Invoke();
        }

        public bool IsJumpButtonPressed()
        {
            return InputHelper.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A);
        }

        public bool IsMoveRightPressed()
        {
            return InputHelper.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight);
        }

        public bool IsMoveLeftPressed()
        {
            return InputHelper.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft);
        }

        public bool IsMoveDownPressed()
        {
            return InputHelper.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown);
        }

        public bool IsInteractPressed()
        {
            return InputHelper.IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp);
        }

        public bool IsPunchPressed()
        {
            return InputHelper.IsKeyDown(Keys.H) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X);
        }

        public bool IsSprintButtonPressed()
        {
            return InputHelper.IsKeyDown(Keys.LeftShift) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftTrigger);
        }

        public bool IsRewindPressed()
        {
            return InputHelper.IsKeyDown(Keys.E) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y);
        }

        public bool IsEnterCommandPressed()
        {
            return InputHelper.IsKeyDown(Keys.Enter);
        }

        public bool IsContinueChatPressed()
        {
            return InputHelper.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A);
        }

        public bool IsTestLevelPressed()
        {
            return InputHelper.IsKeyDown(Keys.T) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start);
        }

        public bool IsMoveUpPressed()
        {
            return InputHelper.IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickUp);
        }

        public bool IsWeaponFirePressed()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X);
        }

        public bool IsPauseButtonDown()
        {
            return InputHelper.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back);
        }

        public bool IsChangeHotBarTileDown()
        {
            return InputHelper.IsKeyDown(Keys.Tab) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder);
        }

        public bool IsBrushButtonPressed()
        {
            return InputHelper.IsKeyDown(Keys.B) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X);
        }

        public bool IsEraserButtonPressed()
        {
            return InputHelper.IsKeyDown(Keys.N) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y);
        }
    }
}