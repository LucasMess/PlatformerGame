using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Adam.Misc;

namespace Adam
{
    public class Apple
    {
        Rectangle collRectangle;
        SoundFx levelFinishedSound;
        bool wasPicked;
        Timer changeLevelTimer = new Timer();

        public Apple(int x, int y)
        {
            collRectangle = new Rectangle(x, y, Main.Tilesize, Main.Tilesize);
            levelFinishedSound = new SoundFx("Sounds/Menu/level_complete");
            levelFinishedSound.MaxVolume = .2f;
        }

        public void Update()
        {
            Player player = GameWorld.Instance.player;
            if (player.GetCollRectangle().Intersects(collRectangle))
            {
                levelFinishedSound.PlayOnce();
                wasPicked = true;
            }

            if (wasPicked)
            {
                changeLevelTimer.Increment();
            }

            if (changeLevelTimer.TimeElapsedInMilliSeconds > 3000)
            {
                Main.Instance.ChangeState(GameState.MainMenu, GameMode.None);
            }
        }
    }
}
