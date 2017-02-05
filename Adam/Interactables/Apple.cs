using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam
{
    public class Apple
    {
        private readonly Timer _changeLevelTimer = new Timer();
        private readonly Rectangle _collRectangle;
        private readonly SoundFx _levelFinishedSound;
        private bool _wasPicked;

        public Apple(int x, int y)
        {
            _collRectangle = new Rectangle(x, y, AdamGame.Tilesize, AdamGame.Tilesize);
            _levelFinishedSound = new SoundFx("Sounds/Menu/level_complete");
            _levelFinishedSound.MaxVolume = .2f;
        }

        public void Update()
        {
            var player = GameWorld.Player;
            if (player.GetCollRectangle().Intersects(_collRectangle))
            {
                _levelFinishedSound.PlayOnce();
                _wasPicked = true;
            }

            if (_changeLevelTimer.TimeElapsedInMilliSeconds > 3000)
            {
                AdamGame.ChangeState(GameState.MainMenu, GameMode.None, false);
            }
        }
    }
}