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
        public int[] visibleTileArray = new int[0];
        public int[] visibleLightArray = new int[0];
        Light[] lightArray;
        Light playerLight;

        public GameMode CurrentGameMode;
        public Player player;
        public Apple apple;
        public bool debuggingMode;
        public Background background = new Background();
        PopUp popUp = new PopUp();

        public GameTime GetGameTime()
        {
            return gameTime;
        }

        PlaceNotification placeNotification;

        public int TimesUpdated;

        public bool SimulationPaused;
        public bool isOnDebug;
        public bool levelComplete;
        public static Random RandGen;
        public static Texture2D SpriteSheet;
        public static Texture2D UI_SpriteSheet;
        public static Texture2D Particle_SpriteSheet;
        public LightEngine lightEngine;
        public Main game1;
        public Camera camera;
        public LevelEditor levelEditor = new LevelEditor();
        public ChunkManager chunkManager = new ChunkManager();

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
        Textbox textBox;
        Thread tilesThread;

        string temp = "hi";

        public GameWorld() { }

        public GameWorld(Main game1)
        {
            instance = this;

            this.game1 = game1;

            placeNotification = new PlaceNotification();
            RandGen = new Random();
            SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_14");
            UI_SpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap");
            Particle_SpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");

            lightEngine = new LightEngine();
            worldData = new WorldData();
            textBox = new Textbox(300, 300, 100);

            //tilesThread = new Thread(new ThreadStart(UpdateVisibleIndexes));
            //tilesThread.IsBackground = true;
            //tilesThread.Start();
        }

        public bool TryLoadFromFile(GameMode CurrentGameMode)
        {
            byte[] tileIDs = worldData.TileIDs;
            byte[] wallIDs = worldData.WallIDs;
            this.Content = Main.Content;

            this.CurrentGameMode = CurrentGameMode;
            Main.ObjectiveTracker.Clear();
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

            try
            {
                chunkManager.ConvertToChunks(worldData.LevelWidth, worldData.LevelHeight);
            }
            catch (ArgumentException e)
            {
                Main.MessageBox.Show(e.Message);
                return false;
            }

            return true;
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
                t.DefineTexture();
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

            textBox.Update();

            if (InputHelper.IsKeyDown(Keys.T))
            {
                Main.TextInputBox.Show("Enter a creative name:",this);
            }

            if (CurrentLevel == GameMode.Edit)
            {
                levelEditor.Update(gameTime, CurrentLevel);
            }
            else
            {
                camera.UpdateSmoothly(player.GetCollRectangle(), worldData.LevelWidth, worldData.LevelHeight, !player.IsDead());

                if (player.IsDead())
                {
                    camera.ZoomIn();
                }
                else
                {
                    camera.ResetZoom();
                }
            }

            TimesUpdated++;

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
                    enemy.Update();
                }
                if (entity is Obstacle)
                {
                    Obstacle obstacle = (Obstacle)entity;
                    obstacle.Update();
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
                if (entity.ToDelete)
                {
                    if (entity.Light != null)
                    {
                        lightEngine.RemoveDynamicLight(entity.Light);
                    }
                    entities.Remove(entity);
                }

            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle p = particles[i];
                if (p.ToDelete)
                {
                    lightEngine.RemoveDynamicLight(p.light);
                    particles.Remove(p);
                }
            }

            while (particles.Count > 1000)
            {
                particles.Remove(particles.ElementAt<Particle>(0));
            }

            if (camera != null)
                UpdateVisibleIndexes();
        }

        float lastCameraZoom;
        private void UpdateVisibleIndexes()
        {
            visibleTileArray = chunkManager.GetVisibleIndexes();
            //visibleLightArray = chunkManager.GetVisibleIndexes();

            //if (player != null && camera != null)
            //{
            //    if (player.IsDead() == false)
            //    {
            //        float currentZoom = camera.GetZoom();
            //        if (lastCameraZoom != currentZoom)
            //        {
            //            visibleTileArray = new int[(((int)(30 / currentZoom) * (int)(50 / currentZoom)))];
            //            visibleLightArray = new int[(((int)(60 / currentZoom) * (int)(100 / currentZoom)))];
            //            lastCameraZoom = camera.GetZoom();
            //        }

                    ////defines which tiles are in range
                    //int initial = camera.tileIndex - 17 * worldData.LevelWidth - 25;
                    //int maxHoriz = (int)(50 / currentZoom);
                    //int maxVert = (int)(30 / currentZoom);
                    //int i = 0;

                    //for (int v = 0; v < maxVert; v++)
                    //{
                    //    for (int h = 0; h < maxHoriz; h++)
                    //    {
                    //        visibleTileArray[i] = initial + worldData.LevelWidth * v + h;
                    //        i++;
                    //    }
                    //}
                    //initial = camera.tileIndex - 17 * 2 * worldData.LevelWidth - 25 * 2;
                    //maxHoriz = (int)(100 / currentZoom);
                    //maxVert = (int)(60 / currentZoom);
                    //i = 0;
                    //for (int v = 0; v < maxVert; v++)
                    //{
                    //    for (int h = 0; h < maxHoriz; h++)
                    //    {
                    //        visibleLightArray[i] = initial + worldData.LevelWidth * v + h;
                    //        i++;
                    //    }
                    //}
            //    }
            //}

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

        public void DrawGlows(SpriteBatch spriteBatch)
        {
            lightEngine.DrawGlows(spriteBatch);
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            placeNotification.Draw(spriteBatch);
            textBox.Draw(spriteBatch);

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
                    enemy.Revive();
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

        public Player GetPlayer()
        {
            return player;
        }
    }
}
