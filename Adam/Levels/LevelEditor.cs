using Adam.Misc;
using Adam.Misc.Sound;
using Adam.Particles;
using Adam.PlayerCharacter;
using Adam.UI;
using Adam.UI.Elements;
using Adam.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static Adam.AdamGame;

namespace Adam.Levels
{
    /// <summary>
    /// Resposible for user interface and input for the level editor.
    /// </summary>
    public static class LevelEditor
    {
        private static Timer _switchEditAndPlayTimer = new Timer(true);
        private static readonly SoundFx[] Construction = new SoundFx[3];
        private static readonly Timer IdleTimerForSave = new Timer(true);
        private static ButtonBar _buttonBar;
        private static SoundFx _close, _open, _select;
        private static SoundFx _destruction;
        private static SoundFx _testSound = new SoundFx("Sounds/Level Editor/test_level");
        private static bool _hasChangedSinceLastSave;
        private static Inventory _inventory;
        private static bool _inventoryKeyPressed;
        private static byte _lastUsedTile = 1;
        private static byte _lastUsedWall = 100;
        private static Minimap _miniMap;
        private static Rectangle _mouseRectInGameWorld;
        private static bool _recentlyChanged;
        private static SoundFx _wallMode;
        public static readonly Brush Brush = new Brush();
        public static Rectangle EditorRectangle;
        public static int IndexOfMouse;
        public static bool OnWallMode;
        public static TileType SelectedId = TileType.Grass;
        private static int lastAddedTile = -1;
        private static int lastRemovedTile = -1;

        private static TileType[] WorldDataIds => OnWallMode ? GameWorld.WorldData.WallIDs : GameWorld.WorldData.TileIDs;
        private static Tile[] CurrentArray => OnWallMode ? GameWorld.WallArray : GameWorld.TileArray;

        public static List<Line> InteractableConnections = new List<Line>();
        public static Tile ForceUpdateTile;

        public static void Load()
        {
            _inventory = new Inventory();
            _buttonBar = new ButtonBar();
            HotBar.Initialize();
            _miniMap = new Minimap();
            _miniMap.StartUpdating();

            for (var i = 1; i <= Construction.Length; i++)
            {
                Construction[i - 1] = new SoundFx("Sounds/Level Editor/construct" + i);
            }
            _destruction = new SoundFx("Sounds/Level Editor/destroy1");

            _wallMode = new SoundFx("Sounds/Level Editor/changeMode");
            _close = new SoundFx("Sounds/Level Editor/open");
            _open = new SoundFx("Sounds/Level Editor/close");
            _select = new SoundFx("Sounds/Level Editor/select");

            Player player = GameWorld.GetPlayer();
            //if (player.RespawnPos == Vector2.Zero)
            //{
            //    player.SetPosition(new Vector2(GameWorld.WorldData.LevelWidth * AdamGame.Tilesize / 2,
            //    GameWorld.WorldData.LevelHeight * AdamGame.Tilesize / 2));
            //}
            //else
            //{
                player.SetPosition(player.RespawnPos);
            //}


        }

        //private void EntityScroll_TileSelected(TileSelectedArgs e)
        //{
        //    if (e.Id != SelectedId)
        //    {
        //        _select.Reset();
        //        _select.PlayNewInstanceOnce();
        //        SelectedId = (byte) e.Id;
        //    }
        //}

        //private void TileScroll_TileSelected(TileSelectedArgs e)
        //{
        //    if (e.Id != SelectedId)
        //    {
        //        _select.Reset();
        //        _select.PlayNewInstanceOnce();
        //        SelectedId = (byte) e.Id;
        //    }
        //}

        /// <summary>
        /// Updates all UI components and checks for input.
        /// </summary>
        public static void Update()
        {
            if (GameDebug.IsTyping)
                return;

            GameWorld.Player.Health = GameWorld.Player.MaxHealth;

            SoundtrackManager.PlayLevelEditorTheme();

            _inventory.Update();
            _buttonBar.Update();
            HotBar.Update();
            Brush.Update();
            CheckIfOnInventory();
            CheckIfPositioningPlayer();
            CheckIfChangedToWallMode();
            CheckForCameraMovement();
            CheckForMouseInput();

            ForceUpdateTile?.Update();

            // Auto-save functionality.
            if (IdleTimerForSave.TimeElapsedInSeconds > 1 && _hasChangedSinceLastSave)
            {
                SaveLevel();
            }
        }

        public static void SaveLevel()
        {
            _hasChangedSinceLastSave = false;
            DataFolder.SaveLevel();
        }

