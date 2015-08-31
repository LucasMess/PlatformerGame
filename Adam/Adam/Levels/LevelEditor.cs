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

namespace Adam.Levels
{
    public class LevelEditor
    {
        GameWorld gameWorld;
        TileScroll tileScroll = new TileScroll();
        EntityScroll entityScroll = new EntityScroll();
        Minimap miniMap;
        public ActionBar actionBar = new ActionBar();
        TileDescription tileDescription = new TileDescription();
        public Brush brush = new Brush();
        public bool onInventory;
        public bool onWallMode;
        bool inventoryKeyPressed;
        float blackScreenOpacity;
        bool recentlyChanged;

        public Rectangle editorRectangle;
        public int IndexOfMouse;
        public byte selectedID = 1;

        byte lastUsedTile = 1;
        byte lastUsedWall = 100;

        SoundFx[] construction = new SoundFx[3];
        SoundFx destruction;
        SoundFx wallMode;
        SoundFx close, open, select;
        Rectangle mouseRect;

        public void Load()
        {
            miniMap = new Minimap();
            miniMap.StartUpdating();
            onInventory = false;
            tileScroll.Load();
            tileScroll.TileSelected += TileScroll_TileSelected;

            entityScroll.Load();
            entityScroll.TileSelected += EntityScroll_TileSelected;

            for (int i = 1; i <= construction.Length; i++)
            {
                construction[i - 1] = new SoundFx("Sounds/Level Editor/construct" + i);
            }
            destruction = new SoundFx("Sounds/Level Editor/destroy1");

            wallMode = new SoundFx("Sounds/Level Editor/changeMode");
            close = new SoundFx("Sounds/Level Editor/open");
            open = new SoundFx("Sounds/Level Editor/close");
            select = new SoundFx("Sounds/Level Editor/select");

            editorRectangle = new Rectangle(GameWorld.Instance.worldData.LevelWidth * Main.Tilesize / 2, GameWorld.Instance.worldData.LevelHeight * Main.Tilesize / 2, Main.DefaultResWidth, Main.DefaultResHeight);
        }

        private void EntityScroll_TileSelected(TileSelectedArgs e)
        {
            if (e.ID != selectedID)
            {
                select.Reset();
                select.PlayNewInstanceOnce();
                selectedID = (byte)e.ID;
            }

        }

        private void TileScroll_TileSelected(TileSelectedArgs e)
        {
            if (e.ID != selectedID)
            {
                select.Reset();
                select.PlayNewInstanceOnce();
                selectedID = (byte)e.ID;
            }
        }

        public void Update(GameTime gameTime, GameMode CurrentLevel)
        {
            gameWorld = GameWorld.Instance;
            tileScroll.Update();
            entityScroll.Update();
            actionBar.Update();
            brush.Update();
            tileDescription.Update();

            CheckIfOnInventory();
            CheckIfWantsToSave();
            CheckIfWantsToOpen();
            CheckIfPositioningPlayer();
            CheckIfChangedToWallMode();

            float deltaOpacity = .05f;

            if (!onInventory)
            {
                CheckForCameraMovement();
                CheckForInput();

                blackScreenOpacity -= deltaOpacity;
            }
            else
            {
                blackScreenOpacity += deltaOpacity;
            }

            if (blackScreenOpacity > .7) blackScreenOpacity = .7f;
            if (blackScreenOpacity < 0) blackScreenOpacity = 0;
        }

        public void ChangeToWallMode()
        {
            onWallMode = !onWallMode;
            wallMode.PlayNewInstanceOnce();
            wallMode.Reset();

            if (onWallMode)
            {
                lastUsedTile = selectedID;
                selectedID = lastUsedWall;
            }
            else
            {
                lastUsedWall = selectedID;
                selectedID = lastUsedTile;
            }


            tileScroll.Load();
        }

