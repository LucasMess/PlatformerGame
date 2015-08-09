using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Adam.Levels
{
    class LevelEditor
    {
        GameWorld gameWorld;

        public Rectangle editorRectangle;
        public void Update(GameTime gameTime, Level CurrentLevel, Camera camera)
        {
            gameWorld = GameWorld.Instance;

            camera.UpdateSmoothly(editorRectangle, GameWorld.Instance.worldData.mainMap.Width,GameWorld.Instance.worldData.mainMap.Height);

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

            if (InputHelper.IsLeftMousePressed())
            {

            }

            foreach (int index in gameWorld.visibleTileArray)
            {
                if (index >= 0 && index < gameWorld.tileArray.Length)
                {
                    Tile t = gameWorld.tileArray[index];
                    if (gameWorld.tileArray[index].drawRectangle.Intersects(InputHelper.MouseRectangleRenderTarget))
                    {
                        t.ID = 1;
                    }

                    t.DefineTexture();
                    t.FindConnectedTextures(gameWorld.tileArray, gameWorld.worldData.mainMap.Width);
                }


            }
        }
    }
}
