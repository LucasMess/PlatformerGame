using System;
using System.Collections.Generic;
using Adam.Misc;
using Adam.Misc.Sound;
using Adam.Particles;
using Adam.UI;
using Adam.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam.Levels
{
    /// <summary>
    /// Responsible for user interface and input for the level editor.
    /// </summary>
    public static class LevelEditor
    {
        private static readonly SoundFx[] Construction = new SoundFx[3];
        private static readonly Timer IdleTimerForSave = new Timer();
        private static ButtonBar _buttonBar;
        private static HotBar _hotBar;
        private static SoundFx _close, _open, _select;
        private static SoundFx _destruction;
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
        public static byte SelectedId = 1;

        private static Tile[] CurrentArray => OnWallMode ? GameWorld.WallArray : GameWorld.TileArray;

        public static void Load()
        {
            _inventory = new Inventory();
            _buttonBar = new ButtonBar();
            _hotBar = new HotBar();
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

            EditorRectangle = new Rectangle(GameWorld.WorldData.LevelWidth * Main.Tilesize / 2,
                GameWorld.WorldData.LevelHeight * Main.Tilesize / 2, Main.DefaultResWidth, Main.DefaultResHeight);
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
            GameWorld.Player.Health = GameWorld.Player.MaxHealth;

            SoundtrackManager.PlayLevelEditorTheme();

            _inventory.Update();
            _buttonBar.Update();
            _hotBar.Update();
            Brush.Update();
            CheckIfOnInventory();
            CheckIfPositioningPlayer();
            CheckIfChangedToWallMode();
            CheckForCameraMovement();
            CheckForMouseInput();

            // Auto-save functionality.
            if (IdleTimerForSave.TimeElapsedInSeconds > 1 && _hasChangedSinceLastSave)
            {
                _hasChangedSinceLastSave = false;
                DataFolder.SaveLevel();
            }
        }

        /// <summary>
        /// Tests level as the player.
        /// </summary>
        public static void TestLevel()
        {
            DataFolder.PlayLevel(DataFolder.CurrentLevelFilePath);
        }

        /// <summary>
        /// Changes the array that will be modified and changes the opacity of the foreground tiles.
        /// </summary>
        public static void ChangeToWallMode()
        {
            OnWallMode = !OnWallMode;
            _wallMode.PlayNewInstanceOnce();
            _wallMode.Reset();

            if (OnWallMode)
            {
                _lastUsedTile = SelectedId;
                SelectedId = _lastUsedWall;
            }
            else
            {
                _lastUsedWall = SelectedId;
                SelectedId = _lastUsedTile;
            }
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
                    Inventory.StartAnimation();
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
            Main.Camera.UpdateSmoothly(EditorRectangle, GameWorld.WorldData.LevelWidth,
                GameWorld.WorldData.LevelHeight, true);
            const int speed = 15;

            if (InputHelper.IsKeyDown(Keys.A))
            {
                IdleTimerForSave.Reset();
                EditorRectangle.X -= speed;
            }
            if (InputHelper.IsKeyDown(Keys.D))
            {
                IdleTimerForSave.Reset();
                EditorRectangle.X += speed;
            }
            if (InputHelper.IsKeyDown(Keys.W))
            {
                IdleTimerForSave.Reset();
                EditorRectangle.Y -= speed;
            }
            if (InputHelper.IsKeyDown(Keys.S))
            {
                IdleTimerForSave.Reset();
                EditorRectangle.Y += speed;
            }


            //Prevent camera box from moving out of screen
            if (EditorRectangle.X < 0)
            {
                EditorRectangle.X = 0;
            }
            if (EditorRectangle.X > (GameWorld.WorldData.LevelWidth * Main.Tilesize) - EditorRectangle.Width)
            {
                EditorRectangle.X = (GameWorld.WorldData.LevelWidth * Main.Tilesize) - EditorRectangle.Width;
            }
            if (EditorRectangle.Y < 0)
            {
                EditorRectangle.Y = 0;
            }
            if (EditorRectangle.Y > (GameWorld.WorldData.LevelHeight * Main.Tilesize) - EditorRectangle.Height)
            {
                EditorRectangle.Y = (GameWorld.WorldData.LevelHeight * Main.Tilesize) - EditorRectangle.Height;
            }
        }

        /// <summary>
        /// Checks for input to draw, erase or select tiles using the mouse.
        /// </summary>
        private static void CheckForMouseInput()
        {
            InputHelper.GetMouseRectGameWorld(ref _mouseRectInGameWorld);
            IndexOfMouse = (_mouseRectInGameWorld.Center.Y / Main.Tilesize * GameWorld.WorldData.LevelWidth) +
                           (_mouseRectInGameWorld.Center.X / Main.Tilesize);

            if (!IsIntersectingUi())
            {
                if (InputHelper.IsLeftMousePressed())
                {
                    UpdateSelectedTiles(SelectedId);
                }

                if (InputHelper.IsRightMousePressed())
                {
                    UpdateSelectedTiles(0);
                }

                if (InputHelper.IsMiddleMousePressed())
                {
                    SelectedId = CurrentArray[IndexOfMouse].Id;
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
                UpdateSelectedTiles(200);
            }
        }

        /// <summary>
        /// Updates all the tiles highlighted by the brush.
        /// </summary>
        /// <param name="desiredId"></param>
        private static void UpdateSelectedTiles(int desiredId)
        {
            foreach (var i in Brush.SelectedIndexes)
            {
                if (i < 0 || i > CurrentArray.Length)
                    continue;
                int tileId = CurrentArray[i].Id;

                //Wants to destroy. Any block can be destroyed.
                if (desiredId == 0)
                {
                    //Check to see if block is already air.
                    if (tileId == 0)
                        continue;
                    CurrentArray[i].Destroy();
                    CurrentArray[i].Id = (byte)desiredId;
                    if (OnWallMode)
                    {
                        CurrentArray[i].IsWall = true;
                    }
                    else
                        CurrentArray[i].IsWall = false;

                    Destroy(CurrentArray[i]);
                }

                //Wants to build, but only if there is air.
                else
                {
                    if (tileId == 0)
                    {
                        CurrentArray[i].Id = (byte)desiredId;
                        if (OnWallMode)
                        {
                            CurrentArray[i].IsWall = true;
                        }
                        else
                            CurrentArray[i].IsWall = false;
                        Construct(CurrentArray[i]);
                    }
                }
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
            UpdateTilesAround(t.TileIndex);
            Construction[Main.Random.Next(0, 3)].Play();
            Main.Camera.Shake();
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
            _destruction.Play();
            CreateDestructionParticles(t.GetDrawRectangle());
            UpdateTilesAround(t.TileIndex);
            Main.Camera.Shake();
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
                    t.DefineTexture();
                    t.FindConnectedTextures(CurrentArray,
                        GameWorld.WorldData.LevelWidth);
                    t.DefineTexture();
                    //t.AddRandomlyGeneratedDecoration(CurrentArray,+GameWorld.WorldData.LevelWidth);
                }
            }
        }

        /// <summary>
        /// Creates construction specific particles around an area.
        /// </summary>
        /// <param name="rect"></param>
        private static void CreateConstructionParticles(Rectangle rect)
        {
            for (var i = 0; i < 3; i++)
            {
                GameWorld.ParticleSystem.Add(new SmokeParticle(rect.Center.X, rect.Center.Y,
                    new Vector2(Main.Random.Next(-10, 10) / 10f, Main.Random.Next(-10, 10) / 10f)));
            }
        }

        /// <summary>
        /// Creates destruction specific particles around an area.
        /// </summary>
        /// <param name="rect"></param>
        private static void CreateDestructionParticles(Rectangle rect)
        {
            for (var i = 0; i < 3; i++)
            {
                GameWorld.ParticleSystem.Add(new SmokeParticle(rect.Center.X, rect.Center.Y,
                    new Vector2(Main.Random.Next(-10, 10) / 10f, Main.Random.Next(-10, 10) / 10f)));
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
            _hotBar.Draw(spriteBatch);
            _miniMap.Draw(spriteBatch);
            _inventory.DrawOnTop(spriteBatch);
        }

        /// <summary>
        /// Check if mouse is not over UI elements that cannot be clicked through.
        /// </summary>
        /// <returns></returns>
        private static bool IsIntersectingUi()
        {
            if (InputHelper.MouseRectangle.Intersects(_buttonBar.GetCollRectangle()))
                return true;
            if (Inventory.IsOpen && InputHelper.MouseRectangle.Intersects(_inventory.GetCollRectangle()))
                return true;

            return false;
        }
    }
}