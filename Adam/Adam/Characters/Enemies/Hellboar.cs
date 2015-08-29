using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    public class Hellboar : Enemy, ICollidable, INewtonian, IAnimated
    {
        bool isAngry;
        List<Rectangle> rects;

        double idleTimer;
        double chargingTimer;
        bool isWalking;
        bool destinationSet;
        bool isCharging;
        bool isStunned;
        int countTilCharge;

        SoundFx playerSeen;
        SoundFx fire;
        SoundFx breath;
        SoundFx charging;
        SoundFx crash;
        SoundFx tweet;

        public Hellboar(int x, int y)
        {
            collRectangle = new Rectangle(x, y, 50 * 2, 76);
            sourceRectangle = new Rectangle(0, 0, 68, 60);
            Texture = ContentHelper.LoadTexture("Enemies/hellboar_spritesheet");

            playerSeen = new SoundFx("Sounds/Hellboar/playerSeen", this);
            fire = new SoundFx("Sounds/Hellboar/fire", this);
            crash = new SoundFx("Sounds/Hellboar/crash", this);
            breath = new SoundFx("Sounds/Hellboar/breath", this);
            tweet = new SoundFx("Sounds/Hellboar/tweet", this);
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
            if (isStunned)
            {
                tweet.PlayIfStopped();
                isAngry = false;

            }
        }

        private void CheckIfCharging()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();

            if (!isAngry)
            {
                countTilCharge = 0;
                return;
            }
            else
            {
                chargingTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (chargingTimer > 1)
                {
                    countTilCharge++;
                    breath.PlayNewInstanceOnce();
                    breath.Reset();
                    chargingTimer = 0;
                }

                if (countTilCharge > 2)
                {
                    isCharging = true;
                }
            }

            if (isCharging)
            {
                if (!destinationSet)
                {
                    int fastSpeed = 5;
                    if (IsPlayerToTheRight())
                    {
                        velocity.X = fastSpeed;
                        isFacingRight = true;
                    }
                    else
                    {
                        velocity.X = -fastSpeed;
                        isFacingRight = false;
                    }
                    destinationSet = true;
                }
            }

        }

        private void CheckForPlayer()
        {
            if (isCharging)
                return;

            GameWorld gameWorld = GameWorld.Instance;
            Player player = GameWorld.Instance.GetPlayer();

            if (CollisionRay.IsPlayerInSight(this, player, gameWorld, out rects))
            {
                isAngry = true;
                playerSeen.PlayOnce();
                fire.PlayIfStopped();
                if (!isCharging)
                    isFacingRight = IsPlayerToTheRight();
            }
            else
            {
                isAngry = false;
                playerSeen.Reset();
            }
        }


        private void WalkRandomly()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            if (isAngry && !isCharging)
            {
                CurrentAnimationState = AnimationState.Transforming;
                velocity.X = 0;
            }
            else
            {
                idleTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (idleTimer > 5000 && !isWalking)
                {
                    idleTimer = 0;
                    isWalking = true;
                    CurrentAnimationState = AnimationState.Walking;
                    velocity.X = 2f;

                    if (GameWorld.RandGen.Next(0, 2) == 0)
                    {
                        velocity.X = -velocity.X;
                        isFacingRight = false;
                    }
                    else
                    {
                        isFacingRight = true;
                    }
                }

                if (idleTimer > 1000 && isWalking)
                {
                    idleTimer = 0;
                    isWalking = false;
                    CurrentAnimationState = AnimationState.Still;
                    velocity.X = 0;
                }
            }
        }

        private void Stun()
        {
            //Change animation to stun
            velocity.X = 0;
            isCharging = false;
            destinationSet = false;
            countTilCharge = 0;
            isAngry = false;
            isStunned = true;
            crash.PlayOnce();
            crash.Reset();
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            if (isCharging)
            {
                Stun();
            }
            velocity.X = 0;
            CurrentAnimationState = AnimationState.Still;
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = 0;
            if (isCharging)
            {
                Stun();
            }
            CurrentAnimationState = AnimationState.Still;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
        }

        void IAnimated.Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    animation.Update(gameTime, DrawRectangle, animationData[0]);
                    break;
                case AnimationState.Walking:
                    animation.Update(gameTime, DrawRectangle, animationData[1]);
                    break;
                case AnimationState.Transforming:
                    animation.Update(gameTime, DrawRectangle, animationData[2]);
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

        public override byte ID
        {
            get
            {
                return 205;
            }
        }

        protected override int MaxHealth
        {
            get
            {
                return EnemyDB.Hellboar_MaxHealth;
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

        Animation animation;
        public Animation Animation
        {
            get
            {
                if (animation == null)
                    animation = new Animation(Texture, DrawRectangle, sourceRectangle);
                return animation;
            }
        }

        AnimationData[] animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (animationData == null)
                    animationData = new AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                        new Adam.AnimationData(250,4,1,AnimationType.Loop),
                        new Adam.AnimationData(250,4,2,AnimationType.PlayOnce),
                    };
                return animationData;
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
                return new Rectangle(collRectangle.X - 18, collRectangle.Y - 44, 136, 120);
            }
        }
    }
}
