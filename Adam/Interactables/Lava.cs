using Adam.Levels;
using Adam.Misc;
using Adam.PlayerCharacter;

namespace Adam.Interactables
{
    class Lava : Interactable
    {

        private static readonly SoundFx bubblingSound = new SoundFx("Sounds/Tiles/lava");

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            GameWorld.Player.TakeDamage(null, 100);
            base.OnEntityTouch(tile, entity);
        }

        public override void Update(Tile tile)
        {
            bubblingSound.PlayIfStopped();
        }
    }
}
