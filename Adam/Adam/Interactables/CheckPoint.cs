using Adam.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    public class CheckPoint : Entity
    {
        AnimationData opening;
        SoundFx quack, openSound;
        bool isOpen;

        public CheckPoint(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Objects/checkPoint");
            drawRectangle = new Rectangle(x, y - Main.Tilesize * 2, 32, 96);
            sourceRectangle = new Rectangle(0, 0, 16, 48);
            collRectangle = new Rectangle(drawRectangle.Center.X - 50, drawRectangle.Y, 100,drawRectangle.Height);

            opening = new AnimationData(32, 4, 0, AnimationType.PlayOnce);
            animation = new Animation(Texture, drawRectangle, sourceRectangle);

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
                animation.Update(GameWorld.Instance.gameTime, drawRectangle, opening);
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
            player.respawnPos = new Vector2(this.drawRectangle.X, this.drawRectangle.Y);

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
            animation = new Animation(Texture, drawRectangle, sourceRectangle);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }
}
