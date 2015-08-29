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

namespace Adam.Enemies
{
    public class Snake : Enemy
    {
        double projCooldownTimer;

        public override byte ID
        {
            get
            {
                return 201;
            }
        }

        protected override int MaxHealth
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

        public Snake(int x, int y)
        {
            //Sets up specific variables for the snake
            frameCount = new Vector2(8, 0);
            sourceRectangle = new Rectangle(0, 0, 64, 96);
            drawRectangle = new Rectangle(x, y - 64, 64, 96);

            //Textures and sound effects, single is for rectangle pieces explosion
            Texture = Content.Load<Texture2D>("Enemies/Snake");

            //Creates animation
            animation = new Animation(Texture, drawRectangle, 240, 0, AnimationType.Loop);
        }

        public override void Update()
        {
            base.Update();

            collRectangle = new Rectangle(drawRectangle.X + 8, drawRectangle.Y + 12, drawRectangle.Width - 16, drawRectangle.Height - 12);

            if (projCooldownTimer > 3 && !isDead)
            {
                if (GameWorld.RandGen.Next(0, 1000) < 50)
                {
                    GameWorld.Instance.entities.Add(new ParabolicProjectile(this, GameWorld.Instance, Content, ProjectileSource.Snake));
                    PlayAttackSound();
                    projCooldownTimer = 0;
                }
            }
            projCooldownTimer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isDead)
                return;

            if (IsPlayerToTheRight())
                animation.isFlipped = true;
            else animation.isFlipped = false;

            animation.Draw(spriteBatch);
        }
    }
}
