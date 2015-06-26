using Adam;
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
        public GameTimer timer;
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
        public List<NonPlayableCharacter> noobList = new List<NonPlayableCharacter>();
        public List<Entity> entities = new List<Entity>();

        public GameTime gameTime;

        public MapDataPacket(GameWorld map)
        {
            apple = map.apple;
            timer = map.timer;
            isPaused = map.isPaused;
            levelComplete = map.levelComplete;


            cloudList = map.cloudList;
            gemList = map.gemList;
            chestList = map.chestList;
            effectList = map.effectList;
            projectileList = map.projectileList;
            climbablesList = map.climbablesList;
            techList = map.techList;
            doorList = map.doorList;
            keyList = map.keyList;
            noobList = map.noobList;
            entities = map.entities;

            gameTime = map.gameTime;
        }
    }
}
