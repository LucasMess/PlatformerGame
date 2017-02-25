using Adam.Levels;
using Adam.Misc;
using Adam.Particles;
using Microsoft.Xna.Framework;

namespace Adam.Interactables
{
    class Torch : Interactable
    {

        private Timer _fireParticleTimer = new Timer();
        private Timer _smokeParticleTimer = new Timer();
        private Timer _heatTimer = new Timer();
        public override void Update(Tile tile)
        {

            _fireParticleTimer.Increment();
            _smokeParticleTimer.Increment();
            _heatTimer.Increment();
            if (_fireParticleTimer.TimeElapsedInMilliSeconds > 500)
            {
                _fireParticleTimer.Reset();
                float velY = (float)-AdamGame.Random.NextDouble();
                GameWorld.ParticleSystem.Add(ParticleType.Flame, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Center.Y - 10), new Vector2(0, velY), Color.White);

            }

            if (_heatTimer.TimeElapsedInMilliSeconds > 100)
            {
                _heatTimer.Reset();
                float velY = (float)-AdamGame.Random.NextDouble();
                GameWorld.ParticleSystem.Add(ParticleType.HeatEffect, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Top ), CalcHelper.GetRandXAndY(new Rectangle(-10,-10,20,0))/10, Color.White);
            }
            if (_smokeParticleTimer.TimeElapsedInMilliSeconds > 400)
            {
                _smokeParticleTimer.Reset();
                float velY = (float)-AdamGame.Random.NextDouble();
                GameWorld.ParticleSystem.Add(ParticleType.Flame, new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Center.Y - 10), new Vector2(0, velY), new Color(30, 30, 30, 150));
            }
            base.Update(tile);
        }
    }
}
