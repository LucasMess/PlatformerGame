﻿using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{

    class AnimatedTile : Tile
    {
        double frameTimer, restartTimer;
        int restartWait;
        int currentFrame, switchFrame;
        bool hasRandomStartingPoint;
        Vector2 frameCount;
        Vector2 startingPosition;
        Rectangle startingRectangle;

        public AnimatedTile(int ID, Rectangle rectangle)
        {
            this.ID = ID;
            this.rectangle = rectangle;
            this.tilesize = Game1.TILESIZE / 2;
        }

        public override void DefineTexture(ContentManager Content)
        {
            startingPosition = new Vector2(0, 0);
            Vector2 size =  new Vector2(1,1);
            texture = Content.Load<Texture2D>("Tiles/Spritemaps/spritemap_9");
            switch (ID)
            {
                case 8: //Metal
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12, 2);
                    break;
                case 9: //tall Grass
                    frameCount = new Vector2(12, 0);
                    startingPosition = new Vector2(0, 11);
                    break;
                case 11: //Torch
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(12,0);
                    size.Y = 2;
                    rectangle.Height = Game1.TILESIZE * 2;
                    break;
                case 12: //Chandelier
                    frameCount = new Vector2(0, 0);
                    startingPosition = new Vector2(14, 6);
                    break;
                case 24: //Lava
                    frameCount = new Vector2(4, 0);
                    startingPosition = new Vector2(0, 15);
                    hasRandomStartingPoint = true;
                    break;
            }
            sourceRectangle = new Rectangle((int)(startingPosition.X * tilesize), (int)(startingPosition.Y * tilesize), (int)(tilesize*size.X), (int)(tilesize*size.Y));
            startingRectangle = sourceRectangle;

            if (hasRandomStartingPoint)
            {
                int randX = Map.randGen.Next(0, (int)frameCount.X);
                sourceRectangle.X += randX * tilesize;
                currentFrame += randX;
            }

        }

        public override void Update(GameTime gameTime)
        {
            switch (ID)
            {
                case 7: //something
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += tilesize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
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
                            sourceRectangle.X += tilesize;
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
                            sourceRectangle.X += tilesize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 12*16;
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
                            sourceRectangle.X += tilesize;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 12*16;
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
                            sourceRectangle.X += tilesize;
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
            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }
    }
}
