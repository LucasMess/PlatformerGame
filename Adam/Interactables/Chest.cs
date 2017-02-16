using Adam.Interactables;
using Adam.Levels;
using Adam.Misc;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    public class Chest : Interactable
    {
        bool _isOpen;
        SoundFx _openSound;
        Rectangle _collRectangle;

        public bool IsGolden { get; set; }

        public Chest(Tile tile)
        {
            _openSound = new SoundFx("Sounds/Chest/open");
            _collRectangle = new Rectangle(tile.DrawRectangle.X, tile.DrawRectangle.Y, AdamGame.Tilesize * 2, AdamGame.Tilesize);
            tile.AnimationStopped = true;

            CanBeLinkedToOtherInteractables = true;
            CanBeLinkedByOtherInteractables = true;
        }

        public override void Update(Tile t)
        {
            t.AnimationStopped = !_isOpen;
            Player player = GameWorld.Player;
            if (player.GetCollRectangle().Intersects(_collRectangle) && !_isOpen)
            {
                // If player presses open button, open chest.
                if (InputHelper.IsKeyDown(Keys.W))
                {
                    OnPlayerAction(t, player);
                }
            }
        }

        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (!_isOpen)
            {
                _openSound.PlayOnce();
                _isOpen = true;

                int maxGems = AdamGame.Random.Next(10, 20);
                for (int i = 0; i < maxGems; i++)
                {
                    GameWorld.Entities.Add(new Gem(_collRectangle.Center.X, _collRectangle.Center.Y));
                }
            }

            base.OnPlayerAction(tile, player);
        }
    }
}
