using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Interactables;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.Misc.Sound;
using ThereMustBeAnotherWay.Network;
using ThereMustBeAnotherWay.Particles;
using ThereMustBeAnotherWay.PlayerCharacter;
using ThereMustBeAnotherWay.Projectiles;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static ThereMustBeAnotherWay.TMBAW_Game;
using System.Diagnostics;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay.Levels
{
    [Serializable]
    public static class GameWorld
    {
        public static readonly ParticleSystem ParticleSystem = new ParticleSystem();
        private static Texture2D defaultSpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_29");
        public static Texture2D SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_29");
        public static readonly Texture2D UiSpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap_4");
        public static readonly Texture2D ParticleSpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");
        private static GameTimer _stopMovingTimer = new GameTimer(true);
        private static readonly Background Background = new Background();
        public static readonly ChunkManager ChunkManager = new ChunkManager();
        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        private static List<Cloud> _clouds;
        public static bool IsTestingLevel;
        public static List<Entity> Entities;
        public static bool IsOnDebug;
        private static List<Player> _players = new List<Player>(4);
        //Basic tile grid and the visible tile grid
        public static Tile[] TileArray;
        public static int TimesUpdated;
        public static Tile[] WallArray;
        public static WorldData WorldData = new WorldData();
        public static PlayerTrail PlayerTrail = new PlayerTrail();
        private static Dictionary<string, Entity> _entityDict = new Dictionary<string, Entity>();
        public static Stopwatch updateTimer = new Stopwatch();
        public static Stopwatch drawTimer = new Stopwatch();

        /// <summary>
        /// Returns the color data of the spritesheet used for most of the game's textures.
        /// </summary>
        public static Color[] SpriteSheetColorData = new Color[SpriteSheet.Width * SpriteSheet.Height];

        public static void Initialize()
        {
            SpriteSheet.GetData<Color>(SpriteSheetColorData);
            _clouds = new List<Cloud>();
            Entities = new List<Entity>();
            TileArray = new Tile[0];
            WallArray = new Tile[0];
            _players.Add(new Player(PlayerIndex.One));
            ProjectileSystem.Initialize();
            //_players.Add(new Player(PlayerIndex.Two));
        }

        public static bool TryLoadFromFile(GameMode currentGameMode)
        {
            Overlay.FadeToBlack();
            Cursor.Hide();

            if (WorldData.IsTopDown)
            {
                SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_level_select_1");
            }
            else
            {
                SpriteSheet = defaultSpriteSheet;
            }

            LoadingScreen.LoadingText = "Where did I put that file?";
            var tileIDs = WorldData.TileIDs;
            var wallIDs = WorldData.WallIDs;

            LoadingScreen.LoadingText = "Starting up world...";
            _clouds = new List<Cloud>();
            Entities = new List<Entity>();

            var width = WorldData.LevelWidth;
            var height = WorldData.LevelHeight;

            var maxClouds = width / 20;
            for (var i = 0; i < maxClouds; i++)
            {
                _clouds.Add(new Cloud(new Vector2(TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight), maxClouds, i));
            }

            LevelEditor.InteractableConnections.Clear();

            TileArray = new Tile[tileIDs.Length];
            WallArray = new Tile[tileIDs.Length];

            LoadingScreen.LoadingText = "Getting tiles from junkyard...";
            ConvertToTiles(TileArray, tileIDs);
            ConvertToTiles(WallArray, wallIDs, true);

            LoadingScreen.LoadingText = "Lighting up the world...";

            LoadingScreen.LoadingText = "Finding cardboard backgrounds...";
            Background.Load();

            LoadingScreen.LoadingText = "Wait, you are editing it???";
            if (currentGameMode == GameMode.Edit)
                LevelEditor.Load();
            try
            {
                ChunkManager.ConvertToChunks(WorldData.LevelWidth, WorldData.LevelHeight);
            }
            catch (ArgumentException e)
            {
                TMBAW_Game.MessageBox.Show(e.Message);
                return false;
            }

            if (Session.IsHost)
            {
                Session.SendEntityUpdates();
            }

            Session.WaitForPlayers();

            SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);

            StoryTracker.OnLevelLoad();

            TMBAW_Game.Camera.ResetZoom();
            Overlay.FadeIn();

            return true;
        }

        internal static void GetIndicesOfTilesWithId(TileType tileType, object type)
        {
            throw new NotImplementedException();
        }

        public static void PrepareLevelForTesting()
        {
            Entities = new List<Entity>();

            //LightingEngine.RemoveAllLights();

            foreach (Tile tile in TileArray)
            {
                tile.DefineTexture();
            }

            foreach (Tile tile in WallArray)
            {
                tile.DefineTexture();
            }

            SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);
        }

        private static void ConvertToTiles(Tile[] array, TileType[] ids, bool isWall = false)
        {
            var width = WorldData.LevelWidth;
            var height = WorldData.LevelHeight;

            for (var i = 0; i < ids.Length; i++)
            {
                var xcoor = (i % width) * TMBAW_Game.Tilesize;
                var ycoor = ((i - (i % width)) / width) * TMBAW_Game.Tilesize;


                array[i] = new Tile(xcoor, ycoor);
                var t = array[i];
                t.Id = (TileType)ids[i];
                t.TileIndex = i;
                if (isWall)
                    t.IsWall = true;
            }

            foreach (var t in array)
            {
                t.DefineTexture();
            }

            foreach (var t in array)
            {
                t.FindConnectedTextures(ids, width);
                t.AddRandomlyGeneratedDecoration(array, WorldData.LevelWidth);
                t.DefineTexture();
            }

            foreach (var t in array)
            {
                t.ReadMetaData();
            }
        }

        public static void UpdateVisual()
        {
            var cameraRect = GetPlayers()[0].GetCollRectangle();
            TMBAW_Game.Camera.UpdateSmoothly(cameraRect, WorldData.LevelWidth, WorldData.LevelHeight, !GetPlayers()[0].IsPlayingDeathAnimation);

            if (TMBAW_Game.TimeFreeze.IsTimeFrozen())
                ParticleSystem.UpdateStartEvent_TimeConstant.Set();
            //ParticleSystem.UpdateTimeConstant();


            Background.Update();
            WorldData.Update();
        }

        public static void UpdateWorld()
        {
            ParticleSystem.UpdateStartEvent.Set();
            //ParticleSystem.Update();
            Weather.Update();

            if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
            {
                foreach (Player player in GetPlayers())
                {
                    player.ComplexAnimation.RemoveAllFromQueue();
                    player.AddAnimationToQueue("editMode");
                }
                LevelEditor.Update();
            }
            else
            {
                if (IsTestingLevel)
                {
                    if (GetPlayers()[0].IsTestLevelPressed())
                        LevelEditor.GoBackToEditing();
                }
                UpdateVisual();
            }

            TimesUpdated++;

            ProjectileSystem.Update();

            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity.ToDelete)
                {
                    entity.Destroy();
                }
            }
            foreach (Player player in GetPlayers())
                player.Update();

            if (TMBAW_Game.CurrentGameMode == GameMode.Play)
            {
                foreach (Player player in GameWorld.GetPlayers())
                    PlayerTrail.Add(player);
            }

            foreach (var c in _clouds)
            {
                c.CheckOutOfRange();
                c.Update();
            }
            for (var i = 0; i < Entities.Count; i++)
            {
                var entity = Entities[i];
                if (entity.IsDead)
                    continue;
                entity.Update();

                // Check enemy collision with other enemies.
                for (int j = i + 1; j < Entities.Count; j++)
                {
                    if (Entities[i].IsTouchingEntity(Entities[j]))
                    {
                        if (Entities[i].Position.X > Entities[j].Position.X)
                        {
                            Entities[i].SetX(Entities[j].Position.X + Entities[j].CollRectangle.Width / 2);
                        }
                        else
                        {
                            Entities[i].SetX(Entities[j].Position.X - Entities[i].CollRectangle.Width / 2);
                        }
                        Entities[i].ForceUpdateCollisionRectangle();
                    }
                }
            }


            foreach (var tileNumber in ChunkManager.GetVisibleIndexes())
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    TileArray[tileNumber]?.Update();
                }
            }

            if (Session.IsHost)
            {
                Session.SendEntityUpdates();
            }

        }

        public static void DrawWalls(SpriteBatch spriteBatch)
        {
            int[] indexes = ChunkManager.GetVisibleIndexes();
            if (indexes == null)
                return;
            foreach (var tileNumber in indexes)
            {
                if (tileNumber > 0 && tileNumber < TileArray.Length)
                {
                    WallArray[tileNumber].Draw(spriteBatch);
                }
            }
        }

        public static void DrawWallShadows(SpriteBatch spriteBatch)
        {
            int[] indexes = ChunkManager.GetVisibleIndexes();
            if (indexes == null)
                return;
            foreach (var tileNumber in indexes)
            {
                if (tileNumber > 0 && tileNumber < TileArray.Length)
                {
                    WallArray[tileNumber].DrawWallShadow(spriteBatch);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawBehindTiles(spriteBatch);

            int[] indexes = ChunkManager.GetVisibleIndexes();
            if (indexes != null)
            {
                foreach (var tileNumber in indexes)
                {
                    if (tileNumber >= 0 && tileNumber < TileArray.Length)
                    {
                        TileArray[tileNumber].Draw(spriteBatch);
                    }
                }
            }

            foreach (Player player in GameWorld.GetPlayers())
                player.Draw(spriteBatch);

            for (var i = 0; i < Entities.Count; i++)
            {
                if (!Entities[i].IsDead)
                    Entities[i].Draw(spriteBatch);
            }

            ProjectileSystem.Draw(spriteBatch);

            //ParticleSystem.DrawNormalParticles(spriteBatch);

            if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
                LevelEditor.Draw(spriteBatch);


        }

        public static void DrawRipples(SpriteBatch spriteBatch)
        {
            ParticleSystem.DrawEffectParticles(spriteBatch);
            foreach (var tile in TileArray)
            {
                if (tile != null)
                    tile.DrawRipples(spriteBatch);
            }
        }

        public static void DrawLights(SpriteBatch spriteBatch)
        {
            int[] indices = ChunkManager.GetVisibleIndexes();
            if (indices != null)
            {
                foreach (var tileNumber in indices)
                {
                    if (tileNumber >= 0 && tileNumber < TileArray.Length)
                    {
                        TileArray[tileNumber]?.DrawLight(spriteBatch);
                    }
                }
            }

            foreach (Entity en in Entities)
            {
                en.DrawLight(spriteBatch);
            }

            ProjectileSystem.DrawLights(spriteBatch);
        }

        public static void DrawGlows(SpriteBatch spriteBatch)
        {
            foreach (var tileNumber in ChunkManager.GetVisibleIndexes())
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    TileArray[tileNumber]?.DrawGlow(spriteBatch);
                }
            }

            foreach (Entity en in Entities)
            {
                en.DrawGlow(spriteBatch);
            }

            ProjectileSystem.DrawGlow(spriteBatch);
        }

        public static void DrawBackground(SpriteBatch spriteBatch)
        {
            Background.Draw(spriteBatch);

            if (_clouds != null)
                foreach (var c in _clouds)
                {
                    if (WorldData.HasClouds)
                        c.Draw(spriteBatch);
                }
        }

        public static void DrawUi(SpriteBatch spriteBatch)
        {
            if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawUi(spriteBatch);

        }

        public static void ResetWorld()
        {
            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity is Enemy enemy)
                {
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
        public static Tile GetTile(int index, bool isWall = false)
        {
            if (isWall)
            {
                if (index >= 0 && index < WallArray.Length)
                    return WallArray[index];
            }
            else
            {
                if (index >= 0 && index < TileArray.Length)
                    return TileArray[index];
            }
            return Tile.Default;
        }

        /// <summary>
        ///     Returns the tile below the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Tile GetTileBelow(int index, bool isWall = false)
        {
            index += WorldData.LevelWidth;
            return GetTile(index, isWall);
        }

        /// <summary>
        ///     Returns the tile above the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Tile GetTileAbove(int index, bool isWall = false)
        {
            index -= WorldData.LevelWidth;
            return GetTile(index, isWall);
        }

        /// <summary>
        /// Returns a list with all the players.
        /// </summary>
        /// <returns></returns>
        public static List<Player> GetPlayers()
        {
            return _players;
        }

        public static void AddEntityAt(int tileIndex, Entity entity)
        {
            // Only add entities during loading if it is host.
            if (Session.IsHost)
            {
                foreach (var e in Entities)
                {
                    // An entity from that tile already exists.
                    if (e.TileIndexSpawn == tileIndex)
                    {
                        return;
                    }
                }

                entity.TileIndexSpawn = tileIndex;
                Guid id = Guid.NewGuid();
                AddEntity(id.ToString(), entity);
            }
        }

        public static void RemoveEntityAt(int tileIndex)
        {
            foreach (var e in Entities)
            {
                // An entity from that tile already exists.
                if (e.TileIndexSpawn == tileIndex)
                {
                    Entities.Remove(e);
                    break;
                }
            }
        }

        public static Entity[] GetAllEntities()
        {
            return Entities.ToArray();
        }

        public static void SetEntityDictionary(Dictionary<string, Entity> entities)
        {
            _entityDict.Clear();
            _entityDict = entities;
        }

        public static Entity GetEntityById(string id)
        {
            if (_entityDict.ContainsKey(id))
            {
                return _entityDict[id];
            }
            else return null;
        }

        public static void AddEntity(string id, Entity entity)
        {
            Entities.Add(entity);
            _entityDict.Add(id, entity);
        }

        public static List<Tile> GetTilesWithId(TileType type)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (var tile in TileArray)
            {
                if (tile.Id == type)
                {
                    tiles.Add(tile);
                }
            }

            return tiles;
        }
    }
}