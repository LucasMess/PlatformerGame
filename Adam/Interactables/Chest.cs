using ThereMustBeAnotherWay.Interactables;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ThereMustBeAnotherWay
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
            _collRectangle = new Rectangle(tile.DrawRectangle.X, tile.DrawRectangle.Y, TMBAW_Game.Tilesize * 2, TMBAW_Game.Tilesize);
            tile.AnimationStopped = true;

            CanBeLinkedToOtherInteractables = true;
            CanBeLinkedByOtherInteractables = true;
        }

        public override void Update(Tile t)
        {
            t.AnimationStopped = !_isOpen;
            foreach (Player player in GameWorld.GetPlayers())
                if (player.GetCollRectangle().Intersects(_collRectangle) && !_isOpen)
                {
                    // If player presses open button, open chest.
                    if (InputSystem.IsKeyDown(Keys.W))
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

                int maxGems = TMBAW_Game.Random.Next(10, 20);
                for (int i = 0; i < maxGems; i++)
                {
                    GameWorld.Entities.Add(new Gem(_collRectangle.Center.X, _collRectangle.Center.Y));
                }
            }

            base.OnPlayerAction(tile, player);
        }
    }
}
