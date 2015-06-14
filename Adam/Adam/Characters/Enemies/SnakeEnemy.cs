using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Enemies
{
    class SnakeEnemy : Enemy
    {
        public SnakeEnemy(int x, int y, ContentManager Content, Map map)
        {
            this.Content = Content;
            this.map = map;

            //Sets up specific variables for the snake
            CurrentEnemyType = EnemyType.Snake;
            health = EnemyDB.Snake_MaxHealth;
            frameCount = new Vector2(8, 0);
            sourceRectangle = new Rectangle(0, 0, 64, 96);
            drawRectangle = new Rectangle(x, y - 64, 64, 96);

            //Textures and sound effects, single is for rectangle pieces explosion
            texture = Content.Load<Texture2D>("Enemies/Snake");
            singleTexture = Content.Load<Texture2D>("Enemies/snake_single");

            meanSound = Content.Load<SoundEffect>("Sounds/SnakeHit");
            meanSoundInstance = meanSound.CreateInstance();
            attackSound = Content.Load<SoundEffect>("Sounds/snake_spit");
            attackSoundInstance = attackSound.CreateInstance();
            deathSound = Content.Load<SoundEffect>("Sounds/snake_death");
            deathSoundInstance = deathSound.CreateInstance();

            //Creates animation
            animation = new Animation(texture, drawRectangle, 240, 0, AnimationType.Loop);

            base.Initialize();
        }

        public override void Update(Player player, GameTime gameTime, List<Entity> entities)
        {
            base.Update(player, gameTime, entities);

            collRectangle = new Rectangle(drawRectangle.X + 8, drawRectangle.Y + 12, drawRectangle.Width - 16, drawRectangle.Height - 12);
            damageBox = new Rectangle(collRectangle.X - 5, collRectangle.Y - 10, collRectangle.Width  + 10, collRectangle.Height/2);

            if (!isInRange)
                return;

            animation.Update(gameTime);

            if (projCooldownTimer > 3 && !isDead)
            {
                if (Map.randGen.Next(0, 1000) < 50)
                {
                    projList.Add(new ParabolicProjectile(this, map, Content, ProjectileSource.Snake));
                    PlayAttackSound();
                    projCooldownTimer = 0;
                }
            }
            projCooldownTimer += gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var proj in projList)
            {
                proj.Update(player, gameTime);
            }
            foreach (var proj in projList)
            {
                if (proj.toDelete)
                {
                    projList.Remove(proj);
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (isDead)
                return;
            if (!isInRange)
                return;

            if (isPlayerToTheRight)
                animation.isFlipped = true;
            else animation.isFlipped = false;

            animation.Draw(spriteBatch);
        }
    }
}
