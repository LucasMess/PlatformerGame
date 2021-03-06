﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.Misc.Interfaces;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;
using ThereMustBeAnotherWay.Particles;

namespace ThereMustBeAnotherWay.Interactables
{
    public class CheckPoint : Interactable
    {
        SoundFx _breakingSound = new SoundFx("Sounds/Tiles/checkpoint_break");
        GameTimer _particleTimer = new GameTimer();
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
                        ParticleSystem.Add(Particles.ParticleType.BlueFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)TMBAW_Game.Random.NextDouble(), Color.White);

                        x *= -1;
                        y *= -1;
                        ParticleSystem.Add(Particles.ParticleType.BlueFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)TMBAW_Game.Random.NextDouble(), Color.White);
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
