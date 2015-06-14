using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    class CDPlayer : PowerUp
    {
        public CDPlayer(Vector2 position)
        {
            texture = ContentHelper.LoadTexture("Objects/CDplayer_new");
            drawRectangle = new Rectangle((int)position.X, (int)position.Y, 32, 32);
            sourceRectangle = new Rectangle(0, 0, 32, 32);

            animation = new Animation(texture, drawRectangle, 70, 0, AnimationType.Loop);
            loop = ContentHelper.LoadSound("Sounds/loop");
            loopInstance = loop.CreateInstance();
            velocity.Y = -10f;
            IsCollidable = true;
        }

        public override void Update(GameTime gameTime, Player player, Map map)
        {
            base.Update(gameTime, player, map);

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
                effects.Add(eff);
                effectTimer = 0;
            }



            if (wasPickedUp)
            {
                player.Chronoshift(Evolution.Modern);
            }           
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);


            foreach (var eff in effects)
                eff.Draw(spriteBatch);
        }
    }
}
