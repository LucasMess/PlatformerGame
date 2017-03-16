using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Interfaces;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;

namespace Adam.Interactables
{
    public class CheckPoint : Interactable
    {
        SoundFx _breakingSound = new SoundFx("Sounds/Tiles/checkpoint_break");
        Timer _particleTimer = new Timer();
        int lastDeg = 0;
        int changeInDeg = 10;
        int radius = 50;
        bool active = false;

        public CheckPoint(Tile tile)
        {
            tile.AnimationStopped = true;
        }

        public override void Update(Tile tile)
        {
            if (active && radius < 500)
            {
                _particleTimer.Increment();
                if (_particleTimer.TimeElapsedInMilliSeconds > 10)
                {
                    _particleTimer.Reset();
                    radius++;
                    Vector2 position = new Vector2(tile.GetDrawRectangle().Center.X, tile.GetDrawRectangle().Center.Y);
                    lastDeg %= 360;
                    for (int i = lastDeg; i < lastDeg + changeInDeg; i += 1)
                    {
                        double rads = Math.PI * i / 180;
                        float x = (float)Math.Cos(rads);
                        float y = (float)Math.Sin(rads);
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.RewindFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)AdamGame.Random.NextDouble(), Color.White);

                        x *= -1;
                        y *= -1;
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.RewindFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)AdamGame.Random.NextDouble(), Color.White);
                    }
                    lastDeg += changeInDeg;


                }
            }


            base.Update(tile);
        }

        public override void OnEntityTouch(Tile tile, Entity entity)
        {

            if (entity is Player)
            {
                _breakingSound.PlayOnce();
                Player player = (Player)entity;
                player.SetRespawnPoint(tile.DrawRectangle.X, tile.DrawRectangle.Y);
                tile.AnimationStopped = false;
                base.OnEntityTouch(tile, entity);

                active = true;




            }
        }
    }
}
