using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Particles;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Interactables
{
    class Torch : Interactable
    {

        private GameTimer _fireParticleTimer = new GameTimer();
        private GameTimer _smokeParticleTimer = new GameTimer();
        private GameTimer _heatTimer = new GameTimer();
        public override void Update(Tile tile)
        {

            _fireParticleTimer.Increment();
            _smokeParticleTimer.Increment();
            _heatTimer.Increment();
            //Flame
            if (_fireParticleTimer.TimeElapsedInMilliSeconds > 500)
            {
                _fireParticleTimer.Reset();
                float velY = (float)-TMBAW_Game.Random.NextDouble();
                Color color = Color.White;
                if (tile.Id == TMBAW_Game.TileType.GreenTorch)
                {
                    color = Color.Green;
                }
                ParticleSystem.Add(ParticleType.Flame, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Center.Y - 10), new Vector2(0, velY), color);

            }
            // Heat effect.
            if (_heatTimer.TimeElapsedInMilliSeconds > 100)
            {
                _heatTimer.Reset();
                float velY = (float)-TMBAW_Game.Random.NextDouble();
                ParticleSystem.Add(ParticleType.HeatEffect, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Top), CalcHelper.GetRandXAndY(new Rectangle(-10, -10, 20, 0)) / 10, Color.White);
            }
            // Smoke
            if (_smokeParticleTimer.TimeElapsedInMilliSeconds > 400)
            {
                _smokeParticleTimer.Reset();
                float velY = (float)-TMBAW_Game.Random.NextDouble();
                ParticleSystem.Add(ParticleType.Flame, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Center.Y - 10), new Vector2(0, velY), new Color(30, 30, 30, 150));
            }
            base.Update(tile);
        }
    }
}
