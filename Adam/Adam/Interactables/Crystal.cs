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

            collRectangle = sourceTile.drawRectangle;

            int rand = GameWorld.RandGen.Next(1, 9);
            breakSound = new SoundFx("Sounds/Crystal/Glass_0" + rand, GameWorld.Instance.player);
        }

        public void Update()
        {
            Player player = GameWorld.Instance.player;
            player.Score += 1000;
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