        private void CheckIfChangedToWallMode()
        {
            if (InputHelper.IsKeyDown(Keys.L) && !recentlyChanged)
            {
                ChangeToWallMode();
                recentlyChanged = true;

            }
            if (InputHelper.IsKeyUp(Keys.L))
            {
                recentlyChanged = false;
            }
        }

        private void CheckIfWantsToSave()
        {
            if (InputHelper.IsKeyDown(Keys.F2))
            {
                GameWorld.Instance.worldData.SaveLevelLocally();
            }
        }

        private void CheckIfWantsToOpen()
        {
            if (InputHelper.IsKeyDown(Keys.F1))
            {
                GameWorld.Instance.worldData.OpenLevelLocally(true);
            }
        }

        private void CheckIfOnInventory()
        {
            if (InputHelper.IsKeyDown(Keys.E))
            {
                if (!inventoryKeyPressed)
                {
                    if (!onInventory)
                    {
                        open.PlayNewInstanceOnce();
                        open.Reset();
                    }
                    else
                    {
                        close.PlayNewInstanceOnce();
                        close.Reset();
                    }
                    onInventory = !onInventory;
                    inventoryKeyPressed = true;
                }
            }
            if (InputHelper.IsKeyUp(Keys.E))
            {
                inventoryKeyPressed = false;
            }
        }

        private void CheckForCameraMovement()
        {
            gameWorld.camera.UpdateSmoothly(editorRectangle, GameWorld.Instance.worldData.LevelWidth, GameWorld.Instance.worldData.LevelHeight, true);
            int speed = 15;

            if (InputHelper.IsKeyDown(Keys.A))
            {
                editorRectangle.X -= speed;
            }
            if (InputHelper.IsKeyDown(Keys.D))
            {
                editorRectangle.X += speed;
            }
            if (InputHelper.IsKeyDown(Keys.W))
            {
                editorRectangle.Y -= speed;
            }
            if (InputHelper.IsKeyDown(Keys.S))
            {
                editorRectangle.Y += speed;
            }


            //Prevent camera box from moving out of screen
            if (editorRectangle.X < 0)
            {
                editorRectangle.X = 0;
            }
            if (editorRectangle.X > (GameWorld.Instance.worldData.LevelWidth * Main.Tilesize) - editorRectangle.Width)
            {
                editorRectangle.X = (GameWorld.Instance.worldData.LevelWidth * Main.Tilesize) - editorRectangle.Width;
            }
            if (editorRectangle.Y < 0)
            {
                editorRectangle.Y = 0;
            }
            if (editorRectangle.Y > (GameWorld.Instance.worldData.LevelHeight * Main.Tilesize) - editorRectangle.Height)
            {
                editorRectangle.Y = (GameWorld.Instance.worldData.LevelHeight * Main.Tilesize) - editorRectangle.Height;
            }
        }

        private void CheckForInput()
        {
            InputHelper.GetMouseRectGameWorld(ref mouseRect);
            IndexOfMouse = (mouseRect.Center.Y / Main.Tilesize * gameWorld.worldData.LevelWidth) + (mouseRect.Center.X / Main.Tilesize);

            if (InputHelper.IsLeftMousePressed())
            {
                UpdateSelectedTiles(selectedID);
            }

            if (InputHelper.IsRightMousePressed())
            {
                UpdateSelectedTiles(0);
            }

            if (InputHelper.IsMiddleMousePressed())
            {
                selectedID = CurrentArray[IndexOfMouse].ID;
            }


        }


        private void CheckIfPositioningPlayer()
        {
            if (InputHelper.IsKeyDown(Keys.P))
            {
                foreach (int index in gameWorld.visibleTileArray)
                {
                    if (index >= 0 && index < gameWorld.tileArray.Length)
                    {
                        //Check index of mouse
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(mouseRect))
                        {
                            gameWorld.tileArray[index].ID = 200;
                            gameWorld.tileArray[index].DefineTexture();
                        }
                    }
                }
            }
        }

