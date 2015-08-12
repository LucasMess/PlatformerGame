using Adam;
using Adam.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Network
{
    [Serializable]
    class MapDataPacket
    {
        public Player player;
        public Apple apple;
        public bool isPaused;
        public bool levelComplete;

        public List<Cloud> cloudList = new List<Cloud>();
        public List<Gem> gemList = new List<Gem>();
        public List<Chest> chestList = new List<Chest>();
        public List<Enemy> enemyList = new List<Enemy>();
        public List<Particle> effectList = new List<Particle>();
        public List<PlayerWeaponProjectile> projectileList = new List<PlayerWeaponProjectile>();
        public List<Climbables> climbablesList = new List<Climbables>();
        public List<Tech> techList = new List<Tech>();
        public List<Door> doorList = new List<Door>();
        public List<Key> keyList = new List<Key>();
        public List<Entity> entities = new List<Entity>();

        public GameTime gameTime;

        public MapDataPacket(GameWorld map)
        {

        }
    }

    [Serializable]
    class WorldConfigFile
    {
        //public Apple apple;
        //public Tile[] tiles, walls;
        //public List<Chest> chestList = new List<Chest>();
        //public List<Climbables> climbablesList = new List<Climbables>();
        //public List<Key> keyList = new List<Key>();
        //public List<Entity> entities = new List<Entity>();
        public int[] tileIDs;
        public int[] wallIDs;
        //public string levelName;

        public WorldConfigFile(GameWorld gw)
        {
            int size = gw.tileArray.Length;
            tileIDs = new int[size];
            wallIDs = new int[size];

            for (int i = 0; i < size; i++)
            {
                tileIDs[i] = gw.tileArray[i].ID;
                wallIDs[i] = gw.wallArray[i].ID;
            }

            //apple = gw.apple;
            //tiles = gw.tileArray;
            //walls = gw.wallArray;
            //chestList = gw.chestList;
            //climbablesList = gw.climbablesList;
            //keyList = gw.keyList;
            //entities = gw.entities;
        }

        public void LoadIntoEditor()
        {
            Load();
            GameWorld.Instance.game1.LoadWorldFromFile(GameMode.Editor);
        }

        public void LoadIntoPlay()
        {
            Load();   
            GameWorld.Instance.game1.LoadWorldFromFile(GameMode.Play);
        }

        private void Load()
        {
            GameWorld gw = GameWorld.Instance;
            gw.worldData.tileIDs = tileIDs;
            gw.worldData.wallIDs = wallIDs;
        }
    }
}
