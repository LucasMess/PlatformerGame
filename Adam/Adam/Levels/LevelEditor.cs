using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adam.UI;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Levels
{
    class LevelEditor
    {
        GameWorld gameWorld;
        TileScroll tileScroll = new TileScroll();

        public Rectangle editorRectangle;
        byte selectedID = 1;

        public void Load()
        {
            tileScroll.Load();
            tileScroll.TileSelected += TileScroll_TileSelected;
        }

        private void TileScroll_TileSelected(TileSelectedArgs e)
        {
            selectedID = (byte)e.ID;
        }

        public void Update(GameTime gameTime, Level CurrentLevel, Camera camera)
        {
            gameWorld = GameWorld.Instance;
            tileScroll.Update();

            camera.UpdateSmoothly(editorRectangle, GameWorld.Instance.worldData.mainMap.Width, GameWorld.Instance.worldData.mainMap.Height);

            if (InputHelper.IsKeyDown(Keys.A))
            {
                editorRectangle.X -= 5;
            }
            if (InputHelper.IsKeyDown(Keys.D))
            {
                editorRectangle.X += 5;
            }
            if (InputHelper.IsKeyDown(Keys.W))
            {
                editorRectangle.Y -= 5;
            }
            if (InputHelper.IsKeyDown(Keys.S))
            {
                editorRectangle.Y += 5;
            }

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
                            CreateConstructionParticles(t.drawRectangle);
                        }
                    }

                    if (InputHelper.IsRightMousePressed())
                    {
                        if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleGameWorld) && t.ID != 0)
                        {
                            t.ID = 0;
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

                    t.DefineTexture();
                    t.FindConnectedTextures(gameWorld.tileArray, gameWorld.worldData.mainMap.Width);
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
            for(int w = 0; w < 4; w++)
            {
                for (int h = 0; h < 4; h++)
                {                   
                    rects[i] = new Rectangle((w * 4)+ tile.sourceRectangle.X, (h * 4) + tile.sourceRectangle.Y, 4, 4);
                    i++;
                }
            }

            foreach(Rectangle r in rects)
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
