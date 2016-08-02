using System;
using System.Collections.Generic;
using Adam.Characters;
using Adam.Characters.Enemies;
using Adam.Interactables;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Sound;
using Adam.Network;
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
        public static readonly ParticleSystem ParticleSystem = new ParticleSystem();
        public static readonly Texture2D SpriteSheet = ContentHelper.LoadTexture("Tiles/spritemap_21");
        public static readonly Texture2D UiSpriteSheet = ContentHelper.LoadTexture("Tiles/ui_spritemap_4");
        public static readonly Texture2D ParticleSpriteSheet = ContentHelper.LoadTexture("Tiles/particles_spritemap");
        private static Timer _stopMovingTimer = new Timer();
        private static readonly Background Background = new Background();
        public static readonly ChunkManager ChunkManager = new ChunkManager();
        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        private static List<Cloud> _clouds;
        public static GameMode CurrentGameMode;
        public static bool TestingFromLevelEditor;
        public static List<Entity> Entities;
        public static bool IsOnDebug;
        public static readonly Player Player = new Player();
        public static List<Projectile> PlayerProjectiles;
        //Basic tile grid and the visible tile grid
        public static Tile[] TileArray;
        public static int TimesUpdated;
        public static Tile[] WallArray;
        public static WorldData WorldData = new WorldData();

        public static bool TryLoadFromFile(GameMode currentGameMode)
        {
            LoadingScreen.LoadingText = "Where did I put that file?";
            var tileIDs = WorldData.TileIDs;
            var wallIDs = WorldData.WallIDs;

            LoadingScreen.LoadingText = "Starting up world...";
            CurrentGameMode = currentGameMode;
            _clouds = new List<Cloud>();
            Entities = new List<Entity>();
            PlayerProjectiles = new List<Projectile>();

            var width = WorldData.LevelWidth;
            var height = WorldData.LevelHeight;

            if (WorldData.MetaData == null)
                WorldData.MetaData = new string[width * height];

            var maxClouds = width / 100;
            for (var i = 0; i < maxClouds; i++)
            {
                _clouds.Add(new Cloud(new Vector2(Main.UserResWidth, Main.UserResHeight), maxClouds, i));
            }

            TileArray = new Tile[tileIDs.Length];
            WallArray = new Tile[tileIDs.Length];

            LoadingScreen.LoadingText = "Getting tiles from junkyard...";
            ConvertToTiles(TileArray, tileIDs);
            ConvertToTiles(WallArray, wallIDs);

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
                var xcoor = (i % width) * Main.Tilesize;
                var ycoor = ((i - (i % width)) / width) * Main.Tilesize;


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

        public static void Update()
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

            if (Main.CurrentGameMode == GameMode.Edit)
            {
                LevelEditor.Update();
            }
            else
            {
                SoundtrackManager.PlayTrack(WorldData.SoundtrackId, true);

                var cameraRect = Player.GetCollRectangle();

                Main.Camera.UpdateSmoothly(cameraRect, WorldData.LevelWidth, WorldData.LevelHeight, Player.IsDead);
            }

            TimesUpdated++;

            Background.Update();
            WorldData.Update();

            for (var i = Entities.Count - 1; i >= 0; i--)
            {
                var entity = Entities[i];
                if (entity.ToDelete)
                {
                    entity.Destroy();
                }
            }

            Player.Update();

            foreach (var c in _clouds)
            {
                c.CheckOutOfRange();
                c.Update();
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
                        var enemy = (Enemy)entity;
                        enemy.Update();
                    }
                    if (entity is Item)
                    {
                        var power = (Item)entity;
                        power.Update();
                    }
                    if (entity is Projectile)
                    {
                        var proj = (Projectile)entity;
                        proj.Update();
                    }
                    if (entity is NonPlayableCharacter)
                    {
                        var npc = (NonPlayableCharacter)entity;
                        npc.Update();
                    }
                    if (entity is Sign)
                    {
                        var sign = (Sign)entity;
                        sign.Update();
                    }
                    if (entity is CheckPoint)
                    {
                        var ch = (CheckPoint)entity;
                        ch.Update();
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
        }

        public static void DrawWalls(SpriteBatch spriteBatch)
        {
             foreach (var tileNumber in ChunkManager.GetVisibleIndexes())
            {
                if (tileNumber > 0 && tileNumber < TileArray.Length)
                {
                    WallArray[tileNumber].Draw(spriteBatch);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.DrawBehindTiles(spriteBatch);

            foreach (var c in _clouds)
            {
                if (WorldData.HasClouds)
                    c.Draw(spriteBatch);
            }

           

            foreach (var tileNumber in ChunkManager.GetVisibleIndexes())
            {
                if (tileNumber >= 0 && tileNumber < TileArray.Length)
                {
                    TileArray[tileNumber].Draw(spriteBatch);
                }
            }

            Player.Draw(spriteBatch);

            for (var i = 0; i < Entities.Count; i++)
            {
                if (!Entities[i].IsDead)
                    Entities[i].Draw(spriteBatch);
            }

            ParticleSystem.Draw(spriteBatch);

            if (CurrentGameMode == GameMode.Edit)
                LevelEditor.Draw(spriteBatch);
        }

        public static void DrawBackground(SpriteBatch spriteBatch)
        {
            Background.Draw(spriteBatch);
        }

        public static void DrawUi(SpriteBatch spriteBatch)
        {
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