using Adam.Levels;
using Adam.Misc;
using Adam.Particles;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace Adam.Interactables
{
    class Lava : Interactable
    {

        Timer heatTimer = new Timer();
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
                if (GameWorld.GetTileAbove(tile.TileIndex).Id == AdamGame.TileType.Air)
                    GameWorld.ParticleSystem.Add(ParticleType.HeatEffect, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Top), CalcHelper.GetRandXAndY(new Rectangle(-10, -10, 10, 0)) / 10, Color.White);
            }
        }
    }
}
