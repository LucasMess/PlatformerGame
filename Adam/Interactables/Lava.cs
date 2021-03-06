﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Particles;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Interactables
{
    class Lava : Interactable
    {

        GameTimer heatTimer = new GameTimer();
        private static readonly SoundFx bubblingSound = new SoundFx("Sounds/Tiles/lava");

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            entity.TakeDamage(null, 20);
            base.OnEntityTouch(tile, entity);
        }

        public override void Update(Tile tile)
        {
            heatTimer.Increment();
            bubblingSound.PlayIfStopped();

            if (heatTimer.TimeElapsedInMilliSeconds > 500)
            {
                heatTimer.Reset();
                if (GameWorld.GetTileAbove(tile.TileIndex).Id == TMBAW_Game.TileType.Air)
                    ParticleSystem.Add(ParticleType.HeatEffect, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Top), CalcHelper.GetRandXAndY(new Rectangle(-10, -10, 10, 0)) / 10, Color.White);
            }
        }
    }
}
