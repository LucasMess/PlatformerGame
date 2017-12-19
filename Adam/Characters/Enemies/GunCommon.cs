using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.Levels;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    class GunCommon : Enemy
    {
        public override int MaxHealth => 100;

        protected override SoundFx MeanSound => null;

        protected override SoundFx AttackSound => null;

        protected override SoundFx DeathSound => null;

        protected override Rectangle DrawRectangle => new Rectangle(CollRectangle.X - 8, CollRectangle.Y - 16, 48, 80);

        public GunCommon(int x, int y)
        {
            Weight = 90;
            Behavior = new GunCommonBehavior();
            Behavior.Initialize(this);
            
            Texture = ContentHelper.LoadTexture("Temp/enemy_gun");
            SetPosition(new Vector2(x, y));
            // Animation information.
            CollRectangle = new Rectangle(0, 0, 24, 64);
            SourceRectangle = new Rectangle(0, 0, 24, 40);

            // Animation textures.
            _complexAnimation.AddAnimationData("idle",
            new ComplexAnimData(0, Texture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 400, 4, true));
            _complexAnimation.AddAnimationData("walk",
                new ComplexAnimData(150, Texture, new Rectangle(6, 7, 12, 66), 240, 24, 40, 125, 4, true));
            _complexAnimation.AddAnimationData("jump",
                new ComplexAnimData(200, Texture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 250, 4, false));
            _complexAnimation.AddAnimationData("climb",
                new ComplexAnimData(900, Texture, new Rectangle(6, 7, 12, 66), 160, 24, 40, 75, 4, true));
            _complexAnimation.AddAnimationData("fall", 
                new ComplexAnimData(1000, Texture, new Rectangle(6, 7, 12, 66), 120, 24, 40, 125, 4, true));


            Sounds.AddSoundRef("jump", "Sounds/Frog/frog_jump");
            Sounds.AddSoundRef("idle", "Sounds/Frog/frog_croak");
        }

    }

    class GunCommonBehavior : Behavior.Behavior
    {
        Timer shootingTimer = new Timer();

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
        }

        public override void Update(Entity entity)
        {
            bool facingRight = entity.IsPlayerToTheRight();

            if (entity.IsTouchingGround)
            {
                int velX = 5;
                if (!facingRight) velX *= -1;
                entity.SetVelX(velX);
            }
            
            shootingTimer.Increment();
            if (shootingTimer.TimeElapsedInMilliSeconds > 1000)
            {
                shootingTimer.Reset();
                GameWorld.EnemyProjectiles.Add(new GunCommonProjectile(entity.Position, facingRight));
            }

        }
    }

    class GunCommonProjectile : Projectiles.ProjectileSystem
    {
        public GunCommonProjectile(Vector2 position, bool toRight)
        {
            SetPosition(position);
            float velX = 20f;
            if (!toRight) velX *= -1;

            SetVelX(velX);

            Texture = GameWorld.SpriteSheet;
            SourceRectangle = new Rectangle(256, 176, 16, 16);

        }

        public override void Update()
        {



            base.Update();
        }
    }
}
