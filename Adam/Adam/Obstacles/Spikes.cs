using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Obstacles
{
    public class Spikes : Obstacle
    {
        public Spikes(int x, int y)
        {
            CollRectangle = new Rectangle(x + 4, y + 8, 24, 16);
            SourceRectangle = new Rectangle(0, 0, 32, 32);
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X - 4, CollRectangle.Y - 8, 32, 32);
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsTouchingPlayer)
            {
                Player.Player player = GameWorld.Instance.Player;
                player.TakeDamage(this,player.MaxHealth);
            }
        }


    }
}
