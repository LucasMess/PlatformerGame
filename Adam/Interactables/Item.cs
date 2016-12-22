using Adam.Levels;
using Adam.Misc;
using Adam.Particles;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Adam.Interactables
{
    public abstract class Item : Entity
    {
        public bool WasPickedUp;
        protected Rectangle TopMidBound;
        protected double ElapsedTime;
        protected SoundFx LoopSound;
        protected SoundFx PickUpSound = new SoundFx("Sounds/Items/gold"+AdamGame.Random.Next(0,5));
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
            //if (Math.Abs(Velocity.Y) > 180)
            //{
            //    BounceSound?.Play();
            //}
        }

        private void SpawnSparkles(PickedUpArgs e)
        {

            for (int i = 0; i < 2; i++)
            {
                float randY = (float)(AdamGame.Random.Next(-1, 0) * AdamGame.Random.NextDouble());
                float randX = (float)(AdamGame.Random.Next(-1, 2) * AdamGame.Random.NextDouble());
                GameWorld.ParticleSystem.GetNextParticle().ChangeParticleType(ParticleType.Round_Common, CalcHelper.GetRandXAndY(CollRectangle), new Vector2(randX,randY), Color.Yellow);
            }
        }

        public override void Update()
        {
            Player player = GameWorld.GetPlayer();
            GameTime gameTime = AdamGame.GameTime;

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
            if (Texture == null) Texture = AdamGame.DefaultTexture;
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
