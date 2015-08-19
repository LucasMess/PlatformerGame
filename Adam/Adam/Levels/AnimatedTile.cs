using Adam;
using Adam.Lights;
using Adam.Obstacles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{

    public class SpecialTile : Tile
    {
        double frameTimer, restartTimer;
        int restartWait;
        int currentFrame, switchFrame;
        bool hasRandomStartingPoint;
        Vector2 frameCount;
        Vector2 startingPosition;
        Rectangle startingRectangle;
        Rectangle originalPosition;
        Liquid liquid;
        Chest chest;

        public SpecialTile(byte ID, Rectangle drawRectangle)
        {
            texture = GameWorld.SpriteSheet;
            this.ID = ID;
            base.drawRectangle = drawRectangle;
            originalPosition = base.drawRectangle;
            if (GameWorld.Instance != null)
                TileIndex = (int)(base.drawRectangle.Center.Y / Main.Tilesize * GameWorld.Instance.worldData.LevelWidth) + (int)(base.drawRectangle.Center.X / Main.Tilesize);
            smallTileSize = Main.Tilesize / 2;
            DefineTexture();
        }

        public override void DefineTexture()
        {
            startingPosition = new Vector2(0, 0);
            Vector2 size = new Vector2(1, 1);

            switch (ID)
            {
                case 7: //short grass
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12, 16);
                    sunlightPassesThrough = true;
                    break;
                case 9: //tall Grass
                    frameCount = new Vector2(12, 0);
                    startingPosition = new Vector2(0, 16);
                    sunlightPassesThrough = true;
                    break;
                case 8: //Metal
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12, 2);
                    break;
                case 11: //Torch
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12, 0);
                    size.Y = 2;
                    drawRectangle.Height = Main.Tilesize * 2;
                    break;
                case 12: //Chandelier
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(0, 17);
                    size.X = 2;
                    drawRectangle.Width = Main.Tilesize * 2;
                    break;
                case 17: //Daffodyls
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12, 10 + (2 * GameWorld.RandGen.Next(0, 2)));
                    size.Y = 2;
                    drawRectangle.Height = Main.Tilesize * 2;
                    drawRectangle.Y = originalPosition.Y - Main.Tilesize;
                    sunlightPassesThrough = true;
                    break;
                case 19: //Chest
                    chest = new Chest(this);               
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12, 24);
                    size.X = 1.5f;
                    drawRectangle.Width = (int)(Main.Tilesize * size.X);
                    break;
                case 23: //Water
                    liquid = new Liquid(drawRectangle.X, drawRectangle.Y, Liquid.Type.Water);
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(4, 15);
                    hasRandomStartingPoint = true;
                    break;
                case 24: //Lava
                    liquid = new Liquid(drawRectangle.X, drawRectangle.Y, Liquid.Type.Lava);
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(0, 15);
                    hasRandomStartingPoint = true;
                    sunlightPassesThrough = true;
                    break;
                case 25: //Poison
                    liquid = new Liquid(drawRectangle.X, drawRectangle.Y, Liquid.Type.Poison);
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(8, 15);
                    hasRandomStartingPoint = true;
                    break;
                case 31: //Tree
                    frameCount = new Vector2(0, 0);
                    startingPosition = new Vector2(16, 0);
                    size.X = 6;
                    size.Y = 7;
                    drawRectangle.Height = Main.Tilesize * 7;
                    drawRectangle.Width = Main.Tilesize * 6;
                    drawRectangle.Y = originalPosition.Y - (32 * 6);
                    drawRectangle.X = originalPosition.X - (16 * 5);
                    sunlightPassesThrough = true;
                    break;
                case 33: //Big Rock
                    frameCount = new Vector2(0, 0);
                    startingPosition = new Vector2(14, 17);
                    size.X = 2;
                    size.Y = 2;
                    drawRectangle.Height = Main.Tilesize * 2;
                    drawRectangle.Width = Main.Tilesize * 2;
                    drawRectangle.Y = originalPosition.Y - 32;
                    break;
                case 34: //Small Rock
                    frameCount = new Vector2(0, 0);
                    startingPosition = new Vector2(11, 18);
                    size.X = 2;
                    drawRectangle.Width = Main.Tilesize * 2;
                    break;

            }
            sourceRectangle = new Rectangle((int)(startingPosition.X * smallTileSize), (int)(startingPosition.Y * smallTileSize), (int)(smallTileSize * size.X), (int)(smallTileSize * size.Y));
            startingRectangle = sourceRectangle;

            if (hasRandomStartingPoint)
            {
                int randX = GameWorld.RandGen.Next(0, (int)frameCount.X);
                sourceRectangle.X += randX * smallTileSize;
                currentFrame += randX;
            }

        }

        public void Animate(GameTime gameTime)
        {
            liquid?.Update(gameTime);
            chest?.Update();

            switch (ID)
            {
                case 7: //Short grass
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        frameTimer = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        currentFrame++;
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = startingRectangle.X;
                    }
                    break;
                case 9: //Tall grass
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += smallTileSize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                case 8://Metal
                    switchFrame = 100;
                    restartWait = 2000;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    restartTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (restartTimer < restartWait)
                        break;
                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += smallTileSize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 12 * 16;
                        restartTimer = 0;
                    }
                    break;
                case 11: //torch
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += sourceRectangle.Width;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle = startingRectangle;
                    }
                    break;
                case 12: //chandelier
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += sourceRectangle.Width;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle = startingRectangle;
                    }
                    break;
                case 17: //flowers
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        frameTimer = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        currentFrame++;
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = startingRectangle.X;
                    }
                    break;
                case 23://Water
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += smallTileSize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = startingRectangle.X;
                    }
                    break;
                case 24: //Lava
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += smallTileSize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = startingRectangle.X;
                    }
                    break;
                case 25: //Poison
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += smallTileSize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = startingRectangle.X;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White);
        }
    }
}
