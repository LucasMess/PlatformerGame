using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Network
{
    public class PlayerDataPacket
    {
        Vector2 position;
        Vector2 velocity;
        Evolution CurrentEvolution;
        Player.AnimationState CurrentAnimationState;

        public void Update(Player player)
        {
        }
    }
}
