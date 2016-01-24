using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adam.UI;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;
using Adam.UI.Elements;
using Adam.Misc.Helpers;
using Adam.Interactables;
using Adam.Misc.Sound;

namespace Adam.Levels
{
    public class LevelEditor
    {
        GameWorld _gameWorld;
        TileScroll _tileScroll = new TileScroll();
        EntityScroll _entityScroll = new EntityScroll();
        Minimap _miniMap;
        public ActionBar ActionBar = new ActionBar();
        TileDescription _tileDescription = new TileDescription();
        public Brush Brush = new Brush();
        private bool _hasChangedSinceLastSave;
        public bool OnInventory;
        public bool OnWallMode;
        bool _inventoryKeyPressed;
        float _blackScreenOpacity;
        bool _recentlyChanged;
        bool _onPortalLinkMode;
        Portal _selectedPortal;

        public Rectangle EditorRectangle;
        public int IndexOfMouse;
        public byte SelectedId = 1;

        byte _lastUsedTile = 1;
        byte _lastUsedWall = 100;

        SoundFx[] _construction = new SoundFx[3];
        SoundFx _destruction;
        SoundFx _wallMode;
        SoundFx _close, _open, _select;
        Rectangle _mouseRect;

        Timer _idleTimerForSave = new Timer();

        public void Load()
        {
            _miniMap = new Minimap();
            _miniMap.StartUpdating();
            OnInventory = false;
            _tileScroll.Load();
            _tileScroll.TileSelected += TileScroll_TileSelected;

            _entityScroll.Load();
            _entityScroll.TileSelected += EntityScroll_TileSelected;

            for (int i = 1; i <= _construction.Length; i++)
            {
                _construction[i - 1] = new SoundFx("Sounds/Level Editor/construct" + i);
            }
            _destruction = new SoundFx("Sounds/Level Editor/destroy1");

            _wallMode = new SoundFx("Sounds/Level Editor/changeMode");
            _close = new SoundFx("Sounds/Level Editor/open");
            _open = new SoundFx("Sounds/Level Editor/close");
            _select = new SoundFx("Sounds/Level Editor/select");

            EditorRectangle = new Rectangle(GameWorld.Instance.WorldData.LevelWidth * Main.Tilesize / 2, GameWorld.Instance.WorldData.LevelHeight * Main.Tilesize / 2, Main.DefaultResWidth, Main.DefaultResHeight);
        }

        private void EntityScroll_TileSelected(TileSelectedArgs e)
        {
            if (e.Id != SelectedId)
            {
                _select.Reset();
                _select.PlayNewInstanceOnce();
                SelectedId = (byte)e.Id;
            }

        }

        private void TileScroll_TileSelected(TileSelectedArgs e)
        {
            if (e.Id != SelectedId)
            {
                _select.Reset();
                _select.PlayNewInstanceOnce();
                SelectedId = (byte)e.Id;
            }
        }

        public void Update(GameTime gameTime, GameMode currentLevel)
        {
            GameWorld.Instance.Player.Health = GameWorld.Instance.Player.MaxHealth;

            SoundtrackManager.PlayLevelEditorTheme();

            _gameWorld = GameWorld.Instance;
            _tileScroll.Update();
            _entityScroll.Update();
            ActionBar.Update();
            Brush.Update();
            _tileDescription.Update();

            CheckIfOnInventory();
            CheckIfPositioningPlayer();
            CheckIfChangedToWallMode();

            const float deltaOpacity = .05f;

            if (!OnInventory)
            {
                CheckForCameraMovement();
                CheckForInput();

                _blackScreenOpacity -= deltaOpacity;
            }
            else
            {
                _blackScreenOpacity += deltaOpacity;
            }

            if (_blackScreenOpacity > .7) _blackScreenOpacity = .7f;
            if (_blackScreenOpacity < 0) _blackScreenOpacity = 0;

            // Auto-save functionality.
            if (_idleTimerForSave.TimeElapsedInSeconds > 1 && _hasChangedSinceLastSave)
            {
                _hasChangedSinceLastSave = false;
                DataFolder.SaveLevel();
            }
        }

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


            _tileScroll.Load();
        }

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

        private void CheckIfOnInventory()
        {
            if (InputHelper.IsKeyDown(Keys.E))
            {
                if (!_inventoryKeyPressed)
                {
                    if (!OnInventory)
                    {
                        _open.PlayNewInstanceOnce();
                        _open.Reset();
                    }
                    else
                    {
                        _close.PlayNewInstanceOnce();
                        _close.Reset();
                    }
                    OnInventory = !OnInventory;
                    _inventoryKeyPressed = true;
                }
            }
            if (InputHelper.IsKeyUp(Keys.E))
            {
                _inventoryKeyPressed = false;
            }
        }

