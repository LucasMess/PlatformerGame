using Adam;
using Adam.Characters.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Misc;
using Adam.Misc.Interfaces;

namespace Adam.Enemies
{
    public class Snake : Enemy, IAnimated, INewtonian, ICollidable
    {
        double projCooldownTimer;
        Vector2 frameCount;

        public override byte ID
        {
            get
            {
                return 201;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDB.Snake_MaxHealth;
            }
        }

        SoundFx meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                if (meanSound == null)
                    meanSound = new SoundFx("Sounds/Snake/mean");
                return meanSound;
            }
        }

        SoundFx attackSound;
        protected override SoundFx AttackSound
        {
            get
            {
                if (attackSound == null)
                    attackSound = new SoundFx("Sounds/Snake/attack");
                return attackSound;
            }
        }

        SoundFx deathSound;
        protected override SoundFx DeathSound
        {
            get
            {
                if (deathSound == null)
                    deathSound = new SoundFx("Sounds/Snake/death");
                return deathSound;
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
                    animationData = new Adam.AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                    };
                return animationData;
            }
        }

        public AnimationState CurrentAnimationState
        {
            get; set;
        }

        public float GravityStrength { get; set; } = Main.Gravity;

        public bool IsFlying
        {
            get; set;
        }

        public bool IsJumping
        {
            get; set;
        }

        public bool IsAboveTile
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        private Rectangle _respawnRect;
        public override Rectangle RespawnLocation
        {
            get
            {
                if (_respawnRect == new Rectangle(0, 0, 0, 0))
                {
                    _respawnRect = collRectangle;
                }
                return _respawnRect;
            }
        }

        public Snake(int x, int y)
        {
            //Sets up specific variables for the snake
            frameCount = new Vector2(8, 0);
            sourceRectangle = new Rectangle(0, 0, 64, 96);
            collRectangle = new Rectangle(x, y - 64, 64, 96);

            //Textures and sound effects, single is for rectangle pieces explosion
            Texture = ContentHelper.LoadTexture("Enemies/Snake");

            //Creates animation
            animation = new Animation(Texture, DrawRectangle, 240, 0, AnimationType.Loop);
        }

        public override void Update()
        {
            base.Update();            

            if (projCooldownTimer > 3 && !IsDead())
            {
                if (GameWorld.RandGen.Next(0, 1000) < 50)
                {
                    GameWorld.Instance.entities.Add(new ParabolicProjectile(this, GameWorld.Instance, ProjectileSource.Snake));
                    PlayAttackSound();
                    projCooldownTimer = 0;
                }
            }
            projCooldownTimer += GameWorld.Instance.GetGameTime().ElapsedGameTime.TotalSeconds;
        }

        public void Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            animation.Update(gameTime, DrawRectangle, animationData[0]);
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
            velocity.X = 0;
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = 0;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {

        }
    }
}