        private void UpdateSelectedTiles(int desiredID)
        {
            foreach (int i in brush.selectedIndexes)
            {
                if (i < 0 || i > CurrentArray.Length)
                    continue;
                int tileID = CurrentArray[i].ID;

                //Wants to destroy. Any block can be destroyed.
                if (desiredID == 0)
                {
                    //Check to see if block is already air.
                    if (tileID == 0)
                        continue;
                    else
                    {
                        CurrentArray[i].ID = (byte)desiredID;
                        if (onWallMode)
                        {
                            CurrentArray[i].isWall = true;
                        }
                        else
                            CurrentArray[i].isWall = false;

                        Destroy(CurrentArray[i]);
                    }
                }

                //Wants to build, but only if there is air.
                else
                {
                    if (tileID == 0)
                    {
                        CurrentArray[i].ID = (byte)desiredID;
                        if (onWallMode)
                        {
                            CurrentArray[i].isWall = true;
                        }
                        else
                            CurrentArray[i].isWall = false;
                        Construct(CurrentArray[i]);
                    }
                    else continue;
                }
            }
        }

        private void Construct(Tile t)
        {
            UpdateTilesAround(t.TileIndex);
            construction[GameWorld.RandGen.Next(0, 3)].Play();
            CreateConstructionParticles(t.drawRectangle);
        }

        private void Destroy(Tile t)
        {
            destruction.Play();
            CreateDestructionParticles(t);
            UpdateTilesAround(t.TileIndex);
        }

        private void UpdateTilesAround(int index)
        {
            List<int> indexes = new List<int>();
            int diameterOfSquare = 2 + brush.size;
            for (int h = 0; h < diameterOfSquare; h++)
            {
                for (int w = 0; w < diameterOfSquare; w++)
                {
                    int brushSize = brush.size;
                    int startingIndex = index - (int)(Math.Truncate((double)(brushSize / 2))) - (int)(Math.Truncate((double)(brushSize / 2)) * gameWorld.worldData.LevelWidth);
                    int i = startingIndex - 1 - gameWorld.worldData.LevelWidth + (h * gameWorld.worldData.LevelWidth) + w;
                    indexes.Add(i);
                }
            }

            foreach (int ind in indexes)
            {
                if (ind >= 0 && ind < gameWorld.tileArray.Length)
                {
                    Tile t = CurrentArray[ind];
                    t.DefineTexture();
                    t.FindConnectedTextures(CurrentArray,
                    gameWorld.worldData.LevelWidth);
                    t.DefineTexture();
                    GameWorld.Instance.lightEngine.UpdateSunLight(ind);
                }
            }

        }

        private void CreateConstructionParticles(Rectangle rect)
        {
            for (int i = 0; i < 10; i++)
            {
                gameWorld.particles.Add(new ConstructionSmokeParticle(rect));
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
                    rects[i] = new Rectangle((w * 4) + tile.sourceRectangle.X, (h * 4) + tile.sourceRectangle.Y, 4, 4);
                    i++;
                }
            }

            foreach (Rectangle r in rects)
            {
                gameWorld.particles.Add(new DestructionTileParticle(tile, r));
            }
        }

        public void DrawBehindTiles(SpriteBatch spriteBatch)
        {
            brush.DrawBehind(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            brush.Draw(spriteBatch);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            if (!onInventory)
                FontHelper.DrawWithOutline(spriteBatch, ContentHelper.LoadFont("Fonts/objectiveHead"), "On Wall Mode: " + onWallMode, new Vector2(5, 5), 2, Color.Yellow, Color.Black);

            miniMap.Draw(spriteBatch);
            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/black"), new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.White * blackScreenOpacity);
            tileDescription.Draw(spriteBatch);
            tileScroll.Draw(spriteBatch);
            entityScroll.Draw(spriteBatch);
            actionBar.Draw(spriteBatch);


        }

        public Tile[] CurrentArray
        {
            get
            {
                if (onWallMode)
                {
                    return gameWorld.wallArray;
                }
                else
                {
                    return gameWorld.tileArray;
                }

            }
        }
    }

}
