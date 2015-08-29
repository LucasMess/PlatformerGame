using Adam;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    public class CDPlayer : Item, IAnimated
    {
        Animation animation;
        public Animation Animation
        {
            get
            {
                if (animation == null)
                    animation = new Animation(Texture, drawRectangle, sourceRectangle);
                return animation;
            }
        }

        AnimationData[] animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (animationData == null)
                    animationData = new AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                    };
                return animationData;
            }
        }

        public AnimationState CurrentAnimationState
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public CDPlayer(Vector2 position)
        {
            Texture = ContentHelper.LoadTexture("Objects/CDplayer_new");
            drawRectangle = new Rectangle((int)position.X, (int)position.Y, 32, 32);
            sourceRectangle = new Rectangle(0, 0, 32, 32);

            animation = new Animation(Texture, drawRectangle, 70, 0, AnimationType.Loop);
            loopSound = new Misc.SoundFx("Sounds/loop");
            velocity.Y = -10f;
        }

        public override void Update()
        {
            GameWorld gameWorld = GameWorld.Instance;
            GameTime gameTime = gameWorld.gameTime;


            animation.Update(gameTime);
            animation.UpdateRectangle(drawRectangle);

            velocity.Y += .3f;
            if (velocity.Y > 5f)
                velocity.Y = 5f;

            effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (effectTimer > 500)
            {
                Particle eff = new Particle();
                eff.CreateMusicNotesEffect(this);
                GameWorld.Instance.particles.Add(eff);
                effectTimer = 0;
            }

            base.Update();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }

        public void Animate()
        {
            throw new NotImplementedException();
        }
    }
}
