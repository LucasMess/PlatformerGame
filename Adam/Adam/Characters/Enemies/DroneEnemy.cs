using Adam.Projectiles;
using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Characters.Enemies;
using Adam.Misc;

namespace Adam.Enemies
{
    public class DroneEnemy : Enemy
    {
        enum AnimationState
        {
            Walking,
            Still,
            Alertmode,
            Firing,
            //Broken,
        }

        bool hasCharged;
        double chargeTimer;
        SoundEffect chargeSound;
        SoundEffect fireSound;
        Color tempCo;
        List<Rectangle> rects = new List<Rectangle>();
        AnimationState CurrentAnimationState = AnimationState.Still;

        public override byte ID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override int MaxHealth
        {
            get
            {
                throw new NotImplementedException();
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

        public DroneEnemy(int x, int y, ContentManager Content, GameWorld map)
        {
            this.Content = Content;
            this.gameWorld = map;

            Texture = Content.Load<Texture2D>("Enemies/Drone_Single");
            chargeSound = Content.Load<SoundEffect>("Sounds/Drone/Drone_Charging");
            fireSound = Content.Load<SoundEffect>("Sounds/Drone/Drone_Fire");

            drawRectangle = new Rectangle(x, y, 32, 72);
            collRectangle = new Rectangle(x, y, 32, 72);
            sourceRectangle = new Rectangle(0, 0, 32, 72);
        }

        public override void Update()
        {
            Player player = GameWorld.Instance.GetPlayer();
            GameTime gameTime = GameWorld.Instance.GetGameTime();
           
            Animate();

            if (CollisionRay.IsPlayerInSight(this, player, GameWorld.Instance, out rects))
            {
                tempCo = Color.Red;
                CurrentAnimationState = AnimationState.Alertmode;
                velocity = Vector2.Zero;
                PlayChargingSound();
            }
            else
            {
                chargeTimer = 0;
                hasCharged = false;
                tempCo = Color.White;
            }

            if (CurrentAnimationState == AnimationState.Alertmode)
            {
                chargeTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (chargeTimer > 3.5)
                {
                    chargeTimer = 0;
                    PlayFireSound();
                    //TODO launch proj
                }
            }

            base.Update();
        }

        private void Animate()
        {
            switch (CurrentAnimationState)
            {
                case AnimationState.Walking:
                    break;
                case AnimationState.Still:
                    break;
                case AnimationState.Alertmode:
                    break;
                case AnimationState.Firing:
                    break;
                default:
                    break;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (isFacingRight)
                spriteBatch.Draw(Texture, drawRectangle, sourceRectangle, tempCo, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            else spriteBatch.Draw(Texture, drawRectangle, sourceRectangle, tempCo, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);

            //foreach (Rectangle r in rects)
            //    spriteBatch.Draw(Content.Load<Texture2D>("Projectiles/laser"), r, Color.White);

            base.Draw(spriteBatch);
        }

        private void PlayChargingSound()
        {
            if (!hasCharged)
            {
                hasCharged = true;
                chargeSound.Play();
            }
        }

        private void PlayFireSound()
        {
            fireSound.Play();
        }
    }
}
