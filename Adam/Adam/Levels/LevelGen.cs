using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class LevelGen
    {
        public Tile[] tileArray;
        public Tile[] wallArray;
        Room room;
        int[] visibleTileArray = new int[30 * 50];
        int playerTilePos;
        int mapWidth, mapHeight;
        int entranceWidth, entranceHeight;
        int tileSize;
        int exitTilePos;

        List<Climbables> climbablesList = new List<Climbables>();

        public LevelGen()
        {
            this.tileSize = Game1.Tilesize;
            mapWidth = 300;
            mapHeight = 300;
            entranceWidth = 4;
            entranceHeight = 4;
            tileArray = new Tile[mapHeight * mapWidth];
            wallArray = new Tile[mapHeight * mapWidth];
            room = new Room(mapWidth, mapHeight);
        }

        public void GenerateNewLevel(ContentManager Content, Player player)
        {
            for (int i = 0; i < tileArray.Length; i++)
            {
                tileArray[i] = new Tile();
                tileArray[i].ID = 2;
                tileArray[i].isSolid = true;
                tileArray[i].TileIndex = i;

                int Xcoor = (i % mapWidth) * tileSize;
                int Ycoor = ((i - (i % mapWidth)) / mapWidth) * tileSize;

                tileArray[i].rectangle = new Rectangle(Xcoor, Ycoor, tileSize, tileSize);
            }

            GenerateEntrance(player);
            GenerateExit();
            GenerateMiners();
            //GeneratePathToExit();
            GenerateClimbables();
            GenerateWalls();
            GenerateBoundaries();

            room.AddExit(exitTilePos, 3, tileArray);

            foreach (Tile t in tileArray)
                t.DefineTexture();
            foreach (Tile w in wallArray)
                w.DefineTexture();
        }

        void GenerateEntrance(Player player)
        {
            //Creates an entrance at the top left corner of the map.
            for (int h = 0; h <= entranceHeight; h++)
            {
                for (int w = 0; w <= entranceWidth; w++)
                {
                    tileArray[w + h * mapWidth].ID = 0;
                    tileArray[w + h * mapWidth].isSolid = false;
                }
            }
            //Put player in the middlish of the room.
            player.Initialize(tileArray[mapWidth * 2 + 2].rectangle.X, tileArray[mapWidth * 2 + 2].rectangle.Y);
        }

        void GenerateCaves()
        {
            foreach (Tile t in tileArray)
            {
                t.randDens = Map.randGen.Next(0, 1000);
            }

            for (int i = 0; i < tileArray.Length; i++)
            {
                if (i - mapWidth - 1 >= 0 && i + mapWidth + 1 < tileArray.Length)
                    tileArray[i].avgDens =
                        (tileArray[i - mapWidth - 1].randDens +
                        tileArray[i - mapWidth + 1].randDens +
                        tileArray[i - mapWidth].randDens +
                        tileArray[i - 1].randDens +
                        tileArray[i + 1].randDens +
                        tileArray[i + mapWidth - 1].randDens +
                        tileArray[i + mapWidth + 1].randDens +
                        tileArray[i + mapWidth].randDens +
                        tileArray[i].randDens)
                        / 9;
            }

            for (int i = 0; i < tileArray.Length; i++)
            {
                if (i - mapWidth - 1 >= 0 && i + mapWidth + 1 < tileArray.Length)
                    tileArray[i].tempDens =
                        (tileArray[i - mapWidth - 1].avgDens +
                        tileArray[i - mapWidth + 1].avgDens +
                        tileArray[i - mapWidth].avgDens +
                        tileArray[i - 1].avgDens +
                        tileArray[i + 1].avgDens +
                        tileArray[i + mapWidth - 1].avgDens +
                        tileArray[i + mapWidth + 1].avgDens +
                        tileArray[i + mapWidth].avgDens +
                        tileArray[i].avgDens)
                        / 9;
            }

            for (int i = 0; i < tileArray.Length; i++)
            {
                if (i - mapWidth - 1 >= 0 && i + mapWidth + 1 < tileArray.Length)
                    tileArray[i].temp2Dens =
                        (tileArray[i - mapWidth - 1].tempDens +
                        tileArray[i - mapWidth + 1].tempDens +
                        tileArray[i - mapWidth].tempDens +
                        tileArray[i - 1].tempDens +
                        tileArray[i + 1].tempDens +
                        tileArray[i + mapWidth - 1].tempDens +
                        tileArray[i + mapWidth + 1].tempDens +
                        tileArray[i + mapWidth].tempDens +
                        tileArray[i].tempDens)
                        / 9;
            }


            for (int i = 0; i < tileArray.Length; i++)
            {
                if (tileArray[i].temp2Dens < 500)
                {
                    tileArray[i].ID = 0;
                    tileArray[i].isSolid = false;
                }
            }


        }

        //test
        void GenerateMiners()
        {
            MinerManager manager = new MinerManager(mapWidth, mapHeight);
            manager.Initialize(0,exitTilePos);
            manager.BeginExcavation(ref tileArray);
        }

        void GenerateExit()
        {
            bool exitMade = false;
            int lastTileRowPos = tileArray.Length - mapWidth;
            int currentWidthTilePos = 0;
            while (!exitMade)
            {
                for (int i = lastTileRowPos; i < tileArray.Length - entranceWidth; i++)
                {
                    int randomNumber = Map.randGen.Next(0, 200); //[0,200)
                    if (randomNumber == 1)
                    {
                        exitTilePos = lastTileRowPos + currentWidthTilePos - (mapHeight * entranceHeight);
                        //Creates an exit at specified location
                        room.AddExit(exitTilePos, entranceWidth, tileArray);
                        exitMade = true;
                        break;
                    }
                    else
                    {
                        if (currentWidthTilePos >= mapWidth - 1 - entranceWidth)
                            currentWidthTilePos = 0;
                        else currentWidthTilePos++;
                    }
                }
            }
        }

        void GeneratePathToExit()
        {
            bool reachedExit = false;
            int stPos = mapWidth * entranceHeight;
            int availableX = exitTilePos % mapWidth;
            int availableY = (exitTilePos / mapHeight) - (stPos / mapHeight);

            while (!reachedExit)
            {
                int currentPos = exitTilePos - (availableX) - (availableY * mapHeight);

                if (availableX == 0 && availableY == 0)
                {
                    reachedExit = true;
                    break;
                }

                int rand = Map.randGen.Next(0, 2);

                if (availableX == 0)
                {
                    room.Add(currentPos, 3, ref tileArray);
                    availableY--;
                }
                else if (availableY == 0)
                {
                    room.Add(currentPos, 3, ref tileArray);
                    availableX--;
                }
                else if (rand == 0) //down
                {
                    room.Add(currentPos, 3, ref tileArray);
                    availableY--;
                }
                else if (rand == 1)//right
                {
                    room.Add(currentPos, 3, ref tileArray);
                    availableX--;
                }


            }
        }

        void GenerateWalls()
        {
            for (int i = 0; i < wallArray.Length; i++)
            {
                wallArray[i] = new Tile();

                int rand = Map.randGen.Next(0, 2);
                if (rand == 0)
                    wallArray[i].ID = 101;
                else wallArray[i].ID = 102;

                int Xcoor = (i % mapWidth) * tileSize;
                int Ycoor = ((i - (i % mapWidth)) / mapWidth) * tileSize;

                wallArray[i].rectangle = new Rectangle(Xcoor, Ycoor, tileSize, tileSize);
            }
        }

        void GenerateClimbables()
        {
            for (int i = 0; i < tileArray.Length; i++)
            {
                if (i + mapWidth < tileArray.Length)
                {
                    if (tileArray[i].isSolid && tileArray[i + mapWidth].ID == 0)
                    {
                        int rand = Map.randGen.Next(0, 15);
                        if (rand == 1)
                        {
                            tileArray[i + mapWidth].ID = 14;
                            climbablesList.Add(new Climbables(tileArray[i + mapWidth].rectangle.X, tileArray[i + mapWidth].rectangle.Y));
                        }
                    }
                }
            }

            for (int i = 0; i < tileArray.Length; i++)
            {
                if (i + mapWidth < tileArray.Length)
                {
                    if (tileArray[i].ID == 14 && tileArray[i + mapWidth].ID == 0)
                    {
                        tileArray[i + mapWidth].ID = 14;
                        climbablesList.Add(new Climbables(tileArray[i + mapWidth].rectangle.X, tileArray[i + mapWidth].rectangle.Y));
                    }
                }
            }

        }

        void GenerateBoundaries()
        {
            //leftboundary
            for (int i = 0; i < tileArray.Length; i += mapWidth)
            {
                tileArray[i].ID = 4;
                tileArray[i].isSolid = true;
            }
            //rightboundary
            for (int i = mapWidth - 1; i < tileArray.Length; i += mapWidth)
            {
                tileArray[i].ID = 4;
                tileArray[i].isSolid = true;
            }
            //topboundary
            for (int i = 0; i < mapWidth; i ++)
            {
                tileArray[i].ID = 4;
                tileArray[i].isSolid = true;
            }
            //bottomboundary
            for (int i = tileArray.Length - mapWidth; i < tileArray.Length ; i++)
            {
                tileArray[i].ID = 4;
                tileArray[i].isSolid = true;
            }
        }

        public void Update(Player player)
        {
            playerTilePos = (int)(player.topMidBound.Y / tileSize * mapWidth) + (int)(player.topMidBound.X / tileSize);

            //defines which tiles are in range
            int initial = playerTilePos - 17 * mapWidth - 25;
            int maxHoriz = 50;
            int maxVert = 30;
            int i = 0;

            for (int v = 0; v < maxVert; v++)
            {
                for (int h = 0; h < maxHoriz; h++)
                {
                    visibleTileArray[i] = initial + mapWidth * v + h;
                    i++;
                }
            }

            foreach (var vine in climbablesList)
            {
                if (vine.IsOnPlayer(player))
                {
                    player.isOnVines = true;
                    break;
                }
                else player.isOnVines = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < tileArray.Length)
                {
                    if (tileArray[tileNumber].texture != null)
                        tileArray[tileNumber].Draw(spriteBatch);
                }
            }
            foreach (Tile t in tileArray)
                t.Draw(spriteBatch);
        }

        public void DrawBehind(SpriteBatch spriteBatch)
        {
            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < wallArray.Length)
                {
                    if (wallArray[tileNumber].texture != null)
                        wallArray[tileNumber].Draw(spriteBatch);
                }
            }

        }

        public void PlayerCollision(Player player)
        {
            playerTilePos = (int)(player.topMidBound.Y / tileSize * mapWidth) + (int)(player.topMidBound.X / tileSize);
            if (player.isGhost)
                return;

            int[] q = new int[12];
            q[0] = playerTilePos - mapWidth - 1;
            q[1] = playerTilePos - mapWidth;
            q[2] = playerTilePos - mapWidth + 1;
            q[3] = playerTilePos - 1;
            q[4] = playerTilePos;
            q[5] = playerTilePos + 1;
            q[6] = playerTilePos + mapWidth - 1;
            q[7] = playerTilePos + mapWidth;
            q[8] = playerTilePos + mapWidth + 1;
            q[9] = playerTilePos + mapWidth + mapWidth - 1;
            q[10] = playerTilePos + mapWidth + mapWidth;
            q[11] = playerTilePos + mapWidth + mapWidth + 1;

            //test = q;

            //check the tiles around the player for collision
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= tileArray.Length - 1 && tileArray[quadrant].isSolid == true && player.isPlayerDead == false)
                {
                    if (player.yRect.Intersects(tileArray[quadrant].rectangle))
                    {
                        if (player.collRectangle.Y < tileArray[quadrant].rectangle.Y) //hits bot
                        {
                            player.velocity.Y = 0f;
                            player.collRectangle.Y = tileArray[quadrant].rectangle.Y - player.collRectangle.Height;
                            player.isJumping = false;
                        }
                        if (player.collRectangle.Y > tileArray[quadrant].rectangle.Y) //hits top
                        {
                            player.velocity.Y = 0f;
                            player.collRectangle.Y = tileArray[quadrant].rectangle.Y + tileArray[quadrant].rectangle.Height + 1;
                        }
                    }
                    else if (player.xRect.Intersects(tileArray[quadrant].rectangle))
                    {
                        if (player.collRectangle.X < tileArray[quadrant].rectangle.X) //hits right
                        {
                            player.velocity.X = 0f;
                            player.collRectangle.X = tileArray[quadrant].rectangle.X - player.collRectangle.Width - 1;
                        }
                        if (player.collRectangle.X > tileArray[quadrant].rectangle.X) //hits left
                        {
                            player.velocity.X = 0f;
                            player.collRectangle.X = tileArray[quadrant].rectangle.X + tileArray[quadrant].rectangle.Width + 1;
                        }
                    }
                }
            }
        }

        void AverageRandomDensity(int numberOfPasses)
        {

        }

    }
}
