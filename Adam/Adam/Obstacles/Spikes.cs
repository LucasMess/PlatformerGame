using Adam;
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
            collRectangle = new Rectangle(x + 4, y + 8, 24, 16);
            sourceRectangle = new Rectangle(0, 0, 32, 32);
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(collRectangle.X - 4, collRectangle.Y - 8, 32, 32);
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsTouchingPlayer)
            {
                Player player = GameWorld.Instance.player;
                player.KillAndRespawn();
            }
        }


    }
}
