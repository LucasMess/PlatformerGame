using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Particles;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ThereMustBeAnotherWay.Interactables
{
    public abstract class Item : Entity
    {
        public bool WasPickedUp;
        protected Rectangle TopMidBound;
        private Timer _pickUpTimer = new Timer();
        protected SoundFx LoopSound;
        protected SoundFx PickUpSound = new SoundFx("Sounds/Items/gold" + TMBAW_Game.Random.Next(0, 5));
        protected static SoundFx BounceSound;
        protected double EffectTimer;

        protected delegate void PickedUpHander(PickedUpArgs e);
        protected event PickedUpHander OnPlayerPickUp;


        public Item()
        {
            OnPlayerPickUp += SpawnSparkles;
            CollidedWithTerrain += Item_CollidedWithTerrain;
            CollidedWithTileBelow += Item_CollidedWithTileBelow;
            BounceSound = new SoundFx("Sounds/Items/item_pop", this);
            Weight = 70;
        }

        private void Item_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            if (Velocity.Y < 3)
            {
                SetVelY(-7);
            }
        }

        private void Item_CollidedWithTerrain(Entity entity, Tile tile)
        {
            if (Math.Abs(Velocity.Y) > 3)
            {
                BounceSound?.PlayIfStopped();
            }
        }

        private void SpawnSparkles(PickedUpArgs e)
        {

            for (int i = 0; i < 2; i++)
            {
                float randY = (float)(TMBAW_Game.Random.Next(-1, 0) * TMBAW_Game.Random.NextDouble());
                float randX = (float)(TMBAW_Game.Random.Next(-1, 2) * TMBAW_Game.Random.NextDouble());
                GameWorld.ParticleSystem.Add(ParticleType.Tiny, CalcHelper.GetRandXAndY(CollRectangle), new Vector2(randX, randY), Color.Yellow);
            }
        }

        public override void Update()
        {
            Player player = GameWorld.GetPlayer();
            GameTime gameTime = TMBAW_Game.GameTime;

            _pickUpTimer.Increment();

            if (player.GetCollRectangle().Intersects(DrawRectangle) && _pickUpTimer.TimeElapsedInMilliSeconds > 500)
            {
                OnPlayerPickUp?.Invoke(new PickedUpArgs(player));
                PickUpSound?.PlayOnce();
                ToDelete = true;
                LoopSound?.Stop();
            }

            LoopSound?.PlayIfStopped();

            base.Update();
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
