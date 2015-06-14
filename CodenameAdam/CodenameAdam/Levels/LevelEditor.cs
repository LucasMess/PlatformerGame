using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class LevelEditor
    {
        int width;
        int height;
        int currentTilePos;

        Tile[] tileArray;
        Texture selectedTileTexture;
        Rectangle selectedTileSourceRect;

        public Vector2 cameraPos;
        bool hasLoaded;
        Cursor cursor;
        ContentManager Content;

        public void Initialize()
        {
            width = 500;
            height = 500;
            cursor = new Cursor();
            cameraPos = new Vector2(0, 0);

            tileArray = new Tile[width * height];
        }

        public void Load(ContentManager Content)
        {
            cursor.Load(Content);
            this.Content = Content;

            selectedTileTexture = Content.Load<Texture2D>("Tiles/spritemap_3");

            int currentTile = 0;
            for (int h = 0; h < width; h++)
            {
                for (int w = 0; w < height; w++)
                {
                    tileArray[currentTile] = new Tile();
                    tileArray[currentTile].TileIndex = currentTile;
                    tileArray[currentTile].ID = 1;
                    currentTile++;
                }
            }

            foreach (var t in tileArray)
            {
                t.DefineTexture(Content);
            }
        }

        public void Update(GameTime gameTime)
        {
            cursor.Update(gameTime, cameraPos);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPos.Y -= 5f;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPos.X -= 5f;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPos.X += 5f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPos.Y += 5f;

            currentTilePos = (int)(cursor.positionOnGame.Y / Game1.Tilesize) * width + (int)(cursor.positionOnGame.X / Game1.Tilesize);

            selectedTileSourceRect = cursor.sourceRectangle;

            if (currentTilePos >= 0 && currentTilePos < tileArray.Length)
            {
                Tile tile = tileArray[currentTilePos];
                if (cursor.isPressed && tile.hasTexture == false)
                {
                    tile.texture = cursor.texture;
                    tile.sourceRectangle = cursor.sourceRectangle;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < tileArray.Length; i++)
            {

                tileArray[i].Draw(spriteBatch);

            }
            cursor.Draw(spriteBatch);

            if (currentTilePos >= 0 && currentTilePos < tileArray.Length)
                tileArray[currentTilePos].DebugDraw(spriteBatch);


        }


        public void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Content.Load<SpriteFont>("debug"), "On Game Position:" + cursor.positionOnGame.X + "," + cursor.positionOnGame.Y, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("debug"), "On Screen Position:" + cursor.positionOnScreen.X + "," + cursor.positionOnScreen.Y, new Vector2(0, 20), Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("debug"), "Camera Position:" + cameraPos.X + "," + cameraPos.Y, new Vector2(0, 40), Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("debug"), "Tile Pos:" + currentTilePos, new Vector2(0, 60), Color.White);
        }


    }
}
