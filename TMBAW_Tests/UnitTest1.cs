using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adam;

namespace TMBAW_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TileEquals_CorrectValue()
        {
            Tile tile = new Tile(0, 0);
            Tile other = new Tile(100, 100);
            tile.Id = AdamGame.TileType.AquaantCrystal;
            other.Id = AdamGame.TileType.Mud;

            bool val = (tile.Equals(other));
            Assert.AreEqual(val, false);

            Tile same = new Tile(100, 200);
            same.Id = AdamGame.TileType.Mud;
            val = (other.Equals(same));
            Assert.AreEqual(val, true);
        }
    }
}
