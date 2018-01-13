using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Particles;
using Microsoft.Xna.Framework;
using System.Collections;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    class FallingBoulderBehavior : Behavior
    {
        Vector2 originalPosition;
        bool isFalling;
        const float RecoilVelocity = -5f;
        const double TimeBetweenFalls = 2000;
        const double TimeBetweenImpactAndRecoil = 1000;
        GameTimer fallAgainTimer = new GameTimer();
        GameTimer riseTimer = new GameTimer();
        SoundFx _impactSound = new SoundFx("Sounds/Tiles/boulder_fall");

        public override void Initialize(Entity entity)
        {
            originalPosition = entity.Position;

            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;

            base.Initialize(entity);
        }

        private void Entity_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            if (isFalling)
            {
                for (int i = 0; i < 20; i++)
                {
                    GameWorld.ParticleSystem.Add(ParticleType.Smoke, CalcHelper.GetRandXAndY(new Rectangle(entity.GetCollRectangle().X, entity.GetCollRectangle().Bottom, entity.GetCollRectangle().Width, 1)),
                                CalcHelper.GetRandXAndY(new Rectangle(-40, -5, 80, 10))/10f, Color.White);
                }
            }
            isFalling = false;
        }

        public override void Update(Entity entity)
        {
            entity.SetVelX(0);

            if (isFalling)
            {
                _impactSound.Reset();
                fallAgainTimer.Increment();
                if (fallAgainTimer.TimeElapsedInMilliSeconds > TimeBetweenFalls)
                    entity.ObeysGravity = true;
            }
            else
            {
                _impactSound.PlayOnce();
                riseTimer.Increment();
                if (riseTimer.TimeElapsedInMilliSeconds > TimeBetweenImpactAndRecoil)
                {
                    entity.ObeysGravity = false;
                    entity.SetVelY(RecoilVelocity);
                    if (entity.Position.Y <= originalPosition.Y)
                    {
                        isFalling = true;
                        entity.SetVelY(0);
                        entity.SetY(originalPosition.Y);
                        fallAgainTimer.Reset();
                        riseTimer.Reset();
                    }
                }
            }

            base.Update(entity);
        }
    }
}
