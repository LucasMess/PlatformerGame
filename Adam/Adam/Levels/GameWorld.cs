using System;
using System.Collections.Generic;
using System.Linq;
using Adam.Characters.Enemies;
using Adam.Characters.Non_Playable;
using Adam.Interactables;
using Adam.Lights;
using Adam.Misc;
using Adam.Misc.Sound;
using Adam.Network;
using Adam.Obstacles;
using Adam.Particles;
using Adam.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Levels
{
    [Serializable]
    sealed public class GameWorld
    {
        private static GameWorld _instance;

        public static GameWorld Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("The instance of Gameworld has not yet been created.");
                else return _instance;
            }
        }

        //Basic tile grid and the visible tile grid
        public Tile[] TileArray;
        public Tile[] WallArray;
        public int[] VisibleTileArray = new int[0];
        public int[] VisibleLightArray = new int[0];
        Light[] _lightArray;
        Light _playerLight;

        public static ParticleSystem ParticleSystem = new ParticleSystem();
        public GameMode CurrentGameMode;
        public Player.Player Player;
        public bool DebuggingMode;
        public Background Background = new Background();

        Adam.Misc.Timer _stopMovingTimer = new Misc.Timer();
        bool _playerMovingRight;

        public GameTime GetGameTime()
        {
            return GameTime;
        }

        PlaceNotification _placeNotification;

        public int TimesUpdated;

        public bool SimulationPaused;
        public bool IsOnDebug;
        public bool LevelComplete;
        public static Random RandGen;
        public static Texture2D SpriteSheet;
        public static Texture2D UiSpriteSheet;
        public static Texture2D ParticleSpriteSheet;
        public LightEngine LightEngine;
        public Main Game1;
        public Camera Camera;
        public LevelEditor LevelEditor = new LevelEditor();
        public ChunkManager ChunkManager = new ChunkManager();

        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        public List<Cloud> CloudList;
        public List<Key> KeyList; //This one is tricky... it could be moved to the WorldData.
        public List<Entity> Entities;
        public List<Particle> Particles;
        public GameTime GameTime;
        public WorldData WorldData;
        public GameWorld() { }

        public GameWorld(Main game1)
        {
            _instance = this;

            this.Game1 = game1;

            _placeNotification = new PlaceNotification();
            RandGen = new Random();
            SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_18");
            UiSpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap");
            ParticleSpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");

            LightEngine = new LightEngine();
            WorldData = new WorldData();

            //tilesThread = new Thread(new ThreadStart(UpdateVisibleIndexes));
            //tilesThread.IsBackground = true;
            //tilesThread.Start();
        }

        public bool TryLoadFromFile(GameMode currentGameMode)
        {
            LoadingScreen.LoadingText = "Where did I put that file?";
            byte[] tileIDs = WorldData.TileIDs;
            byte[] wallIDs = WorldData.WallIDs;

            LoadingScreen.LoadingText = "Starting up world...";
            this.CurrentGameMode = currentGameMode;
            Main.ObjectiveTracker.Clear();
            CloudList = new List<Cloud>();
            KeyList = new List<Key>();
            Entities = new List<Entity>();
            Particles = new List<Particle>();
            ChunkManager = new ChunkManager();

            Player = Game1.Player;

            int width = WorldData.LevelWidth;
            int height = WorldData.LevelHeight;

            if (WorldData.MetaData == null)
                WorldData.MetaData = new string[width * height];

            int maxClouds = width / 100;
            for (int i = 0; i < maxClouds; i++)
            {
                CloudList.Add(new Cloud(new Vector2(Main.UserResWidth, Main.UserResHeight), maxClouds, i));
            }

            TileArray = new Tile[tileIDs.Length];
            WallArray = new Tile[tileIDs.Length];

            LoadingScreen.LoadingText = "Getting tiles from junkyard...";
            ConvertToTiles(TileArray, tileIDs);
            ConvertToTiles(WallArray, wallIDs);

            LoadingScreen.LoadingText = "Lighting up the world...";
            LightEngine.Load();

            _playerLight = new Light();
            _playerLight.Load();

            LoadingScreen.LoadingText = "Finding cardboard backgrounds...";
            Background.Load();

            LoadingScreen.LoadingText = "Wait, you are editing it???";
            if (currentGameMode == GameMode.Edit)
                LevelEditor.Load();

            _placeNotification.Show(WorldData.LevelName);

            try
            {
                ChunkManager.ConvertToChunks(WorldData.LevelWidth, WorldData.LevelHeight);
            }
            catch (ArgumentException e)
            {
                Main.MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }

        private void ConvertToTiles(Tile[] array, byte[] ds)
        {
            int width = WorldData.LevelWidth;
            int height = WorldData.LevelHeight;

            for (int i = 0; i < ds.Length; i++)
            {
                int xcoor = (i % width) * Main.Tilesize;
                int ycoor = ((i - (i % width)) / width) * Main.Tilesize;


                array[i] = new Tile(xcoor, ycoor);
                Tile t = array[i];
                t.Id = (byte)ds[i];
                t.TileIndex = i;
            }

            foreach (Tile t in array)
            {
                t.DefineTexture();
                t.FindConnectedTextures(array, width);
                t.DefineTexture();
                if (CurrentGameMode == GameMode.Play)
                {
                    t.AddRandomlyGeneratedDecoration(array, WorldData.LevelWidth);
                    t.DefineTexture();
                }

            }

        }

        public void Update(GameTime gameTime, GameMode currentLevel, Camera camera)
        {
            ParticleSystem.Update();

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

            this.GameTime = gameTime;
            this.Camera = camera;

            if (currentLevel == GameMode.Edit)
            {
                LevelEditor.Update(gameTime, currentLevel);
            }
            else
            {
                SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);

                Rectangle cameraRect = Player.GetCollRectangle();

                camera.UpdateSmoothly(cameraRect, WorldData.LevelWidth, WorldData.LevelHeight, Player.IsDead);
            }

            TimesUpdated++;

            Background.Update(camera);
            _placeNotification.Update(gameTime);
            WorldData.Update(gameTime);
            LightEngine.Update();
            UpdateInBackground();




            foreach (Cloud c in CloudList)
            {
                c.CheckOutOfRange();
                c.Update(gameTime);
            }

            if (CurrentGameMode == GameMode.Play)
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    Entity entity = Entities[i];
                    if (entity.IsDead)
                        continue;

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
                        proj.Update(Player, gameTime);
                    }
                    if (entity is NonPlayableCharacter)
                    {
                        NonPlayableCharacter npc = (NonPlayableCharacter)entity;
                        npc.Update(gameTime, Player);
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

            _playerLight.Update(Player);

            foreach (Key key in KeyList)
            {
                key.Update(Player);
                if (key.ToDelete)
                {
                    KeyList.Remove(key);
                    break;
                }
            }

            foreach (int tileNumber in VisibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    TileArray[tileNumber]?.Update(gameTime);
                }
            }
        }

        public void UpdateInBackground()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update(GameTime);
            }

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entity entity = Entities[i];
                if (entity.ToDelete)
                {
                    if (entity.Light != null)
                    {
                        LightEngine.RemoveDynamicLight(entity.Light);
                    }
                    Entities.Remove(entity);
                }

            }

            for (int i = Particles.Count - 1; i >= 0; i--)
            {
                Particle p = Particles[i];
                if (p.ToDelete)
                {
                    LightEngine.RemoveDynamicLight(p.light);
                    Particles.Remove(p);
                }
            }

            while (Particles.Count > 10000000)
            {
                Particles.Remove(Particles.ElementAt<Particle>(0));
            }

            if (Camera != null)
                UpdateVisibleIndexes();
        }

        private void UpdateVisibleIndexes()
        {
            VisibleTileArray = ChunkManager.GetVisibleIndexes();
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            LightEngine.DrawLights(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawBehindTiles(spriteBatch);

            foreach (int tileNumber in VisibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    if (TileArray[tileNumber].Texture != null)
                        TileArray[tileNumber].Draw(spriteBatch);
                }
            }

            foreach (Key key in KeyList)
            {
                key.Draw(spriteBatch);
            }
            for (int i = 0; i < Entities.Count; i++)
            {
                if (!Entities[i].IsDead)
                    Entities[i].Draw(spriteBatch);
            }

            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.Draw(spriteBatch);



        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            ParticleSystem.Draw(spriteBatch);

            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Draw(spriteBatch);
            }
        }

        public void DrawClouds(SpriteBatch spriteBatch)
        {
            foreach (Cloud c in CloudList)
            {
                if (WorldData.HasClouds == true)
                    c.Draw(spriteBatch);
            }
        }

        public void DrawInBack(SpriteBatch spriteBatch)
        {

            foreach (int tileNumber in VisibleTileArray)
            {
                if (tileNumber > 0 && tileNumber < TileArray.Length)
                {
                    if (WallArray[tileNumber].Texture != null)
                        WallArray[tileNumber].Draw(spriteBatch);
                }
            }
        }

        public void DrawAfterLights(SpriteBatch spriteBatch)
        {
        }

        public void DrawGlows(SpriteBatch spriteBatch)
        {
            LightEngine.DrawGlows(spriteBatch);
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            Background.Draw(spriteBatch);
        }

        public void DrawUi(SpriteBatch spriteBatch)
        {
            _placeNotification.Draw(spriteBatch);

            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawUi(spriteBatch);
        }

        public void ResetWorld()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entity entity = Entities[i];
                if (entity is Enemy)
                {
                    Enemy enemy = (Enemy)entity;
                    enemy.Revive();
                }
                if (entity is Food)
                {
                    Entities.Remove(entity);
                }
            }
        }

        public Player.Player GetPlayer()
        {
            return Player;
        }
    }
}
