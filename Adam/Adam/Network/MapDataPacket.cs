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
    public class MapDataPacket
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
        public List<Tech> techList = new List<Tech>();
        public List<Door> doorList = new List<Door>();
        public List<Key> keyList = new List<Key>();
        public List<Entity> entities = new List<Entity>();

        public GameTime gameTime;

        public MapDataPacket(GameWorld map)
        {

        }
    }       
    
}
