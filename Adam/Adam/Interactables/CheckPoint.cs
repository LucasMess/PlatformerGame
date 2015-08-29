using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    public class CheckPoint : Entity, IAnimated
    {
        AnimationData opening;
        SoundFx quack, openSound;
        bool isOpen;

        Animation animation;
        public Animation Animation
        {
            get
            {
                if (animation == null)
                    animation = new Animation(Texture, DrawRectangle, sourceRectangle);
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
                        new AnimationData(250,4,0,AnimationType.PlayOnce),
                    };
                return animationData;
            }
        }

        public AnimationState CurrentAnimationState
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return  new Rectangle(collRectangle.X + 50, collRectangle.Y, 32, 96);
            }
        }

        public CheckPoint(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Objects/checkPoint");
            sourceRectangle = new Rectangle(0, 0, 16, 48);
            collRectangle = new Rectangle(x, y - Main.Tilesize * 2, 100, DrawRectangle.Height);

            opening = new AnimationData(32, 4, 0, AnimationType.PlayOnce);
            animation = new Animation(Texture, DrawRectangle, sourceRectangle);

            quack = new SoundFx("Backgrounds/Splash/quack");
            openSound = new SoundFx("Sounds/Menu/checkPoint");
        }
        public override void Update()
        {
            if (GameWorld.Instance.player.collRectangle.Intersects(collRectangle))
            {
                if (!isOpen)
                {
                    Open();
                }
            }

            if (isOpen)
            {
                animation.Update(GameWorld.Instance.gameTime, DrawRectangle, opening);
            }
        }

        private void Open()
        {
            //Closes all other checkpoints.
            for (int i = 0; i < GameWorld.Instance.entities.Count; i++)
            {
                Entity en = GameWorld.Instance.entities[i];
                if (en is CheckPoint)
                {
                    if (en == this)
                        continue;
                    CheckPoint ch = (CheckPoint)en;
                    ch.Close();
                }
            }

            //Open this checkpoint.
            isOpen = true;
            quack.PlayIfStopped();
            openSound.PlayIfStopped();

            //Sets respawn point;
            Player player = GameWorld.Instance.player;
            player.respawnPos = new Vector2(this.DrawRectangle.X, this.DrawRectangle.Y);

            //Particle effects
            for (int i = 0; i < 100; i++)
            {
                Particle par = new Particle();
                par.CreateSparkles(this);
                GameWorld.Instance.particles.Add(par);
            }

        }

        public void Close()
        {
            isOpen = false;
            opening.Reset();
            animation = new Animation(Texture, DrawRectangle, sourceRectangle);
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
