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
        Vector2 _position;
        Vector2 _velocity;

        public PlayerPacket(Player.Player player)
        {
            _position = new Vector2(player.GetCollRectangle().X, player.GetCollRectangle().Y);
            _velocity = player.GetVelocity();
        }
    }
}
