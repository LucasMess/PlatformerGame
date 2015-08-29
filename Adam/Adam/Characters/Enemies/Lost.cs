using Adam;
using Adam.Characters.Enemies;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class Lost : Enemy
    {
        enum AnimationState
        {
            Hiding,
            Flying,
        }
        AnimationState CurrentAnimationState = AnimationState.Hiding;
        AnimationData still;
        SoundFx ghost, ghost2;
        double ghostTimer;
        int timerEnd;
        Vector2 maxVelocity;

        public override byte ID
        {
            get
            {
                return 204;
            }
        }

        protected override int MaxHealth
        {
            get
            {
                return EnemyDB.Lost_MaxHealth;
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override SoundFx AttackSound
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Lost(int x, int y)
        {
            maxVelocity = new Vector2(1, 1);

            Texture = Content.Load<Texture2D>("Enemies/lost");
            ghost = new SoundFx("Sounds/Lost/ghost");
            ghost2 = new SoundFx("Sounds/Lost/ghost2");

            collRectangle = new Rectangle(x, y, 48 - 8, 80 - 12);
            drawRectangle = new Rectangle(collRectangle.X - 8, collRectangle.Y - 12, 48, 80);
            sourceRectangle = new Rectangle(0, 0, 24, 40);

            still = new AnimationData(200, 4, 0, AnimationType.Loop);
            animation = new Animation(Texture, drawRectangle, sourceRectangle);

            timerEnd = GameWorld.RandGen.Next(3, 8);
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.GetPlayer();

            Animate();


            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            int buffer = 5;
            if (collRectangle.Y < player.collRectangle.Y - buffer)
            {
                velocity.Y = maxVelocity.Y;
            }
            else if (collRectangle.Y > player.collRectangle.Y + buffer)
            {
                velocity.Y = -maxVelocity.Y;
            }
            else
            {
                velocity.Y = 0;
            }

            if (collRectangle.X < player.collRectangle.X - buffer)
            {
                velocity.X = maxVelocity.X;
            }
            else if (collRectangle.X > player.collRectangle.X + buffer)
            {
                velocity.X = -maxVelocity.X;
            }
            else
            {
                velocity.X = 0;
            }

            drawRectangle = new Rectangle(collRectangle.X - 8, collRectangle.Y - 12, 48, 80);

            opacity = 1f;
            CurrentAnimationState = AnimationState.Flying;

            if (IsPlayerToTheRight() && !player.isFacingRight)
            {
                CurrentAnimationState = AnimationState.Hiding;
                velocity = new Vector2(0, 0);
                opacity = .5f;
            }
            if (!IsPlayerToTheRight() && player.isFacingRight)
            {
                CurrentAnimationState = AnimationState.Hiding;
                velocity = new Vector2(0, 0);
                opacity = .5f;
            }

            isFacingRight = IsPlayerToTheRight();

            ghostTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (ghostTimer > timerEnd)
            {
                ghostTimer = 0;
                if (GameWorld.RandGen.Next(0, 2) == 0)
                {
                    ghost.PlayIfStopped();
                }
                else ghost2.PlayIfStopped();
            }

            base.Update();

        }

        private void Animate()
        {
            animation.Update(GameWorld.Instance.gameTime, drawRectangle, still);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isDead)
                return;
            Color color = Color.White * opacity;
            animation.Color = color;
            animation.Draw(spriteBatch);

            if (isFacingRight)
            {
                animation.isFlipped = false;
                //spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            }
            else
            {
                animation.isFlipped = true;
                // spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}
