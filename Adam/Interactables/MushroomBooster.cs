using Adam.PlayerCharacter;

namespace Adam.Interactables
{
    class MushroomBooster : Interactable
    {
        const float WeakBoost = -10f;
        const float StrongBoost = -20;

        public MushroomBooster()
        {
            
        }

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            if (entity is Player)
            {
                Player player = (Player)entity;
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
