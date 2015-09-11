﻿using Adam;
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
    public abstract class Item : Entity
    {
        public bool wasPickedUp;
        protected Rectangle topMidBound;
        protected double elapsedTime;
        protected SoundFx loopSound;
        protected SoundFx pickUpSound;
        protected SoundFx bounceSound;
        private int tileIndex;
        protected double effectTimer;

        protected delegate void PickedUpHander(PickedUpArgs e);
        protected event PickedUpHander OnPlayerPickUp;


        public Item()
        {

        }

        public override void Update()
        {
            GameWorld gameWorld = GameWorld.Instance;
            Player player = gameWorld.player;
            GameTime gameTime = gameWorld.gameTime;

            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (player.GetCollRectangle().Intersects(DrawRectangle) && elapsedTime > 500)
            {
                if (OnPlayerPickUp != null)
                OnPlayerPickUp(new PickedUpArgs(player));
                pickUpSound?.PlayOnce();
                ToDelete = true;
                loopSound?.Stop();
            }

            loopSound?.PlayIfStopped();

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null) Texture = Main.DefaultTexture;
            spriteBatch.Draw(Texture, DrawRectangle, Color.White);
        }
    }

    /// <summary>
    /// Used to indicate what player picked up the item.
    /// </summary>
    public class PickedUpArgs : EventArgs
    {
        public Player Player { get; private set; }
        public PickedUpArgs(Player player)
        {
            Player = player;
        }
    }
}
