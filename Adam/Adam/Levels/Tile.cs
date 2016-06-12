using System.Collections.Generic;
using Adam.Characters;
using Adam.Characters.Enemies;
using Adam.Interactables;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam
{
    public sealed class Tile
    {
        public delegate void TileHandler(Tile t);

        private const int SmallTileSize = 32;
        private const float DefaultOpacity = 1;
        private const float MaxOpacity = .5f;

        public static Dictionary<int, string> Names = new Dictionary<int, string>
        {
            {1, "Grass"},
            {2, "Stone"},
            {3, "Marble Floor"},
            {4, "Hellrock"},
            {5, "Sand"},
            {6, "Mesa"},
            {7, "Short Grass"},
            {8, "Metal"},
            {9, "Tall Grass"},
            {10, "Gold Brick"},
            {11, "Torch"},
            {12, "Chandelier"},
            {13, "Door"},
            {14, "Vine"},
            {15, "Ladder"},
            {16, "Chain"},
            {17, "Flower"},
            {18, "Marble Column"},
            {19, "Chest"},
            {20, "Marble Brick"},
            {21, "Scaffolding"},
            {22, "Spikes"},
            {23, "Water"},
            {24, "Lava"},
            {25, "Poison"},
            {26, "Golden Apple"},
            {27, "Golden Chest"},
            {28, "Health Apple"},
            {29, "Marble Ceiling"},
            {30, ""},
            {31, "Tree"},
            {32, "Small Rock"},
            {33, "Big Rock"},
            {34, "Medium Rock"},
            {35, "Pebbles"},
            {36, "Sign"},
            {37, "Checkpoint"},
            {38, "Stone Brick"},
            {39, "Snow"},
            {40, "Snowy Grass"},
            {41, "Compressed Void"},
            {42, "Flame Spitter"},
            {43, "Machine Gun"},
            {44, "Cactus"},
            {45, "Mushroom Booster"},
            {46, "Void Ladder"},
            {47, "Wooden Platform"},
            {48, "Aquaant Crystal"},
            {49, "Heliaura Crystal"},
            {50, "Sentistract Sludge"},
            {51, "Void Fire Spitter"},
            {52, "Sapphire Crystal"},
            {53, "Ruby Crystal"},
            {54, "Emerald Crystal"},
            {55, "Skull"},
            {56, "Stalagmite"},
            {57, "Mud"},
            {58, "Portal"},
            {59, "Bed"},
            {60, "Bookshelf"},
            {61, "Painting"},
            {62, "Tree of Knowledge"},
            {63, "Tree Bark"},
            {100, "Gold Brick Wall"},
            {101, "Stone Wall"},
            {102, "Dirt Wall"},
            {103, "Fence"},
            {104, "Marble Wall"},
            {105, "Sand Wall"},
            {106, "Hellstone Wall"},
            {107, "Stone Brick Wall"},
            {108, "Mesa Wall"},
            {109, "Wallpaper"},
            {110, "Nothing"},
            {111, "Tree Bark"},
            {200, "Player"},
            {201, "Snake"},
            {202, "Frog"},
            {203, "God"},
            {204, "Lost"},
            {205, "Hellboar"},
            {206, "Falling Boulder (Desert)"},
            {207, "Bat"},
            {208, "Duck"},
            {209, "Being of Sight"}
        };

        private readonly bool _isSampleTile;
        private bool _animationPlaysOnce;
        private List<Tile> _cornerPieces = new List<Tile>();
        private int _currentFrame;
        private Vector2 _frameCount;
        private double _frameTimer;
        private bool _hasAddedEntity;
        private bool _hasConnectPattern;
        private bool _hasRandomStartingPoint;
        private bool _isInvisible;
        private float _opacity = 1;
        private Rectangle _originalPosition;
        private Vector2 _positionInSpriteSheet;
        private double _restartTimer;
        private double _restartWait;
        private Vector2 _sizeOfTile = new Vector2(1, 1);
        private Rectangle _startingPosition;
        private Rectangle _startingRectangle;
        private int _switchFrame;
        private bool _wasInitialized;
        public Color Color = Color.White;
        public Rectangle DrawRectangle;
        public byte Id;
        public bool IsClimbable;
        public bool IsSolid;
        public bool IsWall;
        public Rectangle SourceRectangle;
        public byte SubId;
        public Texture2D Texture;

        /// <summary>
        ///     Constructor used when DefineTexture() will NOT be called.
        /// </summary>
        private Tile()
        {
            SetToDefaultSourceRect();
        }

        /// <summary>
        ///     Default constructor for game world tiles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Tile(int x, int y)
        {
            _originalPosition = new Rectangle(x, y, 0, 0);
            DrawRectangle = new Rectangle(x, y, Main.Tilesize, Main.Tilesize);
            SetToDefaultSourceRect();
        }

        /// <summary>
        ///     Constructor used when the tile will be used in the UI.
        /// </summary>
        /// <param name="sampleTile"></param>
        public Tile(bool sampleTile)
        {
            _isSampleTile = true;
            SetToDefaultSourceRect();
        }

        /// <summary>
        ///     Returns true if the animation was specifically told not to run.
        /// </summary>
        public bool AnimationStopped { get; set; }

        public bool IsBrushTile { get; set; }
        public int TileIndex { get; set; }

        /// <summary>
        ///     After the IDs have been defined, this will give the tile the correct location of its texture in the spritemap.
        /// </summary>
        public void DefineTexture()
        {
            //Air ID is 0, so it can emit sunlight.
            if (Id != 0)
            {
                Texture = GameWorld.SpriteSheet;
            }
            else
            {
                Texture = null;
                return;
            }

            Vector2 startingPoint;

            #region DefiningTextures

            switch (Id)
            {
                case 1: //Grass
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(0, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    // Random plain tile.
                    switch (SubId)
                    {
                        case 101:
                            _positionInSpriteSheet = new Vector2(12, 17);
                            break;
                        case 102:
                            _positionInSpriteSheet = new Vector2(13, 17);
                            break;
                        case 103:
                            _positionInSpriteSheet = new Vector2(11, 17);
                            break;
                        case 104:
                            _positionInSpriteSheet = new Vector2(10, 17);
                            break;
                    }
                    break;
                case 2: //Stone
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(4, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 3: //Marble Floor
                    IsSolid = true;
                    switch (SubId)
                    {
                        case 0: //Foundation
                            _positionInSpriteSheet = new Vector2(14, 5);
                            break;
                        case 1: //Foundation Right
                            _positionInSpriteSheet = new Vector2(15, 5);
                            break;
                        case 2: //Foundation Left
                            _positionInSpriteSheet = new Vector2(13, 5);
                            break;
                    }
                    break;
                case 4: //Hellrock
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 5: //Sand
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(8, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 6: //Mesa
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(8, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 7: //ShortGrass
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(12, 16);
                    break;
                case 8: //Metal
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(12, 2);
                    IsSolid = true;
                    break;
                case 9: //Tall Grass
                    _frameCount = new Vector2(12, 0);
                    _positionInSpriteSheet = new Vector2(0, 16);
                    break;
                case 10: // Gold.
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(0, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 11: // Torch.
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(12, 0);
                    break;
                case 12: //Chandelier
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.X = 2;
                    _positionInSpriteSheet = new Vector2(0, 17);
                    break;
                case 13: //Door
                    IsSolid = true;
                    break;
                case 14: //Vines
                    _positionInSpriteSheet = new Vector2(15, 7);
                    IsClimbable = true;
                    break;
                case 15: //Ladders
                    _positionInSpriteSheet = new Vector2(13, 7);
                    IsClimbable = true;
                    break;
                case 16: //Chains
                    _positionInSpriteSheet = new Vector2(14, 7);
                    IsClimbable = true;
                    break;
                case 17: //Daffodyls
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(12, 10 + Main.Random.Next(0, 3)*2);
                    DrawRectangle.Y = _originalPosition.Y - Main.Tilesize;
                    _hasRandomStartingPoint = true;
                    break;
                case 18: //Marble Column
                    switch (SubId)
                    {
                        case 0: //middle
                            _positionInSpriteSheet = new Vector2(13, 3);
                            break;
                        case 1: //top
                            _positionInSpriteSheet = new Vector2(12, 3);
                            break;
                        case 2: //bot
                            _positionInSpriteSheet = new Vector2(14, 3);
                            break;
                    }
                    break;
                case 19: //chest
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.X = 1.5f;
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(15, 30);
                    _animationPlaysOnce = true;
                    DrawRectangle.X = _originalPosition.X + Main.Tilesize/4;
                    DrawRectangle.Y = _originalPosition.Y - Main.Tilesize;
                    var chest = new Chest(this);
                    break;
                case 20: // Marble Brick
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(24, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 21: //scaffolding
                    _positionInSpriteSheet = new Vector2(13, 6);
                    IsSolid = true;
                    break;
                case 22: //spikes
                    _positionInSpriteSheet = new Vector2(17, 13);
                    break;
                case 23: //water
                    _frameCount = new Vector2(4, 0);
                    _hasRandomStartingPoint = true;
                    _positionInSpriteSheet = new Vector2(4, 15);

                    if (SubId == 1)
                        _positionInSpriteSheet = new Vector2(8, 24);
                    break;
                case 24: //lava
                    _frameCount = new Vector2(4, 0);
                    _hasRandomStartingPoint = true;
                    _positionInSpriteSheet = new Vector2(0, 15);
                    if (SubId == 1)
                        _positionInSpriteSheet = new Vector2(8, 25);
                    break;
                case 25: // Poisoned Water.
                    _frameCount = new Vector2(4, 0);
                    _hasRandomStartingPoint = true;
                    _positionInSpriteSheet = new Vector2(8, 15);
                    break;
                case 26: // Golden Apple.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(8, 26);
                    break;
                case 27: //golden chest
                    _positionInSpriteSheet = new Vector2(15, 3);
                    break;
                case 29: //Marble ceiling
                    IsSolid = true;
                    switch (SubId)
                    {
                        case 0: //Plain
                            _positionInSpriteSheet = new Vector2(15, 3);
                            break;
                        case 1: //Right ledge
                            _positionInSpriteSheet = new Vector2(15, 4);
                            break;
                        case 2: //Left ledge
                            _positionInSpriteSheet = new Vector2(13, 4);
                            break;
                    }

                    break;
                case 30: // Vacant.
                    break;
                case 31: //Tree
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.X = 4;
                    _sizeOfTile.Y = 6;

                    DrawRectangle.Y = _originalPosition.Y - (32*((int) _sizeOfTile.Y - 1));
                    DrawRectangle.X = _originalPosition.X - (16*(int) _sizeOfTile.X);
                    _positionInSpriteSheet = new Vector2(16, 0);
                    break;
                case 32: //Small Rock
                    _positionInSpriteSheet = new Vector2(13, 18);
                    break;
                case 33: //Big Rock
                    _frameCount = new Vector2(0, 0);
                    _sizeOfTile.X = 2;
                    _sizeOfTile.Y = 2;
                    DrawRectangle.Y = _originalPosition.Y - 32;
                    _positionInSpriteSheet = new Vector2(14, 17);
                    break;
                case 34: //Medium Rock
                    _positionInSpriteSheet = new Vector2(11, 18);
                    break;
                case 36: //Sign
                    _positionInSpriteSheet = new Vector2(12, 4);
                    break;
                case 37: //Checkpoint
                    if (GameWorld.CurrentGameMode == GameMode.Edit)
                    {
                        _positionInSpriteSheet = new Vector2(8, 29);
                    }
                    else
                    {
                        if (!_hasAddedEntity)
                        {
                            GameWorld.Entities.Add(new CheckPoint(DrawRectangle.X, DrawRectangle.Y));
                            _hasAddedEntity = true;
                        }
                    }
                    break;
                case 38: //Stone Brick
                    IsSolid = true;
                    startingPoint = new Vector2(0, 10);
                    _hasConnectPattern = true;
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 39: //Ice
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 10);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 40: //Snow Covered Grass
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(8, 10);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 41: //Void tile
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(16, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 42: // Flamespitter
                    _frameCount = new Vector2(8, 0);
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(12, 29);

                    break;
                case 43: // Machine Gun
                    _frameCount = new Vector2(8, 0);
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(12, 28);
                    break;
                case 44: // Cacti
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.X = 2;
                    _sizeOfTile.Y = 2;
                    switch (Main.Random.Next(0, 4))
                    {
                        case 0: // One branch normal.
                            _positionInSpriteSheet = new Vector2(20, 2);
                            break;
                        case 1: // Two branch normal.
                            _positionInSpriteSheet = new Vector2(20, 4);
                            break;
                        case 2: // One branch flower.
                            _positionInSpriteSheet = new Vector2(22, 2);
                            break;
                        case 3: // Two branch flower.
                            _positionInSpriteSheet = new Vector2(22, 4);
                            break;
                    }
                    break;
                case 45: // Mushroom Booster
                    _positionInSpriteSheet = new Vector2(19, 26);
                    break;
                case 46: // Void ladder.
                    _positionInSpriteSheet = new Vector2(14, 8);
                    IsClimbable = true;
                    break;
                case 47: // Wooden platform.
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(14, 26);
                    break;
                case 48: // Blue crystal.
                    _frameCount = new Vector2(2, 0);
                    _positionInSpriteSheet = new Vector2(20, 27);
                    new Crystal(this, 3);
                    break;
                case 49: // Yellow crystal.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(20, 29);
                    new Crystal(this, 1);
                    break;
                case 50: // Green sludge.
                    _frameCount = new Vector2(6, 0);
                    _positionInSpriteSheet = new Vector2(14, 27);
                    new Crystal(this, 2);
                    break;
                case 51: // Void FireSpitter.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(20, 28);
                    break;
                case 52: // Sapphire Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(21, 24);
                    new Crystal(this, 3);
                    break;
                case 53: // Ruby Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(22, 25);
                    new Crystal(this, 4);
                    break;
                case 54: // Emerald Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(21, 25);
                    new Crystal(this, 2);
                    break;
                case 55: // Skull.
                    _positionInSpriteSheet = new Vector2(22, 24);
                    break;
                case 56: // Stalagmite
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(23, 24);
                    break;
                case 57: // Mud.
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(4, 29);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 58: // Portal.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 3;
                    _sizeOfTile.X = 2;
                    DrawRectangle.Height = (int) _sizeOfTile.Y*Main.Tilesize;
                    DrawRectangle.Width = (int) _sizeOfTile.X*Main.Tilesize;
                    switch (SubId)
                    {
                        case 0:
                            _positionInSpriteSheet = new Vector2(8, 30);
                            break;
                    }

                    if (!_isSampleTile && !_wasInitialized)
                    {
                        new Portal(this);
                        _wasInitialized = true;
                    }
                    break;
                case 59: // Bed.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 2;
                    _sizeOfTile.X = 3;
                    _positionInSpriteSheet = new Vector2(10, 30);
                    break;
                case 60: // Bookshelf.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 3;
                    _sizeOfTile.X = 2;
                    _positionInSpriteSheet = new Vector2(13, 30);
                    break;
                case 61: // Painting.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 2;
                    _sizeOfTile.X = 2;
                    _positionInSpriteSheet = new Vector2(10, 32);
                    break;
                case 62: // Tree of Knowledge
                    _sizeOfTile.X = 50;
                    _sizeOfTile.Y = 25;
                    //Texture = ContentHelper.LoadTexture("Tiles/tree of knowledge big");
                    _positionInSpriteSheet = new Vector2(0, 0);

                    DrawRectangle.Y = _originalPosition.Y - (32*((int) _sizeOfTile.Y - 1));
                    DrawRectangle.X = _originalPosition.X - (16*(int) _sizeOfTile.X);
                    break;
                case 63: // Tree Bark
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(28, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;

                    #region Wall Textures

                case 100: //Gold Brick Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 101: //Stone Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(20, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 102: //Dirt Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(0, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 103: //Fences
                    switch (SubId)
                    {
                        case 0: //Plain
                            _positionInSpriteSheet = new Vector2(12, 7);
                            break;
                        case 1: //Top point
                            _positionInSpriteSheet = new Vector2(12, 6);
                            break;
                    }
                    break;
                case 104: //Marble wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(12, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 105: // Sand Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 24);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 106: //Hellstone Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(0, 24);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 107: //Stone Brick Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(8, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 108: // Mesa Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(0, 29);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case 109: // Wallpaper.
                    switch (SubId)
                    {
                        case 0: //Plain
                            _positionInSpriteSheet = new Vector2(23, 8);
                            break;
                        case 1: //Top point
                            _positionInSpriteSheet = new Vector2(23, 7);
                            break;
                        case 2: //Bottom point
                            _positionInSpriteSheet = new Vector2(23, 9);
                            break;
                    }
                    break;
                case 110: // Black.
                    _positionInSpriteSheet = new Vector2(13, 9);
                    break;
                case 111: // Tree of Knowledge
                    _sizeOfTile.X = 10;
                    _sizeOfTile.Y = 10;
                    _positionInSpriteSheet = new Vector2(24, 5);

                    DrawRectangle.Y = _originalPosition.Y - (32*((int) _sizeOfTile.Y - 1));
                    DrawRectangle.X = _originalPosition.X - (16*(int) _sizeOfTile.X);
                    break;

                    #endregion

                case 200: //Player
                    if (GameWorld.CurrentGameMode == GameMode.Edit)
                        _positionInSpriteSheet = new Vector2(17, 12);
                    else
                    {
                        if (!_hasAddedEntity)
                        {
                            GameWorld.GetPlayer().Initialize(DrawRectangle.X, DrawRectangle.Y);
                            _hasAddedEntity = true;
                            _isInvisible = true;
                        }
                    }
                    break;
                case 201: //Snake
                    
                    break;
                case 202: //Frog
                    if (GameWorld.CurrentGameMode == GameMode.Edit)
                    {
                        _positionInSpriteSheet = new Vector2(21, 12);
                    }
                    else
                    {
                        if (!_hasAddedEntity)
                        {
                            GameWorld.Entities.Add(new Frog(DrawRectangle.X, DrawRectangle.Y));
                            _hasAddedEntity = true;
                            _isInvisible = true;
                        }
                    }
                    break;
                case 203: // NPC
                    _positionInSpriteSheet = new Vector2(18, 13);
                    _isInvisible = true;
                    if (!_isSampleTile && !_wasInitialized)
                    {
                        new NonPlayableCharacter(this);
                        _wasInitialized = true;
                    }
                    break;
                case 204: //Lost
                   
                    break;
                case 205: //Hellboar
                  
                    break;
                case 206: //Falling Boulder
                   
                    break;
                case 207: //Bat
                   
                    break;
                case 208: //Duck
                   
                    break;
                case 209: //Flying Wheel
                   
                    break;
            }

            #endregion

            DefineSourceRectangle();
            DefineDrawRectangle();
            _startingRectangle = SourceRectangle;

            if (_hasRandomStartingPoint)
            {
                var randX = Main.Random.Next(0, (int) _frameCount.X);
                SourceRectangle.X += randX*SmallTileSize;
                _currentFrame += randX;
            }
        }

        /// <summary>
        ///     Takes all the variables given in DefineTexture method and returns the appropriate source rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineSourceRectangle()
        {
            //return new Rectangle((int)(startingPosition.X * SmallTileSize), (int)(startingPosition.Y * SmallTileSize), (int)(SmallTileSize * sizeOfTile.X), (int)(SmallTileSize * sizeOfTile.Y));
            SourceRectangle = new Rectangle((int) (_positionInSpriteSheet.X*SmallTileSize),
                (int) (_positionInSpriteSheet.Y*SmallTileSize), (int) (SmallTileSize*_sizeOfTile.X),
                (int) (SmallTileSize*_sizeOfTile.Y));
        }

        /// <summary>
        ///     Takes all the variables given in DefineTexture method and returns the appropriate draw rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineDrawRectangle()
        {
            if (_isSampleTile)
            {
                var width = (int) (_sizeOfTile.X*Main.Tilesize);
                var height = (int) (_sizeOfTile.Y*Main.Tilesize);

                if (width > height)
                {
                    width = Main.Tilesize;
                    height = (int) (Main.Tilesize/_sizeOfTile.X);
                }
                if (height > width)
                {
                    width = (int) (Main.Tilesize/_sizeOfTile.Y);
                    height = Main.Tilesize;
                }
                if (height == width)
                {
                    width = Main.Tilesize;
                    height = Main.Tilesize;
                }
                // Console.WriteLine("Name:{0}, Width:{1}, Height:{2}", name, width, height);
                DrawRectangle = new Rectangle(DrawRectangle.X, DrawRectangle.Y, width, height);
            }
            else
                DrawRectangle = new Rectangle(DrawRectangle.X, DrawRectangle.Y, (int) (_sizeOfTile.X*Main.Tilesize),
                    (int) (_sizeOfTile.Y*Main.Tilesize));
        }

        /// <summary>
        ///     This updates the animation of the tile.
        /// </summary>
        public void Update()
        {
            OnTileUpdate?.Invoke(this);

            Animate();
            ChangeOpacity();
        }

        private void Animate()
        {
            if (_frameCount.X > 1 && !AnimationStopped)
            {
                switch (Id)
                {
                    case 8: //Metal
                        _switchFrame = 100;
                        _restartWait = 2000;
                        _frameTimer += Main.GameTime.ElapsedGameTime.TotalMilliseconds;
                        _restartTimer += Main.GameTime.ElapsedGameTime.TotalMilliseconds;

                        if (_restartTimer < _restartWait)
                            break;
                        if (_frameTimer >= _switchFrame)
                        {
                            if (_frameCount.X != 0)
                            {
                                _frameTimer = 0;
                                SourceRectangle.X += SmallTileSize;
                                _currentFrame++;
                            }
                        }

                        if (_currentFrame >= _frameCount.X)
                        {
                            _currentFrame = 0;
                            SourceRectangle.X = 12*16;
                            _restartTimer = 0;
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
            var gameTime = Main.GameTime;

            _switchFrame = 120;
            _frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_frameTimer >= _switchFrame)
            {
                if (_frameCount.X != 0)
                {
                    _frameTimer = 0;
                    SourceRectangle.X += SourceRectangle.Width;
                    _currentFrame++;
                }
            }

            if (_currentFrame >= _frameCount.X)
            {
                if (_animationPlaysOnce)
                {
                    _currentFrame--;
                    SourceRectangle.X -= SourceRectangle.Width;
                }
                else
                {
                    _currentFrame = 0;
                    SourceRectangle.X = _startingRectangle.X;
                }
            }
        }

        private void ChangeOpacity()
        {
            if (GameWorld.CurrentGameMode == GameMode.Edit)
            {
                if (LevelEditor.OnWallMode)
                {
                    if (!IsWall)
                    {
                        _opacity -= .05f;

                        if (_opacity < MaxOpacity)
                        {
                            _opacity = MaxOpacity;
                        }
                    }
                }
                else
                {
                    _opacity += .05f;
                    if (_opacity > DefaultOpacity)
                    {
                        _opacity = DefaultOpacity;
                    }
                }
            }
        }

        public void Destroy()
        {
            if (OnTileDestroyed != null)
                OnTileDestroyed(this);

            _wasInitialized = false;
            _sizeOfTile = new Vector2(1, 1);
            DefineDrawRectangle();
            DefineSourceRectangle();
            SetToDefaultSourceRect();
            GameWorld.WorldData.MetaData[TileIndex] = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                if (!_isInvisible || (_isInvisible && GameWorld.CurrentGameMode == GameMode.Edit))
                    spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color*_opacity);
            }
            if (_hasConnectPattern)
            {
                foreach (var c in _cornerPieces)
                {
                    if (LevelEditor.OnWallMode)
                    {
                        c._opacity = GetOpacity();
                    }
                    else c._opacity = 1;
                    c.Draw(spriteBatch);
                }
            }
        }

        public void DrawByForce(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color);
        }

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.Red);
        }

        /// <summary>
        ///     This is used for the tiles that have special textures for corners. In the spritesheet they are arranged in the same
        ///     way. This includes grass, sand, stone, and mesa.
        /// </summary>
        /// <param name="array">The tile array that will be analyzed.</param>
        /// <param name="mapWidth">The width of the map in tiles.</param>
        public void FindConnectedTextures(Tile[] array, int mapWidth)
        {
            _cornerPieces = new List<Tile>();

            // Wallpaper.
            if (Id == 109)
            {
                var indexAbove = TileIndex - mapWidth;
                var indexBelow = TileIndex + mapWidth;
                if (array[indexAbove].Id != 109)
                {
                    SubId = 1;
                }
                else if (array[indexBelow].Id != 109)
                {
                    SubId = 2;
                }
                else SubId = 0;
            }

            //Marble columns
            if (Id == 18)
            {
                var indexAbove = TileIndex - mapWidth;
                var indexBelow = TileIndex + mapWidth;
                if (array[indexAbove].Id != 18)
                {
                    SubId = 1;
                }
                else if (array[indexBelow].Id != 18)
                {
                    SubId = 2;
                }
                else SubId = 0;
            }

            //Marble Floor
            else if (Id == 3)
            {
                if (array[TileIndex - 1].Id != 3)
                    SubId = 2;
                else if (array[TileIndex + 1].Id != 3)
                    SubId = 1;
                else SubId = 0;
            }


            //Marble Ceiling
            else if (Id == 29)
            {
                if (array[TileIndex + 1].Id != 29)
                    SubId = 1;
                else if (array[TileIndex - 1].Id != 29)
                    SubId = 2;
                else SubId = 0;
            }

            //Fences
            else if (Id == 103)
            {
                if (array[TileIndex - mapWidth].Id != 103)
                    SubId = 1;
                else SubId = 0;
            }

            // Water.
            else if (Id == 23)
            {
                if (array[TileIndex - mapWidth].Id == 0)
                    SubId = 1;
                else SubId = 0;
            }

            // Lava.
            else if (Id == 24)
            {
                if (array[TileIndex - mapWidth].Id == 0)
                    SubId = 1;
                else SubId = 0;
            }


            //Default Connected Textures Pattern
            //"Please don't change this was a headache to make." -Lucas 2015

            if (!_hasConnectPattern)
                return;

            var m = TileIndex;
            var t = m - mapWidth;
            var b = m + mapWidth;
            var tl = t - 1;
            var tr = t + 1;
            var ml = m - 1;
            var mr = m + 1;
            var bl = b - 1;
            var br = b + 1;

            if (br >= array.Length || tl < 0)
                return;

            var topLeft = array[tl];
            var top = array[t];
            var topRight = array[tr];
            var midLeft = array[ml];
            var mid = array[m];
            var midRight = array[mr];
            var botLeft = array[bl];
            var bot = array[b];
            var botRight = array[br];

            if (topLeft.Id == mid.Id &&
                top.Id == mid.Id &&
                topRight.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                botLeft.Id == mid.Id &&
                bot.Id == mid.Id &&
                botRight.Id == mid.Id)
                SubId = 0;

            if (topLeft.Id == mid.Id &&
                top.Id == mid.Id &&
                topRight.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                botLeft.Id == mid.Id &&
                bot.Id == mid.Id &&
                botRight.Id != mid.Id)
                SubId = 0;

            if (topLeft.Id == mid.Id &&
                top.Id == mid.Id &&
                topRight.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                botLeft.Id != mid.Id &&
                bot.Id == mid.Id &&
                botRight.Id == mid.Id)
                SubId = 0;

            if (topLeft.Id != mid.Id &&
                top.Id == mid.Id &&
                topRight.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                botLeft.Id == mid.Id &&
                bot.Id == mid.Id &&
                botRight.Id == mid.Id)
                SubId = 0;

            if (top.Id != mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id == mid.Id)
                SubId = 4;

            if (top.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id == mid.Id)
                SubId = 5;

            if (top.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id == mid.Id)
                SubId = 6;

            if (topLeft.Id == mid.Id &&
                top.Id == mid.Id &&
                topRight.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                botLeft.Id == mid.Id &&
                bot.Id == mid.Id &&
                botRight.Id == mid.Id)
                SubId = 0;

            if (top.Id == mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id == mid.Id)
                SubId = 8;

            if (top.Id != mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id != mid.Id)
                SubId = 9;

            if (top.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id == mid.Id)
                SubId = 10;

            if (top.Id != mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id == mid.Id)
                SubId = 11;

            if (top.Id == mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id != mid.Id)
                SubId = 12;

            if (top.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id != mid.Id)
                SubId = 13;

            if (top.Id == mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id != mid.Id)
                SubId = 14;

            if (top.Id == mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id == mid.Id)
                SubId = 15;

            if (top.Id != mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id != mid.Id)
                SubId = 16;

            if (top.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id != mid.Id)
                SubId = 17;

            if (top.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id != mid.Id)
                SubId = 18;

            if (top.Id == mid.Id &&
                midLeft.Id != mid.Id &&
                midRight.Id != mid.Id &&
                bot.Id != mid.Id)
                SubId = 19;

            //Special
            if (botRight.Id != mid.Id &&
                midRight.Id == mid.Id &&
                bot.Id == mid.Id)
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 1;
                _cornerPieces.Add(corner);
            }

            if (botLeft.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                bot.Id == mid.Id)
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 2;
                _cornerPieces.Add(corner);
            }

            if (topLeft.Id != mid.Id &&
                midLeft.Id == mid.Id &&
                top.Id == mid.Id)
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 3;
                _cornerPieces.Add(corner);
            }

            if (topRight.Id != mid.Id &&
                midRight.Id == mid.Id &&
                top.Id == mid.Id)
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 7;
                _cornerPieces.Add(corner);
            }

            foreach (var corners in _cornerPieces)
            {
                corners.DefineTexture();
            }
        }

        public void AddRandomlyGeneratedDecoration(Tile[] array, int mapWidth)
        {
            //Add decoration on top of grass tile.
            if (Id == 1 && SubId == 5)
            {
                var indexAbove = TileIndex - mapWidth;
                if (array[indexAbove].Id == 0)
                {
                    var rand = Main.Random.Next(0, 10);
                    if (rand == 0) //flower
                    {
                        array[indexAbove].Id = 17;
                    }
                    else if (rand == 1 || rand == 2) //tall grass
                    {
                        array[indexAbove].Id = 9;
                    }
                    else //short grass
                    {
                        array[indexAbove].Id = 7;
                    }

                    array[indexAbove].DefineTexture();
                }
            }

            // Random decorations for sand.
            if (Id == 5 && SubId == 5)
            {
                var indexAbove = TileIndex - mapWidth*2;
                var indexToRight = TileIndex - mapWidth + 1;
                var indexTopRight = indexAbove + 1;
                if (array[indexAbove].Id == 0 && array[indexToRight].Id == 0 && array[indexTopRight].Id == 0)
                {
                    var rand = Main.Random.Next(0, 100);
                    if (rand > 80)
                        array[indexAbove].Id = 44;

                    array[indexAbove].DefineTexture();
                }
            }

            // Random decoration for hellstone.
            if (Id == 4 && SubId == 5)
            {
                var indexAbove = TileIndex - mapWidth;
                if (array[indexAbove].Id == 0)
                {
                    var rand = Main.Random.Next(0, 10);

                    // Skull.
                    if (rand == 0)
                    {
                        array[indexAbove].Id = 55;
                    }
                    array[indexAbove].DefineTexture();
                }
            }

            // Hellstone stalagmmite.
            if (Id == 4 && SubId == 13)
            {
                if (Main.Random.Next(0, 5) == 1)
                {
                    var indexBelow = TileIndex + mapWidth;
                    var indexTwoBelow = indexBelow + mapWidth;
                    if (array[indexBelow].Id == 0 && array[indexTwoBelow].Id == 0)
                    {
                        array[indexBelow].Id = 56;
                        array[indexBelow].DefineTexture();
                    }
                }
            }

            // Randomly generate different plain textures for certain tiles.
            // Grass
            if (Id == 1 && SubId == 0 && Main.Random.Next(0, 100) > 80)
            {
                switch (Main.Random.Next(0, 4))
                {
                    case 0:
                        SubId = 101;
                        break;
                    case 1:
                        SubId = 102;
                        break;
                    case 2:
                        SubId = 103;
                        break;
                    case 3:
                        SubId = 104;
                        break;
                }

                DefineTexture();
            }
        }

        private Vector2 GetPositionInSpriteSheetOfConnectedTextures(Vector2 startingPoint)
        {
            var position = new Vector2();
            switch (SubId)
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
            return _opacity;
        }

        public event TileHandler OnTileUpdate;
        public event TileHandler OnTileDestroyed;
        public event TileHandler OnPlayerInteraction;

        /// <summary>
        ///     Sets the source rectangle to the no texture found texture.
        /// </summary>
        private void SetToDefaultSourceRect()
        {
            SourceRectangle = new Rectangle(12*SmallTileSize, 8*SmallTileSize, SmallTileSize, SmallTileSize);
        }

        /// <summary>
        ///     Returns the tile's texture position in the spritesheet. This needs to be multiplied by 16 to get the coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInSpriteSheet()
        {
            return _positionInSpriteSheet;
        }

        public Rectangle GetDrawRectangle()
        {
            return DrawRectangle;
        }

        /// <summary>
        ///     Returns the size of this tile as a ratio of the default tile size.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSize()
        {
            return _sizeOfTile;
        }

        /// <summary>
        ///     Returns the friction constant for this tile.
        /// </summary>
        /// <returns></returns>
        public float GetFrictionConstant()
        {
            switch (Id)
            {
                case 0:
                    return 1f;
                default:
                    return .95f;
            }
        }
    }
}