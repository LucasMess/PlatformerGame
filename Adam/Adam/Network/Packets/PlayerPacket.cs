using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Network.Packets
{
    [Serializable]
    public class PlayerPacket : DataPacket
    {
        Vector2 position;
        Vector2 velocity;

        public PlayerPacket(Player player)
        {
            position = new Vector2(player.GetCollRectangle().X, player.GetCollRectangle().Y);
            velocity = player.GetVelocity();
        }
    }
}
