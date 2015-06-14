using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    public enum WeaponType
    {
        None, Stick, Bow, Sword, Shotgun, LaserGun
    }

    class Weapon
    {
        public WeaponType CurrentWeaponType;
        public Texture2D texture;
        public Rectangle rectangle, sourceRectangle;
        ContentManager Content;
        Vector2 origin;
        Player player;
        Map map;
        GameTime gameTime;
        double fireTimer;
        bool isFlipped;
        public float rotation;
        public Vector2 tipPos;
        public List<PlayerWeaponProjectile> projectileList = new List<PlayerWeaponProjectile>();
        List<Particle> effectList = new List<Particle>();

        public Weapon()
        {
            rectangle = new Rectangle();
            CurrentWeaponType = WeaponType.LaserGun;
        }

        public void Load()
        {
            this.Content = Game1.Content;
            texture = Content.Load<Texture2D>("Weapon");
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            rectangle.Width = texture.Width;
            rectangle.Height = texture.Height;
        }

        public void SwitchWeapon(WeaponType CurrentWeaponType)
        {
            this.CurrentWeaponType = CurrentWeaponType;
        }

        private void CheckWeaponFire()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && player.hasFiredWeapon == false) //&& player.CurrentEvolution == Evolution.Future)
            {
                //projectileList.Add(new PlayerWeaponProjectile(player, Content));
                //player.PlayAttackSound();
                //player.hasFired = true;
                //fireTimer = 0;
            }

            fireTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (fireTimer > .5)
            {
                player.hasFiredWeapon = false;
            }
        }

        private void CheckCollisions()
        {
            foreach (var proj in projectileList)
            {
                if (HasHitTerrain(proj))
                {
                    effectList.Add(new Particle(proj, map.tileArray));
                    projectileList.Remove(proj);
                    break;
                }
            }

            foreach (Enemy enemy in map.enemyList)
            {
                foreach (var proj in projectileList)
                {
                    if (HasHitEnemy(proj, enemy) == true)
                    {
                        effectList.Add(new Particle(enemy, proj));
                        projectileList.Remove(proj);
                        enemy.BeMean();
                        enemy.health -= 10;

                        if (enemy.health <= 0)
                        {
                            Rectangle[] rectangles;
                            enemy.GetDisintegratedRectangles(out rectangles);

                            foreach (var rec in rectangles)
                            {
                                Particle eff = new Particle();
                                eff.CreateEnemyDisintegrationEffect(enemy, rec, proj);
                                effectList.Add(eff);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void Update(Player player, Map map, GameTime gameTime)
        {
            this.player = player;
            this.gameTime = gameTime;
            this.map = map;

            rectangle.X = player.collRectangle.X + player.collRectangle.Width / 2;
            rectangle.Y = player.collRectangle.Y + player.collRectangle.Height * 3 / 5;

            origin = new Vector2(4, 6);

            CheckWeaponFire();
            CheckCollisions();

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
            if (yDiff < 0 && xDiff < 0)
                rotation += 3.14f;
            if (yDiff > 0 && xDiff < 0)
                rotation += 3.14f;

            if (rotation > 3.14 / 2 && rotation < 3.14 * 3 / 2)
            {
                isFlipped = true;
            }
            else isFlipped = false;

            //gets tip of gun position for projectile
            double hypotenuseWeapon = Math.Sqrt(Math.Pow((double)rectangle.Width, 2) + Math.Pow((double)rectangle.Height, 2));
            double xCoor = xComp * hypotenuseWeapon;
            double yCoor = yComp * hypotenuseWeapon;

            if (xComp < 0)
                tipPos = new Vector2((float)(rectangle.X + xCoor), (float)(rectangle.Y + yCoor - rectangle.Height / 2));
            else tipPos = new Vector2((float)(rectangle.X + xCoor), (float)(rectangle.Y + yCoor + rectangle.Height / 2));

            foreach (var proj in projectileList)
                proj.Update(gameTime);
            foreach (var eff in effectList)
                eff.Update(gameTime);

            if (effectList.Count != 0)
            {
                for (int i = effectList.Count - 1; i >= 0; i--)
                {
                    if (effectList.ElementAt(i).ToDelete())
                        effectList.Remove(effectList.ElementAt(i));
                }
            }
        }

        private bool HasHitTerrain(Projectile projectile)
        {
            int projectileTilePos = (int)(projectile.topMidBound.Y / Game1.Tilesize * map.mapTexture.Width) + (int)(projectile.topMidBound.X / Game1.Tilesize);

            int[] q = new int[9];
            q[0] = projectileTilePos - map.mapTexture.Width - 1;
            q[1] = projectileTilePos - map.mapTexture.Width;
            q[2] = projectileTilePos - map.mapTexture.Width + 1;
            q[3] = projectileTilePos - 1;
            q[4] = projectileTilePos;
            q[5] = projectileTilePos + 1;
            q[6] = projectileTilePos + map.mapTexture.Width - 1;
            q[7] = projectileTilePos + map.mapTexture.Width;
            q[8] = projectileTilePos + map.mapTexture.Width + 1;

            //test = q;

            //check the tiles around the projectile for collision
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= map.tileArray.Length - 1 && map.tileArray[quadrant].isSolid == true)
                {
                    if (projectile.collRectangle.Intersects(map.tileArray[quadrant].rectangle))
                    {
                        projectile.tileHit = quadrant;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasHitEnemy(Projectile projectile, Enemy enemy)
        {
            int projectileTilePos = (int)(projectile.topMidBound.Y / Game1.Tilesize * map.mapTexture.Width) + (int)(projectile.topMidBound.X / Game1.Tilesize);

            int[] q = new int[9];
            q[0] = projectileTilePos - map.mapTexture.Width - 1;
            q[1] = projectileTilePos - map.mapTexture.Width;
            q[2] = projectileTilePos - map.mapTexture.Width + 1;
            q[3] = projectileTilePos - 1;
            q[4] = projectileTilePos;
            q[5] = projectileTilePos + 1;
            q[6] = projectileTilePos + map.mapTexture.Width - 1;
            q[7] = projectileTilePos + map.mapTexture.Width;
            q[8] = projectileTilePos + map.mapTexture.Width + 1;

            if (projectile.collRectangle.Intersects(enemy.drawRectangle) && enemy.isDead == false)
                return true;
            else return false;
        }
        
        public void CreateBurstEffect(Projectile projectile)
        {
            for (int i = 0; i < 50; i++)
            {
                Particle effect = new Particle();
                effect.CreateWeaponBurstEffect(player,projectile, Content);
                effectList.Add(effect);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (player != null)
            {
                //if (player.isFacingRight == true)
                //    spriteBatch.Draw(texture, rectangle, Color.White);
                //else spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);

                //if (!isFlipped)
                //    spriteBatch.Draw(texture, rectangle, null, Color.White, rotation, origin, SpriteEffects.None, 0);
                //else spriteBatch.Draw(texture, rectangle, null, Color.White, rotation, origin, SpriteEffects.FlipVertically, 0);

                foreach (var proj in projectileList)
                    proj.Draw(spriteBatch);
                foreach (var eff in effectList)
                    eff.Draw(spriteBatch);

                //debug
                spriteBatch.Draw(Content.Load<Texture2D>("Tiles/black"), new Rectangle((int)tipPos.X, (int)tipPos.Y, 1, 1), Color.Red);
            }
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            foreach (var proj in projectileList)
                proj.DrawLights(spriteBatch);
            foreach (var eff in effectList)
                eff.DrawLights(spriteBatch);
        }



    }
}