        private void CheckForCameraMovement()
        {
            _gameWorld.Camera.UpdateSmoothly(EditorRectangle, GameWorld.Instance.WorldData.LevelWidth, GameWorld.Instance.WorldData.LevelHeight, true);
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

        private void CheckForInput()
        {
            InputHelper.GetMouseRectGameWorld(ref _mouseRect);
            IndexOfMouse = (_mouseRect.Center.Y / Main.Tilesize * _gameWorld.WorldData.LevelWidth) + (_mouseRect.Center.X / Main.Tilesize);

            //if (onPortalLinkMode)
            //{
            //    selectedPortal.ConnectingLine = new Line(selectedPortal.Position, new Vector2(mouseRect.X, mouseRect.Y));

            //    if (InputHelper.IsLeftMousePressed())
            //    {
            //        // If mouse is on portal.
            //        if (CurrentArray[IndexOfMouse].ID == 58)
            //        {
            //            Portal link = (Portal)GameWorld.Instance.worldData.PortalLinks.TryGetValue(IndexOfMouse);
            //            if (link.PortalID != selectedPortal.PortalID)
            //            {
            //                selectedPortal.LinkTo(link);
            //                onPortalLinkMode = false;
            //            }
            //        }
            //    }
            //    return;
            //}

            if (InputHelper.IsLeftMousePressed())
            {
                if (InputHelper.IsKeyDown(Keys.LeftAlt))
                    SpecialInteractionTile();
                else UpdateSelectedTiles(SelectedId);
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


        private void CheckIfPositioningPlayer()
        {
            if (InputHelper.IsKeyDown(Keys.P))
            {
                foreach (int index in _gameWorld.VisibleTileArray)
                {
                    if (index >= 0 && index < _gameWorld.TileArray.Length)
                    {
                        //Check index of mouse
                        if (_gameWorld.TileArray[index].DrawRectangle.Intersects(_mouseRect))
                        {
                            _gameWorld.TileArray[index].Id = 200;
                            _gameWorld.TileArray[index].DefineTexture();
                        }
                    }
                }
            }
        }

        private void UpdateSelectedTiles(int desiredId)
        {
            foreach (int i in Brush.SelectedIndexes)
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
                    else
                    {
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
                    else continue;
                }
            }
        }

        private void SpecialInteractionTile()
        {
            // Portal
            //if (CurrentArray[IndexOfMouse].ID == 58)
            //{
            //    onPortalLinkMode = true;
            //    selectedPortal = (Portal)GameWorld.Instance.worldData.PortalLinks.TryGetValue(IndexOfMouse);
            //    Console.WriteLine("On portal link mode!");   
            //}
        }

        private void Construct(Tile t)
        {
            _idleTimerForSave.Reset();
            _hasChangedSinceLastSave = true;
            UpdateTilesAround(t.TileIndex);
            _construction[GameWorld.RandGen.Next(0, 3)].Play();
            //CreateConstructionParticles(t.DrawRectangle);
        }

        private void Destroy(Tile t)
        {
            _idleTimerForSave.Reset();
            _hasChangedSinceLastSave = true;
            _destruction.Play();
            CreateDestructionParticles(t);
            UpdateTilesAround(t.TileIndex);
        }

        private void UpdateTilesAround(int index)
        {
            List<int> indexes = new List<int>();
            int diameterOfSquare = 2 + Brush.Size;
            for (int h = 0; h < diameterOfSquare; h++)
            {
                for (int w = 0; w < diameterOfSquare; w++)
                {
                    int brushSize = Brush.Size;
                    int startingIndex = index - (int)(Math.Truncate((double)(brushSize / 2))) - (int)(Math.Truncate((double)(brushSize / 2)) * _gameWorld.WorldData.LevelWidth);
                    int i = startingIndex - 1 - _gameWorld.WorldData.LevelWidth + (h * _gameWorld.WorldData.LevelWidth) + w;
                    indexes.Add(i);
                }
            }

            foreach (int ind in indexes)
            {
                if (ind >= 0 && ind < _gameWorld.TileArray.Length)
                {
                    Tile t = CurrentArray[ind];
                    t.DefineTexture();
                    t.FindConnectedTextures(CurrentArray,
                    _gameWorld.WorldData.LevelWidth);
                    t.DefineTexture();
                    GameWorld.Instance.LightEngine.UpdateSunLight(ind);
                }
            }

        }

        private void CreateConstructionParticles(Rectangle rect)
        {
            for (int i = 0; i < 10; i++)
            {
                _gameWorld.Particles.Add(new ConstructionSmokeParticle(rect));
            }
        }

        private void CreateDestructionParticles(Tile tile)
        {
            Rectangle[] rects = new Rectangle[16];
            int i = 0;
            for (int w = 0; w < 4; w++)
            {
                for (int h = 0; h < 4; h++)
                {
                    rects[i] = new Rectangle((w * 4) + tile.SourceRectangle.X, (h * 4) + tile.SourceRectangle.Y, 4, 4);
                    i++;
                }
            }

            foreach (Rectangle r in rects)
            {
                _gameWorld.Particles.Add(new DestructionTileParticle(tile, r));
            }
        }

        public void DrawBehindTiles(SpriteBatch spriteBatch)
        {
            Brush.DrawBehind(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Brush.Draw(spriteBatch);
            _selectedPortal?.ConnectingLine?.Draw(spriteBatch);
        }

        public void DrawUi(SpriteBatch spriteBatch)
        {
            if (!OnInventory)
                FontHelper.DrawWithOutline(spriteBatch, ContentHelper.LoadFont("Fonts/x32"), "On Wall Mode: " + OnWallMode, new Vector2(5, 5), 2, Color.Yellow, Color.Black);

            _miniMap.Draw(spriteBatch);
            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/black"), new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.White * _blackScreenOpacity);
            _tileDescription.Draw(spriteBatch);
            _tileScroll.Draw(spriteBatch);
            _entityScroll.Draw(spriteBatch);
            ActionBar.Draw(spriteBatch);
        }

        public Tile[] CurrentArray
        {
            get
            {
                if (OnWallMode)
                {
                    return _gameWorld.WallArray;
                }
                else
                {
                    return _gameWorld.TileArray;
                }

            }
        }
    }

}
