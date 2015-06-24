using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Adam.Obstacles;

namespace Adam
{
    class Tile
    {
        #region Variables
        public Texture2D texture;
        public Rectangle rectangle;
        public Rectangle sourceRectangle;

        public bool isSolid = false;
        public byte ID = 0;
        public byte subID = 0;
        public int TileIndex { get; set; }
        private int mapWidth;
        protected int tilesize;
        public bool isVoid;
        public bool emitsLight;
        Tile[] array;

        #endregion

        public Tile()
        {
            tilesize = Game1.Tilesize / 2;
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
            }
            else
            {
                emitsLight = true;
                return;
            }

            Vector2 position = Vector2.Zero;
            Vector2 startingPoint;

            switch (ID)
            {
                case 1: //Grass
                    startingPoint = new Vector2(0, 0);
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
                    break;
                case 2: //Stone
                    startingPoint = new Vector2(4, 0);
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
                    break;
                case 3: //Marble
                    subID = 4;
                    switch (subID)
                    {
                        case 0: //Plain 1
                            position = new Vector2(12, 4);
                            break;
                        case 1: //Plain 2
                            position = new Vector2(13, 4);
                            break;
                        case 2: //Plain 3
                            position = new Vector2(14, 4);
                            break;
                        case 3: //Plain 4
                            position = new Vector2(15, 4);
                            break;
                        case 4: //Plain 5
                            position = new Vector2(12, 5);
                            break;
                        case 5: //TopColumn
                            position = new Vector2(12, 3);
                            break;
                        case 6: //MiddleColumn
                            position = new Vector2(13, 3);
                            break;
                        case 7: //BottomColumn
                            position = new Vector2(14, 3);
                            break;
                        case 8: //Random 1
                            position = new Vector2(13, 5);
                            break;
                        case 9: //Random 2
                            position = new Vector2(14, 5);
                            break;
                        case 10: //Random 3
                            position = new Vector2(15, 5);
                            break;
                        case 11: //Plain 6
                            position = new Vector2(15, 3);
                            break;
                        case 12: //Shiny one
                            position = new Vector2(12, 5);
                            break;
                    }
                    break;
                case 4: //Hellrock
                    startingPoint = new Vector2(4, 5);
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
                    break;
                case 5: //Sand
                    startingPoint = new Vector2(8, 5);
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
                    break;
                case 6: //vacant
                    break;
                case 7: //ShortGrass
                    emitsLight = true;
                    isVoid = true;
                    break;
                case 8: //Metal
                    isVoid = true;
                    break;
                case 9://Tall Grass
                    isVoid = true;
                    emitsLight = true;
                    break;
                case 10: //Gold
                    startingPoint = new Vector2(0, 5);
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
                    break;
                case 11: //torch
                    isVoid = true;
                    emitsLight = true;
                    break;
                case 12: //Chandelier
                    isVoid = true;
                    emitsLight = true;
                    break;
                case 13: //Door
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
                    isVoid = true;
                    break;
                case 18://Marble Column
                    switch (subID)
                    {
                        case 0:
                            position = new Vector2(13, 3);
                            break;
                        case 1:
                            position = new Vector2(12, 3);
                            break;
                        case 2:
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
                    break;
                case 22: //spikes
                    isVoid = true;
                    break;
                case 24: //lava
                    isVoid = true;
                    break;
                case 26: //apple
                    isVoid = true;
                    break;
                case 27: //golden chest
                    isVoid = true;
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
                    emitsLight = true;
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
                    break;
                case 105://Sand Wall
                    position = new Vector2(15, 9);
                    break;
                #endregion
            }

            //Gets the position in the Vector2 form and converts it to pixel coordinates.
            sourceRectangle = new Rectangle((int)(position.X * tilesize), (int)(position.Y * tilesize), tilesize, tilesize);


        }

        /// <summary>
        /// This updates the animation of the tile.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            //Not used for normal textures, only animated textures.
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            if (!isVoid)
                if (texture != null)
                    spritebatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.Red);
        }

        /// <summary>
        /// This is used for the tiles that have special textures for corners. In the spritesheet they are arranged in the same way. This includes grass, sand, stone, and mesa.
        /// </summary>
        /// <param name="array">The tile array that will be analyzed.</param>
        /// <param name="mapWidth">The width of the map in tiles.</param>
        public void FindConnectedTextures(Tile[] array, int mapWidth)
        {
            //Please don't change this was a headache to make. -Lucas 2015
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
                    int rand = GameWorld.randGen.Next(0, 10);
                    if (rand == 0) //flower
                    {
                        array[indexAbove] = new AnimatedTile(17, array[indexAbove].rectangle);
                    }
                    else if (rand == 1 || rand == 2) //tall grass
                    {
                        array[indexAbove] = new AnimatedTile(9, array[indexAbove].rectangle);
                    }
                    else //short grass
                    {
                        array[indexAbove] = new AnimatedTile(7, array[indexAbove].rectangle);
                    }

                    array[indexAbove].DefineTexture();
                }
            }

            
        }

    }
}
