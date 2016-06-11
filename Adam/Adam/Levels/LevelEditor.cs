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
    public class LevelEditor
    {
        private readonly SoundFx[] _construction = new SoundFx[3];
        private readonly Timer _idleTimerForSave = new Timer();
        private ButtonBar _buttonBar;
        private SoundFx _close, _open, _select;
        private SoundFx _destruction;
        private GameWorld _gameWorld;
        private bool _hasChangedSinceLastSave;
        private Inventory _inventory;
        private bool _inventoryKeyPressed;
        private byte _lastUsedTile = 1;
        private byte _lastUsedWall = 100;
        private Minimap _miniMap;
        private Rectangle _mouseRectInGameWorld;
        private bool _recentlyChanged;
        private SoundFx _wallMode;
        public readonly Brush Brush = new Brush();
        public Rectangle EditorRectangle;
        public int IndexOfMouse;
        public bool OnWallMode;
        public byte SelectedId = 1;

        private Tile[] CurrentArray => OnWallMode ? _gameWorld.WallArray : _gameWorld.TileArray;

        public void Load()
        {
            _inventory = new Inventory();
            _buttonBar = new ButtonBar();
            _miniMap = new Minimap();
            _miniMap.StartUpdating();

            for (var i = 1; i <= _construction.Length; i++)
            {
                _construction[i - 1] = new SoundFx("Sounds/Level Editor/construct" + i);
            }
            _destruction = new SoundFx("Sounds/Level Editor/destroy1");

            _wallMode = new SoundFx("Sounds/Level Editor/changeMode");
            _close = new SoundFx("Sounds/Level Editor/open");
            _open = new SoundFx("Sounds/Level Editor/close");
            _select = new SoundFx("Sounds/Level Editor/select");

            EditorRectangle = new Rectangle(GameWorld.Instance.WorldData.LevelWidth * Main.Tilesize / 2,
                GameWorld.Instance.WorldData.LevelHeight * Main.Tilesize / 2, Main.DefaultResWidth, Main.DefaultResHeight);
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
        /// <param name="gameTime"></param>
        /// <param name="currentLevel"></param>
        public void Update(GameTime gameTime, GameMode currentLevel)
        {
            GameWorld.Instance.Player.Health = GameWorld.Instance.Player.MaxHealth;

            SoundtrackManager.PlayLevelEditorTheme();

            _gameWorld = GameWorld.Instance;
            _inventory.Update();
            _buttonBar.Update();
            Brush.Update();
            CheckIfOnInventory();
            CheckIfPositioningPlayer();
            CheckIfChangedToWallMode();
            CheckForCameraMovement();
            CheckForMouseInput();

            // Auto-save functionality.
            if (_idleTimerForSave.TimeElapsedInSeconds > 1 && _hasChangedSinceLastSave)
            {
                _hasChangedSinceLastSave = false;
                DataFolder.SaveLevel();
            }
        }

        /// <summary>
        /// Changes the array that will be modified and changes the opacity of the foreground tiles.
        /// </summary>
        public void ChangeToWallMode()
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
        private void CheckIfChangedToWallMode()
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
        private void CheckIfOnInventory()
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
        private void CheckForCameraMovement()
        {
            _gameWorld.Camera.UpdateSmoothly(EditorRectangle, GameWorld.Instance.WorldData.LevelWidth,
                GameWorld.Instance.WorldData.LevelHeight, true);
            const int speed = 15;

            if (InputHelper.IsKeyDown(Keys.A))
            {
                _idleTimerForSave.Reset();
                EditorRectangle.X -= speed;
            }
            if (InputHelper.IsKeyDown(Keys.D))
            {
                _idleTimerForSave.Reset();
                EditorRectangle.X += speed;
            }
            if (InputHelper.IsKeyDown(Keys.W))
            {
                _idleTimerForSave.Reset();
                EditorRectangle.Y -= speed;
            }
            if (InputHelper.IsKeyDown(Keys.S))
            {
                _idleTimerForSave.Reset();
                EditorRectangle.Y += speed;
            }


            //Prevent camera box from moving out of screen
            if (EditorRectangle.X < 0)
            {
                EditorRectangle.X = 0;
            }
            if (EditorRectangle.X > (GameWorld.Instance.WorldData.LevelWidth * Main.Tilesize) - EditorRectangle.Width)
            {
                EditorRectangle.X = (GameWorld.Instance.WorldData.LevelWidth * Main.Tilesize) - EditorRectangle.Width;
            }
            if (EditorRectangle.Y < 0)
            {
                EditorRectangle.Y = 0;
            }
            if (EditorRectangle.Y > (GameWorld.Instance.WorldData.LevelHeight * Main.Tilesize) - EditorRectangle.Height)
            {
                EditorRectangle.Y = (GameWorld.Instance.WorldData.LevelHeight * Main.Tilesize) - EditorRectangle.Height;
            }
        }

        /// <summary>
        /// Checks for input to draw, erase or select tiles using the mouse.
        /// </summary>
        private void CheckForMouseInput()
        {
            InputHelper.GetMouseRectGameWorld(ref _mouseRectInGameWorld);
            IndexOfMouse = (_mouseRectInGameWorld.Center.Y / Main.Tilesize * _gameWorld.WorldData.LevelWidth) +
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
        private void CheckIfPositioningPlayer()
        {
            if (InputHelper.IsKeyDown(Keys.P))
            {
                foreach (var index in _gameWorld.VisibleTileArray)
                {
                    if (index >= 0 && index < _gameWorld.TileArray.Length)
                    {
                        //Check index of mouse
                        if (_gameWorld.TileArray[index].DrawRectangle.Intersects(_mouseRectInGameWorld))
                        {
                            _gameWorld.TileArray[index].Id = 200;
                            _gameWorld.TileArray[index].DefineTexture();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates all the tiles highlighted by the brush.
        /// </summary>
        /// <param name="desiredId"></param>
        private void UpdateSelectedTiles(int desiredId)
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
        private void Construct(Tile t)
        {
            _idleTimerForSave.Reset();
            _hasChangedSinceLastSave = true;
            UpdateTilesAround(t.TileIndex);
            _construction[GameWorld.RandGen.Next(0, 3)].Play();
            Main.Camera.Shake();
            CreateConstructionParticles(t.DrawRectangle);
        }

        /// <summary>
        /// Provides sound and effects for destruction.
        /// </summary>
        /// <param name="t"></param>
        private void Destroy(Tile t)
        {
            _idleTimerForSave.Reset();
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
        private void UpdateTilesAround(int index)
        {
            var indexes = new List<int>();
            var diameterOfSquare = 2 + Brush.Size;
            for (var h = 0; h < diameterOfSquare; h++)
            {
                for (var w = 0; w < diameterOfSquare; w++)
                {
                    var brushSize = Brush.Size;
                    var startingIndex = index - (int)(Math.Truncate((double)(brushSize / 2))) -
                                        (int)(Math.Truncate((double)(brushSize / 2)) * _gameWorld.WorldData.LevelWidth);
                    var i = startingIndex - 1 - _gameWorld.WorldData.LevelWidth + (h * _gameWorld.WorldData.LevelWidth) +
                            w;
                    indexes.Add(i);
                }
            }

            foreach (var ind in indexes)
            {
                if (ind >= 0 && ind < _gameWorld.TileArray.Length)
                {
                    var t = CurrentArray[ind];
                    t.DefineTexture();
                    t.FindConnectedTextures(CurrentArray,
                        _gameWorld.WorldData.LevelWidth);
                    t.DefineTexture();
                    //t.AddRandomlyGeneratedDecoration(CurrentArray,+_gameWorld.WorldData.LevelWidth);
                }
            }
        }

        /// <summary>
        /// Creates construction specific particles around an area.
        /// </summary>
        /// <param name="rect"></param>
        private void CreateConstructionParticles(Rectangle rect)
        {
            for (var i = 0; i < 3; i++)
            {
                GameWorld.ParticleSystem.Add(new SmokeParticle(rect.Center.X, rect.Center.Y,
                    new Vector2(GameWorld.RandGen.Next(-10, 10) / 10f, GameWorld.RandGen.Next(-10, 10) / 10f)));
            }
        }

        /// <summary>
        /// Creates destruction specific particles around an area.
        /// </summary>
        /// <param name="rect"></param>
        private void CreateDestructionParticles(Rectangle rect)
        {
            for (var i = 0; i < 3; i++)
            {
                GameWorld.ParticleSystem.Add(new SmokeParticle(rect.Center.X, rect.Center.Y,
                    new Vector2(GameWorld.RandGen.Next(-10, 10) / 10f, GameWorld.RandGen.Next(-10, 10) / 10f)));
            }
        }

        /// <summary>
        /// Draw in gameworld.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsIntersectingUi())
                Brush.Draw(spriteBatch);
        }

        /// <summary>
        /// Draws behind gameworld tiles.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawBehindTiles(SpriteBatch spriteBatch)
        {
            if (!IsIntersectingUi())
                Brush.DrawBehind(spriteBatch);
        }

        /// <summary>
        /// Draws on screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawUi(SpriteBatch spriteBatch)
        {
            _inventory.Draw(spriteBatch);
            _buttonBar.Draw(spriteBatch);
            _miniMap.Draw(spriteBatch);
        }

        /// <summary>
        /// Check if mouse is not over UI elements that cannot be clicked through.
        /// </summary>
        /// <returns></returns>
        private bool IsIntersectingUi()
        {
            if (InputHelper.MouseRectangle.Intersects(_buttonBar.GetCollRectangle()))
                return true;
            if (Inventory.IsOpen && InputHelper.MouseRectangle.Intersects(_inventory.GetCollRectangle()))
                return true;

            return false;
        }
    }
}