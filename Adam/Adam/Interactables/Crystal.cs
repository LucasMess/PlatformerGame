using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;

namespace Adam.Interactables
{
    public class Crystal
    {
        Rectangle collRectangle;
        bool broken;
        SoundFx breakSound;
        Tile sourceTile;
        byte gemID;

        public Crystal(Tile sourceTile, byte gemID)
        {
            this.gemID = gemID;
            this.sourceTile = sourceTile;

            sourceTile.OnTileDestroyed += SourceTile_OnTileDestroyed;
            sourceTile.OnTileUpdate += Update;

            collRectangle = sourceTile.drawRectangle;

            int rand = GameWorld.RandGen.Next(1, 9);
            breakSound = new SoundFx("Sounds/Crystal/Glass_0" + rand, GameWorld.Instance.player);
        }

        private void SourceTile_OnTileDestroyed(Tile t)
        {
            sourceTile.OnTileUpdate -= Update;
            sourceTile.OnTileUpdate -= SourceTile_OnTileDestroyed;
        }

        public void Update(Tile t)
        {
            Player player = GameWorld.Instance.player;

            if (player.GetCollRectangle().Intersects(collRectangle) && !broken)
            {
                breakSound.Play();
                broken = true;
                Gem.GenerateIdentical(gemID, sourceTile, GameWorld.RandGen.Next(6, 15));
                sourceTile.ID = 0;
                sourceTile.DefineTexture();
                GameWorld.Instance.lightEngine.UpdateSunLight(sourceTile.TileIndex);
            }
        }
    }
}
