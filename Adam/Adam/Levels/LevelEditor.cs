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

        public Rectangle editorRectangle;
        double updateTimer;
        byte selectedID = 1;

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


            editorRectangle = new Rectangle(GameWorld.Instance.worldData.mainMap.Width * Game1.Tilesize / 2, GameWorld.Instance.worldData.mainMap.Height * Game1.Tilesize / 2, Game1.DefaultResWidth, Game1.DefaultResHeight);
        }

        private void TileScroll_TileSelected(TileSelectedArgs e)
        {
            selectedID = (byte)e.ID;
        }

        public void Update(GameTime gameTime, Level CurrentLevel)
        {
            gameWorld = GameWorld.Instance;
            tileScroll.Update();

            CheckForCameraMovement();
            CheckForInput();
            CheckForNewTiles();
        }

        private void CheckForCameraMovement()
        {
            gameWorld.camera.UpdateSmoothly(editorRectangle, GameWorld.Instance.worldData.mainMap.Width, GameWorld.Instance.worldData.mainMap.Height);
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

            if (editorRectangle.X < 0)
            {
                editorRectangle.X = 0;
            }
            if (editorRectangle.X > (GameWorld.Instance.worldData.mainMap.Width * Game1.Tilesize) - editorRectangle.Width)
            {
                editorRectangle.X = (GameWorld.Instance.worldData.mainMap.Width * Game1.Tilesize) - editorRectangle.Width;
            }
            if (editorRectangle.Y < 0)
            {
                editorRectangle.Y = 0;
            }
            if (editorRectangle.Y > (GameWorld.Instance.worldData.mainMap.Height * Game1.Tilesize) - editorRectangle.Height)
            {
                editorRectangle.Y = (GameWorld.Instance.worldData.mainMap.Height * Game1.Tilesize) - editorRectangle.Height;
            }
        }

        private void CheckForInput()
        {

            foreach (int index in gameWorld.visibleTileArray)
            {
                if (index >= 0 && index < gameWorld.tileArray.Length)
                {
                    Tile t = gameWorld.tileArray[index];
                    if (InputHelper.IsLeftMousePressed())
                    {
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld) && t.ID == 0)
                        {
                            t.ID = selectedID;
                            construction[GameWorld.RandGen.Next(0, 3)].Play();
                            CreateConstructionParticles(t.drawRectangle);
                        }
                    }

                    if (InputHelper.IsRightMousePressed())
                    {
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld) && t.ID != 0)
                        {
                            t.ID = 0;
                            destruction.Play();
                            CreateDestructionParticles(t);
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

        private void CheckForNewTiles()
        {
            updateTimer += gameWorld.gameTime.ElapsedGameTime.TotalSeconds;
            if (updateTimer > .1)
            {
                foreach (int index in gameWorld.visibleTileArray)
                {
                    if (index >= 0 && index < gameWorld.tileArray.Length)
                    {
                        Tile t = gameWorld.tileArray[index];
                        t.DefineTexture();
                        t.FindConnectedTextures(gameWorld.tileArray,
                        gameWorld.worldData.mainMap.Width);
                        //t.AddRandomlyGeneratedDecoration(gameWorld.tileArray, gameWorld.worldData.mainMap.Width);
                    }
                }
                updateTimer = 0;

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

        public void Draw(SpriteBatch spriteBatch)
        {
            tileScroll.Draw(spriteBatch);
        }
    }

}
