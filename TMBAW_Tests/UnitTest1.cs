using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThereMustBeAnotherWay;

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
            tile.Id = TMBAW_Game.TileType.AquaantCrystal;
            other.Id = TMBAW_Game.TileType.Mud;

            bool val = (tile.Equals(other));
            Assert.AreEqual(val, false);

            Tile same = new Tile(100, 200);
            same.Id = TMBAW_Game.TileType.Mud;
            val = (other.Equals(same));
            Assert.AreEqual(val, true);
        }
    }
}
