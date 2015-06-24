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

namespace Adam
{
    public enum ProjectileSource
    {
        Player, Snake,
    }

    class Projectile : Entity
    {
        public Rectangle topMidBound, botMidBound;
        public Vector2 velocity;
        protected PointLight light;
        public int tileHit;
        public bool toDelete;
        protected bool IsInactive;
        public ProjectileSource CurrentProjectileSource;

        protected float rotation;
        protected bool isFlipped;
        protected double effTimer;
        protected GameTime gameTime;
        protected GameWorld map;
        protected Player player;
        protected Enemy enemy;
        protected List<Particle> effectList = new List<Particle>();


        public Projectile()
        {

        }

        protected void CreateParticleEffect(GameTime gameTime)
        {
            effTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (effTimer > 20 && !IsInactive)
            {
                effectList.Add(new Particle(this));
                effTimer = 0;
            }

            foreach (var eff in effectList)
            {
                eff.Update(gameTime);
            }

            int maxCount = effectList.Count() - 1;
            for (int i = 0; i < effectList.Count; i++)
            {
                int k = maxCount - i;
                if (effectList[k].ToDelete())
                {
                    effectList.Remove(effectList[k]);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Snake:
                    animation.Draw(spriteBatch);
                    break;
                case ProjectileSource.Player:
                    spriteBatch.Draw(texture, collRectangle, null, Color.White, rotation, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                    if (light != null)
                        light.DrawGlow(spriteBatch);
                    foreach (var eff in effectList)
                        eff.Draw(spriteBatch);
                    break;
            }

            foreach (var eff in effectList)
                eff.Draw(spriteBatch);

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
    }

    class PlayerWeaponProjectile : Projectile
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
                    texture = Content.Load<Texture2D>("Projectiles/laser");

                    light = new PointLight();
                    light.Create(new Vector2(collRectangle.Center.X, collRectangle.Center.Y));
                    light.SetColor(Color.Red);

                    MouseState mouse = Mouse.GetState();
                    Vector2 center = new Vector2((Game1.PrefferedResWidth / 2) + (player.collRectangle.Width / 2),
                        (Game1.PrefferedResHeight * 3 / 5) + (player.collRectangle.Height / 2));

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

            if (texture == null) return;
            collRectangle = new Rectangle(player.weapon.rectangle.X + player.weapon.texture.Width, player.weapon.rectangle.Y
                + player.weapon.texture.Height / 2, texture.Width, texture.Height);

            collRectangle = new Rectangle((int)(player.weapon.tipPos.X), (int)(player.weapon.tipPos.Y), texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            topMidBound = new Rectangle(collRectangle.X + texture.Width / 2, collRectangle.Y + texture.Height / 4, 1, 1);
            botMidBound = new Rectangle(collRectangle.X + texture.Width / 2, collRectangle.Y + (3 * texture.Height / 4), 1, 1);

            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 5, texture.Width, texture.Height - 10);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, texture.Width - 20, texture.Height);

            if (light != null)
                light.Update(new Vector2(collRectangle.Center.X, collRectangle.Center.Y));

            CreateTrailEffect();

            foreach (var eff in effectList)
            {
                eff.Update(gameTime);
                if (eff.ToDelete())
                {
                    effectList.Remove(eff);
                    break;
                }
            }
        }


        private void CreateTrailEffect()
        {

        }

    }

    //Only use this with enemies
    class LinearProjectile : Projectile
    {

        public LinearProjectile()
        {

        }

        public override void Update()
        {

        }
    }

    //Only use this with enemies
    class ParabolicProjectile : Projectile
    {
        public ParabolicProjectile(Enemy enemy, GameWorld map, ContentManager Content, ProjectileSource CurrentProjectileSource)
        {
            this.CurrentProjectileSource = CurrentProjectileSource;
            this.map = map;
            this.Content = Content;
            this.enemy = enemy;

            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Snake:
                    texture = Content.Load<Texture2D>("Projectiles/venom_dark");
                    collRectangle = new Rectangle(enemy.drawRectangle.X, enemy.drawRectangle.Y, 32, 32);
                    animation = new Animation(texture, collRectangle, 200, 0, AnimationType.Loop);
                    if (!enemy.isPlayerToTheRight)
                    {
                        velocity = new Vector2(-10, -15);
                        animation.isFlipped = true;
                    }
                    else velocity = new Vector2(10, -15);
                    break;
            }

        }

        public void Update(Player player, GameTime gameTime)
        {
            this.player = player;
            this.gameTime = gameTime;

            switch (CurrentProjectileSource)
            {
                case ProjectileSource.Snake:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;

                    animation.UpdateRectangle(collRectangle);
                    animation.Update(gameTime);
                    CreateParticleEffect(gameTime);

                    velocity.Y += .8f;

                    if (velocity.Y > 10)
                        velocity.Y = 10f;

                    velocity.X = velocity.X * 0.995f;
                    break;
            }

            CheckCollisionWithPlayer();
            CheckIfOutsideBoundaries();

            if (IsInactive && effectList.Count == 0)
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
            if (collRectangle.Y > map.mapTexture.Height * Game1.Tilesize)
                IsInactive = true;
        }
    }


}



