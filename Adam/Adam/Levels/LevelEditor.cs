using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adam.UI;
using Microsoft.Xna.Framework.Graphics;
using Adam.Misc;

namespace Adam.Levels
{
    class LevelEditor
    {
        GameWorld gameWorld;
        TileScroll tileScroll = new TileScroll();
        ActionBar actionBar = new ActionBar();
        public Brush brush = new Brush();
        public bool onInventory;
        public bool onWallMode;
        float blackScreenOpacity;
        bool recentlyChanged;

        public Rectangle editorRectangle;
        public int IndexOfMouse;
        public byte selectedID = 1;

        SoundFx[] construction = new SoundFx[3];
        SoundFx destruction;

        public void Load()
        {
            tileScroll.Load();
            tileScroll.TileSelected += TileScroll_TileSelected;

            for (int i = 1; i <= construction.Length; i++)
            {
                construction[i - 1] = new SoundFx("Sounds/Level Editor/construct" + i);
            }
            destruction = new SoundFx("Sounds/Level Editor/destroy1");


            editorRectangle = new Rectangle(GameWorld.Instance.worldData.width * Game1.Tilesize / 2, GameWorld.Instance.worldData.height * Game1.Tilesize / 2, Game1.DefaultResWidth, Game1.DefaultResHeight);
        }

        private void TileScroll_TileSelected(TileSelectedArgs e)
        {
            selectedID = (byte)e.ID;
        }

        public void Update(GameTime gameTime, GameMode CurrentLevel)
        {
            gameWorld = GameWorld.Instance;
            tileScroll.Update();
            actionBar.Update();
            brush.Update();

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

        private void CheckIfChangedToWallMode()
        {
            if (InputHelper.IsKeyDown(Keys.L) && !recentlyChanged)
            {
                onWallMode = !onWallMode;
                recentlyChanged = true;
                tileScroll.Load();
            }
            if (InputHelper.IsKeyUp(Keys.L))
            {
                recentlyChanged = false;
            }
        }

        private void CheckIfWantsToSave()
        {
            if (InputHelper.IsKeyDown(Keys.F8))
            {
                GameWorld.Instance.worldData.SaveLevelLocally();
            }
        }

        private void CheckIfWantsToOpen()
        {
            if (InputHelper.IsKeyDown(Keys.F9))
            {
                GameWorld.Instance.worldData.OpenLevelLocally();
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
            gameWorld.camera.UpdateSmoothly(editorRectangle, GameWorld.Instance.worldData.width, GameWorld.Instance.worldData.height);
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
            if (editorRectangle.X > (GameWorld.Instance.worldData.width * Game1.Tilesize) - editorRectangle.Width)
            {
                editorRectangle.X = (GameWorld.Instance.worldData.width * Game1.Tilesize) - editorRectangle.Width;
            }
            if (editorRectangle.Y < 0)
            {
                editorRectangle.Y = 0;
            }
            if (editorRectangle.Y > (GameWorld.Instance.worldData.height * Game1.Tilesize) - editorRectangle.Height)
            {
                editorRectangle.Y = (GameWorld.Instance.worldData.height * Game1.Tilesize) - editorRectangle.Height;
            }
        }

        private void CheckForInput()
        {
            foreach (int index in gameWorld.visibleTileArray)
            {
                if (index >= 0 && index < gameWorld.tileArray.Length)
                {
                    //Check index of mouse
                    if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
                    {
                        IndexOfMouse = index;
                    }

                    //Check input
                    Tile t = gameWorld.tileArray[index];
                    if (InputHelper.IsLeftMousePressed())
                    {
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
                        {
                            UpdateSelectedTiles(selectedID);
                        }
                    }

                    if (InputHelper.IsRightMousePressed())
                    {
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld))
                        {
                            UpdateSelectedTiles(0);

                        }
                    }

                    if (InputHelper.IsMiddleMousePressed())
                    {
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld) && t.ID != 0)
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
                        Destroy(CurrentArray[i]);
                    }
                }

                //Wants to build, but only if there is air.
                else
                {
                    if (tileID == 0)
                    {
                        CurrentArray[i].ID = (byte)desiredID;
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
                }
            }

            GameWorld.Instance.lightEngine.UpdateSunLight(index);

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
            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/black"), new Rectangle(0, 0, Game1.UserResWidth, Game1.UserResHeight), Color.White * blackScreenOpacity);
            tileScroll.Draw(spriteBatch);
            actionBar.Draw(spriteBatch);

            spriteBatch.DrawString(ContentHelper.LoadFont("Fonts/objectiveHead"), onWallMode.ToString(), new Vector2(0, 0), Color.Red);
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
