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
        Texture2D texture;
        SoundFx error;
                        
        public void TrySetSpawn(int index)
        {
            WorldData data = GameWorld.Instance.worldData;
            Tile[] tiles = GameWorld.Instance.tileArray;
            int width = data.LevelWidth;

            //if (tiles[index].isSolid)

            data.SpawnPoint = new Microsoft.Xna.Framework.Vector2(tiles[index].drawRectangle.X, tiles[index].drawRectangle.Y);
        }

        private void ThrowInvalidError()
        {
            //Play error sound.
        }

    }
}
