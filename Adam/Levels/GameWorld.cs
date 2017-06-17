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

namespace ThereMustBeAnotherWay.Levels
{
    [Serializable]
    public static class GameWorld
    {
        public static readonly ParticleSystem ParticleSystem = new ParticleSystem();
        public static readonly Texture2D SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_28");
        public static readonly Texture2D UiSpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap_4");
        public static readonly Texture2D ParticleSpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");
        private static Timer _stopMovingTimer = new Timer(true);
        private static readonly Background Background = new Background();
        public static readonly ChunkManager ChunkManager = new ChunkManager();
        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        private static List<Cloud> _clouds;
        public static bool IsTestingLevel;
        public static List<Entity> Entities;
        public static List<Projectile> PlayerProjectiles;
        public static List<Projectile> EnemyProjectiles;
        public static bool IsOnDebug;
        public static PlayerCharacter.Player Player = new PlayerCharacter.Player();
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
            PlayerProjectiles = new List<Projectile>();
            EnemyProjectiles = new List<Projectile>();
            TileArray = new Tile[0];
            WallArray = new Tile[0];
        }

        public static bool TryLoadFromFile(GameMode currentGameMode)
        {
            Cursor.Hide();

            LoadingScreen.LoadingText = "Where did I put that file?";
            var tileIDs = WorldData.TileIDs;
            var wallIDs = WorldData.WallIDs;

            LoadingScreen.LoadingText = "Starting up world...";
            _clouds = new List<Cloud>();
            Entities = new List<Entity>();
            PlayerProjectiles = new List<Projectile>();

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

            LightingEngine.GenerateLights();

            if (Session.IsHost)
            {
                Session.SendEntityUpdates();
            }

            Session.WaitForPlayers();

            SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);

            StoryTracker.OnLevelLoad();

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
            var cameraRect = Player.GetCollRectangle();
            TMBAW_Game.Camera.UpdateSmoothly(cameraRect, WorldData.LevelWidth, WorldData.LevelHeight, !Player.IsPlayingDeathAnimation);

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
                Player.ComplexAnimation.RemoveAllFromQueue();
                Player.AddAnimationToQueue("editMode");
                LevelEditor.Update();
            }
            else
            {
                if (IsTestingLevel)
                {
                    if (Player.IsTestLevelPressed())
                        LevelEditor.GoBackToEditing();
                }
                UpdateVisual();
            }

            TimesUpdated++;

            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity.ToDelete)
                {
                    entity.Destroy();
                }
            }

            Player.Update();
            if (TMBAW_Game.CurrentGameMode == GameMode.Play)
                PlayerTrail.Add(Player);

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
            }

            // Player projectile update and deletion.
            foreach (var proj in PlayerProjectiles)
            {
                proj.Update();

                foreach (var entity in Entities)
                {
                    if (entity.IsDead) continue;
                    if (proj.IsTouchingEntity(entity))
                    {
                        proj.OnCollisionWithEntity(entity);
                    }
                }
            }
            for (int i = PlayerProjectiles.Count - 1; i >= 0; i--)
            {
                Projectile proj = PlayerProjectiles[i];
                if (proj.ToDelete)
                {
                    PlayerProjectiles.Remove(proj);
                    continue;
                }
            }

            // Enemy projectile update and deletion.
            foreach (var proj in EnemyProjectiles)
            {
                proj.Update();
                if (proj.IsTouchingEntity(GetPlayer()))
                {
                    proj.OnCollisionWithEntity(GetPlayer());
                }

            }
            for (int i = EnemyProjectiles.Count - 1; i >= 0; i--)
            {
                Projectile proj = EnemyProjectiles[i];
                if (proj.ToDelete)
                {
                    EnemyProjectiles.Remove(proj);
                    continue;
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

            Player.Draw(spriteBatch);

            for (var i = 0; i < Entities.Count; i++)
            {
                if (!Entities[i].IsDead)
                    Entities[i].Draw(spriteBatch);
            }


            foreach (var proj in PlayerProjectiles)
            {
                proj.Draw(spriteBatch);
            }

            foreach (var proj in EnemyProjectiles)
            {
                proj.Draw(spriteBatch);
            }


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

        public static void DrawSunlight(SpriteBatch spriteBatch)
        {
            LightingEngine.DrawSunlight(spriteBatch);

            //if (LightingEngine.LASTMODIFIEDINDICES != null)
            //    foreach (var ind in LightingEngine.LASTMODIFIEDINDICES)
            //    {
            //        TileArray[ind].DebugDraw(spriteBatch);
            //    }
        }

        public static void DrawOtherLights(SpriteBatch spriteBatch)
        {
            LightingEngine.DrawOtherLights(spriteBatch);
        }

        public static void DrawGlows(SpriteBatch spriteBatch)
        {
            LightingEngine.DrawGlows(spriteBatch);
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

        public static Player GetPlayer()
        {
            return Player;
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