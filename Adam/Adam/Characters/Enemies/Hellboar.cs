using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam.Characters.Enemies
{
    public class Hellboar : Enemy, INewtonian, IAnimated
    {
        bool _isAngry;
        List<Rectangle> _rects;

        double _idleTimer;
        double _chargingTimer;
        bool _isWalking;
        bool _destinationSet;
        bool _isCharging;
        bool _isStunned;
        int _countTilCharge;

        SoundFx _playerSeen;
        SoundFx _fire;
        SoundFx _breath;
        SoundFx _charging;
        SoundFx _crash;
        SoundFx _tweet;

        public Hellboar(int x, int y)
        {
            CollRectangle = new Rectangle(x, y, 50 * 2, 76);
            SourceRectangle = new Rectangle(0, 0, 68, 60);
            Texture = ContentHelper.LoadTexture("Enemies/hellboar_spritesheet");

            _playerSeen = new SoundFx("Sounds/Hellboar/playerSeen", this);
            _fire = new SoundFx("Sounds/Hellboar/fire", this);
            _crash = new SoundFx("Sounds/Hellboar/crash", this);
            _breath = new SoundFx("Sounds/Hellboar/breath", this);
            _tweet = new SoundFx("Sounds/Hellboar/tweet", this);
        }


        public override void Update()
        {
            CheckForPlayer();
            CheckIfCharging();
            CheckIfStunned();
            WalkRandomly();

            base.Update();
        }

        private void CheckIfStunned()
        {
            if (_isStunned)
            {
                _tweet.PlayIfStopped();
                _isAngry = false;

            }
        }

        private void CheckIfCharging()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();

            if (!_isAngry)
            {
                _countTilCharge = 0;
                return;
            }
            else
            {
                _chargingTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_chargingTimer > 1)
                {
                    _countTilCharge++;
                    _breath.PlayNewInstanceOnce();
                    _breath.Reset();
                    _chargingTimer = 0;
                }

                if (_countTilCharge > 2)
                {
                    _isCharging = true;
                }
            }

            if (_isCharging)
            {
                CurrentAnimationState = AnimationState.Charging;
                if (!_destinationSet)
                {
                    int fastSpeed = 5;
                    if (IsPlayerToTheRight())
                    {
                        Velocity.X = fastSpeed;
                        IsFacingRight = true;                        
                    }
                    else
                    {
                        Velocity.X = -fastSpeed;
                        IsFacingRight = false;
                    }
                    _destinationSet = true;
                }
            }

        }

        private void CheckForPlayer()
        {
            if (_isCharging)
                return;

            GameWorld gameWorld = GameWorld.Instance;
            Player player = GameWorld.Instance.GetPlayer();

            if (CollisionRay.IsPlayerInSight(this, player, gameWorld, out _rects))
            {
                _isAngry = true;
                _playerSeen.PlayOnce();
                _fire.PlayIfStopped();
                if (!_isCharging)
                    IsFacingRight = IsPlayerToTheRight();
            }
            else
            {
                _isAngry = false;
                _playerSeen.Reset();
            }
        }


        private void WalkRandomly()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            if (_isAngry && !_isCharging)
            {
                CurrentAnimationState = AnimationState.Transforming;
                Velocity.X = 0;
            }
            else
            {
                _idleTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_idleTimer > 5000 && !_isWalking)
                {
                    _idleTimer = 0;
                    _isWalking = true;
                    CurrentAnimationState = AnimationState.Walking;
                    Velocity.X = 2f;

                    if (GameWorld.RandGen.Next(0, 2) == 0)
                    {
                        Velocity.X = -Velocity.X;
                        IsFacingRight = false;
                    }
                    else
                    {
                        IsFacingRight = true;
                    }
                }

                if (_idleTimer > 1000 && _isWalking)
                {
                    _idleTimer = 0;
                    _isWalking = false;
                    CurrentAnimationState = AnimationState.Still;
                    Velocity.X = 0;
                }
            }
        }

        private void Stun()
        {
            //Change animation to stun
            Velocity.X = 0;
            _isCharging = false;
            _destinationSet = false;
            _countTilCharge = 0;
            _isAngry = false;
            _isStunned = true;
            _crash.PlayOnce();
            _crash.Reset();
        }

        public void OnCollisionWithTerrainRight(Entity entity, Tile tile)
        {
            if (_isCharging)
            {
                Stun();
            }
            Velocity.X = 0;
            CurrentAnimationState = AnimationState.Still;
        }

        public void OnCollisionWithTerrainLeft(Entity entity, Tile tile)
        {
            Velocity.X = 0;
            if (_isCharging)
            {
                Stun();
            }
            CurrentAnimationState = AnimationState.Still;
        }

        void IAnimated.Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    _animation.Update(gameTime, DrawRectangle, _animationData[0]);
                    break;
                case AnimationState.Walking:
                    _animation.Update(gameTime, DrawRectangle, _animationData[1]);
                    break;
                case AnimationState.Transforming:
                    _animation.Update(gameTime, DrawRectangle, _animationData[2]);
                    break;
                case AnimationState.Charging:
                    _animation.Update(gameTime, DrawRectangle, _animationData[3]);
                    break;
            }
        }

        public float GravityStrength
        {
            get { return Main.Gravity; }
            set
            {
                GravityStrength = value;
            }
        }


        public bool IsFlying { get; set; }

        public bool IsJumping { get; set; }

        public bool IsAboveTile { get; set; }

        public override byte Id
        {
            get
            {
                return 205;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDb.HellboarMaxHealth;
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                return null;
            }
        }

        Animation _animation;
        public Animation Animation
        {
            get
            {
                if (_animation == null)
                    _animation = new Animation(Texture, DrawRectangle, SourceRectangle);
                return _animation;
            }
        }

        AnimationData[] _animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (_animationData == null)
                    _animationData = new AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                        new Adam.AnimationData(250,4,1,AnimationType.Loop),
                        new Adam.AnimationData(250,4,2,AnimationType.PlayOnce),
                        new Adam.AnimationData(100,5,3,AnimationType.Loop),
                    };
                return _animationData;
            }
        }

        public Misc.Interfaces.AnimationState CurrentAnimationState
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X - 18, CollRectangle.Y - 44, 136, 120);
            }
        }
    }
}
