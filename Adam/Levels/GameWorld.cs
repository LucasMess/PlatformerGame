using Adam.Characters.Enemies;
using Adam.Interactables;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Sound;
using Adam.Network;
using Adam.Particles;
using Adam.PlayerCharacter;
using Adam.Projectiles;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Adam.AdamGame;

namespace Adam.Levels
{
    [Serializable]
    public static class GameWorld
    {
        public static readonly ParticleSystem ParticleSystem = new ParticleSystem();
        public static readonly Texture2D SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_26");
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
        public static bool IsOnDebug;
        public static PlayerCharacter.Player Player = new PlayerCharacter.Player();
        //Basic tile grid and the visible tile grid
        public static Tile[] TileArray;
        public static int TimesUpdated;
        public static Tile[] WallArray;
        public static WorldData WorldData = new WorldData();
        public static PlayerTrail PlayerTrail = new PlayerTrail();

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
            TileArray = new Tile[0];
            WallArray = new Tile[0];
        }

        public static bool TryLoadFromFile(GameMode currentGameMode)
        {
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
                _clouds.Add(new Cloud(new Vector2(AdamGame.DefaultResWidth, AdamGame.DefaultResHeight), maxClouds, i));
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
                AdamGame.MessageBox.Show(e.Message);
                return false;
            }

            LightingEngine.GenerateLights();

            Session.WaitForPlayers();

            return true;
        }

        public static void PrepareLevelForTesting()
        {
            Entities = new List<Entity>();

            ConvertToTiles(TileArray, WorldData.TileIDs);
            ConvertToTiles(WallArray, WorldData.WallIDs, true);
        }

        private static void ConvertToTiles(Tile[] array, TileType[] ids, bool isWall = false)
        {
            var width = WorldData.LevelWidth;
            var height = WorldData.LevelHeight;

            for (var i = 0; i < ids.Length; i++)
            {
                var xcoor = (i % width) * AdamGame.Tilesize;
                var ycoor = ((i - (i % width)) / width) * AdamGame.Tilesize;


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
            AdamGame.Camera.UpdateSmoothly(cameraRect, WorldData.LevelWidth, WorldData.LevelHeight, !Player.IsPlayingDeathAnimation);

            if (AdamGame.TimeFreeze.IsTimeFrozen())
                ParticleSystem.UpdateTimeConstant();


            Background.Update();
            WorldData.Update();
        }

        public static void UpdateWorld()
        {
            ParticleSystem.Update();
            Weather.Update();

            if (Session.IsActive)
            {
                //if (Session.IsHost)
                //{
                //}
                //else
                //{
                //    Session.EntityPacket?.ExtractTo();
                //}
            }

            if (AdamGame.CurrentGameMode == GameMode.Edit)
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

            if (AdamGame.CurrentGameMode == GameMode.Play)
                SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);

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
            if (AdamGame.CurrentGameMode == GameMode.Play)
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


            foreach (var tileNumber in ChunkManager.GetVisibleIndexes())
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    TileArray[tileNumber]?.Update();
                }
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

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (AdamGame.CurrentGameMode == GameMode.Edit)
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


            ParticleSystem.DrawNormalParticles(spriteBatch);

            if (AdamGame.CurrentGameMode == GameMode.Edit)
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
            LightingEngine.DrawLights(spriteBatch);

            //if (LightingEngine.LASTMODIFIEDINDICES != null)
            //    foreach (var ind in LightingEngine.LASTMODIFIEDINDICES)
            //    {
            //        TileArray[ind].DebugDraw(spriteBatch);
            //    }
        }

        public static void DrawBackground(SpriteBatch spriteBatch)
        {
            Background.Draw(spriteBatch);

            if (_clouds != null)
                foreach (var c in _clouds)
                {
                    //if (WorldData.HasClouds)
                    c.Draw(spriteBatch);
                }
        }

        public static void DrawUi(SpriteBatch spriteBatch)
        {
            if (AdamGame.CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawUi(spriteBatch);

        }

        public static void ResetWorld()
        {
            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity is Enemy)
                {
                    var enemy = (Enemy)entity;
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
        public static Tile GetTileBelow(int index)
        {
            index += WorldData.LevelWidth;
            return GetTile(index);
        }

        /// <summary>
        ///     Returns the tile above the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Tile GetTileAbove(int index)
        {
            index -= WorldData.LevelWidth;
            return GetTile(index);
        }

        public static Player GetPlayer()
        {
            return Player;
        }

        public static void AddEntityAt(int tileIndex, Entity entity)
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
            Entities.Add(entity);
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
    }
}