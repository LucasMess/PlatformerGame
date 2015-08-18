using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Adam;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using Adam.Enemies;
using System.Threading;
using Adam.Interactables;
using Adam.Obstacles;
using Adam.Network;
using Adam.Characters.Enemies;
using Adam.UI;
using Adam.UI.Information;
using Adam.Levels;
using Adam.Characters.Non_Playable;
using Adam.Noobs;
using System.ComponentModel;
using Adam.Lights;

namespace Adam
{
    [Serializable]
    sealed public class GameWorld
    {
        private static GameWorld instance;

        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                    throw new Exception("The instance of Gameworld has not yet been created.");
                else return instance;
            }
        }

        //Basic tile grid and the visible tile grid
        public Tile[] tileArray;
        public Tile[] wallArray;
        public int[] visibleTileArray = new int[30 * 50];
        int[] visibleLightArray = new int[60 * 100];
        Light[] lightArray;
        Light playerLight;

        public GameMode CurrentGameMode;
        public Player player;
        public Apple apple;
        public bool debuggingMode;
        public Background background = new Background();
        PopUp popUp = new PopUp();
        PlaceNotification placeNotification;
        int enemyTilePos;
        int gemTilePos;

        public int TimesUpdated;

        public bool SimulationPaused;
        public bool isOnDebug;
        public bool levelComplete;
        public static Random RandGen;
        public static Texture2D SpriteSheet;
        public static Texture2D UI_SpriteSheet;
        public LightEngine lightEngine;
        public Main game1;
        public Camera camera;
        public LevelEditor levelEditor = new LevelEditor();

        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        public List<Cloud> cloudList;
        public List<Gem> gemList; //can be moved to entities
        public List<Chest> chestList; //can be moved to entities
        public List<Key> keyList; //This one is tricky... it could be moved to the WorldData.
        public List<Entity> entities;
        public List<Particle> particles;
        ContentManager Content;
        public GameTime gameTime;
        public WorldData worldData;


        public GameWorld() { }

        public GameWorld(Main game1)
        {
            instance = this;

            this.game1 = game1;

            placeNotification = new PlaceNotification();
            RandGen = new Random();
            SpriteSheet = ContentHelper.LoadTexture("Tiles/Spritemaps/spritemap_13");
            UI_SpriteSheet = ContentHelper.LoadTexture("Level Editor/ui_spritemap");
            lightEngine = new LightEngine();
            worldData = new WorldData();
        }

        public void LoadFromFile(GameMode CurrentGameMode)
        {
            byte[] tileIDs = worldData.TileIDs;
            byte[] wallIDs = worldData.WallIDs;
            this.Content = Main.Content;

            this.CurrentGameMode = CurrentGameMode;
            cloudList = new List<Cloud>();
            gemList = new List<Gem>();
            chestList = new List<Chest>();
            keyList = new List<Key>();
            entities = new List<Entity>();
            particles = new List<Particle>();

            player = game1.player;
            popUp.Load(Content);

            int width = worldData.LevelWidth;
            int height = worldData.LevelHeight;

            int maxClouds = width / 100;
            for (int i = 0; i < maxClouds; i++)
            {
                cloudList.Add(new Cloud(Content, new Vector2(Main.UserResWidth, Main.UserResHeight), maxClouds, i));
            }

            tileArray = new Tile[tileIDs.Length];
            wallArray = new Tile[tileIDs.Length];

            ConvertToTiles(tileArray, tileIDs);
            ConvertToTiles(wallArray, wallIDs);

            lightEngine.Load();

            playerLight = new Light();
            playerLight.Load(Content);

            background.Load();

            if (CurrentGameMode == GameMode.Edit)
                levelEditor.Load();

            if (worldData.song != null)
                MediaPlayer.Play(worldData.song);

            placeNotification.Show(worldData.LevelName);
        }

        private void ConvertToTiles(Tile[] array, byte[] IDs)
        {
            int width = worldData.LevelWidth;
            int height = worldData.LevelHeight;

            for (int i = 0; i < IDs.Length; i++)
            {
                int Xcoor = (i % width) * Main.Tilesize;
                int Ycoor = ((i - (i % width)) / width) * Main.Tilesize;


                array[i] = new Tile();
                Tile t = array[i];
                t.ID = (byte)IDs[i];
                t.TileIndex = i;
                t.drawRectangle = new Rectangle(Xcoor, Ycoor, Main.Tilesize, Main.Tilesize);
            }

            foreach (Tile t in array)
            {
                t.DefineTexture();
                t.FindConnectedTextures(array, width);
                if (CurrentGameMode == GameMode.Play)
                {
                    t.AddRandomlyGeneratedDecoration(array, worldData.LevelWidth);
                    t.DefineTexture();
                }

            }

        }

        public void Update(GameTime gameTime, GameMode CurrentLevel, Camera camera)
        {
            this.Content = Main.Content;
            this.gameTime = gameTime;
            this.camera = camera;

            if (CurrentLevel == GameMode.Edit)
            {
                levelEditor.Update(gameTime, CurrentLevel);
            }
            else
            {
                camera.UpdateSmoothly(player.collRectangle, worldData.LevelWidth, worldData.LevelHeight, !player.isDead);

                if (player.isDead)
                {
                    camera.ZoomIn();
                }
                else
                {
                    camera.ZoomOut();
                }
            }

            TimesUpdated++;
            if (player.hasChronoshifted)
                return;

            popUp.Update(gameTime, player);
            background.Update(camera);
            placeNotification.Update(gameTime);
            worldData.Update(gameTime);
            lightEngine.Update();
            UpdateInBackground();

            if (apple != null)
                apple.Update(player, gameTime, this, game1);



            foreach (Cloud c in cloudList)
            {
                c.CheckOutOfRange();
                c.Update(gameTime);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity is Enemy)
                {
                    Enemy enemy = (Enemy)entity;
                    enemy.Update(player, gameTime);
                }
                if (entity is Obstacle)
                {
                    Obstacle obstacle = (Obstacle)entity;
                    obstacle.Update(gameTime, player, this);
                }
                if (entity is Item)
                {
                    Item power = (Item)entity;
                    power.Update();
                }
                if (entity is Projectile)
                {
                    Projectile proj = (Projectile)entity;
                    proj.Update(player, gameTime);
                }
                if (entity is NonPlayableCharacter)
                {
                    NonPlayableCharacter npc = (NonPlayableCharacter)entity;
                    npc.Update(gameTime, player);
                }
                if (entity is Door)
                {
                    Door door = (Door)entity;
                    door.Update(gameTime, player, tileArray);
                }
                if (entity is Sign)
                {
                    Sign sign = (Sign)entity;
                    sign.Update();
                }
                if (entity is CheckPoint)
                {
                    CheckPoint ch = (CheckPoint)entity;
                    ch.Update();
                }
            }

            playerLight.Update(player);

            foreach (Key key in keyList)
            {
                key.Update(player);
                if (key.toDelete)
                {
                    keyList.Remove(key);
                    break;
                }
            }

            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < tileArray.Length)
                {
                    tileArray[tileNumber]?.Update(gameTime);
                }
            }
        }

        public void UpdateInBackground()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
            }


            for (int i = entities.Count - 1; i >= 0; i--)
            {
                Entity entity = entities[i];
                if (entity.toDelete)
                    entities.Remove(entity);
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle p = particles[i];
                if (p.ToDelete())
                    particles.Remove(p);
            }

            if (camera != null)
                UpdateVisibleIndexes();
        }

        private void UpdateVisibleIndexes()
        {
            if (player.isDead == false)
            {
                //defines which tiles are in range
                int initial = camera.tileIndex - 17 * worldData.LevelWidth - 25;
                int maxHoriz = 50;
                int maxVert = 30;
                int i = 0;

                for (int v = 0; v < maxVert; v++)
                {
                    for (int h = 0; h < maxHoriz; h++)
                    {
                        visibleTileArray[i] = initial + worldData.LevelWidth * v + h;
                        i++;
                    }
                }
                initial = camera.tileIndex - 17 * 2 * worldData.LevelWidth - 25 * 2;
                maxHoriz = 100;
                maxVert = 60;
                i = 0;
                for (int v = 0; v < maxVert; v++)
                {
                    for (int h = 0; h < maxHoriz; h++)
                    {
                        visibleLightArray[i] = initial + worldData.LevelWidth * v + h;
                        i++;
                    }
                }
            }
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            lightEngine.DrawLights(spriteBatch);

            if (player.weapon != null)
                player.weapon.DrawLights(spriteBatch);
            foreach (Particle ef in particles)
                ef.DrawLights(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentGameMode == GameMode.Edit)
                levelEditor.DrawBehindTiles(spriteBatch);

            if (apple != null)
                apple.Draw(spriteBatch);
            foreach (Gem gem in gemList)
            {
                gem.Draw(spriteBatch);
            }
            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < tileArray.Length)
                {
                    if (tileArray[tileNumber].texture != null)
                        tileArray[tileNumber].Draw(spriteBatch);
                }
            }
            foreach (Key key in keyList)
            {
                key.Draw(spriteBatch);
            }
            for (int i = 0; i < entities.Count; i++)
                entities[i].Draw(spriteBatch);
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }

            if (CurrentGameMode == GameMode.Edit)
                levelEditor.Draw(spriteBatch);

            lightEngine.DrawGlows(spriteBatch);

        }

        public void DrawClouds(SpriteBatch spriteBatch)
        {
            foreach (Cloud c in cloudList)
            {
                if (worldData.HasClouds == true)
                    c.Draw(spriteBatch);
            }
        }

        public void DrawInBack(SpriteBatch spriteBatch)
        {

            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber > 0 && tileNumber < tileArray.Length)
                {
                    if (wallArray[tileNumber].texture != null)
                        wallArray[tileNumber].Draw(spriteBatch);
                }
            }
        }

        public void DrawInFront(SpriteBatch spriteBatch)
        {
           
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            placeNotification.Draw(spriteBatch);

            if (CurrentGameMode == GameMode.Edit)
                levelEditor.DrawUI(spriteBatch);
        }

        public void ResetWorld()
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                Entity entity = entities[i];
                if (entity is Enemy)
                {
                    Enemy enemy = (Enemy)entity;
                    enemy.health = enemy.maxHealth;
                    enemy.isDead = false;
                }
                if (entity is Food)
                {
                    entities.Remove(entity);
                }
            }
        }

        public MapDataPacket GetMapDataPacket()
        {
            MapDataPacket m = new MapDataPacket(this);
            return m;
        }

        public void UpdateFromDataPacket(MapDataPacket m)
        {
            apple = m.apple;
            isOnDebug = m.isPaused;
            levelComplete = m.levelComplete;

            gameTime = m.gameTime;
        }
    }
}
