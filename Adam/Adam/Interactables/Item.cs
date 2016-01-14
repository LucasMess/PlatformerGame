using Adam;
using Adam.Misc;
using Adam.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Interactables
{
    public abstract class Item : Entity
    {
        public bool WasPickedUp;
        protected Rectangle TopMidBound;
        protected double ElapsedTime;
        protected SoundFx LoopSound;
        protected SoundFx PickUpSound = new SoundFx("Sounds/coin");
        protected SoundFx BounceSound;
        private int _tileIndex;
        protected double EffectTimer;

        protected delegate void PickedUpHander(PickedUpArgs e);
        protected event PickedUpHander OnPlayerPickUp;


        public Item()
        {
            OnPlayerPickUp += SpawnSparkles;
            CollidedWithTerrain += Item_CollidedWithTerrain;
            BounceSound = new SoundFx("Sounds/Items/item_pop",this);
        }

        private void Item_CollidedWithTerrain(Entity entity, Tile tile)
        {
            if (Math.Abs(Velocity.Y) > 3)
            {
                BounceSound?.Play();
            }
        }

        private void SpawnSparkles(PickedUpArgs e)
        {

            for (int i = 0; i < 2; i++)
            {
                float randY = (float)(GameWorld.RandGen.Next(-1, 0) * GameWorld.RandGen.NextDouble());
                float randX = (float)(GameWorld.RandGen.Next(-1, 2) * GameWorld.RandGen.NextDouble());
                RoundCommonParticle par = new RoundCommonParticle(CollRectangle.Center.X, CollRectangle.Center.Y, new Vector2(randX,randY), Color.Yellow);
                GameWorld.ParticleSystem.Add(par);
            }
        }

        public override void Update()
        {
            GameWorld gameWorld = GameWorld.Instance;
            Player.Player player = gameWorld.Player;
            GameTime gameTime = gameWorld.GameTime;

            ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (player.GetCollRectangle().Intersects(DrawRectangle) && ElapsedTime > 500)
            {
                if (OnPlayerPickUp != null)
                OnPlayerPickUp(new PickedUpArgs(player));
                PickUpSound?.PlayOnce();
                ToDelete = true;
                LoopSound?.Stop();
            }

            LoopSound?.PlayIfStopped();

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
        public Player.Player Player { get; private set; }
        public PickedUpArgs(Player.Player player)
        {
            Player = player;
        }
    }
}
