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
using Adam.Misc;
using Adam.Particles;

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

        public static ParticleSystem ParticleSystem = new ParticleSystem();
        public GameMode CurrentGameMode;
        public Player player;
        public bool debuggingMode;
        public Background background = new Background();

        Adam.Misc.Timer stopMovingTimer = new Misc.Timer();
        bool playerMovingRight;

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
        public List<Key> keyList; //This one is tricky... it could be moved to the WorldData.
        public List<Entity> entities;
        public List<Particle> particles;
        ContentManager Content;
        public GameTime gameTime;
        public WorldData worldData;
        public GameWorld() { }

        int distance;

        public GameWorld(Main game1)
        {
            instance = this;

            this.game1 = game1;

            placeNotification = new PlaceNotification();
            RandGen = new Random();
            SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_16");
            UI_SpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap");
            Particle_SpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");

            lightEngine = new LightEngine();
            worldData = new WorldData();

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
            keyList = new List<Key>();
            entities = new List<Entity>();
            particles = new List<Particle>();

            player = game1.player;

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
            ParticleSystem.Update();
            //for (int i = 0; i < 1; i++)
            //{
            //    SmokeParticle par = new SmokeParticle(player.GetCollRectangle().Center.X - 4, player.GetCollRectangle().Bottom);
            //    ParticleSystem.Add(par);
            //}

            if (Session.IsActive)
            {
                if (Session.IsHost)
                {
                    
                }
                else
                {
                    Session.EntityPacket?.ExtractTo(this);
                }
            }

            this.Content = Main.Content;
            this.gameTime = gameTime;
            this.camera = camera;

            if (CurrentLevel == GameMode.Edit)
            {
                levelEditor.Update(gameTime, CurrentLevel);
            }
            else
            {
                SoundtrackManager.PlayTrack(worldData.SoundtrackID, true);

                Rectangle cameraRect = player.GetCollRectangle();
                //cameraRect.X += (int)(player.GetVelocity().X * 50);

                //stopMovingTimer.Increment();
                //if (InputHelper.IsKeyDown(Keys.A))
                //{
                //    stopMovingTimer.Reset();
                //    distance+=2;
                //}
                //if (InputHelper.IsKeyDown(Keys.D))
                //{
                //    stopMovingTimer.Reset();
                //    distance-=2;
                //}

                //if (stopMovingTimer.TimeElapsedInMilliSeconds > 3000)
                //{
                //    distance = 0;
                //}

                //int max = 200;
                //if (distance > max)
                //    distance = max;
                //if (distance < -max)
                //    distance = -max;

                //cameraRect.X -= distance;


                camera.UpdateSmoothly(cameraRect, worldData.LevelWidth, worldData.LevelHeight, !player.IsDead());

                //if (player.IsDead())
                //{
                //    camera.ZoomIn();
                //}
                //else
                //{
                //    camera.ResetZoom();
                //}
            }

            TimesUpdated++;

            background.Update(camera);
            placeNotification.Update(gameTime);
            worldData.Update(gameTime);
            lightEngine.Update();
            UpdateInBackground();




            foreach (Cloud c in cloudList)
            {
                c.CheckOutOfRange();
                c.Update(gameTime);
            }

            if (CurrentGameMode == GameMode.Play)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    Entity entity = entities[i];
                    if (entity is Enemy)
                    {
                        Enemy enemy = (Enemy)entity;
                        if (!enemy.IsDead())
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

            while (particles.Count > 10000000)
            {
                particles.Remove(particles.ElementAt<Particle>(0));
            }

            if (camera != null)
                UpdateVisibleIndexes();
        }

        private void UpdateVisibleIndexes()
        {
            visibleTileArray = chunkManager.GetVisibleIndexes();
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            lightEngine.DrawLights(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentGameMode == GameMode.Edit)
                levelEditor.DrawBehindTiles(spriteBatch);

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
            {
                if (!entities[i].IsDead())
                    entities[i].Draw(spriteBatch);
            }

            if (CurrentGameMode == GameMode.Edit)
                levelEditor.Draw(spriteBatch);



        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            ParticleSystem.Draw(spriteBatch);

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }
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

        public void DrawAfterLights(SpriteBatch spriteBatch)
        {
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

        public Player GetPlayer()
        {
            return player;
        }
    }
}
