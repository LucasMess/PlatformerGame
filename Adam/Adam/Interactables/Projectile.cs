using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Adam.Lights;
using Adam.Characters.Enemies;
using Adam.Levels;
using Adam.PlayerCharacter;
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

        protected void CreateParticleEffect(GameTime gameTime)
        {
            EffTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EffTimer > 20 && !IsInactive)
            {
                GameWorld.Particles.Add(new Particle(this));
                EffTimer = 0;
            }
        }

        public virtual void Update(Player player, GameTime gameTime)
        {
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Enemy:
                    //animation.Draw(spriteBatch);
                    break;
                case ProjectileSource.Player:
                    spriteBatch.Draw(Texture, CollRectangle, null, Color.White, Rotation, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                    if (Light != null)
                        Light.DrawGlow(spriteBatch);
                    break;
                default:
                    base.Draw(spriteBatch);
                    break;
            }
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Player:
                    if (Light != null)
                        Light.Draw(spriteBatch);
                    break;
            }

        }

        public override void Destroy()
        {
            expirationTimer.Destroy();
            expirationTimer.SetTimeReached -= Destroy;
            base.Destroy();
        }

        public void Animate()
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerWeaponProjectile : Projectile
    {
        public PlayerWeaponProjectile()
        {
            CurrentProjectileSource = ProjectileSource.Player;
            Player player = GameWorld.GetPlayer();
            Texture = ContentHelper.LoadTexture("Projectile");
            CollRectangle = new Rectangle(player.GetCollRectangle().Center.X - 8, player.GetCollRectangle().Center.Y - 4, 16, 8);
            CurrentCollisionType = CollisionType.Bouncy;
            
            float xVel = 8;
            float yVel = -10;

            DamageOnHit = 5;

            if (!player.IsFacingRight) xVel *= -1;
            Velocity = new Vector2(xVel, yVel);

            GameWorld.PlayerProjectiles.Add(this);
        }

        public PlayerWeaponProjectile(Player player, ContentManager content)
        { 
        //{
        //    CurrentProjectileSource = ProjectileSource.Player;
        //    this.player = player;
        //    this.Content = Content;
        //    player.weapon.CurrentWeaponType = WeaponType.LaserGun;
        //    switch (player.weapon.CurrentWeaponType)
        //    {
        //        case WeaponType.Stick:
        //            break;
        //        case WeaponType.Bow:
        //            break;
        //        case WeaponType.Sword:
        //            break;
        //        case WeaponType.Shotgun:
        //            break;
        //        case WeaponType.LaserGun:
        //            Texture = ContentHelper.LoadTexture("Projectiles/laser");

        //            //light = new PointLight();
        //            //light.Create(new Vector2(collRectangle.Center.X, collRectangle.Center.Y));
        //            //light.SetColor(Color.Red);

        //            MouseState mouse = Mouse.GetState();
        //            Vector2 center = new Vector2((Main.UserResWidth / 2) + (player.GetCollRectangle().Width / 2),
        //                (Main.UserResHeight * 3 / 5) + (player.GetCollRectangle().Height / 2));

        //            //Find the unit vector according to where the mouse is
        //            double xDiff = (mouse.X - center.X);
        //            double yDiff = (mouse.Y - center.Y);
        //            double x2 = Math.Pow(xDiff, 2.0);
        //            double y2 = Math.Pow(yDiff, 2.0);
        //            double magnitude = Math.Sqrt(x2 + y2);
        //            double xComp = xDiff / magnitude;
        //            double yComp = yDiff / magnitude;

        //            //arctangent for rotation of proj, also takes into account periodicity
        //            rotation = (float)Math.Atan(yDiff / xDiff);
        //            if (yDiff < 0 && xDiff > 0)
        //                rotation += 3.14f;
        //            if (yDiff > 0 && xDiff > 0)
        //                rotation += 3.14f;

        //            //Multiply unit vectors by max speed
        //            float linearSpeed = 20f;
        //            velocity = new Vector2((float)(linearSpeed * xComp), (float)(linearSpeed * yComp));

        //            player.weapon.CreateBurstEffect(this);
        //            break;
        //        default:
        //            break;
        //    }

        //    if (Texture == null) return;
        //    collRectangle = new Rectangle(player.weapon.rectangle.X + player.weapon.texture.Width, player.weapon.rectangle.Y
        //        + player.weapon.texture.Height / 2, Texture.Width, Texture.Height);

        //    collRectangle = new Rectangle((int)(player.weapon.tipPos.X), (int)(player.weapon.tipPos.Y), Texture.Width, Texture.Height);
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

        public override void Update(Player player, GameTime gameTime)
        {

            this.GameTime = gameTime;
            CollRectangle.X += (int)Velocity.X;
            CollRectangle.Y += (int)Velocity.Y;

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

    public class FlyinGameWorldheelProjectile : LinearProjectile
    {
        public FlyinGameWorldheelProjectile(int x, int y, int xVel, int yVel)
        {
            Texture = Main.DefaultTexture;
            CollRectangle = new Rectangle(x, y, 16, 16);
            Velocity = new Vector2(xVel, yVel);
            Light = new DynamicPointLight(this, 1, true, Color.MediumPurple, 1);
            GameWorld.LightEngine.AddDynamicLight(Light);

        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public void OnCollisionWithTerrainAnywhere(Entity entity, Tile tile)
        {
            Destroy();
        }

        public override void Update(Player player, GameTime gameTime)
        {
            GameWorld.Particles.Add(new TrailParticle(this, Color.MediumPurple));
            GameWorld.Particles.Add(new TrailParticle(this, Color.MediumPurple));

            base.Update(player, gameTime);
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

        public override void Update(Player player, GameTime gameTime)
        {
            this.Player = player;
            this.GameTime = gameTime;

            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Enemy:
                    CollRectangle.X += (int)Velocity.X;
                    CollRectangle.Y += (int)Velocity.Y;

                  //  animation.UpdateRectangle(collRectangle);
                   // animation.Update(gameTime);
                    CreateParticleEffect(gameTime);

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
            if (CollRectangle.Y > GameWorld.WorldData.LevelHeight * Main.Tilesize)
                IsInactive = true;
        }
    }


}



