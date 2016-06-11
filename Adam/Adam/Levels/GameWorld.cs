using System;
using System.Collections.Generic;
using System.Linq;
using Adam.Characters;
using Adam.Characters.Enemies;
using Adam.Interactables;
using Adam.Lights;
using Adam.Misc;
using Adam.Misc.Sound;
using Adam.Network;
using Adam.Obstacles;
using Adam.Particles;
using Adam.PlayerCharacter;
using Adam.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Levels
{
    [Serializable]
    public static class GameWorld
    {
        public static ParticleSystem ParticleSystem = new ParticleSystem();
        public static Random RandGen;
        public static Texture2D SpriteSheet;
        public static Texture2D UiSpriteSheet;
        public static Texture2D ParticleSpriteSheet;
        private static PlaceNotification _placeNotification;
        private static Light[] _lightArray;
        private static Light _playerLight;
        private static bool _playerMovingRight;
        private static Timer _stopMovingTimer = new Timer();
        public static Background Background = new Background();
        public static Camera Camera;
        public static ChunkManager ChunkManager = new ChunkManager();
        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        public static List<Cloud> CloudList;
        public static GameMode CurrentGameMode;
        public static bool DebuggingMode;
        public static List<Projectile> EnemyProjectiles;
        public static List<Entity> Entities;
        public static Main Game1;
        public static GameTime GameTime;
        public static bool IsOnDebug;
        public static List<Key> KeyList; //This one is tricky... it could be moved to the WorldData.
        public static bool LevelComplete;
        public static LightEngine LightEngine;
        public static List<Particle> Particles;
        public static Player Player;
        public static List<Projectile> PlayerProjectiles;
        public static bool SimulationPaused;
        //Basic tile grid and the visible tile grid
        public static Tile[] TileArray;
        public static int TimesUpdated;
        public static int[] VisibleLightArray = new int[0];
        public static int[] VisibleTileArray = new int[0];
        public static Tile[] WallArray;
        public static WorldData WorldData;

        public static void Initialize()
        {
            _placeNotification = new PlaceNotification();
            RandGen = new Random();
            SpriteSheet = ContentHelper.LoadTexture("Tiles/new spritemap");
            UiSpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap_4");
            ParticleSpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");
            Player = new Player();
            LightEngine = new LightEngine();
            WorldData = new WorldData();
        }

        public static GameTime GetGameTime()
        {
            return GameTime;
        }

        public static bool TryLoadFromFile(GameMode currentGameMode)
        {
            LoadingScreen.LoadingText = "Where did I put that file?";
            var tileIDs = WorldData.TileIDs;
            var wallIDs = WorldData.WallIDs;

            LoadingScreen.LoadingText = "Starting up world...";
            CurrentGameMode = currentGameMode;
            Main.ObjectiveTracker.Clear();
            CloudList = new List<Cloud>();
            KeyList = new List<Key>();
            Entities = new List<Entity>();
            Particles = new List<Particle>();
            PlayerProjectiles = new List<Projectile>();
            EnemyProjectiles = new List<Projectile>();

            var width = WorldData.LevelWidth;
            var height = WorldData.LevelHeight;

            if (WorldData.MetaData == null)
                WorldData.MetaData = new string[width*height];

            var maxClouds = width/100;
            for (var i = 0; i < maxClouds; i++)
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

        private static void ConvertToTiles(Tile[] array, byte[] ds)
        {
            var width = WorldData.LevelWidth;
            var height = WorldData.LevelHeight;

            for (var i = 0; i < ds.Length; i++)
            {
                var xcoor = (i%width)*Main.Tilesize;
                var ycoor = ((i - (i%width))/width)*Main.Tilesize;


                array[i] = new Tile(xcoor, ycoor);
                var t = array[i];
                t.Id = ds[i];
                t.TileIndex = i;
            }

            foreach (var t in array)
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

        public static void Update(GameTime gameTime, GameMode currentLevel, Camera camera)
        {
            ParticleSystem.Update();

            if (Session.IsActive)
            {
                if (Session.IsHost)
                {
                }
                else
                {
                    Session.EntityPacket?.ExtractTo();
                }
            }

            GameTime = gameTime;
            Camera = camera;

            if (currentLevel == GameMode.Edit)
            {
                LevelEditor.Update(gameTime, currentLevel);
            }
            else
            {
                SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);

                var cameraRect = Player.GetCollRectangle();

                camera.UpdateSmoothly(cameraRect, WorldData.LevelWidth, WorldData.LevelHeight, Player.IsDead);
            }

            TimesUpdated++;

            Background.Update(camera);
            _placeNotification.Update(gameTime);
            WorldData.Update(gameTime);
            LightEngine.Update();
            UpdateInBackground();

            Player.Update();

            foreach (var c in CloudList)
            {
                c.CheckOutOfRange();
                c.Update(gameTime);
            }

            if (CurrentGameMode == GameMode.Play)
            {
                for (var i = 0; i < Entities.Count; i++)
                {
                    var entity = Entities[i];
                    if (entity.IsDead)
                        continue;

                    if (entity is Enemy)
                    {
                        var enemy = (Enemy) entity;
                        enemy.Update();
                    }
                    if (entity is Obstacle)
                    {
                        var obstacle = (Obstacle) entity;
                        obstacle.Update();
                    }
                    if (entity is Item)
                    {
                        var power = (Item) entity;
                        power.Update();
                    }
                    if (entity is Projectile)
                    {
                        var proj = (Projectile) entity;
                        proj.Update(Player, gameTime);
                    }
                    if (entity is NonPlayableCharacter)
                    {
                        var npc = (NonPlayableCharacter) entity;
                        npc.Update();
                    }
                    if (entity is Sign)
                    {
                        var sign = (Sign) entity;
                        sign.Update();
                    }
                    if (entity is CheckPoint)
                    {
                        var ch = (CheckPoint) entity;
                        ch.Update();
                    }
                }
            }

            foreach (var key in KeyList)
            {
                key.Update(Player);
                if (key.ToDelete)
                {
                    KeyList.Remove(key);
                    break;
                }
            }

            foreach (var tileNumber in VisibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    TileArray[tileNumber]?.Update(gameTime);
                }
            }
        }

        public static void UpdateInBackground()
        {
            for (var i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update(GameTime);
            }

            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity.ToDelete)
                {
                    entity.Destroy();
                }
            }

            for (var i = Particles.Count - 1; i >= 0; i--)
            {
                var p = Particles[i];
                if (p.ToDelete)
                {
                    LightEngine.RemoveDynamicLight(p.light);
                    Particles.Remove(p);
                }
            }

            while (Particles.Count > 10000000)
            {
                Particles.Remove(Particles.ElementAt(0));
            }

            if (Camera != null)
                UpdateVisibleIndexes();
        }

        private static void UpdateVisibleIndexes()
        {
            VisibleTileArray = ChunkManager.GetVisibleIndexes();
        }

        public static void DrawLights(SpriteBatch spriteBatch)
        {
            LightEngine.DrawLights(spriteBatch);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawBehindTiles(spriteBatch);

            foreach (var tileNumber in VisibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    if (TileArray[tileNumber].Texture != null)
                        TileArray[tileNumber].Draw(spriteBatch);
                }
            }

            Player.Draw(spriteBatch);

            foreach (var key in KeyList)
            {
                key.Draw(spriteBatch);
            }
            for (var i = 0; i < Entities.Count; i++)
            {
                if (!Entities[i].IsDead)
                    Entities[i].Draw(spriteBatch);
            }
            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.Draw(spriteBatch);
        }

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            ParticleSystem.Draw(spriteBatch);

            for (var i = 0; i < Particles.Count; i++)
            {
                Particles[i].Draw(spriteBatch);
            }
        }

        public static void DrawClouds(SpriteBatch spriteBatch)
        {
            foreach (var c in CloudList)
            {
                if (WorldData.HasClouds)
                    c.Draw(spriteBatch);
            }
        }

        public static void DrawInBack(SpriteBatch spriteBatch)
        {
            foreach (var tileNumber in VisibleTileArray)
            {
                if (tileNumber > 0 && tileNumber < TileArray.Length)
                {
                    if (WallArray[tileNumber].Texture != null)
                        WallArray[tileNumber].Draw(spriteBatch);
                }
            }
        }

        public static void DrawAfterLights(SpriteBatch spriteBatch)
        {
        }

        public static void DrawGlows(SpriteBatch spriteBatch)
        {
            LightEngine.DrawGlows(spriteBatch);
        }

        public static void DrawBackground(SpriteBatch spriteBatch)
        {
            Background.Draw(spriteBatch);
        }

        public static void DrawUi(SpriteBatch spriteBatch)
        {
            _placeNotification.Draw(spriteBatch);

            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawUi(spriteBatch);
        }

        public static void ResetWorld()
        {
            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity is Enemy)
                {
                    var enemy = (Enemy) entity;
                    enemy.Revive();
                }
                if (entity is Food)
                {
                    Entities.Remove(entity);
                }
            }
        }

        /// <summary>
        ///     Returns the tile at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Tile GetTile(int index)
        {
            if (index >= 0 && index < TileArray.Length)
                return TileArray[index];
            return null;
        }

        /// <summary>
        ///     Returns the tile below the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Tile GetTileBelow(int index)
        {
            index += WorldData.LevelWidth;
            if (index >= 0 && index < TileArray.Length)
                return TileArray[index];
            return null;
        }

        /// <summary>
        ///     Returns the tile above the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Tile GetTileAbove(int index)
        {
            index += WorldData.LevelHeight;
            if (index >= 0 && index < TileArray.Length)
                return TileArray[index];
            return null;
        }

        public static Player GetPlayer()
        {
            return Player;
        }
    }
}