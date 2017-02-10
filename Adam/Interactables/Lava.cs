using Adam.Levels;
using Adam.Misc;

namespace Adam.Interactables
{
    class Lava : Interactable
    {

        private static readonly SoundFx bubblingSound = new SoundFx("Sounds/Tiles/lava");

        public override void OnPlayerTouch(Tile tile)
        {
            GameWorld.Player.TakeDamage(null, 100);
            base.OnPlayerTouch(tile);
        }

        public override void Update(Tile tile)
        {
            bubblingSound.PlayIfStopped();
        }
    }
}
