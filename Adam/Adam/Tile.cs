using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CodenameAdam
{
    class Tile
    {
        #region Variables
        //General Variables
        public Texture2D texture;
        public Rectangle rectangle, sourceRectangle;

        public bool isSolid = false;
        public int ID = 0;
        public int subID = 0;
        public int tileNumber;
        public int tilesize;
        public int randSeed;

        //Variables for Animation
        int currentFrame, switchFrame;
        double frameTimer;
        Vector2 frameCount;
        #endregion

        public Tile(int tilesize)
        {
            this.tilesize = tilesize;
            rectangle.Width = tilesize;
            rectangle.Height = tilesize;
            sourceRectangle = new Rectangle(0, 0, tilesize, tilesize);
        }

        public void DefineTexture(ContentManager Content)
        {
            switch (ID)
            {
                #region Grass Textures
                case 1: //Grass
                    switch (subID)
                    {
                        case 0: //Grass on Top
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassTop");                            
                            break;
                        case 1: //Fully Dirt
                            texture = Content.Load<Texture2D>("Tiles/Grass/Dirt");
                            break;
                        case 2: //Grass Top Right End
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassTopRight");
                            break;
                        case 3: //Grass Top Left End
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassTopLeft");
                            break;
                        case 4: //Grass Top Right Cont
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassTopRight");
                            break;
                        case 5: //Grass Top Left Cont
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassTopLeft");
                            break;
                        case 6: //Grass Left Inner Corner
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassInnerLeft");
                            break;
                        case 7: //Grass Right Inner Corner
                            texture = Content.Load<Texture2D>("Tiles/Grass/GrassInnerRight");
                            break;
                    }
                    break;
                #endregion
                case 2: //Stone                
                    Random tempRand = new Random(randSeed);
                    switch (tempRand.Next(0,3))
                    {
                        case 0:
                            texture = Content.Load<Texture2D>("Tiles/Stone/StoneA");
                            break;
                        case 1:
                            texture = Content.Load<Texture2D>("Tiles/Stone/StoneB");
                            break;
                        case 2:
                            texture = Content.Load<Texture2D>("Tiles/Stone/StoneC");
                            break;
                        case 3:
                            texture = Content.Load<Texture2D>("Tiles/Stone/StoneD");
                            break;
                    }

                    break;
                case 3: //vacant
                    break;
                case 4: //vacant
                    break;
                case 5: //vacant
                    break;
                case 6: //vacant
                    break;
                case 7: //vacant
                    break;
                #region Metal Textures
                case 8: //Metal
                    texture = Content.Load<Texture2D>("Tiles/Metal/Metal");
                    frameCount = new Vector2(texture.Width / tilesize, texture.Height / tilesize);
                    break;
                #endregion
                #region Tall Grass Textures
                case 9://Tall Grass
                    texture = Content.Load<Texture2D>("Tiles/Decorative/TallGrass");
                    frameCount = new Vector2(texture.Width / tilesize, texture.Height / tilesize);
                    break;
                #endregion
                #region Gold Textures
                case 10://Gold Bricks
                    texture = Content.Load<Texture2D>("Tiles/Gold/GoldBrick");
                    break;
                #endregion
                #region Wall Textures
                case 100://Gold Brick Wall
                    texture = Content.Load<Texture2D>("Tiles/Walls/GoldBrickWall");
                    break;
                case 101://Stone Wall
                    texture = Content.Load<Texture2D>("Tiles/Walls/StoneWall");
                    break;
                case 102://Dirt Wall
                    texture = Content.Load<Texture2D>("Tiles/Walls/DirtWall");
                    break;
                #endregion
            }

        }

        public void Update(GameTime gameTime)
        {
            switch (ID)
            {
                #region Tall Grass Animation
                case 9:
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += texture.Width / (int)frameCount.X;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                #endregion
                #region Metal Animation
                case 8:
                    switchFrame = 120;
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += texture.Width / (int)frameCount.X;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                #endregion
            }


        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        public void GiveSeed(int randSeed)
        {
            this.randSeed = randSeed;
        }
    }
}
