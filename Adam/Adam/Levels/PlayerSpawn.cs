using Adam.Misc;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Levels
{
    public class PlayerSpawn
    {
        Texture2D _texture;
        SoundFx _error;
                        
        public void TrySetSpawn(int index)
        {
            WorldData data = GameWorld.Instance.WorldData;
            Tile[] tiles = GameWorld.Instance.TileArray;
            int width = data.LevelWidth;

            //if (tiles[index].isSolid)

            data.SpawnPoint = new Microsoft.Xna.Framework.Vector2(tiles[index].DrawRectangle.X, tiles[index].DrawRectangle.Y);
        }

        private void ThrowInvalidError()
        {
            //Play error sound.
        }

    }
}
