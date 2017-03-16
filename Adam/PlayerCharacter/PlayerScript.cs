using Adam.Levels;
using Adam.Misc;
using Adam.Particles;
using Adam.PlayerCharacter;
using Adam.Projectiles;
using Microsoft.Xna.Framework;
using System;

namespace Adam
{
    public class PlayerScript : Behavior
    {
        Player _player;
        public static bool IsDoingAction = false;

        private const float MaxWalkVelX = 6f;
        private const float MaxRunVelX = 10f;
        private const float MoveJumpAcc = .05f;
        const float JumpAcc = -12f;
        const float WalkAcc = .3f;
        const float RunAcc = .4f;
        const float DashSpeed = 24f;
        const float ClimbingSpeed = 4f;
        public const double RewindCooldown = 4000;

        Timer _idleTimer = new Timer(true);
        Timer _lastJumpTimer = new Timer(true);
        Timer _weaponFireRateTimer = new Timer(true);
        Timer _particleTimer = new Timer();

        public static Timer TimeSinceLastPunch = new Timer(true);


        SoundFx _stepSound = new SoundFx("Sounds/Movement/walk1");

        private bool hasFireballsAround;

        public void Initialize(Player player)
        {
            this._player = player;
            player.PlayerDamaged += OnPlayerDamaged;
            player.CollidedWithTileBelow += Player_CollidedWithTileBelow;

            player.DamagePointsProj = 50;

            player.FocusMechanic.OnFocusLevelChange += FocusMechanic_OnFocusLevelChange;
        }

        private void FocusMechanic_OnFocusLevelChange(Player player, int focusLevel)
        {
            if (focusLevel == 0)
            {
                hasFireballsAround = false;
            }

            if (focusLevel > 80)
            {
                hasFireballsAround = true;
            }
        }

        private void Player_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            entity.IsJumping = false;
            entity.RemoveAnimationFromQueue("fall");
            entity.RemoveAnimationFromQueue("jump");
        }

