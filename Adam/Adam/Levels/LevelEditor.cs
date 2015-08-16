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
        ActionBar actionBar = new ActionBar();
        TileDescription tileDescription = new TileDescription();
        public Brush brush = new Brush();
        public bool onInventory;
        public bool onWallMode;
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

        public void Load()
        {

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

            editorRectangle = new Rectangle(GameWorld.Instance.worldData.width * Main.Tilesize / 2, GameWorld.Instance.worldData.height * Main.Tilesize / 2, Main.DefaultResWidth, Main.DefaultResHeight);
        }

        private void EntityScroll_TileSelected(TileSelectedArgs e)
        {
            selectedID = (byte)e.ID;
        }

        private void TileScroll_TileSelected(TileSelectedArgs e)
        {
            selectedID = (byte)e.ID;
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
            if (InputHelper.IsKeyDown(Keys.Tab))
            {
                onInventory = true;
            }
            else onInventory = false;
        }

        private void CheckForCameraMovement()
        {
            gameWorld.camera.UpdateSmoothly(editorRectangle, GameWorld.Instance.worldData.width, GameWorld.Instance.worldData.height, true);
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
            if (editorRectangle.X > (GameWorld.Instance.worldData.width * Main.Tilesize) - editorRectangle.Width)
            {
                editorRectangle.X = (GameWorld.Instance.worldData.width * Main.Tilesize) - editorRectangle.Width;
            }
            if (editorRectangle.Y < 0)
            {
                editorRectangle.Y = 0;
            }
            if (editorRectangle.Y > (GameWorld.Instance.worldData.height * Main.Tilesize) - editorRectangle.Height)
            {
                editorRectangle.Y = (GameWorld.Instance.worldData.height * Main.Tilesize) - editorRectangle.Height;
            }
        }

        private void CheckForInput()
        {
            foreach (int index in gameWorld.visibleTileArray)
            {
                if (index >= 0 && index < gameWorld.tileArray.Length)
                {
                    if (CurrentArray[index] == null) return;

                    //Check index of mouse
                    if (CurrentArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
                    {
                        IndexOfMouse = index;
                    }

                    //Prevent building and destroying fast bug
                    if (InputHelper.IsRightMousePressed() && InputHelper.IsLeftMousePressed())
                        continue;

                    //Check input
                    Tile t = CurrentArray[index];
                    if (InputHelper.IsLeftMousePressed())
                    {
                        if (t.drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
                        {
                            UpdateSelectedTiles(selectedID);
                        }
                    }

                    if (InputHelper.IsRightMousePressed())
                    {
                        if (t.drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
                        {
                            UpdateSelectedTiles(0);

                        }
                    }

                    if (InputHelper.IsMiddleMousePressed())
                    {
                        if (t.drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld) && t.ID != 0)
                        {
                            selectedID = t.ID;
                        }
                    }

                }
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
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
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
                    int startingIndex = index - (int)(Math.Truncate((double)(brushSize / 2))) - (int)(Math.Truncate((double)(brushSize / 2)) * gameWorld.worldData.width);
                    int i = startingIndex - 1 - gameWorld.worldData.width + (h * gameWorld.worldData.width) + w;
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
                    gameWorld.worldData.width);
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
