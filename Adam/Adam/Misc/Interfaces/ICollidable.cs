using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Interfaces
{
    delegate void TerrainCollisionHandler(TerrainCollisionEventArgs e);

    interface ICollidable
    {
        //event TerrainCollisionHandler CollidedWithTerrainAbove;
        //event TerrainCollisionHandler CollidedWithTerrainBelow;
        //event TerrainCollisionHandler CollidedWithTerrainRight;
        //event TerrainCollisionHandler CollidedWithTerrainLeft;
        //event TerrainCollisionHandler CollidedWithTerrainAnywhere;

        void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e);
        void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e);
        void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e);
        void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e);
        void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e);

        

    }

    public class TerrainCollisionEventArgs : EventArgs
    {
        Tile tile;
        public TerrainCollisionEventArgs(Tile tile)
        {
            this.tile = tile;
        }

        public Tile Tile { get { return tile; } }
    }
}
