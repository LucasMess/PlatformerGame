using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Adam.Obstacles;
using Adam.Lights;
using Adam.Enemies;
using Adam.Characters.Enemies;
using Adam.Noobs;

namespace Adam
{
    public class Tile
    {
        #region Variables
        public Texture2D texture;
        public Rectangle drawRectangle;
        public Rectangle sourceRectangle;
        SpecialTile specialTile;
        Obstacle obstacle;
        Chest chest;

        public bool isSolid;
        public bool isClimbable;
        public bool isWall;
        public byte ID = 0;
        public byte subID = 0;
        public int TileIndex { get; set; }
        private int mapWidth;
        protected const int SmallTileSize = 16;
        public bool isVoid;
        public bool sunlightPassesThrough;
        public bool levelEditorTransparency;
        public string name = "";
        Tile[] array;
        public Color color = Color.White;
        float opacity = 1;
        const float defaultOpacity = 1;
        const float maxOpacity = .5f;
        bool hasConnectPattern;
        bool hasAddedEntity;
        Vector2 positionInSpriteSheet;

        List<Tile> cornerPieces = new List<Tile>();

        /// <summary>
        /// Returns the tile's texture position in the spritesheet. This needs to be multiplied by 16 to get the coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInSpriteSheet()
        {
            return positionInSpriteSheet;
        }

        public virtual Rectangle GetDrawRectangle()
        {
            return drawRectangle;
        }


        #endregion

        public Tile()
        {
        }

