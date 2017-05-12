using System;

namespace ThereMustBeAnotherWay.Network
{
    [Serializable]
    /// <summary>
    /// Used to send information to the server or client.
    /// </summary>
    public abstract class DataPacket
    {

    }
    
    [Serializable]
    public class Packet
    {
        public byte[] ToByteArray()
        {
            return CalcHelper.ToByteArray(this);
        }

        [Serializable]
        public class TileIdChange : Packet
        {
            public int TileIndex;
            public int TileId;
            public bool IsWall;
        }
    }
}
