using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ThereMustBeAnotherWay;
using ThereMustBeAnotherWay.Levels;

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

        /// <summary>
        /// Covers the edge cases for the get tile function in the gameworld.
        /// </summary>
        //[TestMethod]
        //public void EdgeCases_GetTile()
        //{
        //    Game game = new Game();
        //    TMBAW_Game.Content = new ContentManager(game.Services, "Content");
        //    GameWorld.TileArray = new Tile[16];
        //    GameWorld.WorldData.LevelWidth = 4;
        //    for (int i = 0; i < 16; i++)
        //    {
        //        Tile tile = new Tile(i % 4, i / 4);
        //        tile.Id = TMBAW_Game.TileType.Grass;
        //    }
        //    Tile outOfBounds = GameWorld.GetTileAbove(0);            
        //    Assert.IsTrue(ReferenceEquals(outOfBounds, Tile.Default));

        //    outOfBounds = GameWorld.GetTile(-1);
        //    Assert.IsTrue(ReferenceEquals(outOfBounds, Tile.Default));

        //    outOfBounds = GameWorld.GetTile(17);
        //    Assert.IsTrue(ReferenceEquals(outOfBounds, Tile.Default));

        //    outOfBounds = GameWorld.GetTileBelow(15);
        //    Assert.IsTrue(ReferenceEquals(outOfBounds, Tile.Default));
        //}

        //[TestMethod]
        //public void EdgeCases_DefineTexture()
        //{
        //    GameWorld.TileArray = new Tile[16];
        //    GameWorld.WorldData.LevelWidth = 4;
        //    for (int i = 0; i < 16; i++)
        //    {
        //        Tile tile = new Tile(i % 4, i / 4);
        //        tile.Id = TMBAW_Game.TileType.Grass;
        //        tile.DefineTexture();
        //    }
        //}
    }
}
