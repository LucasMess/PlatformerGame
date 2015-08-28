using Adam;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    public class Item : Entity
    {
        public bool wasPickedUp;
        protected Rectangle topMidBound;
        protected double elapsedTime;
        protected SoundFx loopSound;
        protected SoundFx pickUpSound;
        protected SoundFx bounceSound;
        int tileIndex;
        protected double effectTimer;

        public Item()
        {
            //bounceSound = new SoundFx("Sounds/Items/item_pop", this);
            //pickUpSound = new SoundFx("Sounds/Items/item_pop");
        }

        public override void Update()
        {
            this.gameWorld = GameWorld.Instance;
            Player player = gameWorld.player;
            GameTime gameTime = gameWorld.gameTime;

            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (player.collRectangle.Intersects(drawRectangle) && elapsedTime > 500)
            {
                pickUpSound?.PlayOnce();
                toDelete = true;
                loopSound?.Stop();
            }

            loopSound?.PlayIfStopped();

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null) Texture = Main.DefaultTexture;
            spriteBatch.Draw(Texture, drawRectangle, Color.White);
        }
    }
}
