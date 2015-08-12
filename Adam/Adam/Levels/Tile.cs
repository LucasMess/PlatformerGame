﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Adam.Obstacles;

namespace Adam
{
    public class Tile
    {
        #region Variables
        public Texture2D texture;
        public Rectangle drawRectangle;
        public Rectangle sourceRectangle;
        AnimatedTile animatedTile;

        public bool isSolid = false;
        public byte ID = 0;
        public byte subID = 0;
        public int TileIndex { get; set; }
        private int mapWidth;
        protected int smallTileSize = 16;
        public bool isVoid;
        public bool sunlightPassesThrough;
        public string name = "";
        Tile[] array;
        public Color color = Color.White;
        bool hasConnectPattern;


        #endregion

        public Tile()
        {
        }

        /// <summary>
        /// After the IDs have been defined, this will give the tile the correct location of its texture in the spritemap.
        /// </summary>
        public virtual void DefineTexture()
        {
            //Air ID is 0, so it can emit sunlight.
            if (ID != 0)
            {
                texture = GameWorld.SpriteSheet;
                sunlightPassesThrough = false;
            }
            else
            {
                sunlightPassesThrough = true;
                texture = null;
                return;
            }

            Vector2 position = Vector2.Zero;
            Vector2 startingPoint;

            switch (ID)
            {
                case 1: //Grass
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(0, 0);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 2: //Stone
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(4, 0);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 3: //Marble Floor
                    isSolid = true;
                    switch (subID)
                    {
                        case 0: //Foundation
                            position = new Vector2(14, 5);
                            break;
                        case 1: //Foundation Right
                            position = new Vector2(15, 5);
                            break;
                        case 2: //Foundation Left
                            position = new Vector2(13, 5);
                            break;
                    }
                    break;
                case 4: //Hellrock
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(4, 5);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 5: //Sand
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(8, 0);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 6: //Mesa
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(8, 5);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 7: //ShortGrass
                    position = new Vector2(12, 16);
                    sunlightPassesThrough = true;
                    isVoid = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 8: //Metal
                    position = new Vector2(12, 2);
                    isVoid = true;
                    isSolid = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 9://Tall Grass
                    position = new Vector2(0, 16);
                    isVoid = true;
                    sunlightPassesThrough = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 10: //Gold
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(0, 5);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 11: //torch
                    position = new Vector2(12, 0);
                    isVoid = true;
                    sunlightPassesThrough = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 12: //Chandelier
                    position = new Vector2(0, 17);
                    isVoid = true;
                    sunlightPassesThrough = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 13: //Door
                    isSolid = true;
                    isVoid = true;
                    break;
                case 14: //Vines
                    position = new Vector2(15, 7);
                    break;
                case 15: //Ladders
                    position = new Vector2(13, 7);
                    break;
                case 16: //Chains
                    position = new Vector2(14, 7);
                    break;
                case 17: //Daffodyls
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    position = new Vector2(12, 10);
                    isVoid = true;
                    break;
                case 18://Marble Column
                    switch (subID)
                    {
                        case 0: //middle
                            position = new Vector2(13, 3);
                            break;
                        case 1: //top
                            position = new Vector2(12, 3);
                            break;
                        case 2: //bot
                            position = new Vector2(14, 3);
                            break;
                    }
                    break;
                case 19://chest
                    isVoid = true;
                    break;
                case 20://tech
                    isVoid = true;
                    break;
                case 21://scaffolding
                    position = new Vector2(13, 6);
                    isSolid = true;
                    break;
                case 22: //spikes
                    isVoid = true;
                    break;
                case 24: //lava
                    position = new Vector2(0, 15);
                    isVoid = true;
                    isSolid = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 26: //apple
                    isVoid = true;
                    break;
                case 27: //golden chest
                    isVoid = true;
                    break;
                case 29: //Marble ceiling
                    isSolid = true;
                    switch (subID)
                    {
                        case 0: //Plain
                            position = new Vector2(15, 3);
                            break;
                        case 1: //Right ledge
                            position = new Vector2(15, 4);
                            break;
                        case 2: //Left ledge
                            position = new Vector2(13, 4);
                            break;
                    }

                    break;
                case 30: //Marble ceiling support
                    position = new Vector2(13, 4);
                    isSolid = true;
                    break;
                case 31: //Tree
                    position = new Vector2(18, 4);
                    isVoid = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 32: //Small Rock
                    position = new Vector2(13, 18);
                    break;
                case 33: //Big Rock
                    position = new Vector2(14, 18);
                    isVoid = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 34: //Medium Rock
                    position = new Vector2(11, 18);
                    isVoid = true;
                    animatedTile = new AnimatedTile(ID, drawRectangle);
                    break;
                case 36: //Sign
                    position = new Vector2(12, 4);
                    break;
                case 37: //Checkpoint
                    isVoid = true;
                    break;
                case 38: //Stone Brick
                    isSolid = true;
                    startingPoint = new Vector2(0, 10);
                    hasConnectPattern = true;
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 39: //Ice
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(4, 10);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 40: //Snow Covered Grass
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(8, 10);
                    position = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;

                #region Wall Textures
                case 100://Gold Brick Wall
                    position = new Vector2(12, 8);
                    break;
                case 101://Stone Wall
                    position = new Vector2(13, 8);
                    break;
                case 102://Dirt Wall
                    position = new Vector2(14, 8);
                    break;
                case 103://Fences
                    sunlightPassesThrough = true;
                    switch (subID)
                    {
                        case 0://Plain
                            position = new Vector2(12, 7);
                            break;
                        case 1://Top point
                            position = new Vector2(12, 6);
                            break;
                    }
                    break;
                case 104://Marble wall
                    switch (subID)
                    {
                        case 0: //Plain
                            position = new Vector2(13, 9);
                            break;
                        case 1: //Right Edge
                            position = new Vector2(12, 9);
                            break;
                        case 2: //Left Edge
                            position = new Vector2(14, 4);
                            break;
                    }

                    break;
                case 105://Sand Wall
                    position = new Vector2(15, 9);
                    break;
                #endregion

                case 200: //Player Spawn
                    if (GameWorld.Instance.CurrentLevel == GameMode.Editor)
                        position = new Vector2(17, 12);
                    else
                    {
                        GameWorld.Instance.game1.player.Initialize(drawRectangle.X, drawRectangle.Y);
                    }
                    break;
            }

            //Gets the position in the Vector2 form and converts it to pixel coordinates.
            sourceRectangle = new Rectangle((int)(position.X * smallTileSize), (int)(position.Y * smallTileSize), smallTileSize, smallTileSize);


        }

