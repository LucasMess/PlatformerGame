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
        public override void OnTileUpdate(Tile tile)
        {
            if (_fireParticleTimer.TimeElapsedInMilliSeconds > 500)
            {
                _fireParticleTimer.Reset();
                float velY = (float)-Main.Random.NextDouble() * 60;
                FlameParticle par = new FlameParticle(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Center.Y - 10, new Vector2(0, velY));
                GameWorld.ParticleSystem.Add(par);
            }
            if (_smokeParticleTimer.TimeElapsedInMilliSeconds > 400)
            {
                _smokeParticleTimer.Reset();
                float velY = (float)-Main.Random.NextDouble() * 60;
                SmokeParticle par =new SmokeParticle(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Top - 10, new Vector2(0, velY), new Color(30,30,30, 150));
                GameWorld.ParticleSystem.Add(par);
            }
            base.OnTileUpdate(tile);
        }
    }
}
