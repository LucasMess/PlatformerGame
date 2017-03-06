using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.AdamGame;

namespace Adam
{
    public partial class Tile
    {
        public class TileProperties
        {
            public bool IsSolid { get; set; } = false;
            public Rectangle SourceRectangle { get; set; }
        }

        public static readonly Dictionary<TileType, TileProperties> Properties = new Dictionary<TileType, TileProperties>()
        {
            { TileType.Air, new TileProperties()
            {
                IsSolid = false,
            } },
            { TileType.Grass, new TileProperties()
            {

            } },

        };
    }
}
