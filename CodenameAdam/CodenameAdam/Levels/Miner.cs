using Adam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class Miner
    {
        Room room;
        public int tilePos;
        int mapWidth, mapHeight;
        int turns;
        public bool toDelete;
        bool isInfinite;
        int maxTurns = 5;

        public enum Direction { upright, upleft, downright, downleft };
        Direction chosenDirection;

        public Miner(int mapWidth, int mapHeight, int tilePos)
        {
            this.mapHeight = mapHeight;
            this.mapWidth = mapWidth;
            this.tilePos = tilePos;
            room = new Room(mapWidth, mapHeight);
            DefineDirection();
        }
        public Miner(int mapWidth, int mapHeight, int exitOrEntrancePos, Direction direction)
        {
            this.mapHeight = mapHeight;
            this.mapWidth = mapWidth;
            this.tilePos = exitOrEntrancePos;
            room = new Room(mapWidth, mapHeight);
            this.chosenDirection = direction;
            isInfinite = true;
        }

        void DefineDirection()
        {
            int rand = Map.randGen.Next(0, 3);
            switch (rand)
            {
                case 1:
                    chosenDirection = Direction.downleft;
                    break;
                case 2:
                    chosenDirection = Direction.downright;
                    break;
                case 3:
                    chosenDirection = Direction.upleft;
                    break;
                case 4:
                    chosenDirection = Direction.upright;
                    break;
            }
        }

        public void Update(ref Tile[] tileArray)
        {
            Mine(ref tileArray);
            GetNextMovement();
            turns++;
        }

        public void Mine(ref Tile[] tileArray)
        {
            room.Add(tilePos, 4, ref tileArray);
        }

        public void GetNextMovement()
        {
            int rand = Map.randGen.Next(0, 2);
            switch (chosenDirection)
            {
                case Direction.upright:
                    if (rand == 0) Move("up"); else Move("right");
                    break;
                case Direction.upleft:
                    if (rand == 0) Move("up"); else Move("left");
                    break;
                case Direction.downright:
                    if (rand == 0) Move("down"); else Move("right");
                    break;
                case Direction.downleft:
                    if (rand == 0) Move("down"); else Move("left");
                    break;
            }
        }

        public bool RequiresAssistance()
        {
            int rand = Map.randGen.Next(0, 100);
            if (rand == 1)
            {
                return true;
            }
            else return false;
        }

        void CheckIfDead(ref Tile[] tileArray)
        {
            if (tileArray[tilePos - mapWidth - 1].isSolid == false &&
                tileArray[tilePos - mapWidth + 1].isSolid == false &&
                tileArray[tilePos - mapWidth].isSolid == false &&
                tileArray[tilePos - 1].isSolid == false &&
                tileArray[tilePos + 1].isSolid == false &&
                tileArray[tilePos + mapWidth - 1].isSolid == false &&
                tileArray[tilePos + mapWidth].isSolid == false &&
                tileArray[tilePos + mapWidth + 1].isSolid == false)
            {
                toDelete = true;
            }

            if (turns > maxTurns && !isInfinite)
                toDelete = true;
        }

        void Move(string direction)
        {
            switch (direction)
            {
                case "up":
                    tilePos -= mapWidth;
                    break;
                case "down":
                    tilePos += mapWidth;
                    break;
                case "left":
                    tilePos -= 1;
                    break;
                case "right":
                    tilePos += 1;
                    break;
            }
        }
    }

    class MinerManager
    {
        int mapWidth, mapHeight;
        List<Miner> minerList = new List<Miner>();
        int maxMiners = 50;

        public MinerManager(int mapWidth, int mapHeight)
        {
            this.mapHeight = mapHeight;
            this.mapWidth = mapWidth;
        }

        public void Initialize(int entrancePos, int exitPos)
        {
            //minerList.Add(new Miner(mapWidth, mapHeight, (int)(((mapWidth * mapHeight) / 2) + mapWidth / 2), Miner.Direction.downright));
            //minerList.Add(new Miner(mapWidth, mapHeight, (int)(((mapWidth * mapHeight) / 2) + mapWidth / 2), Miner.Direction.upright));

            if (exitPos % mapWidth > mapWidth/2)
            minerList.Add(new Miner(mapWidth, mapHeight, exitPos, Miner.Direction.upleft));
            else minerList.Add(new Miner(mapWidth, mapHeight, exitPos, Miner.Direction.upright));
            minerList.Add(new Miner(mapWidth, mapHeight, entrancePos, Miner.Direction.downright));
            //minerList.Add(new Miner(mapWidth, mapHeight, (int)(((mapWidth * mapHeight) / 2) + mapWidth / 2),Miner.Direction.downleft));
        }

        public void BeginExcavation(ref Tile[] tileArray)
        {
            bool minersDead = false;
            int totalMiners = 1;

            while (!minersDead)
            {
                foreach (Miner mi in minerList)
                {
                    mi.Update(ref tileArray);
                    if (mi.RequiresAssistance())
                    {
                        minerList.Add(new Miner(mapWidth, mapHeight, mi.tilePos));
                        totalMiners++;
                        break;
                    }
                    if (mi.toDelete)
                    {
                        minerList.Remove(mi);
                        break;
                    }
                }
                if (totalMiners > maxMiners)
                {
                    minersDead = true;
                }
            }
        }

    }
}
