using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Adam;
using Microsoft.Xna.Framework.Input;
using Adam.Lights;
using Adam.Misc.Interfaces;
using Adam.Characters.Enemies;

namespace Adam
{
    public enum ProjectileSource
    {
        Player, Snake,
    }

    public abstract class Projectile : Entity
    {
        public Rectangle topMidBound, botMidBound;
        public int tileHit;
        protected bool IsInactive;
        public ProjectileSource CurrentProjectileSource;

        protected float rotation;
        protected bool isFlipped;
        protected double effTimer;
        protected GameTime gameTime;
        protected Player player;
        protected Enemy enemy;


        public Projectile()
        {

        }

        protected void CreateParticleEffect(GameTime gameTime)
        {
            effTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (effTimer > 20 && !IsInactive)
            {
                GameWorld.Instance.particles.Add(new Particle(this));
                effTimer = 0;
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
                case ProjectileSource.Snake:
                    //animation.Draw(spriteBatch);
                    break;
                case ProjectileSource.Player:
                    spriteBatch.Draw(Texture, collRectangle, null, Color.White, rotation, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                    if (light != null)
                        light.DrawGlow(spriteBatch);
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
                    if (light != null)
                        light.Draw(spriteBatch);
                    break;
            }

        }

        protected void Destroy()
        {
            if (light != null)
                GameWorld.Instance.lightEngine.RemoveDynamicLight(light);
            GameWorld.Instance.entities.Remove(this);
        }

        public void Animate()
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerWeaponProjectile : Projectile
    {
        public PlayerWeaponProjectile(Player player, ContentManager Content)
        {
            CurrentProjectileSource = ProjectileSource.Player;
            this.player = player;
            this.Content = Content;
            player.weapon.CurrentWeaponType = WeaponType.LaserGun;
            switch (player.weapon.CurrentWeaponType)
            {
                case WeaponType.Stick:
                    break;
                case WeaponType.Bow:
                    break;
                case WeaponType.Sword:
                    break;
                case WeaponType.Shotgun:
                    break;
                case WeaponType.LaserGun:
                    Texture = Content.Load<Texture2D>("Projectiles/laser");

                    //light = new PointLight();
                    //light.Create(new Vector2(collRectangle.Center.X, collRectangle.Center.Y));
                    //light.SetColor(Color.Red);

                    MouseState mouse = Mouse.GetState();
                    Vector2 center = new Vector2((Main.UserResWidth / 2) + (player.collRectangle.Width / 2),
                        (Main.UserResHeight * 3 / 5) + (player.collRectangle.Height / 2));

                    //Find the unit vector according to where the mouse is
                    double xDiff = (mouse.X - center.X);
                    double yDiff = (mouse.Y - center.Y);
                    double x2 = Math.Pow(xDiff, 2.0);
                    double y2 = Math.Pow(yDiff, 2.0);
                    double magnitude = Math.Sqrt(x2 + y2);
                    double xComp = xDiff / magnitude;
                    double yComp = yDiff / magnitude;

                    //arctangent for rotation of proj, also takes into account periodicity
                    rotation = (float)Math.Atan(yDiff / xDiff);
                    if (yDiff < 0 && xDiff > 0)
                        rotation += 3.14f;
                    if (yDiff > 0 && xDiff > 0)
                        rotation += 3.14f;

                    //Multiply unit vectors by max speed
                    float linearSpeed = 20f;
                    velocity = new Vector2((float)(linearSpeed * xComp), (float)(linearSpeed * yComp));

                    player.weapon.CreateBurstEffect(this);
                    break;
                default:
                    break;
            }

            if (Texture == null) return;
            collRectangle = new Rectangle(player.weapon.rectangle.X + player.weapon.texture.Width, player.weapon.rectangle.Y
                + player.weapon.texture.Height / 2, Texture.Width, Texture.Height);

            collRectangle = new Rectangle((int)(player.weapon.tipPos.X), (int)(player.weapon.tipPos.Y), Texture.Width, Texture.Height);
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public override void Update(Player player, GameTime gameTime)
        {
            this.gameTime = gameTime;
            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            topMidBound = new Rectangle(collRectangle.X + Texture.Width / 2, collRectangle.Y + Texture.Height / 4, 1, 1);
            botMidBound = new Rectangle(collRectangle.X + Texture.Width / 2, collRectangle.Y + (3 * Texture.Height / 4), 1, 1);

            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 5, Texture.Width, Texture.Height - 10);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, Texture.Width - 20, Texture.Height);

            CreateTrailEffect();
        }


        private void CreateTrailEffect()
        {

        }

    }

    //Only use this with enemies
    public abstract class LinearProjectile : Projectile
    {
        public LinearProjectile()
        {

        }
    }

    public class FlyingWheelProjectile : LinearProjectile, ICollidable
    {
        public FlyingWheelProjectile(int x, int y, int xVel, int yVel)
        {
            Texture = Main.DefaultTexture;
            collRectangle = new Rectangle(x, y, 16, 16);
            velocity = new Vector2(xVel, yVel);
            light = new DynamicPointLight(this, 1, true, Color.MediumPurple, 1);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);

        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {

        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
            Destroy();
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {

        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {

        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {

        }

        public override void Update(Player player, GameTime gameTime)
        {
            GameWorld.Instance.particles.Add(new TrailParticle(this, Color.MediumPurple));
            GameWorld.Instance.particles.Add(new TrailParticle(this, Color.MediumPurple));

            base.Update(player, gameTime);
        }
    }

    //Only use this with enemies
    public class ParabolicProjectile : Projectile
    {
        public ParabolicProjectile(Enemy enemy, GameWorld map, ContentManager Content, ProjectileSource CurrentProjectileSource)
        {
            this.CurrentProjectileSource = CurrentProjectileSource;
            this.gameWorld = map;
            this.Content = Content;
            this.enemy = enemy;

            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Snake:
                    Texture = Content.Load<Texture2D>("Projectiles/venom_dark");
                    collRectangle = new Rectangle(enemy.collRectangle.X, enemy.collRectangle.Y, 32, 32);
                   // animation = new Animation(Texture, collRectangle, 200, 0, AnimationType.Loop);
                    if (!enemy.isFacingRight)
                    {
                        velocity = new Vector2(-10, -15);
                        //animation.isFlipped = true;
                    }
                    else velocity = new Vector2(10, -15);
                    break;
            }

        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public override void Update(Player player, GameTime gameTime)
        {
            this.player = player;
            this.gameTime = gameTime;

            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Snake:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;

                  //  animation.UpdateRectangle(collRectangle);
                   // animation.Update(gameTime);
                    CreateParticleEffect(gameTime);

                    velocity.Y += .8f;

                    if (velocity.Y > 10)
                        velocity.Y = 10f;

                    velocity.X = velocity.X * 0.995f;
                    break;
            }

            CheckCollisionWithPlayer();
            CheckIfOutsideBoundaries();

            if (IsInactive)
            {
                toDelete = true;
            }
        }

        private void CheckCollisionWithPlayer()
        {
            if (player.collRectangle.Intersects(collRectangle) && !IsInactive)
            {
                IsInactive = true;
                player.TakeDamageAndKnockBack(enemy.GetProjectileDamage());
            }
        }

        private void CheckIfOutsideBoundaries()
        {
            if (collRectangle.Y > gameWorld.worldData.LevelHeight * Main.Tilesize)
                IsInactive = true;
        }
    }


}



