using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay
{
    public class Apple
    {
        private readonly GameTimer _changeLevelTimer = new GameTimer();
        private readonly Rectangle _collRectangle;
        private readonly SoundFx _levelFinishedSound;

        public Apple(int x, int y)
        {
            _collRectangle = new Rectangle(x, y, TMBAW_Game.Tilesize, TMBAW_Game.Tilesize);
            _levelFinishedSound = new SoundFx("Sounds/Menu/level_complete");
        }

        public void Update()
        {
            var player = GameWorld.GetPlayers()[0];
            if (player.GetCollRectangle().Intersects(_collRectangle))
            {
                _levelFinishedSound.PlayOnce();
            }

            if (_changeLevelTimer.TimeElapsedInMilliSeconds > 3000)
            {
                TMBAW_Game.GoToMainMenu();
            }
        }
    }
}