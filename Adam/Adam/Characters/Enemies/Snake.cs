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
using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Interfaces;

namespace Adam.Enemies
{
    public class Snake : Enemy, IAnimated, INewtonian
    {
        double _projCooldownTimer;
        Vector2 _frameCount;

        public override byte Id
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
                return EnemyDb.SnakeMaxHealth;
            }
        }

        SoundFx _meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                if (_meanSound == null)
                    _meanSound = new SoundFx("Sounds/Snake/mean");
                return _meanSound;
            }
        }

        SoundFx _attackSound;
        protected override SoundFx AttackSound
        {
            get
            {
                if (_attackSound == null)
                    _attackSound = new SoundFx("Sounds/Snake/attack");
                return _attackSound;
            }
        }

        SoundFx _deathSound;
        protected override SoundFx DeathSound
        {
            get
            {
                if (_deathSound == null)
                    _deathSound = new SoundFx("Sounds/Snake/death");
                return _deathSound;
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
                    _animationData = new Adam.AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                    };
                return _animationData;
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

        public bool IsAboveTile
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public Snake(int x, int y)
        {
            //Sets up specific variables for the snake
            _frameCount = new Vector2(8, 0);
            SourceRectangle = new Rectangle(0, 0, 64, 96);
            CollRectangle = new Rectangle(x, y - 64, 64, 96);

            //Textures and sound effects, single is for rectangle pieces explosion
            Texture = ContentHelper.LoadTexture("Enemies/Snake");

            //Creates animation
            _animation = new Animation(Texture, DrawRectangle, 240, 0, AnimationType.Loop);
        }

        public override void Update()
        {
            base.Update();            

            if (_projCooldownTimer > 3)
            {
                if (GameWorld.RandGen.Next(0, 1000) < 50)
                {
                    GameWorld.Instance.Entities.Add(new ParabolicProjectile(this, GameWorld.Instance, ProjectileSource.Snake));
                    PlayAttackSound();
                    _projCooldownTimer = 0;
                }
            }
            _projCooldownTimer += GameWorld.Instance.GetGameTime().ElapsedGameTime.TotalSeconds;
        }

        public void Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            _animation.Update(gameTime, DrawRectangle, _animationData[0]);
        }
    }
}
