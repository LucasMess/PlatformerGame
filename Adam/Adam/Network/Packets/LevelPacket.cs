using Adam.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Network.Packets
{
    [Serializable]
    public class LevelPacket : DataPacket
    {
        WorldConfigFile config;

        public LevelPacket(WorldConfigFile config)
        {
            this.config = config;
        }
    }
}
