using Adam.GameData;
using System;

namespace Adam.Network.Packets
{
    [Serializable]
    public class LevelPacket : DataPacket
    {
        WorldConfigFile _config;

        public LevelPacket(WorldConfigFile config)
        {
            this._config = config;
        }

        public WorldConfigFile ExtractConfigFile()
        {
            return _config;
        }
    }
}