        int lastDeg;
        public override void Update(Entity entity)
        {
            _player = (Player)_player.Get();
            _weaponFireRateTimer.Increment();


            if (hasFireballsAround)
            {
                int radius = 200;
                int changeInDeg = 20;
                _particleTimer.Increment();
                if (_particleTimer.TimeElapsedInMilliSeconds > 10)
                {
                    _particleTimer.Reset();
                    Vector2 position = new Vector2(entity.GetCollRectangle().Center.X, entity.GetCollRectangle().Center.Y);
                    lastDeg %= 360;
                    for (int i = lastDeg; i < lastDeg + changeInDeg; i += 1)
                    {
                        double rads = Math.PI * i / 180;
                        float x = (float)Math.Cos(rads);
                        float y = (float)Math.Sin(rads);
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.RewindFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)AdamGame.Random.NextDouble(), Color.White);

                        x *= -1;
                        y *= -1;
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.RewindFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)AdamGame.Random.NextDouble(), Color.White);
                    }
                    lastDeg += changeInDeg;


                }
            }

            base.Update(entity);
        }

        private void OnPlayerDamaged(Rectangle damageArea, int damage)
        {
            _player.Sounds.GetSoundRef("hurt").Play();

            _player.FocusMechanic.ResetFocus();
        }

        public void OnStill(Player player)
        {
            _lastJumpTimer.Increment();
            if (_lastJumpTimer.TimeElapsedInMilliSeconds > 1000 || !player.IsJumpButtonPressed())
            {
                player.GravityStrength = AdamGame.Gravity;
            }

            if (player.IsClimbing)
            {
                player.RemoveAnimationFromQueue("fall");
            }
            else
            {
                player.RemoveAnimationFromQueue("climb");
            }

            // Toggle idle animations.

            if (_idleTimer.TimeElapsedInSeconds > 10)
            {
                player.AddAnimationToQueue("smellPoop");
                player.AnimationEnded += OnSmellPoopAnimationEnd;
                _idleTimer.Reset();
            }

            if (Math.Abs(player.GetVelocity().X) < 1f)
            {
                player.RemoveAnimationFromQueue("walk");
            }
            if (Math.Abs(player.GetVelocity().X) < 7)
            {
                player.RemoveAnimationFromQueue("run");
            }
            if (Math.Abs(player.GetVelocity().X) < 1)
            {
                player.RemoveAnimationFromQueue("slide");
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
            _idleTimer.Reset();
            player.AnimationEnded -= OnSmellPoopAnimationEnd;
        }

        public void OnJumpAction(Player player)
        {
            float jumpAcc = JumpAcc;
            if (player.IsDucking)
            {
                player.WantsToMoveDownPlatform = true;
                player.AddAnimationToQueue("fall");
                return;
            }
            if (player.IsClimbing)
            {
                if (player.IsInteractPressed())
                    return;
                player.IsClimbing = false;
                player.IsJumping = false;

                if (player.IsMoveDownPressed())
                    jumpAcc = 0;
            }
            if (!player.IsJumping)
            {
                player.Sounds.GetSoundRef("jump").Play();
                player.IsJumping = true;
                player.SetVelY(jumpAcc);
                player.ChangePosBy(0, -1);
                player.AddAnimationToQueue("jump");
                player.CollidedWithTileBelow += OnTouchGround;
                _lastJumpTimer.Reset();
                player.GravityStrength = AdamGame.Gravity * .5f;

                if (jumpAcc != 0)
                    for (int i = 0; i < 10; i++)
                    {
                        GameWorld.ParticleSystem.Add(ParticleType.Smoke, new Vector2(CalcHelper.GetRandomX(player.GetCollRectangle()), player.GetCollRectangle().Bottom), new Vector2(AdamGame.Random.Next((int)player.GetVelocity().X - 1, (int)player.GetVelocity().X + 1) / 10f, -AdamGame.Random.Next(1, 10) / 10f), Color.White);
                    }


            }
        }

        private void OnTouchGround(Entity entity, Tile tile)
        {
            _lastJumpTimer.Reset();
            entity.CollidedWithTileBelow -= OnTouchGround;
        }

        public void OnRightMove(Player player)
        {
            if (player.IsDucking)
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
            if (player.IsDucking)
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
            if (player.IsOnVines && player.GetVelocity().Y >= 0)
            {
                OnClimbingUpAction(player);
            }
        }

        public void OnClimbingUpAction(Player player)
        {
            player.SetVelX(0);
            player.AddAnimationToQueue("climb");
            player.SetVelY(-ClimbingSpeed);
            player.IsClimbing = true;
        }

        public void OnClimbingDownAction(Player player)
        {
            player.SetVelX(0);
            player.AddAnimationToQueue("climb");
            player.SetVelY(ClimbingSpeed);
            player.IsClimbing = true;
        }

        public void OnUpAndDownReleased(Player player)
        {
            if (player.IsClimbing)
                player.SetVelY(0);
        }

        public void OnDuckAction(Player player)
        {
            if (player.IsClimbing)
            {
                OnClimbingDownAction(player);
            }
            player.AddAnimationToQueue("duck");
            player.IsDucking = true;
        }

        public void OnDuckActionStop(Player player)
        {
            player.RemoveAnimationFromQueue("duck");
            player.WantsToMoveDownPlatform = false;
            player.IsDucking = false;
        }

        public void OnWeaponFire(Player player)
        {
            if (_weaponFireRateTimer.TimeElapsedInMilliSeconds > 100)
            {
                _weaponFireRateTimer.Reset();
                Vector2 velocity = new Vector2(25, -10);
                if (!player.IsFacingRight)
                    velocity.X *= -1;
                Projectile proj = new Projectile(Projectile.Type.PlayerShuriken, player.Position, velocity, player);
                GameWorld.PlayerProjectiles.Add(proj);

                player.FocusMechanic.AddFocus(20);
            }
        }

        public void OnRewindAction(Player player)
        {
            if (player.rewindTimer.TimeElapsedInMilliSeconds > RewindCooldown)
            {
                RewindTracker.Snapshot snap = player.rewindTracker.StartRewind();
                //player.SetPosition(snap.Position);
                //player.SetVelX(snap.Velocity.X);
                //player.SetVelY(snap.Velocity.Y);
                player.rewindTimer.Reset();
                //Overlay.FlashWhite();
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
