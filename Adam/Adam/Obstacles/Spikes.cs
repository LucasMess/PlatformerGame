﻿using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    public class Spikes : Obstacle
    {
        public Spikes(int x, int y)
        {
            drawRectangle = new Rectangle(x, y, 32, 32);
            collRectangle = new Rectangle(x + 4, y + 8, 24, 16);
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            texture = ContentHelper.LoadTexture("Tiles/spikes");
        }

        public override void Update(GameTime gameTime, Player player, GameWorld map)
        {
            base.Update(gameTime, player, map);

            if (IsTouching)
            {
                player.PlayGoreSound();
                player.KillAndRespawn();
            }
        }


    }
}
