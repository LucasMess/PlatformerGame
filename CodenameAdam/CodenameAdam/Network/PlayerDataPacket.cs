using CodenameAdam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Network
{
    class PlayerDataPacket
    {
        Vector2 position;
        Vector2 velocity;
        Evolution CurrentEvolution;
        Player.AnimationState CurrentAnimationState;

        public void Update(Player player)
        {
            CurrentEvolution = player.CurrentEvolution;
            velocity = player.velocity;
            position = player.position;
            CurrentAnimationState = player.CurrentAnimation;
        }
    }
}
