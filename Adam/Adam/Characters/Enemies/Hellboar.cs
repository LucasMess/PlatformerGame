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
    public class Hellboar : Enemy, ICollidable, INewtonian
    {
        bool isAngry;
        List<Rectangle> rects;
        AnimationData animationData;

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

        enum AnimationState
        {
            Idle, Walking, Transforming,
        }

        AnimationState CurrentAnimation = AnimationState.Idle;

        public Hellboar(int x, int y)
        {
            health = 100;
            collRectangle = new Rectangle(x, y, 50 * 2, 76);
            drawRectangle = new Rectangle(x - 18, y - 44, 68 * 2, 60 * 2);
            sourceRectangle = new Rectangle(0, 0, 68, 60);
            texture = ContentHelper.LoadTexture("Enemies/hellboar_spritesheet");
            CurrentEnemyType = EnemyType.Hellboar;
            animationData = new AnimationData(0, 4, 0, AnimationType.Loop);
            animationData.FrameCount = new Vector2(3, 0);

            playerSeen = new SoundFx("Sounds/Hellboar/playerSeen",this);
            fire = new SoundFx("Sounds/Hellboar/fire", this);
            crash = new SoundFx("Sounds/Hellboar/crash",this);
            breath = new SoundFx("Sounds/Hellboar/breath",this);
            tweet = new SoundFx("Sounds/Hellboar/tweet",this);

            base.Initialize();
        }

        public override void Update(Player player, GameTime gameTime)
        {
            this.player = player;
            this.gameTime = gameTime;
            base.Update(player, gameTime);

            if (isDead) return;

            drawRectangle.X = collRectangle.X - 18;
            drawRectangle.Y = collRectangle.Y - 20;
            damageBox = new Rectangle(collRectangle.X - 5, collRectangle.Y - 20, collRectangle.Width + 10, collRectangle.Height / 2);

            int t = GetTileIndex();
            int[] i = GetNearbyTileIndexes(GameWorld.Instance);

            CheckForPlayer();
            CheckIfCharging();
            CheckIfStunned();
            WalkRandomly();
            Animate();
        }

        private void CheckIfStunned()
        {
            if (isStunned)
            {
                tweet.PlayIfStopped();
            }
        }

        private void CheckIfCharging()
        {
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
                    if (isPlayerToTheRight)
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
            if (isCharging) return;
            if (CollisionRay.IsPlayerInSight(this, player, gameWorld, out rects))
            {
                isAngry = true;
                playerSeen.PlayOnce();
                fire.PlayIfStopped();
                if (!isCharging)
                    isFacingRight = isPlayerToTheRight;
            }
            else
            {
                isAngry = false;
                playerSeen.Reset();
            }
        }


        private void WalkRandomly()
        {
            if (isAngry && !isCharging)
            {
                CurrentAnimation = AnimationState.Transforming;
                velocity.X = 0;
            }
            else
            {
                idleTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (idleTimer > 5000 && !isWalking)
                {
                    idleTimer = 0;
                    isWalking = true;
                    CurrentAnimation = AnimationState.Walking;
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
                    CurrentAnimation = AnimationState.Idle;
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

        private void Animate()
        {
            switch (CurrentAnimation)
            {
                case AnimationState.Idle:
                    sourceRectangle.Y = 0;
                    animationData.SwitchFrame = 125;
                    animationData.FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (animationData.FrameTimer >= animationData.SwitchFrame)
                    {
                        animationData.FrameTimer = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        animationData.CurrentFrame++;
                    }

                    if (animationData.CurrentFrame > animationData.FrameCount.X)
                    {
                        animationData.CurrentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                case AnimationState.Walking:
                    sourceRectangle.Y = sourceRectangle.Height;
                    animationData.SwitchFrame = 125;
                    animationData.FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (animationData.FrameTimer >= animationData.SwitchFrame)
                    {
                        animationData.FrameTimer = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        animationData.CurrentFrame++;
                    }

                    if (animationData.CurrentFrame > animationData.FrameCount.X)
                    {
                        animationData.CurrentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                case AnimationState.Transforming:
                    sourceRectangle.Y = sourceRectangle.Height * 2;
                    animationData.SwitchFrame = 125;
                    animationData.FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (animationData.FrameTimer >= animationData.SwitchFrame)
                    {
                        animationData.FrameTimer = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        animationData.CurrentFrame++;
                    }

                    if (animationData.CurrentFrame > animationData.FrameCount.X)
                    {
                        animationData.CurrentFrame = 3;
                        sourceRectangle.X = sourceRectangle.Width * 3;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //DrawSurroundIndexes(spriteBatch);
            spriteBatch.Draw(Main.DefaultTexture, collRectangle, Color.Blue * .5f);
            spriteBatch.Draw(Main.DefaultTexture, drawRectangle, Color.Green * .5f);

            Color color = Color.White;
            //if (isAngry) color = Color.Blue; else color = Color.White;


            //if (rects != null)
            //    foreach (var rect in rects)
            //        spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), new Rectangle(rect.X, rect.Y, 16, 16), Color.Black);

            base.Draw(spriteBatch);


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
            CurrentAnimation = AnimationState.Idle;
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = 0;
            if (isCharging)
            {
                Stun();
            }
            CurrentAnimation = AnimationState.Idle;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
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
    }
}
