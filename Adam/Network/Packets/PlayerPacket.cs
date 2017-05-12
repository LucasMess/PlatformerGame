using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;

namespace ThereMustBeAnotherWay.Network.Packets
{
    [Serializable]
    public class PlayerPacket : DataPacket
    {
        Vector2 _position;
        Vector2 _velocity;

        public PlayerPacket(Player player)
        {
            _position = new Vector2(player.GetCollRectangle().X, player.GetCollRectangle().Y);
            _velocity = player.GetVelocity();
        }
    }
}
