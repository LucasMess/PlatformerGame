using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.Characters.Scripts
{
    class FallingBoulderScript : Behavior
    {
        Vector2 originalPosition;
        bool isFalling;
        const float RecoilVelocity = -5f;
        const double TimeBetweenFalls = 2000;
        const double TimeBetweenImpactAndRecoil = 1000;
        Timer fallAgainTimer = new Timer();
        Timer riseTimer = new Timer();

        public override void Initialize(Entity entity)
        {
            originalPosition = entity.Position;

            entity.CollidedWithTileBelow += Entity_CollidedWithTileBelow;

            base.Initialize(entity);
        }

        private void Entity_CollidedWithTileBelow(Entity entity, Tile tile)
        {
            isFalling = false;
        }

        public override void Update(Entity entity)
        {
            entity.SetVelX(0);

            if (isFalling)
            {
                fallAgainTimer.Increment();
                if (fallAgainTimer.TimeElapsedInMilliSeconds > TimeBetweenFalls)
                    entity.ObeysGravity = true;
            }
            else
            {
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
