using Adam.Characters.Enemies;
using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timer = Adam.Misc.Timer;

namespace Adam
{
    public enum ProjectileSource
    {
        Player, Enemy,
    }

    public abstract class Projectile : Entity
    {
        private Timer expirationTimer = new Timer();


        public int TileHit;
        protected bool IsInactive;
        public ProjectileSource CurrentProjectileSource;

        public int DamageOnHit { get; set; }
        protected float Rotation;
        protected bool IsFlipped;
        protected double EffTimer;
        protected GameTime GameTime;
        protected Player Player;
        protected Enemy Enemy;


        public Projectile()
        {
            IsCollidable = true;
            expirationTimer.ResetAndWaitFor(2000);
            expirationTimer.SetTimeReached += Destroy;
            GameWorld.Entities.Add(this);
            CollidedWithTerrain += OnTerrainCollision;
        }

        protected virtual void OnTerrainCollision(Entity entity, Tile tile)
        {
            entity.Destroy();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Enemy:
                    //animation.Draw(spriteBatch);
                    break;
                case ProjectileSource.Player:
                    spriteBatch.Draw(Texture, CollRectangle, null, Color.White, Rotation, new Vector2(0, 0),
                        SpriteEffects.FlipHorizontally, 0);
                    break;
                default:
                    base.Draw(spriteBatch);
                    break;
            }
        }

        public override void Destroy()
        {
            expirationTimer.Destroy();
            expirationTimer.SetTimeReached -= Destroy;
            base.Destroy();
        }
    }

    public class PlayerWeaponProjectile : Projectile
    {
        public PlayerWeaponProjectile()
        {
            CurrentProjectileSource = ProjectileSource.Player;
            Player player = GameWorld.GetPlayer();
            Texture = AdamGame.DefaultTexture;
            CollRectangle = new Rectangle(player.GetCollRectangle().Center.X - 8, player.GetCollRectangle().Center.Y - 4, 16, 8);
            CurrentCollisionType = CollisionType.Bouncy;

            float xVel = 8;
            float yVel = -10;

            DamageOnHit = 5;

            if (!player.IsFacingRight) xVel *= -1;
            Velocity = new Vector2(xVel, yVel);

            GameWorld.PlayerProjectiles.Add(this);
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        protected override void OnTerrainCollision(Entity entity, Tile tile)
        {
            // DO NOTHING.
        }

        public override void Update()
        {
            Position += Velocity;

            CreateTrailEffect();

            base.Update();
        }


        private void CreateTrailEffect()
        {

        }

        public override void Destroy()
        {
            GameWorld.PlayerProjectiles.Remove(this);
            base.Destroy();
        }

    }

    //Only use this with enemies
    public abstract class LinearProjectile : Projectile
    {
        public LinearProjectile()
        {

        }
    }

    //Only use this with enemies
    public class ParabolicProjectile : Projectile
    {
        public ParabolicProjectile(Enemy enemy, ProjectileSource currentProjectileSource)
        {
            this.CurrentProjectileSource = currentProjectileSource;
            this.Enemy = enemy;

            switch (currentProjectileSource)
            {
                case ProjectileSource.Enemy:
                    Texture = ContentHelper.LoadTexture("Projectiles/venom_dark");
                    CollRectangle = new Rectangle(enemy.GetCollRectangle().X, enemy.GetCollRectangle().Y, 32, 32);
                    // animation = new Animation(Texture, collRectangle, 200, 0, AnimationType.Loop);
                    if (!enemy.IsFacingRight)
                    {
                        Velocity = new Vector2(-10, -15);
                        //animation.isFlipped = true;
                    }
                    else Velocity = new Vector2(10, -15);
                    break;
            }

        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public override void Update()
        {
            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Enemy:
                    Position += Velocity;

                    Velocity.Y += .8f;

                    if (Velocity.Y > 10)
                        Velocity.Y = 10f;

                    Velocity.X = Velocity.X * 0.995f;
                    break;
            }

            CheckCollisionWithPlayer();
            CheckIfOutsideBoundaries();

            if (IsInactive)
            {
                ToDelete = true;
            }
        }

        private void CheckCollisionWithPlayer()
        {
            if (Player.GetCollRectangle().Intersects(CollRectangle) && !IsInactive)
            {
                IsInactive = true;
            }
        }

        private void CheckIfOutsideBoundaries()
        {
            if (CollRectangle.Y > GameWorld.WorldData.LevelHeight * AdamGame.Tilesize)
                IsInactive = true;
        }
    }


}



