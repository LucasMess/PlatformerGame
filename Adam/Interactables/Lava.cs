using Adam.Levels;
using Adam.Misc;

namespace Adam.Interactables
{
    class Lava : Interactable
    {
        private static readonly SoundFx Bubbling = new SoundFx("Sounds/Tiles/lava");

        public override void OnPlayerTouch()
        {
            GameWorld.Player.TakeDamage(null, 100);
            base.OnPlayerTouch();
        }

        public override void OnTileUpdate(Tile tile)
        {
            Bubbling.PlayIfStopped();
        }
    }
}
