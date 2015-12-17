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
using Adam.UI.Elements;
using Adam.Interactables;

namespace Adam
{
    public class Tile
    {
        #region Variables
        public Texture2D texture;
        public Rectangle drawRectangle;
        public Rectangle sourceRectangle;
        Vector2 frameCount;

        public bool IsBrushTile { get; set; }
        public bool isSolid;
        public bool isClimbable;
        public bool isWall;
        public byte ID = 0;
        public byte subID = 0;
        public int TileIndex { get; set; }

        bool hasRandomStartingPoint;
        Rectangle originalPosition;
        Vector2 sizeOfTile;

        private int mapWidth;
        protected const int SmallTileSize = 16;
        public bool sunlightPassesThrough;
        public bool levelEditorTransparency;
        public string name = "";
        Tile[] array;
        public Color color = Color.White;
        float opacity = 1;
        const float DefaultOpacity = 1;
        const float MaxOpacity = .5f;
        bool hasConnectPattern;
        bool hasAddedEntity;
        bool isSampleTile;
        bool animationPlaysOnce;
        Vector2 positionInSpriteSheet;

        int currentFrame;
        double frameTimer;
        double restartTimer;
        double restartWait;
        int switchFrame;
        Rectangle startingRectangle;
        Rectangle startingPosition;

        List<Tile> cornerPieces = new List<Tile>();

        public delegate void TileHandler(Tile t);
        public event TileHandler OnTileUpdate;
        public event TileHandler OnTileDestroyed;
        public event TileHandler OnPlayerInteraction;

        /// <summary>
        /// Constructor used when DefineTexture() will NOT be called.
        /// </summary>
        public Tile() { }

        /// <summary>
        /// Default constructor for game world tiles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Tile(int x, int y)
        {
            originalPosition = new Rectangle(x, y, 0, 0);
            drawRectangle = new Rectangle(x, y, Main.Tilesize, Main.Tilesize);
        }

        /// <summary>
        /// Constructor used when the tile will be used in the UI.
        /// </summary>
        /// <param name="sampleTile"></param>
        public Tile(bool sampleTile)
        {
            isSampleTile = true;
        }




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

            Vector2 startingPoint;

            Destroy();