        /// <summary>
        /// Tests level as the player.
        /// </summary>
        public static void TestLevel()
        {
            if (AdamGame.CurrentGameMode != GameMode.Play && _switchEditAndPlayTimer.TimeElapsedInMilliSeconds > 1000)
            {

                try
                {
                    SaveLevel();
                    GameWorld.IsTestingLevel = true;
                    GameWorld.PlayerTrail = new PlayerTrail();
                    AdamGame.CurrentGameMode = GameMode.Play;
                    GameWorld.PrepareLevelForTesting();
                    GameWorld.Player.ComplexAnimation.RemoveAllFromQueue();
                    GameWorld.Player.SetVelX(0);
                    GameWorld.Player.SetVelY(0);
                    Overlay.FlashWhite();
                    _switchEditAndPlayTimer.Reset();
                    _testSound.Play();
                    // DataFolder.PlayLevel(DataFolder.CurrentLevelFilePath);
                }
                catch (Exception e)
                {
                    AdamGame.MessageBox.Show(e.Message);
                }
            }
        }

        public static void GoBackToEditing()
        {
            if (AdamGame.CurrentGameMode != GameMode.Edit && _switchEditAndPlayTimer.TimeElapsedInMilliSeconds > 1000)
            {
                try
                {
                    GameWorld.IsTestingLevel = false;
                    AdamGame.CurrentGameMode = GameMode.Edit;
                    GameWorld.PrepareLevelForTesting();
                    Overlay.FlashWhite();
                    _switchEditAndPlayTimer.Reset();
                    _testSound.Play();
                    // DataFolder.PlayLevel(DataFolder.CurrentLevelFilePath);
                }
                catch (Exception e)
                {
                    AdamGame.MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// Changes the array that will be modified and changes the opacity of the foreground tiles.
        /// </summary>
        public static void ChangeToWallMode()
        {
            OnWallMode = !OnWallMode;
            _wallMode.PlayNewInstanceOnce();
            _wallMode.Reset();
        }

        /// <summary>
        /// Checks for input to change to wall mode.
        /// </summary>
        private static void CheckIfChangedToWallMode()
        {
            if (InputHelper.IsKeyDown(Keys.L) && !_recentlyChanged)
            {
                ChangeToWallMode();
                _recentlyChanged = true;
            }
            if (InputHelper.IsKeyUp(Keys.L))
            {
                _recentlyChanged = false;
            }



        }

        /// <summary>
        /// Checks for input to change to open or close the inventory.
        /// </summary>
        private static void CheckIfOnInventory()
        {
            if (InputHelper.IsKeyDown(Keys.E))
            {
                if (!_inventoryKeyPressed)
                {
                    Inventory.OpenOrClose();
                    if (Inventory.IsOpen)
                    {
                        _close.PlayNewInstanceOnce();
                        _close.Reset();
                    }
                    else
                    {
                        _open.PlayNewInstanceOnce();
                        _open.Reset();
                    }
                    _inventoryKeyPressed = true;
                }
            }
            if (InputHelper.IsKeyUp(Keys.E))
            {
                _inventoryKeyPressed = false;
            }
        }

        /// <summary>
        /// Checks for input to move the camera.
        /// </summary>
        private static void CheckForCameraMovement()
        {
            AdamGame.Camera.UpdateSmoothly(GameWorld.Player.GetCollRectangle(), GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight, true);
            float speed = 9f;

            Player player = GameWorld.GetPlayer();

            if (player.IsMoveLeftPressed())
            {
                IdleTimerForSave.Reset();
                GameWorld.Player.MoveBy(-speed, 0);
            }
            if (player.IsMoveRightPressed())
            {
                IdleTimerForSave.Reset();
                GameWorld.Player.MoveBy(speed, 0);
            }
            if (player.IsMoveUpPressed())
            {
                IdleTimerForSave.Reset();
                GameWorld.Player.MoveBy(0, -speed);
            }
            if (player.IsMoveDownPressed())
            {
                IdleTimerForSave.Reset();
                GameWorld.Player.MoveBy(0, speed);
            }
            if (player.IsTestLevelPressed())
            {
                TestLevel();
            }


            //Prevent camera box from moving out of screen
            if (EditorRectangle.X < 0)
            {
                EditorRectangle.X = 0;
            }
            if (EditorRectangle.X > (GameWorld.WorldData.LevelWidth * AdamGame.Tilesize) - EditorRectangle.Width)
            {
                EditorRectangle.X = (GameWorld.WorldData.LevelWidth * AdamGame.Tilesize) - EditorRectangle.Width;
            }
            if (EditorRectangle.Y < 0)
            {
                EditorRectangle.Y = 0;
            }
            if (EditorRectangle.Y > (GameWorld.WorldData.LevelHeight * AdamGame.Tilesize) - EditorRectangle.Height)
            {
                EditorRectangle.Y = (GameWorld.WorldData.LevelHeight * AdamGame.Tilesize) - EditorRectangle.Height;
            }
        }

        /// <summary>
        /// Checks for input to draw, erase or select tiles using the mouse.
        /// </summary>
        private static void CheckForMouseInput()
        {
            _mouseRectInGameWorld = InputHelper.GetMouseRectGameWorld();
            IndexOfMouse = CalcHelper.GetIndexInGameWorld(_mouseRectInGameWorld.X, _mouseRectInGameWorld.Y);

            if (!IsIntersectingUi())
            {
                if (InputHelper.IsLeftMousePressed() && InputHelper.IsRightMousePressed())
                    return;

                if (InputHelper.IsLeftMousePressed() && lastAddedTile != IndexOfMouse)
                {
                    lastAddedTile = IndexOfMouse;
                    lastRemovedTile = -1;
                    UpdateSelectedTiles(SelectedId);
                }
                else if (InputHelper.IsRightMousePressed() && lastRemovedTile != IndexOfMouse)
                {
                    lastRemovedTile = IndexOfMouse;
                    lastAddedTile = -1;
                    UpdateSelectedTiles(0);
                }
                else if (InputHelper.IsMiddleMousePressed())
                {
                    SelectedId = (TileType)WorldDataIds[IndexOfMouse];
                    HotBar.AddToHotBarFromWorld(SelectedId);
                }
            }
        }

        /// <summary>
        /// Checks to see if shortcut to put player spawn point is pressed.
        /// </summary>
        private static void CheckIfPositioningPlayer()
        {
            if (InputHelper.IsKeyDown(Keys.P))
            {
                Brush.Size = 1;
                UpdateSelectedTiles(TileType.Player);
                _hasChangedSinceLastSave = true;
            }
        }

        /// <summary>
        /// Updates all the tiles highlighted by the brush.
        /// </summary>
        /// <param name="desiredId"></param>
        private static void UpdateSelectedTiles(TileType desiredId)
        {
            bool hasChanged = false;
            foreach (var i in Brush.SelectedIndexes)
            {
                if (i < 0 || i > WorldDataIds.Length)
                    continue;
                TileType tileId = WorldDataIds[i];


                //Wants to destroy. Any block can be destroyed.
                if (desiredId == 0)
                {
                    //Check to see if block is already air.
                    if (tileId == 0)
                        continue;
                    CurrentArray[i].ResetToDefault();
                    WorldDataIds[i] = desiredId;

                    hasChanged = true;
                    Destroy(CurrentArray[i]);
                }

                //Wants to build, but only if there is air.
                else
                {
                    if (tileId == 0)
                    {
                        CurrentArray[i].ResetToDefault();
                        WorldDataIds[i] = desiredId;
                        if (OnWallMode)
                        {
                            CurrentArray[i].IsWall = true;
                        }
                        else
                            CurrentArray[i].IsWall = false;
                        Construct(CurrentArray[i]);
                        hasChanged = true;
                    }
                    else
                    {
                        // If there is no air, then the player can interact with the tile.
                        // Only allow interaction if the brush size is 1.
                        if (Brush.Size == 1)
                        {
                            CurrentArray[i].InteractInEditMode();
                        }
                    }
                }
            }
            if (hasChanged)
            {
                UpdateTilesAround(IndexOfMouse);
                UpdateLightingByCorners();
            }
        }

        /// <summary>
        /// Provides sound and effects for construction.
        /// </summary>
        /// <param name="t"></param>
        private static void Construct(Tile t)
        {
            IdleTimerForSave.Reset();
            _hasChangedSinceLastSave = true;
            Construction[AdamGame.Random.Next(0, 3)].PlayIfStopped();
            AdamGame.Camera.Shake();
            CreateConstructionParticles(t.DrawRectangle);
        }

        /// <summary>
        /// Provides sound and effects for destruction.
        /// </summary>
        /// <param name="t"></param>
        private static void Destroy(Tile t)
        {
            IdleTimerForSave.Reset();
            _hasChangedSinceLastSave = true;
            _destruction.PlayIfStopped();
            CreateDestructionParticles(t.GetDrawRectangle());
            AdamGame.Camera.Shake();
        }

        private static void UpdateLightingByCorners()
        {
            if (Brush.Size == 1)
            {
                LightingEngine.UpdateLightingAt(Brush.SelectedIndexes[0], true);
            }
            else
            {
                int leftTop = Brush.SelectedIndexes[0];
                int rightTop = Brush.SelectedIndexes[Brush.Size - 1];
                int leftBot = Brush.SelectedIndexes[(Brush.Size - 1) * (Brush.Size - 2) + 1];
                int rightBot = Brush.SelectedIndexes[Brush.SelectedIndexes.Length - 1];

                LightingEngine.UpdateLightingAt(leftTop, true);
                LightingEngine.UpdateLightingAt(rightTop, true);
                LightingEngine.UpdateLightingAt(leftBot, true);
                LightingEngine.UpdateLightingAt(rightBot, true);
            }
        }

        /// <summary>
        /// Updates all the tiles around the tiles that have just been updated by the brush tool.
        /// </summary>
        /// <param name="index"></param>
        private static void UpdateTilesAround(int index)
        {
            var indexes = new List<int>();
            var diameterOfSquare = 2 + Brush.Size;
            for (var h = 0; h < diameterOfSquare; h++)
            {
                for (var w = 0; w < diameterOfSquare; w++)
                {
                    var brushSize = Brush.Size;
                    var startingIndex = index - (int)(Math.Truncate((double)(brushSize / 2))) -
                                        (int)(Math.Truncate((double)(brushSize / 2)) * GameWorld.WorldData.LevelWidth);
                    var i = startingIndex - 1 - GameWorld.WorldData.LevelWidth + (h * GameWorld.WorldData.LevelWidth) +
                            w;
                    indexes.Add(i);
                }
            }

            foreach (var ind in indexes)
            {
                if (ind >= 0 && ind < GameWorld.TileArray.Length)
                {
                    var t = CurrentArray[ind];
                    t.ResetToDefault();
                    t.Id = WorldDataIds[ind];
                    t.DefineTexture();
                    t.FindConnectedTextures(WorldDataIds,
                        GameWorld.WorldData.LevelWidth);
                    t.DefineTexture();
                    t.AddRandomlyGeneratedDecoration(CurrentArray, GameWorld.WorldData.LevelWidth);
                    CurrentArray[ind] = t;
                    //t.Color = Color.Red;
                }
            }

        }

        /// <summary>
        /// Creates construction specific particles around an area.
        /// </summary>
        /// <param name="rect"></param>
        private static void CreateConstructionParticles(Rectangle rect)
        {
            for (var i = 0; i < 5; i++)
            {
                GameWorld.ParticleSystem.Add(ParticleType.Smoke, CalcHelper.GetRandXAndY(rect), new Vector2(AdamGame.Random.Next(-10, 10) / 10f, AdamGame.Random.Next(-10, 10) / 10f), Color.White);
            }
        }

        /// <summary>
        /// Creates destruction specific particles around an area.
        /// </summary>
        /// <param name="rect"></param>
        private static void CreateDestructionParticles(Rectangle rect)
        {
            for (var i = 0; i < 5; i++)
            {
                GameWorld.ParticleSystem.Add(ParticleType.Smoke, CalcHelper.GetRandXAndY(rect), new Vector2(AdamGame.Random.Next(-10, 10) / 10f, AdamGame.Random.Next(-10, 10) / 10f), Color.White);
            }
        }

        /// <summary>
        /// Draw in gameworld.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!IsIntersectingUi())
                Brush.Draw(spriteBatch);

            //spriteBatch.Draw(Main.DefaultTexture, EditorRectangle, Color.White);
            GameWorld.PlayerTrail.Draw(spriteBatch);

            foreach (var line in InteractableConnections)
            {
                line.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Draws behind gameworld tiles.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawBehindTiles(SpriteBatch spriteBatch)
        {
            if (!IsIntersectingUi())
                Brush.DrawBehind(spriteBatch);
        }

        /// <summary>
        /// Draws on screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawUi(SpriteBatch spriteBatch)
        {
            _inventory.Draw(spriteBatch);
            _buttonBar.Draw(spriteBatch);
            HotBar.Draw(spriteBatch);
            _miniMap.Draw(spriteBatch);
            _inventory.DrawOnTop(spriteBatch);
        }

        /// <summary>
        /// Check if mouse is not over UI elements that cannot be clicked through.
        /// </summary>
        /// <returns></returns>
        private static bool IsIntersectingUi()
        {
            Rectangle mouse = InputHelper.GetMouseInUi();
            if (mouse.Intersects(_buttonBar.GetCollRectangle()))
                return true;
            if (Inventory.IsOpen && mouse.Intersects(_inventory.GetCollRectangle()))
                return true;

            return false;
        }

        public static void TestLevel(Button button)
        {
            TestLevel();
        }

        public static void ChangeToWallMode(Button button)
        {
            ChangeToWallMode();
        }
    }
}