        /// <summary>
        /// After the IDs have been defined, this will give the tile the correct location of its texture in the spritemap.
        /// </summary>
        public virtual void DefineTexture()
        {
            specialTile = null;
            obstacle = null;
            isVoid = false;

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

            positionInSpriteSheet = Vector2.Zero;
            Vector2 startingPoint;

            switch (ID)
            {
                case 1: //Grass
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(0, 0);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    // Random plain tile.
                    switch (subID)
                    {
                        case 101:
                            positionInSpriteSheet = new Vector2(12, 17);
                            break;
                        case 102:
                            positionInSpriteSheet = new Vector2(13, 17);
                            break;
                    }
                    break;
                case 2: //Stone
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(4, 0);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 3: //Marble Floor
                    isSolid = true;
                    switch (subID)
                    {
                        case 0: //Foundation
                            positionInSpriteSheet = new Vector2(14, 5);
                            break;
                        case 1: //Foundation Right
                            positionInSpriteSheet = new Vector2(15, 5);
                            break;
                        case 2: //Foundation Left
                            positionInSpriteSheet = new Vector2(13, 5);
                            break;
                    }
                    break;
                case 4: //Hellrock
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(4, 5);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 5: //Sand
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(8, 0);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 6: //Mesa
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(8, 5);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 7: //ShortGrass
                    positionInSpriteSheet = new Vector2(12, 16);
                    sunlightPassesThrough = true;
                    isVoid = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 8: //Metal
                    positionInSpriteSheet = new Vector2(12, 2);
                    isVoid = true;
                    isSolid = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 9://Tall Grass
                    positionInSpriteSheet = new Vector2(0, 16);
                    isVoid = true;
                    sunlightPassesThrough = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 10: //Gold
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(0, 5);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 11: //torch
                    positionInSpriteSheet = new Vector2(12, 0);
                    isVoid = true;
                    sunlightPassesThrough = true;
                    specialTile = new SpecialTile(this);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, true, Color.Orange, 2,.6f));
                    break;
                case 12: //Chandelier
                    positionInSpriteSheet = new Vector2(0, 17);
                    isVoid = true;
                    sunlightPassesThrough = true;
                    specialTile = new SpecialTile(this);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, true, Color.White, 4,.1f));
                    break;
                case 13: //Door
                    isSolid = true;
                    isVoid = true;
                    break;
                case 14: //Vines
                    positionInSpriteSheet = new Vector2(15, 7);
                    isClimbable = true;
                    break;
                case 15: //Ladders
                    positionInSpriteSheet = new Vector2(13, 7);
                    isClimbable = true;
                    break;
                case 16: //Chains
                    positionInSpriteSheet = new Vector2(14, 7);
                    isClimbable = true;
                    break;
                case 17: //Daffodyls
                    positionInSpriteSheet = new Vector2(12, 10 + GameWorld.RandGen.Next(0,3)*2);
                    specialTile = new SpecialTile(this);
                    isVoid = true;
                    break;
                case 18://Marble Column
                    switch (subID)
                    {
                        case 0: //middle
                            positionInSpriteSheet = new Vector2(13, 3);
                            break;
                        case 1: //top
                            positionInSpriteSheet = new Vector2(12, 3);
                            break;
                        case 2: //bot
                            positionInSpriteSheet = new Vector2(14, 3);
                            break;
                    }
                    break;
                case 19://chest
                    positionInSpriteSheet = new Vector2(12, 24);
                    specialTile = new SpecialTile(this);
                    isVoid = true;
                    break;
                case 20://tech
                    isVoid = true;
                    break;
                case 21://scaffolding
                    positionInSpriteSheet = new Vector2(13, 6);
                    isSolid = true;
                    break;
                case 22: //spikes
                    positionInSpriteSheet = new Vector2(17, 13);
                    obstacle = new Spikes(drawRectangle.X, drawRectangle.Y);
                    break;
                case 23://water
                    positionInSpriteSheet = new Vector2(4, 15);
                    isVoid = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 24: //lava
                    positionInSpriteSheet = new Vector2(0, 15);
                    isVoid = true;
                    specialTile = new SpecialTile(this);
                    FixedPointLight light = new FixedPointLight(drawRectangle, false, Color.OrangeRed, 3,.3f);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, light);
                    break;
                case 25:
                    positionInSpriteSheet = new Vector2(8, 15);
                    isVoid = true;
                    specialTile = new SpecialTile(this);
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
                            positionInSpriteSheet = new Vector2(15, 3);
                            break;
                        case 1: //Right ledge
                            positionInSpriteSheet = new Vector2(15, 4);
                            break;
                        case 2: //Left ledge
                            positionInSpriteSheet = new Vector2(13, 4);
                            break;
                    }

                    break;
                case 30: //Marble ceiling support
                    positionInSpriteSheet = new Vector2(13, 4);
                    isSolid = true;
                    break;
                case 31: //Tree
                    positionInSpriteSheet = new Vector2(18, 4);
                    isVoid = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 32: //Small Rock
                    positionInSpriteSheet = new Vector2(13, 18);
                    break;
                case 33: //Big Rock
                    positionInSpriteSheet = new Vector2(14, 18);
                    isVoid = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 34: //Medium Rock
                    positionInSpriteSheet = new Vector2(11, 18);
                    isVoid = true;
                    specialTile = new SpecialTile(this);
                    break;
                case 36: //Sign
                    positionInSpriteSheet = new Vector2(12, 4);
                    break;
                case 37: //Checkpoint
                    isVoid = true;
                    break;
                case 38: //Stone Brick
                    isSolid = true;
                    startingPoint = new Vector2(0, 10);
                    hasConnectPattern = true;
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 39: //Ice
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(4, 10);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 40: //Snow Covered Grass
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(8, 10);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 41: //Void tile
                    isSolid = true;
                    hasConnectPattern = true;
                    startingPoint = new Vector2(16, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 42: // Flamespitter
                    isSolid = true;
                    positionInSpriteSheet = new Vector2(12, 29);
                    isVoid = true;
                    specialTile = new SpecialTile(this);                    
                    break;
                case 43: // Machine Gun
                    isSolid = true;
                    isVoid = true;
                    positionInSpriteSheet = new Vector2(12,28);
                    specialTile = new SpecialTile(this);
                    break;
                case 44: // Cacti
                    isVoid = true;
                    switch (subID)
                    {
                        case 0: // One branch normal.
                            positionInSpriteSheet = new Vector2(20, 2);
                            break;
                        case 1: // Two branch normal.
                            positionInSpriteSheet = new Vector2(20, 4);
                            break;
                        case 2: // One branch flower.
                            positionInSpriteSheet = new Vector2(22, 2);
                            break;
                        case 3: // Two branch flower.
                            positionInSpriteSheet = new Vector2(22, 4);
                            break;
                    }
                    break;
                case 45: // Mushroom Booster
                    isVoid = true;
                    positionInSpriteSheet = new Vector2(19, 26);
                    specialTile = new SpecialTile(this);
                    break;
                case 46: // Void ladder.
                    positionInSpriteSheet = new Vector2(14, 8);
                    isClimbable = true;
                    break;
                case 47: // Wooden platform.
                    isSolid = true;
                    isVoid = true;
                    positionInSpriteSheet = new Vector2(14, 26);
                    specialTile = new SpecialTile(this);
                    break;
                case 48: // Blue crystal.
                    isVoid = true;
                    positionInSpriteSheet = new Vector2(20, 27);
                    specialTile = new SpecialTile(this);
                    break;
                case 49: // Yellow crystal.
                    isVoid = true;
                    positionInSpriteSheet = new Vector2(20, 29);
                    specialTile = new SpecialTile(this);
                    break;
                case 50: // Green sludge.
                    positionInSpriteSheet = new Vector2(14, 27);
                    specialTile = new SpecialTile(this);
                    break;
                case 51: // Void FireSpitter.
                    isVoid = true;
                    positionInSpriteSheet = new Vector2(20, 28);
                    specialTile = new SpecialTile(this);
                    break;

                #region Wall Textures
                case 100://Gold Brick Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(4, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 101://Stone Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(20, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 102://Dirt Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(0, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 103://Fences
                    sunlightPassesThrough = true;
                    switch (subID)
                    {
                        case 0://Plain
                            positionInSpriteSheet = new Vector2(12, 7);
                            break;
                        case 1://Top point
                            positionInSpriteSheet = new Vector2(12, 6);
                            break;
                    }
                    break;
                case 104://Marble wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(12, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 105://Sand Wall
                    positionInSpriteSheet = new Vector2(15, 9);
                    break;
                case 106: //Hellstone Wall
                    positionInSpriteSheet = new Vector2(14, 9);
                    break;
                case 107: //Stone Brick Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(8, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                #endregion

                case 200: //Player
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                        positionInSpriteSheet = new Vector2(17, 12);
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.game1.player.Initialize(drawRectangle.X, drawRectangle.Y);
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 201: //Snake
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(18, 12);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new Snake(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 202: //Frog
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(21, 12);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new Frog(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 203: //God
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(18, 13);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new God(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 204: //Lost
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(19, 12);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new Lost(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 205: //Hellboar
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(20, 12);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new Hellboar(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 206://Falling Boulder
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(12, 26);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new FallingBoulder(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 207: //Bat
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(12, 8);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new Bat(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 208: //Duck
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(12, 8);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new Duck(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 209: //Flying Wheel
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(12, 8);
                    }
                    else
                    {
                        isVoid = true;
                        if (!hasAddedEntity)
                        {
                            isVoid = true;
                            GameWorld.Instance.entities.Add(new BeingOfSight(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
            }

            //Gets the position in the Vector2 form and converts it to pixel coordinates.
            sourceRectangle = new Rectangle((int)(positionInSpriteSheet.X * SmallTileSize), (int)(positionInSpriteSheet.Y * SmallTileSize), SmallTileSize, SmallTileSize);


        }

        /// <summary>
        /// This updates the animation of the tile.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            specialTile?.Animate(gameTime);
            obstacle?.Update();
            //Not used for normal textures, only animated textures.

            ChangeOpacity();
        }

        private void ChangeOpacity()
        {
            if (GameWorld.Instance.levelEditor.onWallMode)
            {
                if (!isWall)
                {
                    opacity -= .05f;

                    if (opacity < maxOpacity)
                    {
                        opacity = maxOpacity;
                    }
                }
            }
            else
            {
                opacity += .05f;
                if (opacity > defaultOpacity)
                {
                    opacity = defaultOpacity;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (specialTile == null)
            {
                if (!isVoid)
                {
                    if (texture != null)
                        spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color * opacity);
                    if (hasConnectPattern)
                    {
                        foreach (Tile c in cornerPieces)
                        {
                            c.Draw(spriteBatch);
                        }
                    }
                }
            }
            else
            {
                if (texture != null)
                    specialTile.Draw(spriteBatch);
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
            cornerPieces = new List<Tile>();

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

            //Marble Ceiling
            else if (ID == 29)
            {
                if (array[TileIndex + 1].ID != 29)
                    subID = 1;
                if (array[TileIndex - 1].ID != 29)
                    subID = 2;
            }


            //Default Connected Textures Pattern
            //"Please don't change this was a headache to make." -Lucas 2015

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
                subID = 0;

            if (topLeft.ID == mid.ID &&
               top.ID == mid.ID &&
               topRight.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID != mid.ID &&
               bot.ID == mid.ID &&
               botRight.ID == mid.ID)
                subID = 0;

            if (topLeft.ID != mid.ID &&
               top.ID == mid.ID &&
               topRight.ID == mid.ID &&
               midLeft.ID == mid.ID &&
               midRight.ID == mid.ID &&
               botLeft.ID == mid.ID &&
               bot.ID == mid.ID &&
               botRight.ID == mid.ID)
                subID = 0;

            if (top.ID != mid.ID &&
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
                subID = 0;

            if (top.ID == mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID == mid.ID)
                subID = 8;

            if (top.ID != mid.ID &&
               midLeft.ID != mid.ID &&
               midRight.ID != mid.ID &&
               bot.ID != mid.ID)
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

            //Special
            if (botRight.ID != mid.ID &&
               midRight.ID == mid.ID &&
               bot.ID == mid.ID)
            {
                Tile corner = new Tile();
                corner.ID = mid.ID;
                corner.drawRectangle = drawRectangle;
                corner.texture = texture;
                corner.subID = 1;
                cornerPieces.Add(corner);
            }

            if (botLeft.ID != mid.ID &&
               midLeft.ID == mid.ID &&
               bot.ID == mid.ID)
            {
                Tile corner = new Tile();
                corner.ID = mid.ID;
                corner.drawRectangle = drawRectangle;
                corner.texture = texture;
                corner.subID = 2;
                cornerPieces.Add(corner);
            }

            if (topLeft.ID != mid.ID &&
                midLeft.ID == mid.ID &&
                top.ID == mid.ID)
            {
                Tile corner = new Tile();
                corner.ID = mid.ID;
                corner.drawRectangle = drawRectangle;
                corner.texture = texture;
                corner.subID = 3;
                cornerPieces.Add(corner);
            }

            if (topRight.ID != mid.ID &&
               midRight.ID == mid.ID &&
               top.ID == mid.ID)
            {
                Tile corner = new Tile();
                corner.ID = mid.ID;
                corner.drawRectangle = drawRectangle;
                corner.texture = texture;
                corner.subID = 7;
                cornerPieces.Add(corner);
            }

            foreach (Tile corners in cornerPieces)
            {
                corners.DefineTexture();
            }


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
                        array[indexAbove].ID = 17;
                    }
                    else if (rand == 1 || rand == 2) //tall grass
                    {
                        array[indexAbove].ID = 9;
                    }
                    else //short grass
                    {
                        array[indexAbove].ID = 7;
                    }

                    array[indexAbove].DefineTexture();
                }
            }

            // Randomly generate different plain textures for certain tiles.
            // Grass
            if (ID == 1 && subID == 0 && GameWorld.RandGen.Next(0,100) > 80)
            {
                switch (GameWorld.RandGen.Next(0, 2))
                {
                    case 0:
                        subID = 101;
                        break;
                    case 1:
                        subID = 102;
                        break;
                }

                DefineTexture();
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

        public static Dictionary<int, string> Names = new Dictionary<int, string>()
        {
            {1,"Grass" },
            {2,"Stone" },
            {3,"Marble Floor" },
            {4,"Hellrock" },
            {5,"Sand" },
            {6,"Mesa" },
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
            {39,"Snow" },
            {40,"Snowy Grass" },
            {41,"Compressed Void" },
            {42, "Flame Spitter" },
            {43, "Machine Gun" },
            {44, "Cactus" },
            {45, "Mushroom Booster" },
            {46, "Void Ladder" },
            {47, "Wooden Platform" },
            {48, "Blue Crystal" },
            {49, "Yellow Crystal" },
            {50, "Green Sludge" },
            {51, "Void Fire Spitter" },

            {100,"Gold Brick Wall" },
            {101,"Stone Wall" },
            {102,"Dirt Wall" },
            {103,"Fence" },
            {104,"Marble Wall" },
            {105,"Sand Wall" },
            {106,"Hellstone Wall" },
            {107,"Stone Brick Wall" },
            {108,"" },

            {200,"Player" },
            {201,"Snake" },
            {202,"Frog" },
            {203,"God" },
            {204,"Lost" },
            {205,"Hellboar" },
            {206,"Falling Boulder (Desert)" },
            {207,"Bat" },
            {208,"Duck" },
            {209,"Being of Sight" },

        };

    }
}