            #region DefiningTextures
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
                        case 103:
                            positionInSpriteSheet = new Vector2(11, 17);
                            break;
                        case 104:
                            positionInSpriteSheet = new Vector2(10, 17);
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
                    frameCount = new Vector2(4, 0);
                    positionInSpriteSheet = new Vector2(12, 16);
                    sunlightPassesThrough = true;
                    break;
                case 8: //Metal
                    frameCount = new Vector2(4, 0);
                    positionInSpriteSheet = new Vector2(12, 2);
                    isSolid = true;
                    break;
                case 9://Tall Grass
                    frameCount = new Vector2(12, 0);
                    positionInSpriteSheet = new Vector2(0, 16);
                    sunlightPassesThrough = true;
                    break;
                case 10: // Gold.
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(0, 5);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 11: // Torch.
                    frameCount = new Vector2(4, 0);
                    sizeOfTile.Y = 2;
                    positionInSpriteSheet = new Vector2(12, 0);
                    sunlightPassesThrough = true;
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, true, Color.Orange, 2, .6f));
                    break;
                case 12: //Chandelier
                    frameCount = new Vector2(4, 0);
                    sizeOfTile.X = 2;
                    positionInSpriteSheet = new Vector2(0, 17);
                    sunlightPassesThrough = true;
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, true, Color.White, 4, .1f));
                    break;
                case 13: //Door
                    isSolid = true;
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
                    frameCount = new Vector2(4, 0);
                    sizeOfTile.Y = 2;
                    positionInSpriteSheet = new Vector2(12, 10 + GameWorld.RandGen.Next(0, 3) * 2);
                    drawRectangle.Y = originalPosition.Y - Main.Tilesize;
                    hasRandomStartingPoint = true;
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
                    frameCount = new Vector2(4, 0);
                    sizeOfTile.X = 1.5f;
                    sizeOfTile.Y = 2;
                    positionInSpriteSheet = new Vector2(15, 30);
                    animationPlaysOnce = true;
                    drawRectangle.Y = originalPosition.Y - Main.Tilesize;
                    Chest chest = new Chest(this);
                    break;
                case 20:// Marble Brick
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(24,0);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);                    
                    break;
                case 21://scaffolding
                    positionInSpriteSheet = new Vector2(13, 6);
                    isSolid = true;
                    break;
                case 22: //spikes
                    positionInSpriteSheet = new Vector2(17, 13);
                    break;
                case 23://water
                    frameCount = new Vector2(4, 0);
                    hasRandomStartingPoint = true;
                    positionInSpriteSheet = new Vector2(4, 15);
                    
                    if (subID == 1)
                        positionInSpriteSheet = new Vector2(8, 24);
                    new Liquid(this, Liquid.Type.Water);
                    break;
                case 24: //lava
                    frameCount = new Vector2(4, 0);
                    hasRandomStartingPoint = true;
                    positionInSpriteSheet = new Vector2(0, 15);
                    if (subID == 1)
                        positionInSpriteSheet = new Vector2(8, 25);
                    FixedPointLight light = new FixedPointLight(drawRectangle, false, Color.OrangeRed, 3, .3f);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, light);
                    new Liquid(this, Liquid.Type.Lava);
                    break;
                case 25: // Poisoned Water.
                    frameCount = new Vector2(4, 0);
                    hasRandomStartingPoint = true;
                    positionInSpriteSheet = new Vector2(8, 15);
                    break;
                case 26: // Golden Apple.
                    frameCount = new Vector2(4, 0);
                    positionInSpriteSheet = new Vector2(8, 26);
                    break;
                case 27: //golden chest
                    positionInSpriteSheet = new Vector2(15, 3);
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
                case 30: // Vacant.
                    break;
                case 31: //Tree
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.X = 4;
                    sizeOfTile.Y = 6;

                    drawRectangle.Y = originalPosition.Y - (32 * ((int)sizeOfTile.Y - 1));
                    drawRectangle.X = originalPosition.X - (16 * (int)sizeOfTile.X);
                    positionInSpriteSheet = new Vector2(16, 0);
                    break;
                case 32: //Small Rock
                    positionInSpriteSheet = new Vector2(13, 18);
                    break;
                case 33: //Big Rock
                    frameCount = new Vector2(0, 0);
                    sizeOfTile.X = 2;
                    sizeOfTile.Y = 2;
                    drawRectangle.Y = originalPosition.Y - 32;
                    positionInSpriteSheet = new Vector2(14, 17);
                    break;
                case 34: //Medium Rock
                    positionInSpriteSheet = new Vector2(11, 18);
                    break;
                case 36: //Sign
                    positionInSpriteSheet = new Vector2(12, 4);
                    break;
                case 37: //Checkpoint
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(8, 29);
                    }
                    else
                    {
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new CheckPoint(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
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
                    frameCount = new Vector2(8, 0);
                    isSolid = true;
                    positionInSpriteSheet = new Vector2(12, 29);

                    break;
                case 43: // Machine Gun
                    frameCount = new Vector2(8, 0);
                    isSolid = true;
                    positionInSpriteSheet = new Vector2(12, 28);
                    break;
                case 44: // Cacti
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.X = 2;
                    sizeOfTile.Y = 2;
                    switch (GameWorld.RandGen.Next(0, 4))
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
                    positionInSpriteSheet = new Vector2(19, 26);
                    break;
                case 46: // Void ladder.
                    positionInSpriteSheet = new Vector2(14, 8);
                    isClimbable = true;
                    break;
                case 47: // Wooden platform.
                    isSolid = true;
                    positionInSpriteSheet = new Vector2(14, 26);
                    break;
                case 48: // Blue crystal.
                    frameCount = new Vector2(2, 0);
                    positionInSpriteSheet = new Vector2(20, 27);
                    new Crystal(this, 3);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Aqua, 1, .8f));
                    break;
                case 49: // Yellow crystal.
                    frameCount = new Vector2(4, 0);
                    positionInSpriteSheet = new Vector2(20, 29);
                    new Crystal(this, 1);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Yellow, 1, .8f));
                    break;
                case 50: // Green sludge.
                    frameCount = new Vector2(6, 0);
                    positionInSpriteSheet = new Vector2(14, 27);
                    new Crystal(this, 2);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Green, 1, .8f));
                    break;
                case 51: // Void FireSpitter.
                    frameCount = new Vector2(4, 0);
                    positionInSpriteSheet = new Vector2(20, 28);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Red, 1, .8f));
                    break;
                case 52: // Sapphire Crystal.
                    frameCount = new Vector2(1, 0);
                    positionInSpriteSheet = new Vector2(21, 24);
                    new Crystal(this, 3);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Blue, 1, .8f));
                    break;
                case 53: // Ruby Crystal.
                    frameCount = new Vector2(1, 0);
                    positionInSpriteSheet = new Vector2(22, 25);
                    new Crystal(this, 4);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Red, 1, .8f));
                    break;
                case 54: // Emerald Crystal.
                    frameCount = new Vector2(1, 0);
                    positionInSpriteSheet = new Vector2(21, 25);
                    new Crystal(this, 2);
                    GameWorld.Instance.lightEngine.AddFixedLightSource(this, new FixedPointLight(drawRectangle, false, Color.Green, 1, .8f));
                    break;
                case 55: // Skull.
                    positionInSpriteSheet = new Vector2(22, 24);
                    break;
                case 56: // Stalagmite
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.Y = 2;
                    positionInSpriteSheet = new Vector2(23, 24);
                    break;
                case 57: // Mud.
                    hasConnectPattern = true;
                    isSolid = true;
                    startingPoint = new Vector2(4, 29);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 58: // Portal.
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.Y = 3;
                    sizeOfTile.X = 2;
                    drawRectangle.Height = (int)sizeOfTile.Y * Main.Tilesize;
                    drawRectangle.Width = (int)sizeOfTile.X * Main.Tilesize;
                    switch (subID)
                    {
                        case 0:
                            positionInSpriteSheet = new Vector2(8, 30);
                            break;
                    }
                    new Portal(this);
                    break;
                case 59: // Bed.
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.Y = 2;
                    sizeOfTile.X = 3;
                    positionInSpriteSheet = new Vector2(10, 30);
                    break;
                case 60: // Bookshelf.
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.Y = 3;
                    sizeOfTile.X = 2;
                    positionInSpriteSheet = new Vector2(13, 30);
                    break;
                case 61: // Painting.
                    frameCount = new Vector2(1, 0);
                    sizeOfTile.Y = 2;
                    sizeOfTile.X = 2;
                    positionInSpriteSheet = new Vector2(10, 32);
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
                case 105:// Sand Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(4, 24);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 106: //Hellstone Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(0, 24);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 107: //Stone Brick Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(8, 19);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 108: // Mesa Wall
                    hasConnectPattern = true;
                    startingPoint = new Vector2(0, 29);
                    positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 109: // Wallpaper.
                    sunlightPassesThrough = false;
                    switch (subID)
                    {
                        case 0://Plain
                            positionInSpriteSheet = new Vector2(23, 8);
                            break;
                        case 1://Top point
                            positionInSpriteSheet = new Vector2(23, 7);
                            break;
                        case 2://Bottom point
                            positionInSpriteSheet = new Vector2(23, 9);
                            break;
                    }
                    break;
                case 110: // Black.
                    positionInSpriteSheet = new Vector2(13, 9);
                    break;
                #endregion

                case 200: //Player
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                        positionInSpriteSheet = new Vector2(17, 12);
                    else
                    {
                        if (!hasAddedEntity)
                        {
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
                        if (!hasAddedEntity)
                        {
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
                        if (!hasAddedEntity)
                        {
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
                        if (!hasAddedEntity)
                        {
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
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new Hellboar(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 206://Falling Boulder
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(19, 13);
                    }
                    else
                    {
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new FallingBoulder(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 207: //Bat
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(22, 12);
                    }
                    else
                    {
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new Bat(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 208: //Duck
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(22, 13);
                    }
                    else
                    {
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new Duck(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
                case 209: //Flying Wheel
                    sunlightPassesThrough = true;
                    if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                    {
                        positionInSpriteSheet = new Vector2(20, 13);
                    }
                    else
                    {
                        if (!hasAddedEntity)
                        {
                            GameWorld.Instance.entities.Add(new BeingOfSight(drawRectangle.X, drawRectangle.Y));
                            hasAddedEntity = true;
                        }
                    }
                    break;
            }
            #endregion

            DefineSourceRectangle();
            DefineDrawRectangle();
            startingRectangle = sourceRectangle;

            if (hasRandomStartingPoint)
            {
                int randX = GameWorld.RandGen.Next(0, (int)frameCount.X);
                sourceRectangle.X += randX * SmallTileSize;
                currentFrame += randX;
            }

        }

        /// <summary>
        /// Takes all the variables given in DefineTexture method and returns the appropriate source rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineSourceRectangle()
        {
            //return new Rectangle((int)(startingPosition.X * SmallTileSize), (int)(startingPosition.Y * SmallTileSize), (int)(SmallTileSize * sizeOfTile.X), (int)(SmallTileSize * sizeOfTile.Y));
            sourceRectangle = new Rectangle((int)(positionInSpriteSheet.X * SmallTileSize), (int)(positionInSpriteSheet.Y * SmallTileSize), (int)(SmallTileSize * sizeOfTile.X), (int)(SmallTileSize * sizeOfTile.Y));
        }

        /// <summary>
        /// Takes all the variables given in DefineTexture method and returns the appropriate draw rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineDrawRectangle()
        {
            if (isSampleTile)
            {
                int width = (int)(sizeOfTile.X * Main.Tilesize);
                int height = (int)(sizeOfTile.Y * Main.Tilesize);

                if (width > height)
                {
                    width = Main.Tilesize;
                    height = (int)(Main.Tilesize / sizeOfTile.X);
                }
                if (height > width)
                {
                    width = (int)(Main.Tilesize / sizeOfTile.Y);
                    height = Main.Tilesize;
                }
                if (height == width)
                {
                    width = Main.Tilesize;
                    height = Main.Tilesize;
                }
               // Console.WriteLine("Name:{0}, Width:{1}, Height:{2}", name, width, height);
                drawRectangle = new Rectangle(drawRectangle.X, drawRectangle.Y, width, height);

            }
            else drawRectangle = new Rectangle(drawRectangle.X, drawRectangle.Y, (int)(sizeOfTile.X * Main.Tilesize), (int)(sizeOfTile.Y * Main.Tilesize));
        }

        /// <summary>
        /// This updates the animation of the tile.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            if (OnTileUpdate != null)
            {
                OnTileUpdate(this);
            }
            if (OnPlayerInteraction != null&& GameWorld.Instance.GetPlayer().GetCollRectangle().Intersects(drawRectangle) && InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                OnPlayerInteraction(this);

            Animate();
            ChangeOpacity();
        }

        private void Animate()
        {
            if (frameCount.X > 1 && !AnimationStopped)
            {
                switch (ID)
                {
                    case 8://Metal
                        switchFrame = 100;
                        restartWait = 2000;
                        frameTimer += GameWorld.Instance.GetGameTime().ElapsedGameTime.TotalMilliseconds;
                        restartTimer += GameWorld.Instance.GetGameTime().ElapsedGameTime.TotalMilliseconds;

                        if (restartTimer < restartWait)
                            break;
                        if (frameTimer >= switchFrame)
                        {
                            if (frameCount.X != 0)
                            {
                                frameTimer = 0;
                                sourceRectangle.X += SmallTileSize;
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
                    default:
                        DefaultAnimation();
                        break;
                }
            }
        }

        private void DefaultAnimation()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();

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
                if (animationPlaysOnce)
                {
                    currentFrame--;
                    sourceRectangle.X -= sourceRectangle.Width;
                }
                else
                {
                    currentFrame = 0;
                    sourceRectangle.X = startingRectangle.X;
                }
            }
        }

        private void ChangeOpacity()
        {
            if (GameWorld.Instance.levelEditor.onWallMode)
            {
                if (!isWall)
                {
                    opacity -= .05f;

                    if (opacity < MaxOpacity)
                    {
                        opacity = MaxOpacity;
                    }
                }
            }
            else
            {
                opacity += .05f;
                if (opacity > DefaultOpacity)
                {
                    opacity = DefaultOpacity;
                }
            }
        }

        public void Destroy()
        {
            if (OnTileDestroyed != null)
                OnTileDestroyed(this);

            
            sizeOfTile = new Vector2(1, 1);
            DefineDrawRectangle();
            DefineSourceRectangle();

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color * opacity);
            if (hasConnectPattern)
            {
                foreach (Tile c in cornerPieces)
                {
                    if (GameWorld.Instance.levelEditor.onWallMode)
                    {
                        c.opacity = GetOpacity();
                    }
                    else c.opacity = 1;
                    c.Draw(spriteBatch);
                }
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

            // Wallpaper.
            if (ID == 109)
            {
                int indexAbove = TileIndex - mapWidth;
                int indexBelow = TileIndex + mapWidth;
                if (array[indexAbove].ID != 109)
                {
                    subID = 1;
                }
                else if (array[indexBelow].ID != 109)
                {
                    subID = 2;
                }
                else subID = 0;
            }

            //Marble columns
            if (ID == 18)
            {
                int indexAbove = TileIndex - mapWidth;
                int indexBelow = TileIndex + mapWidth;
                if (array[indexAbove].ID != 18)
                {
                    subID = 1;
                }
                else if (array[indexBelow].ID != 18)
                {
                    subID = 2;
                }
                else subID = 0;
            }

            //Marble Floor
            else if (ID == 3)
            {
                if (array[TileIndex - 1].ID != 3)
                    subID = 2;
                else if (array[TileIndex + 1].ID != 3)
                    subID = 1;
                else subID = 0;
            }


            //Marble Ceiling
            else if (ID == 29)
            {
                if (array[TileIndex + 1].ID != 29)
                    subID = 1;
                else if (array[TileIndex - 1].ID != 29)
                    subID = 2;
                else subID = 0;
            }

            //Fences
            else if (ID == 103)
            {
                if (array[TileIndex - mapWidth].ID != 103)
                    subID = 1;
                else subID = 0;
            }

            // Water.
            else if (ID == 23)
            {
                if (array[TileIndex - mapWidth].ID == 0)
                    subID = 1;
                else subID = 0;
            }

            // Lava.
            else if (ID == 24)
            {
                if (array[TileIndex - mapWidth].ID == 0)
                    subID = 1;
                else subID = 0;
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

            // Random decorations for sand.
            if (ID == 5 && subID == 5)
            {
                int indexAbove = TileIndex - mapWidth * 2;
                int indexToRight = TileIndex - mapWidth + 1;
                int indexTopRight = indexAbove + 1;
                if (array[indexAbove].ID == 0 && array[indexToRight].ID == 0 && array[indexTopRight].ID == 0)
                {
                    int rand = GameWorld.RandGen.Next(0, 100);
                    if (rand > 80)
                        array[indexAbove].ID = 44;

                    array[indexAbove].DefineTexture();
                }
            }

            // Random decoration for hellstone.
            if (ID == 4 && subID == 5)
            {
                int indexAbove = TileIndex - mapWidth;
                if (array[indexAbove].ID == 0)
                {
                    int rand = GameWorld.RandGen.Next(0, 10);

                    // Skull.
                    if (rand == 0)
                    {
                        array[indexAbove].ID = 55;
                    }
                    array[indexAbove].DefineTexture();
                }
            }

            // Hellstone stalagmmite.
            if (ID == 4 && subID == 13)
            {
                if (GameWorld.RandGen.Next(0, 5) == 1)
                {
                    int indexBelow = TileIndex + mapWidth;
                    int indexTwoBelow = indexBelow + mapWidth;
                    if (array[indexBelow].ID == 0 && array[indexTwoBelow].ID == 0)
                    {
                        array[indexBelow].ID = 56;
                        array[indexBelow].DefineTexture();
                    }
                }
            }

            // Randomly generate different plain textures for certain tiles.
            // Grass
            if (ID == 1 && subID == 0 && GameWorld.RandGen.Next(0, 100) > 80)
            {
                switch (GameWorld.RandGen.Next(0, 4))
                {
                    case 0:
                        subID = 101;
                        break;
                    case 1:
                        subID = 102;
                        break;
                    case 2:
                        subID = 103;
                        break;
                    case 3:
                        subID = 104;
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

        public float GetOpacity()
        {
            return opacity;
        }

        /// <summary>
        /// Returns true if the animation was specifically told not to run.
        /// </summary>
        public bool AnimationStopped
        {
            get; set;
        }

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
            {20,"Marble Brick" },
            {21,"Scaffolding" },
            {22,"Spikes" },
            {23,"Water" },
            {24,"Lava" },
            {25,"Poison" },
            {26,"Golden Apple" },
            {27,"Golden Chest" },
            {28,"Health Apple" },
            {29,"Marble Ceiling" },
            {30,"" },
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
            {48, "Aquaant Crystal" },
            {49, "Heliaura Crystal" },
            {50, "Sentistract Sludge" },
            {51, "Void Fire Spitter" },
            {52, "Sapphire Crystal" },
            {53, "Ruby Crystal" },
            {54, "Emerald Crystal" },
            {55, "Skull" },
            {56, "Stalagmite" },
            {57, "Mud" },
            {58, "Portal" },
    {59, "Bed" },
    {60, "Bookshelf" },
    {61, "Painting" },



            {100,"Gold Brick Wall" },
            {101,"Stone Wall" },
            {102,"Dirt Wall" },
            {103,"Fence" },
            {104,"Marble Wall" },
            {105,"Sand Wall" },
            {106,"Hellstone Wall" },
            {107,"Stone Brick Wall" },
            {108,"Mesa Wall" },
            {109, "Wallpaper" },
            {110, "Nothing" },

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
