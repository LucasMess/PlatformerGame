﻿using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Sound;
using ThereMustBeAnotherWay.Network;
using ThereMustBeAnotherWay.Particles;
using ThereMustBeAnotherWay.PlayerCharacter;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.UI.Elements;
using ThereMustBeAnotherWay.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static ThereMustBeAnotherWay.TMBAW_Game;
using ThereMustBeAnotherWay.UI.Level_Editor;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay.Levels
{
    /// <summary>
    /// Resposible for user interface and input for the level editor.
    /// </summary>
    public static class LevelEditor
    {
        private static GameTimer _switchEditAndPlayTimer = new GameTimer(true);
        private static readonly SoundFx[] Construction = new SoundFx[3];
        private static readonly GameTimer IdleTimerForSave = new GameTimer(true);
        public static ButtonBar ButtonBar;
        private static SoundFx _close, _open, _select;
        private static SoundFx _destruction;
        private static SoundFx _testSound = new SoundFx("Sounds/Level Editor/test_level");
        private static bool _hasChangedSinceLastSave;
        private static Inventory _inventory;
        private static bool _inventoryKeyPressed;
        private static Minimap _minimap;
        private static Rectangle _mouseRectInGameWorld;
        private static bool _recentlyChanged;
        private static SoundFx _wallMode;
        public static readonly Brush Brush = new Brush();
        public static Rectangle EditorRectangle;
        public static int IndexOfMouse;
        public static bool OnWallMode;
        /// <summary>
        /// Returns true if the user has enabled lighting in edit mode.
        /// </summary>
        public static bool IsLightingEnabled { get; set; } = true;
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
            ButtonBar = new ButtonBar();
            HotBar.Initialize();
            _minimap = new Minimap();
            _minimap.StartUpdating();

            for (var i = 1; i <= Construction.Length; i++)
            {
                Construction[i - 1] = new SoundFx("Sounds/Level Editor/construct" + i);
            }
            _destruction = new SoundFx("Sounds/Level Editor/destroy1");

            _wallMode = new SoundFx("Sounds/Level Editor/changeMode");
            _close = new SoundFx("Sounds/Level Editor/open");
            _open = new SoundFx("Sounds/Level Editor/close");
            _select = new SoundFx("Sounds/Level Editor/select");


            foreach (Player player in GameWorld.GetPlayers())
                player.SetPosition(player.RespawnPos);

            Cursor.Show();
        }


        /// <summary>
        /// Updates all UI components and checks for input.
        /// </summary>
        public static void Update()
        {
            if (GameDebug.IsTyping)
                return;

            foreach (Player player in GameWorld.GetPlayers())
                player.Health = player.MaxHealth;

            SoundtrackManager.PlayLevelEditorTheme();

            _inventory.Update();
            ButtonBar.Update();
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
            if (Session.IsActive) return;
            _hasChangedSinceLastSave = false;
            DataFolder.SaveLevel();
        }

        /// <summary>
        /// Tests level as the player.
        /// </summary>
        public static void TestLevel()
        {
            if (TMBAW_Game.CurrentGameMode != GameMode.Play && _switchEditAndPlayTimer.TimeElapsedInMilliSeconds > 1000)
            {

                try
                {
                    SaveLevel();
                    GameWorld.IsTestingLevel = true;
                    GameWorld.PlayerTrail = new PlayerTrail();
                    TMBAW_Game.CurrentGameMode = GameMode.Play;
                    GameWorld.PrepareLevelForTesting();
                    foreach (Player player in GameWorld.GetPlayers())
                    {
                        player.ComplexAnimation.RemoveAllFromQueue();
                        player.SetVelX(0);
                        player.SetVelY(0);
                    }
                    Overlay.FlashWhite();
                    _switchEditAndPlayTimer.Reset();
                    _testSound.Play();
                    Cursor.Hide();
                    // DataFolder.PlayLevel(DataFolder.CurrentLevelFilePath);
                }
                catch (Exception e)
                {
                    TMBAW_Game.MessageBox.Show(e.Message);
                }
            }
        }

        public static void GoBackToEditing()
        {
            if (TMBAW_Game.CurrentGameMode != GameMode.Edit && _switchEditAndPlayTimer.TimeElapsedInMilliSeconds > 1000)
            {
                try
                {
                    GameWorld.IsTestingLevel = false;
                    TMBAW_Game.CurrentGameMode = GameMode.Edit;
                    GameWorld.PrepareLevelForTesting();
                    Overlay.FlashWhite();
                    _switchEditAndPlayTimer.Reset();
                    _testSound.Play();
                    Cursor.Show();
                    // DataFolder.PlayLevel(DataFolder.CurrentLevelFilePath);
                }
                catch (Exception e)
                {
                    TMBAW_Game.MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// Changes the array that will be modified and changes the opacity of the foreground tiles.
        /// </summary>
        public static void ChangeWallModeTo(bool value)
        {
            OnWallMode = value;
            _wallMode.PlayNewInstanceOnce();
            _wallMode.Reset();
        }

        /// <summary>
        /// Checks for input to change to wall mode.
        /// </summary>
        private static void CheckIfChangedToWallMode()
        {
            if (InputSystem.IsKeyDown(Keys.L) && !_recentlyChanged)
            {
                ChangeWallModeTo(!OnWallMode);
                _recentlyChanged = true;
            }
            if (InputSystem.IsKeyUp(Keys.L))
            {
                _recentlyChanged = false;
            }



        }

        /// <summary>
        /// Checks for input to change to open or close the inventory.
        /// </summary>
        private static void CheckIfOnInventory()
        {
            if (InputSystem.IsKeyDown(Keys.E))
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
            if (InputSystem.IsKeyUp(Keys.E))
            {
                _inventoryKeyPressed = false;
            }
        }

        /// <summary>
        /// Checks for input to move the camera.
        /// </summary>
        private static void CheckForCameraMovement()
        {
            TMBAW_Game.Camera.UpdateSmoothly(GameWorld.GetPlayers()[0].GetCollRectangle(), GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight, true);
            float speed = 9f;

            foreach (Player player in GameWorld.GetPlayers())
            {
                if (player.IsMoveLeftPressed())
                {
                    IdleTimerForSave.Reset();
                    player.MoveBy(-speed, 0);
                }
                if (player.IsMoveRightPressed())
                {
                    IdleTimerForSave.Reset();
                    player.MoveBy(speed, 0);
                }
                if (player.IsMoveUpPressed())
                {
                    IdleTimerForSave.Reset();
                    player.MoveBy(0, -speed);
                }
                if (player.IsMoveDownPressed())
                {
                    IdleTimerForSave.Reset();
                    player.MoveBy(0, speed);
                }
                if (player.IsTestLevelPressed())
                {
                    TestLevel();
                }
            }


            //Prevent camera box from moving out of screen
            if (EditorRectangle.X < 0)
            {
                EditorRectangle.X = 0;
            }
            if (EditorRectangle.X > (GameWorld.WorldData.LevelWidth * TMBAW_Game.Tilesize) - EditorRectangle.Width)
            {
                EditorRectangle.X = (GameWorld.WorldData.LevelWidth * TMBAW_Game.Tilesize) - EditorRectangle.Width;
            }
            if (EditorRectangle.Y < 0)
            {
                EditorRectangle.Y = 0;
            }
            if (EditorRectangle.Y > (GameWorld.WorldData.LevelHeight * TMBAW_Game.Tilesize) - EditorRectangle.Height)
            {
                EditorRectangle.Y = (GameWorld.WorldData.LevelHeight * TMBAW_Game.Tilesize) - EditorRectangle.Height;
            }
        }

        /// <summary>
        /// Checks for input to draw, erase or select tiles using the mouse.
        /// </summary>
        private static void CheckForMouseInput()
        {
            _mouseRectInGameWorld = InputSystem.GetMouseRectGameWorld();
            IndexOfMouse = CalcHelper.GetIndexInGameWorld(_mouseRectInGameWorld.X, _mouseRectInGameWorld.Y);

            if (!IsIntersectingUi())
            {
                if (InputSystem.IsLeftMousePressed() && InputSystem.IsRightMousePressed())
                    return;

                if (InputSystem.IsLeftMousePressed())
                {
                    if (Brush.CurrentBrushMode == Brush.BrushMode.Build && lastAddedTile != IndexOfMouse)
                    {
                        lastAddedTile = IndexOfMouse;
                        lastRemovedTile = -1;
                        UpdateSelectedTiles(SelectedId);
                    }
                    else if (Brush.CurrentBrushMode == Brush.BrushMode.Erase && lastRemovedTile != IndexOfMouse)
                    {
                        lastRemovedTile = IndexOfMouse;
                        lastAddedTile = -1;
                        UpdateSelectedTiles(0);
                    }
                    else if (Brush.CurrentBrushMode == Brush.BrushMode.Select)
                    {
                        Tile tile = GameWorld.GetTile(IndexOfMouse);
                        tile.InteractInEditMode();
                    }
                }
                if (InputSystem.IsRightMousePressed())
                {
                    if (!OnWallMode)
                        ChangeWallModeTo(true);
                    if (Brush.CurrentBrushMode == Brush.BrushMode.Build && lastAddedTile != IndexOfMouse)
                    {
                        // Entities cannot be placed in wall mode.
                        if ((int)SelectedId < 200)
                        {
                            lastAddedTile = IndexOfMouse;
                            lastRemovedTile = -1;
                            UpdateSelectedTiles(SelectedId);
                        }
                    }
                    else if (Brush.CurrentBrushMode == Brush.BrushMode.Erase && lastRemovedTile != IndexOfMouse)
                    {
                        lastRemovedTile = IndexOfMouse;
                        lastAddedTile = -1;
                        UpdateSelectedTiles(0);
                    }
                }
                else if (InputSystem.IsMiddleMousePressed())
                {
                    TileType lastSelectedId = SelectedId;
                    SelectedId = GameWorld.WorldData.TileIDs[IndexOfMouse];
                    if (SelectedId == 0)
                        SelectedId = GameWorld.WorldData.WallIDs[IndexOfMouse];
                    if (SelectedId != 0)
                    {
                        HotBar.AddToHotBarFromWorld(SelectedId);
                        Brush.ChangeBrushMode(Brush.BrushMode.Build);
                    }
                    else
                    {
                        SelectedId = lastSelectedId;
                    }
                }
                else
                {
                    if (OnWallMode)
                        ChangeWallModeTo(false);
                }
            }
        }

        /// <summary>
        /// Checks to see if shortcut to put player spawn point is pressed.
        /// </summary>
        private static void CheckIfPositioningPlayer()
        {
            if (InputSystem.IsKeyDown(Keys.P))
            {
                Brush.Size = 1;
                UpdateSelectedTiles(TileType.Player);
                _hasChangedSinceLastSave = true;
            }
        }

        /// <summary>
        /// Used to change the id of a specific tile when in multiplayer mode.
        /// </summary>
        /// <param name="tileIndex"></param>
        /// <param name="newTileId"></param>
        /// <param name="isWall"></param>
        public static void UpdateTileFromP2P(Packet.TileIdChange packet)
        {
            int tileIndex = packet.TileIndex;
            int newTileId = packet.TileId;
            bool isWall = packet.IsWall;

            Tile[] array = GameWorld.TileArray;
            if (isWall)
            {
                array = GameWorld.WallArray;
            }
            array[tileIndex].ResetToDefault();
            WorldDataIds[tileIndex] = (TileType)newTileId;
            if (newTileId == 0)
            {
                Destroy(array[tileIndex]);
            }
            else
            {
                Construct(array[tileIndex]);
            }
            UpdateTilesAround(tileIndex);
        }

        /// <summary>
        /// Updates all the tiles highlighted by the brush.
        /// </summary>
        /// <param name="desiredId"></param>
        private static void UpdateSelectedTiles(TileType desiredId)
        {
            bool hasChanged = false;
            foreach (var i in Brush.SelectedIndices)
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

                    // Send change over network.
                    byte[] data = new Packet.TileIdChange()
                    {
                        TileIndex = i,
                        TileId = (int)desiredId,
                        IsWall = (CurrentArray == GameWorld.WallArray),
                    }.ToByteArray();
                    Session.Send(data, Steamworks.EP2PSend.k_EP2PSendReliable, Session.BB_TileIdChange);
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

                        // Send change over network.
                        //byte[] data = new Packet.TileIdChange()
                        //{
                        //    TileIndex = i,
                        //    TileId = (int)desiredId,
                        //    IsWall = (CurrentArray == GameWorld.WallArray),
                        //}.ToByteArray();
                        //Session.Send(data, Steamworks.EP2PSend.k_EP2PSendReliable, Session.BB_TileIdChange);
                    }
                }
            }
            if (hasChanged)
            {
                UpdateTilesAround(IndexOfMouse);
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
            Construction[TMBAW_Game.Random.Next(0, 3)].PlayIfStopped();
            TMBAW_Game.Camera.Shake();
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
            TMBAW_Game.Camera.Shake();
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
                    CurrentArray[ind] = t;
                    //t.Color = Color.Red;
                }
            }

            // TODO: Merge these two foreaches so that the code is faster.
            foreach (var ind in indexes)
            {
                if (ind >= 0 && ind < GameWorld.TileArray.Length)
                {
                    var t = CurrentArray[ind];
                    t.FindConnectedTextures(WorldDataIds,
    GameWorld.WorldData.LevelWidth);
                    t.DefineTexture();
                    t.AddRandomlyGeneratedDecoration(CurrentArray, GameWorld.WorldData.LevelWidth);
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
                ParticleSystem.Add(ParticleType.Smoke, CalcHelper.GetRandXAndY(rect), new Vector2(TMBAW_Game.Random.Next(-10, 10) / 10f, TMBAW_Game.Random.Next(-10, 10) / 10f), Color.White);
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
                ParticleSystem.Add(ParticleType.Smoke, CalcHelper.GetRandXAndY(rect), new Vector2(TMBAW_Game.Random.Next(-10, 10) / 10f, TMBAW_Game.Random.Next(-10, 10) / 10f), Color.White);
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
            _minimap.Draw(spriteBatch);
            _inventory.Draw(spriteBatch);
            ButtonBar.Draw(spriteBatch);
            HotBar.Draw(spriteBatch);
            _inventory.DrawOnTop(spriteBatch);
        }

        /// <summary>
        /// Check if mouse is not over UI elements that cannot be clicked through.
        /// </summary>
        /// <returns></returns>
        private static bool IsIntersectingUi()
        {
            Rectangle mouse = InputSystem.GetMouseInUi();
            if (mouse.Intersects(ButtonBar.GetCollRectangle()))
                return true;
            if (Inventory.IsOpen && mouse.Intersects(_inventory.GetCollRectangle()))
                return true;
            if (_minimap.IsIntersecting(mouse))
                return true;

            return false;
        }

        public static void TestLevel(Button button)
        {
            TestLevel();
        }

        public static void ChangeToWallMode(Button button)
        {
            ChangeWallModeTo(!OnWallMode);
        }
    }
}