        /// <summary>
        /// This updates the animation of the tile.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            animatedTile?.Animate(gameTime);
            //Not used for normal textures, only animated textures.
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (animatedTile == null)
            {
                if (texture != null)
                    spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color);
            }
            else
            {
                if (texture != null)
                    animatedTile.Draw(spriteBatch);
            }
        }

        public void DrawByForce(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color);
        }

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.Red);
        }

        /// <summary>
        /// This is used for the tiles that have special textures for corners. In the spritesheet they are arranged in the same way. This includes grass, sand, stone, and mesa.
        /// </summary>
        /// <param name="array">The tile array that will be analyzed.</param>
        /// <param name="mapWidth">The width of the map in tiles.</param>
        public void FindConnectedTextures(Tile[] array, int mapWidth)
        {
            //Marble columns
            if (ID == 18)
            {
                int indexAbove = TileIndex - mapWidth;
                int indexBelow = TileIndex + mapWidth;
                if (array[indexAbove].ID != 18 && array[indexAbove].ID != 0)
                {
                    subID = 1;
                }
                else if (array[indexBelow].ID != 18 && array[indexBelow].ID != 0)
                {
                    subID = 2;
                }
                else subID = 0;
            }

            //Marble Floor
            else if (ID == 3 && subID == 0)
            {
                if (array[TileIndex - 1].ID != 3)
                    subID = 2;
                else if (array[TileIndex + 1].ID != 3)
                    subID = 1;
                else subID = 0;
            }

            //Fences
            else if (ID == 103 && array[TileIndex - mapWidth].ID != 103)
            {
                subID = 1;
            }

            //Marble wall
            else if (ID == 104)
            {
                if (array[TileIndex + 1].ID != 104)
                    subID = 1;
                if (array[TileIndex - 1].ID != 104)
                    subID = 2;
            }

            //Marble Ceiling
            else if (ID == 29)
            {
                if (array[TileIndex + 1].ID != 29)
                    subID = 1;
                if (array[TileIndex - 1].ID != 29)
                    subID = 2;
            }


            //Default Connected Textures Pattern
            //Please don't change this was a headache to make. -Lucas 2015

            if (!hasConnectPattern)
                return;


            this.mapWidth = mapWidth;
            this.array = array;

            int m = TileIndex;
            int t = m - mapWidth;
            int b = m + mapWidth;
            int tl = t - 1;
            int tr = t + 1;
            int ml = m - 1;
            int mr = m + 1;
            int bl = b - 1;
            int br = b + 1;

            if (br >= array.Length || tl < 0)
                return;

            Tile topLeft = array[tl];
            Tile top = array[t];
            Tile topRight = array[tr];
            Tile midLeft = array[ml];
            Tile mid = array[m];
            Tile midRight = array[mr];
            Tile botLeft = array[bl];
            Tile bot = array[b];
            Tile botRight = array[br];

            if (topLeft.ID == mid.ID &&
               top.ID == mid.ID &&
               topRight.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID == mid.ID &&
               bot.ID == mid.ID &&
               botRight.ID == mid.ID)
                subID = 0;

            if (topLeft.ID == mid.ID &&
                top.ID == mid.ID &&
                topRight.ID == mid.ID &&
                midLeft.ID == mid.ID &&
                midRight.ID == mid.ID &&
                botLeft.ID == mid.ID &&
                bot.ID == mid.ID &&
                botRight.ID != mid.ID)
                subID = 1;

            if (topLeft.ID == mid.ID &&
               top.ID == mid.ID &&
               topRight.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID != mid.ID &&
               bot.ID == mid.ID &&
               botRight.ID == mid.ID)
                subID = 2;

            if (topLeft.ID != mid.ID &&
               top.ID == mid.ID &&
               topRight.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID == mid.ID &&
               bot.ID == mid.ID &&
               botRight.ID == mid.ID)
                subID = 3;

            if (topLeft.ID != mid.ID &&
               top.ID != mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID == mid.ID)
                subID = 4;

            if (top.ID != mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID == mid.ID)
                subID = 5;

            if (top.ID != mid.ID &&
               topRight.ID != mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID == mid.ID)
                subID = 6;

            if (topLeft.ID == mid.ID &&
               top.ID == mid.ID &&
               topRight.ID != mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID == mid.ID &&
               bot.ID == mid.ID &&
               botRight.ID == mid.ID)
                subID = 7;

            if (top.ID == mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID == mid.ID)
                subID = 8;

            if (topLeft.ID != mid.ID &&
               top.ID != mid.ID &&
               topRight.ID != mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID != mid.ID &&
               botLeft.ID != mid.ID &&
               bot.ID != mid.ID &&
               botRight.ID != mid.ID)
                subID = 9;

            if (top.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID == mid.ID)
                subID = 10;

            if (top.ID != mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID == mid.ID)
                subID = 11;

            if (top.ID == mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID != mid.ID &&
               bot.ID != mid.ID)
                subID = 12;

            if (top.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID != mid.ID)
                subID = 13;

            if (top.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID != mid.ID)
                subID = 14;

            if (top.ID == mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID == mid.ID)
                subID = 15;

            if (top.ID != mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID != mid.ID)
                subID = 16;

            if (top.ID != mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID != mid.ID)
                subID = 17;

            if (top.ID != mid.ID &&
             midLeft.ID == mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID != mid.ID)
                subID = 18;

            if (top.ID == mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID != mid.ID)
                subID = 19;

        }


        public void AddRandomlyGeneratedDecoration(Tile[] array, int mapWidth)
        {
            //Add decoration on top of grass tile.
            if (ID == 1 && subID == 5)
            {
                int indexAbove = TileIndex - mapWidth;
                if (array[indexAbove].ID == 0)
                {
                    int rand = GameWorld.RandGen.Next(0, 10);
                    if (rand == 0) //flower
                    {
                        array[indexAbove] = new AnimatedTile(17, array[indexAbove].drawRectangle);
                    }
                    else if (rand == 1 || rand == 2) //tall grass
                    {
                        array[indexAbove] = new AnimatedTile(9, array[indexAbove].drawRectangle);
                    }
                    else //short grass
                    {
                        array[indexAbove] = new AnimatedTile(7, array[indexAbove].drawRectangle);
                    }

                    array[indexAbove].DefineTexture();
                }
            }
        }

        private Vector2 GetPositionInSpriteSheetOfConnectedTextures(Vector2 startingPoint)
        {
            Vector2 position = new Vector2();
            switch (subID)
            {
                case 0: //Dirt
                    position = startingPoint + new Vector2(0, 0);
                    break;
                case 1: //Inner bot right corner
                    position = startingPoint + new Vector2(1, 0);
                    break;
                case 2: //Inner bot left corner
                    position = startingPoint + new Vector2(2, 0);
                    break;
                case 3: //Inner top left corner
                    position = startingPoint + new Vector2(3, 0);
                    break;
                case 4: //Top left corner
                    position = startingPoint + new Vector2(0, 1);
                    break;
                case 5: //Top
                    position = startingPoint + new Vector2(1, 1);
                    break;
                case 6: //Top right corner
                    position = startingPoint + new Vector2(2, 1);
                    break;
                case 7: //Inner top right corner
                    position = startingPoint + new Vector2(3, 1);
                    break;
                case 8: //Left
                    position = startingPoint + new Vector2(0, 2);
                    break;
                case 9: //Middle
                    position = startingPoint + new Vector2(1, 2);
                    break;
                case 10: //Right
                    position = startingPoint + new Vector2(2, 2);
                    break;
                case 11: //Top vertical
                    position = startingPoint + new Vector2(3, 2);
                    break;
                case 12: //Bot left corner
                    position = startingPoint + new Vector2(0, 3);
                    break;
                case 13: //Bot
                    position = startingPoint + new Vector2(1, 3);
                    break;
                case 14: //Bot right corner
                    position = startingPoint + new Vector2(2, 3);
                    break;
                case 15: //Middle vertical
                    position = startingPoint + new Vector2(3, 3);
                    break;
                case 16: //Left horizontal
                    position = startingPoint + new Vector2(0, 4);
                    break;
                case 17: //Middle horizontal
                    position = startingPoint + new Vector2(1, 4);
                    break;
                case 18: //Right horizontal
                    position = startingPoint + new Vector2(2, 4);
                    break;
                case 19: //Bot vertical
                    position = startingPoint + new Vector2(3, 4);
                    break;
            }
            return position;
        }

        public static Dictionary<int, Vector2> PositionInSpriteMap = new Dictionary<int, Vector2>()
        {
            {1,new Vector2(0,0) },
            {2,new Vector2(4,0) },
            {3,new Vector2(0,0) },
            {4,new Vector2(0,0) },
            {5,new Vector2(0,0) },
            {6,new Vector2(0,0) },
            {7,new Vector2(0,0) },
            {8,new Vector2(0,0) },
            {9,new Vector2(0,0) },
            {10,new Vector2(0,0) },
            {11,new Vector2(0,0) },
            {12,new Vector2(0,0) },
            {13,new Vector2(0,0) },
            {14,new Vector2(0,0) },
            {15,new Vector2(0,0) },
            {16,new Vector2(0,0) },
            {17,new Vector2(0,0) },
            {18,new Vector2(0,0) },
            {19,new Vector2(0,0) },
            {20,new Vector2(0,0) },
            {21,new Vector2(0,0) },
            {22,new Vector2(0,0) },
            {23,new Vector2(0,0) },
            {24,new Vector2(0,0) },
            {25,new Vector2(0,0) },
            {26,new Vector2(0,0) },
            {27,new Vector2(0,0) },
            {28,new Vector2(0,0) },
            {29,new Vector2(0,0) },
            {30,new Vector2(0,0) },
            {31,new Vector2(0,0) },
            {32,new Vector2(0,0) },
            {33,new Vector2(0,0) },
            {34,new Vector2(0,0) },
            {35,new Vector2(0,0) },
            {36,new Vector2(0,0) },
            {37,new Vector2(0,0) },
            {38,new Vector2(0,0) },
            {39,new Vector2(0,0) },


        };

        public static Dictionary<int, Color> ColorCodes = new Dictionary<int, Color>()
        {
            {1,new Color(0,189,31) },
            {2,new Color(220,220,220) },
            {3,new Color(255,255,255) },
            {4,new Color(0,189,31) },
            {5,new Color(0,189,31) },
            {6,new Color(0,189,31) },
            {7,new Color(0,189,31) },
            {8,new Color(0,189,31) },
            {9,new Color(0,189,31) },
            {10,new Color(0,189,31) },
            {11,new Color(0,189,31) },
            {12,new Color(0,189,31) },
            {13,new Color(0,189,31) },
            {14,new Color(0,189,31) },
            {15,new Color(0,189,31) },
            {16,new Color(0,189,31) },
        };

        public static Dictionary<int, string> TileNames = new Dictionary<int, string>()
        {
            {1,"Grass" },
            {2,"Stone" },
            {3,"Marble Floor" },
            {4,"Hellrock" },
            {5,"Sand" },
            {6,"*" },
            {7,"Short Grass" },
            {8,"Metal" },
            {9,"Tall Grass" },
            {10,"Gold Brick" },
            {11,"Torch" },
            {12,"Chandelier" },
            {13,"Door" },
            {14,"Vine" },
            {15,"Ladder" },
            {16,"Chain" },
            {17,"Flower" },
            {18,"Marble Column" },
            {19,"Chest" },
            {20,"Tech" },
            {21,"Scaffolding" },
            {22,"Spikes" },
            {23,"Water" },
            {24,"Lava" },
            {25,"Poison" },
            {26,"Golden Apple" },
            {27,"Golden Chest" },
            {28,"Health Apple" },
            {29,"Marble Ceiling" },
            {30,"Marble Ceiling Support" },
            {31,"Tree" },
            {32,"Small Rock" },
            {33,"Big Rock" },
            {34,"Medium Rock" },
            {35,"Pebbles" },
            {36,"Sign" },
            {37,"Checkpoint" },
            {38,"Stone Brick" },
            {39,"Ice" },
            {40,"Snow" },

        };

    }
}
