using ThereMustBeAnotherWay.PlayerCharacter;

namespace ThereMustBeAnotherWay.Interactables
{
    class MushroomBooster : Interactable
    {
        public const float WeakBoost = -10f;
        public const float StrongBoost = -20;

        public MushroomBooster()
        {
            
        }

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            if (entity is Player player)
            {
                player.AddAnimationToQueue("jump");
                player.IsJumping = true;
                tile.CurrentFrame = 1;
                if (player.IsJumpButtonPressed())
                {
                    player.SetVelY(StrongBoost);
                }
                else
                {
                    player.SetVelY(WeakBoost);
                }
            }
            else
            {
                entity.SetVelY(WeakBoost);
            }

            tile.AnimationStopped = false;

            base.OnEntityTouch(tile, entity);
        }
    }
}
