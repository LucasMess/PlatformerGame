using Adam.Characters;
using Adam.Characters.Enemies;
using Adam.Interactables;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Adam.AdamGame;

namespace Adam
{
    public sealed class Tile
    {
        public enum CollisionType
        {
            All, FromAbove,
        }
        public CollisionType CurrentCollisionType = CollisionType.All;
        public delegate void TileHandler(Tile t);

        /// <summary>
        /// A default tile that is returned when a query for a out of bounds tile is given.
        /// </summary>
        public static Tile Default = new Tile();

        private const int SmallTileSize = 16;
        private const float DefaultOpacity = 1;
        private const float MaxOpacity = .5f;

        public TileType Id = 0;
        private readonly bool _isSampleTile;
        private bool animationPlaysOnce = false;
        private bool animationResets = false;
        private List<Tile> _cornerPieces = new List<Tile>();
        public int CurrentFrame;
        private Vector2 _frameCount;
        private double _frameTimer;
        private bool _hasAddedEntity;
        private bool _hasConnectPattern;
        private bool _hasRandomStartingPoint;
        private bool _isInvisibleInPlayMode;
        private bool _isInvisibleInEditMode;
        private float _opacity = 1;
        private Rectangle _originalPosition;
        private Vector2 _positionInSpriteSheet;
        private double _restartTimer;
        private double _restartWait;
        private Vector2 _sizeOfTile = new Vector2(1, 1);
        private Rectangle _startingPosition;
        private Rectangle _startingRectangle;
        private const int DefaultAnimationSpeed = 125;
        private int animationSpeed = DefaultAnimationSpeed;
        private bool _wasInitialized;
        private bool _hasTopAndBottomPattern;
        public Color Color = Color.White;
        public Rectangle DrawRectangle;
        private Rectangle _defaultDrawRectangle;
        public Interactable Interactable { get; private set; }
        public bool IsClimbable;
        public bool IsSolid;
        public bool IsWall;
        public Rectangle SourceRectangle;
        public byte SubId;
        public Texture2D Texture;
        private static Rectangle _gridSourceRectangle = new Rectangle(352, 160, 32, 32);

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
            DrawRectangle = new Rectangle(x, y, AdamGame.Tilesize, AdamGame.Tilesize);
            _defaultDrawRectangle = DrawRectangle;
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
            if (Id == 0)
            {
                LetsLightThrough = true;
                Texture = null;
                return;

            }
            else
            {
                Texture = GameWorld.SpriteSheet;
            }

            Vector2 startingPoint;

            #region DefiningTextures

            switch (Id)
            {
                case TileType.Grass: //Grass
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
                case TileType.Stone: //Stone
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(4, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.MarbleFloor: //Marble Floor
                    IsSolid = true;
                    LetsLightThrough = true;
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
                case TileType.Hellrock: //Hellrock
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Sand: //Sand
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(8, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Mesa: //Mesa
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(8, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.ShortGrass: //ShortGrass
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(12, 16);
                    LetsLightThrough = true;
                    break;
                case TileType.Metal: //Metal
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(12, 2);
                    IsSolid = true;
                    break;
                case TileType.TallGrass: //Tall Grass
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(0, 16);
                    LetsLightThrough = true;
                    break;
                case TileType.GoldBrick: // Gold.
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(0, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Torch: // Torch.
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(12, 0);
                    Interactable = new Torch();
                    LetsLightThrough = true;
                    break;
                case TileType.Chandelier: //Chandelier
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.X = 2;
                    _positionInSpriteSheet = new Vector2(0, 17);
                    LetsLightThrough = true;
                    break;
                case TileType.Door: //Door
                    IsSolid = true;
                    LetsLightThrough = true;
                    break;
                case TileType.Vine: //Vines
                    _positionInSpriteSheet = new Vector2(15, 7);
                    IsClimbable = true;
                    LetsLightThrough = true;
                    break;
                case TileType.Ladder: //Ladders
                    _positionInSpriteSheet = new Vector2(13, 7);
                    IsClimbable = true;
                    LetsLightThrough = true;
                    break;
                case TileType.Chain: //Chains
                    _positionInSpriteSheet = new Vector2(14, 7);
                    IsClimbable = true;
                    LetsLightThrough = true;
                    break;
                case TileType.Flower: //Daffodyls
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(12, 10 + AdamGame.Random.Next(0, 3) * 2);
                    DrawRectangle.Y = _originalPosition.Y - AdamGame.Tilesize;
                    _hasRandomStartingPoint = true;
                    LetsLightThrough = true;
                    break;
                case TileType.MarbleColumn: //Marble Column
                    LetsLightThrough = true;
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
                case TileType.Chest: //chest
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.X = 1.5f;
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(15, 30);
                    animationPlaysOnce = true;
                    DrawRectangle.X = _originalPosition.X + AdamGame.Tilesize / 4;
                    DrawRectangle.Y = _originalPosition.Y - AdamGame.Tilesize;
                    Interactable = new Chest(this);
                    LetsLightThrough = true;
                    break;
                case TileType.MarbleBrick: // Marble Brick
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(24, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Scaffolding: //scaffolding
                    _positionInSpriteSheet = new Vector2(13, 6);
                    IsSolid = true;
                    LetsLightThrough = true;
                    CurrentCollisionType = CollisionType.FromAbove;
                    break;
                case TileType.Spikes: //spikes
                    _positionInSpriteSheet = new Vector2(17, 13);
                    LetsLightThrough = true;
                    break;
                case TileType.Water: //water
                    _frameCount = new Vector2(4, 0);
                    _hasRandomStartingPoint = true;
                    _positionInSpriteSheet = new Vector2(4, 15);

                    if (SubId == 1)
                        _positionInSpriteSheet = new Vector2(17, 24);
                    break;
                case TileType.Lava: //lava
                    Interactable = new Lava();
                    animationSpeed = 1000;

                    _frameCount = new Vector2(4, 0);
                    _hasRandomStartingPoint = false;
                    _positionInSpriteSheet = new Vector2(0, 15);
                    if (SubId == 1)
                        _positionInSpriteSheet = new Vector2(8, 25);
                    break;
                case TileType.Poison: // Poisoned Water.
                    _frameCount = new Vector2(4, 0);
                    _hasRandomStartingPoint = true;
                    _positionInSpriteSheet = new Vector2(8, 15);
                    break;
                case TileType.GoldenApple: // Golden Apple.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(8, 26);
                    LetsLightThrough = true;
                    break;
                case TileType.GoldenChest: //golden chest
                    _positionInSpriteSheet = new Vector2(15, 3);
                    break;
                case TileType.MarbleCeiling: //Marble ceiling
                    IsSolid = true;
                    LetsLightThrough = true;
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
                case TileType.Air: // Vacant.
                    break;
                case TileType.Tree: //Tree
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.X = 4;
                    _sizeOfTile.Y = 6;

                    DrawRectangle.Y = _originalPosition.Y - (32 * ((int)_sizeOfTile.Y - 1));
                    DrawRectangle.X = _originalPosition.X - (16 * (int)_sizeOfTile.X);
                    _positionInSpriteSheet = new Vector2(16, 0);
                    LetsLightThrough = true;
                    break;
                case TileType.SmallRock: //Small Rock
                    _positionInSpriteSheet = new Vector2(13, 18);
                    LetsLightThrough = true;
                    break;
                case TileType.BigRock: //Big Rock
                    _frameCount = new Vector2(0, 0);
                    _sizeOfTile.X = 2;
                    _sizeOfTile.Y = 2;
                    DrawRectangle.Y = _originalPosition.Y - 32;
                    _positionInSpriteSheet = new Vector2(14, 17);
                    LetsLightThrough = true;
                    break;
                case TileType.MediumRock: //Medium Rock
                    _positionInSpriteSheet = new Vector2(11, 18);
                    _sizeOfTile.X = 2;
                    LetsLightThrough = true;
                    break;
                case TileType.Sign: //Sign
                    _positionInSpriteSheet = new Vector2(12, 4);
                    LetsLightThrough = true;
                    break;
                case TileType.Checkpoint: //Checkpoint
                    LetsLightThrough = true;
                    _sizeOfTile.X = 1;
                    _sizeOfTile.Y = 3;
                    _frameCount = new Vector2(4, 0);
                    animationPlaysOnce = true;
                    _positionInSpriteSheet = new Vector2(8, 27);
                    Interactable = new CheckPoint(this);
                    _hasAddedEntity = true;
                    break;
                case TileType.StoneBrick: //Stone Brick
                    IsSolid = true;
                    startingPoint = new Vector2(0, 10);
                    _hasConnectPattern = true;
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Snow: //Ice
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 10);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.SnowyGrass: //Snow Covered Grass
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(8, 10);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.CompressedVoid: //Void tile
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(16, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.FlameSpitter: // Flamespitter
                    _frameCount = new Vector2(8, 0);
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(12, 29);
                    LetsLightThrough = true;
                    break;
                case TileType.MachineGun: // Machine Gun
                    _frameCount = new Vector2(8, 0);
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(12, 28);
                    LetsLightThrough = true;
                    break;
                case TileType.Cactus: // Cacti
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.X = 2;
                    _sizeOfTile.Y = 2;
                    switch (AdamGame.Random.Next(0, 4))
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
                    LetsLightThrough = true;
                    break;
                case TileType.MushroomBooster: // Mushroom Booster
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.X = 2;
                    DrawRectangle.Width = (int)_sizeOfTile.X * AdamGame.Tilesize;
                    animationSpeed = 75;
                    _positionInSpriteSheet = new Vector2(19, 26);
                    Interactable = new MushroomBooster();
                    IsSolid = true;
                    AnimationStopped = true;
                    animationPlaysOnce = true;
                    animationResets = true;
                    LetsLightThrough = true;
                    CurrentCollisionType = CollisionType.FromAbove;
                    break;
                case TileType.VoidLadder: // Void ladder.
                    _positionInSpriteSheet = new Vector2(14, 8);
                    IsClimbable = true;
                    LetsLightThrough = true;
                    break;
                case TileType.WoodenPlatform: // Wooden platform.
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(14, 26);
                    LetsLightThrough = true;
                    CurrentCollisionType = CollisionType.FromAbove;
                    break;
                case TileType.AquaantCrystal: // Blue crystal.
                    _frameCount = new Vector2(2, 0);
                    _positionInSpriteSheet = new Vector2(20, 27);
                    new Crystal(this, 3);
                    LetsLightThrough = true;
                    break;
                case TileType.HeliauraCrystal: // Yellow crystal.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(20, 29);
                    new Crystal(this, 1);
                    LetsLightThrough = true;
                    break;
                case TileType.SentistractSludge: // Green sludge.
                    _frameCount = new Vector2(6, 0);
                    _positionInSpriteSheet = new Vector2(14, 27);
                    new Crystal(this, 2);
                    LetsLightThrough = true;
                    break;
                case TileType.VoidFireSpitter: // Void FireSpitter.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(20, 28);
                    LetsLightThrough = true;
                    break;
                case TileType.SapphireCrystal: // Sapphire Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(21, 24);
                    new Crystal(this, 3);
                    LetsLightThrough = true;
                    break;
                case TileType.RubyCrystal: // Ruby Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(22, 25);
                    new Crystal(this, 4);
                    LetsLightThrough = true;
                    break;
                case TileType.EmeraldCrystal: // Emerald Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(21, 25);
                    new Crystal(this, 2);
                    LetsLightThrough = true;
                    break;
                case TileType.Skull: // Skull.
                    _positionInSpriteSheet = new Vector2(22, 24);
                    LetsLightThrough = true;
                    break;
                case TileType.Stalagmite: // Stalagmite
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(23, 24);
                    LetsLightThrough = true;
                    break;
                case TileType.Mud: // Mud.
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(4, 29);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.BackgroundDoor: // Portal.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 3;
                    _sizeOfTile.X = 2;
                    DrawRectangle.Height = (int)_sizeOfTile.Y * AdamGame.Tilesize;
                    DrawRectangle.Width = (int)_sizeOfTile.X * AdamGame.Tilesize;
                    switch (SubId)
                    {
                        case 0:
                            _positionInSpriteSheet = new Vector2(8, 30);
                            break;
                    }

                    if (!_isSampleTile && !_wasInitialized)
                    {
                        new BackgroundDoor();
                        _wasInitialized = true;
                    }
                    LetsLightThrough = true;
                    break;
                case TileType.Bed: // Bed.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 2;
                    _sizeOfTile.X = 3;
                    _positionInSpriteSheet = new Vector2(10, 30);
                    LetsLightThrough = true;
                    break;
                case TileType.Bookshelf: // Bookshelf.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 3;
                    _sizeOfTile.X = 2;
                    _positionInSpriteSheet = new Vector2(13, 30);
                    LetsLightThrough = true;
                    break;
                case TileType.Painting: // Painting.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 2;
                    _sizeOfTile.X = 2;
                    _positionInSpriteSheet = new Vector2(10, 32);
                    LetsLightThrough = true;
                    break;
                case TileType.TreeofKnowledge: // Tree of Knowledge
                    _sizeOfTile.X = 50;
                    _sizeOfTile.Y = 25;
                    //Texture = ContentHelper.LoadTexture("Tiles/tree of knowledge big");
                    _positionInSpriteSheet = new Vector2(0, 0);

                    DrawRectangle.Y = _originalPosition.Y - (32 * ((int)_sizeOfTile.Y - 1));
                    DrawRectangle.X = _originalPosition.X - (16 * (int)_sizeOfTile.X);
                    LetsLightThrough = true;
                    break;
                case TileType.TreeBark: // Tree Bark
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(28, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.PlayerDetector: // Player Detector
                    LetsLightThrough = true;
                    _positionInSpriteSheet = new Vector2(13, 8);
                    Interactable = new PlayerDetector(this);
                    _isInvisibleInPlayMode = true;
                    break;
                case TileType.Teleporter:
                    LetsLightThrough = true;
                    _positionInSpriteSheet = new Vector2(15, 8);
                    Interactable = new Teleporter(this);
                    _isInvisibleInPlayMode = true;
                    break;
                case TileType.Wood:
                    IsSolid = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(28, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Glass:
                    IsSolid = true;
                    LetsLightThrough = true;
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(32, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.SteelBeam:
                    IsSolid = true;
                    switch (AdamGame.Random.Next(0, 5))
                    {
                        case 0:
                            _positionInSpriteSheet = new Vector2(12, 25);
                            break;
                        case 1:
                            _positionInSpriteSheet = new Vector2(13, 25);
                            break;
                        case 2:
                            _positionInSpriteSheet = new Vector2(14, 25);
                            break;
                        case 3:
                            _positionInSpriteSheet = new Vector2(15, 25);
                            break;
                        case 4:
                            _positionInSpriteSheet = new Vector2(16, 25);
                            break;
                    }
                    break;
                case TileType.SnowCover:
                    switch (AdamGame.Random.Next(0, 5))
                    {
                        case 0:
                            _positionInSpriteSheet = new Vector2(12, 24);
                            break;
                        case 1:
                            _positionInSpriteSheet = new Vector2(13, 24);
                            break;
                        case 2:
                            _positionInSpriteSheet = new Vector2(14, 24);
                            break;
                        case 3:
                            _positionInSpriteSheet = new Vector2(15, 24);
                            break;
                        case 4:
                            _positionInSpriteSheet = new Vector2(16, 24);
                            break;
                    }
                    break;
                case TileType.Mosaic:
                    _hasConnectPattern = true;
                    IsSolid = true;
                    startingPoint = new Vector2(24, 14);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.MosaicVase:
                    _sizeOfTile.Y = 2;
                    _positionInSpriteSheet = new Vector2(24, 24);
                    LetsLightThrough = true;
                    break;
                case TileType.MushroomDecor:
                    _positionInSpriteSheet = new Vector2(8, 17);
                    LetsLightThrough = true;
                    break;

                #region Wall Textures

                case TileType.GoldBrickWall: //Gold Brick Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.StoneWall: //Stone Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(20, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.DirtWall: //Dirt Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(0, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Fence: //Fences
                    switch (SubId)
                    {
                        case 0: //Plain
                            _positionInSpriteSheet = new Vector2(12, 7);
                            break;
                        case 1: //Top point
                            _positionInSpriteSheet = new Vector2(12, 6);
                            break;
                    }
                    LetsLightThrough = true;
                    break;
                case TileType.MarbleWall: //Marble wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(12, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.SandWall: // Sand Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(4, 24);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.HellstoneWall: //Hellstone Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(0, 24);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.StoneBrickWall: //Stone Brick Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(8, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.MesaWall: // Mesa Wall
                    _hasConnectPattern = true;
                    startingPoint = new Vector2(0, 29);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.RedWallpaper: // Wallpaper.
                    _hasTopAndBottomPattern = true;
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
                case TileType.Nothing: // Black.
                    _positionInSpriteSheet = new Vector2(13, 9);
                    break;
                case TileType.TreeOfKnowledge: // Tree of Knowledge
                    _sizeOfTile.X = 10;
                    _sizeOfTile.Y = 10;
                    _positionInSpriteSheet = new Vector2(24, 5);

                    DrawRectangle.Y = _originalPosition.Y - (32 * ((int)_sizeOfTile.Y - 1));
                    DrawRectangle.X = _originalPosition.X - (16 * (int)_sizeOfTile.X);
                    break;
                case TileType.LightYellowPaint:
                    _hasTopAndBottomPattern = true;
                    switch (SubId)
                    {
                        case 0: //Plain
                            _positionInSpriteSheet = new Vector2(21, 31);
                            break;
                        case 1: //Top point
                            _positionInSpriteSheet = new Vector2(21, 30);
                            break;
                        case 2: //Bottom point
                            _positionInSpriteSheet = new Vector2(21, 32);
                            break;
                    }
                    break;

                #endregion

                case TileType.Player: //Player
                    if (AdamGame.CurrentGameMode == GameMode.Edit)
                    {
                        _positionInSpriteSheet = new Vector2(17, 12);
                        GameWorld.GetPlayer().RespawnPos = new Vector2(DrawRectangle.X, DrawRectangle.Y);
                    }
                    else if (GameWorld.IsTestingLevel)
                    {

                    }
                    else
                    {
                        if (!_hasAddedEntity)
                        {
                            GameWorld.GetPlayer().Initialize(DrawRectangle.X, DrawRectangle.Y);
                            _hasAddedEntity = true;
                        }
                    }
                    _isInvisibleInPlayMode = true;
                    LetsLightThrough = true;
                    break;
                case TileType.Snake: //Snake
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new Snake(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(18, 12);
                    break;
                case TileType.Frog: //Frog
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new Frog(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(18, 12);
                    break;
                case TileType.NPC: // NPC
                    if (!_isSampleTile)
                        new NonPlayableCharacter(this);
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(18, 12);
                    break;
                case TileType.Lost: //Lost
                    LetsLightThrough = true;
                    break;
                case TileType.Hellboar: //Hellboar
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new Hellboar(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(18, 12);
                    break;
                case TileType.FallingBoulder: //Falling Boulder
                    LetsLightThrough = true;
                    if (AdamGame.CurrentGameMode == GameMode.Edit)
                    {
                        _positionInSpriteSheet = new Vector2(19, 13);
                    }
                    else
                    {
                        if (!_hasAddedEntity)
                        {
                            GameWorld.Entities.Add(new FallingBoulder(DrawRectangle.X, DrawRectangle.Y));
                            _hasAddedEntity = true;
                            _isInvisibleInPlayMode = true;
                        }
                    }
                    break;
                case TileType.Bat: //Bat
                    LetsLightThrough = true;
                    break;
                case TileType.Duck: //Duck
                    LetsLightThrough = true;
                    break;
                case TileType.BeingofSight: //Flying Wheel
                    LetsLightThrough = true;
                    break;
            }

            #endregion

            DefineSourceRectangle();
            DefineDrawRectangle();
            _startingRectangle = SourceRectangle;

            if (_hasRandomStartingPoint)
            {
                var randX = AdamGame.Random.Next(0, (int)_frameCount.X);
                SourceRectangle.X += randX * SmallTileSize;
                CurrentFrame += randX;
            }
        }

        public void ReadMetaData()
        {
            if (IsWall) return;
            Interactable?.ReadMetaData(this);
        }

        private void Tile_OnTileUpdate(Tile t)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Takes all the variables given in DefineTexture method and returns the appropriate source rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineSourceRectangle()
        {
            //return new Rectangle((int)(startingPosition.X * SmallTileSize), (int)(startingPosition.Y * SmallTileSize), (int)(SmallTileSize * sizeOfTile.X), (int)(SmallTileSize * sizeOfTile.Y));
            SourceRectangle = new Rectangle((int)(_positionInSpriteSheet.X * SmallTileSize),
                (int)(_positionInSpriteSheet.Y * SmallTileSize), (int)(SmallTileSize * _sizeOfTile.X),
                (int)(SmallTileSize * _sizeOfTile.Y));
        }

        /// <summary>
        ///     Takes all the variables given in DefineTexture method and returns the appropriate draw rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineDrawRectangle()
        {
            if (_isSampleTile && !IsBrushTile)
            {
                var width = (int)(_sizeOfTile.X * AdamGame.Tilesize);
                var height = (int)(_sizeOfTile.Y * AdamGame.Tilesize);

                if (width > height)
                {
                    width = AdamGame.Tilesize;
                    height = (int)(AdamGame.Tilesize / _sizeOfTile.X);
                }
                if (height > width)
                {
                    width = (int)(AdamGame.Tilesize / _sizeOfTile.Y);
                    height = AdamGame.Tilesize;
                }
                if (height == width)
                {
                    width = AdamGame.Tilesize;
                    height = AdamGame.Tilesize;
                }
                // Console.WriteLine("Name:{0}, Width:{1}, Height:{2}", name, width, height);
                DrawRectangle = new Rectangle(DrawRectangle.X, DrawRectangle.Y, width, height);
            }
            else
                DrawRectangle = new Rectangle(DrawRectangle.X, DrawRectangle.Y, (int)(_sizeOfTile.X * AdamGame.Tilesize),
                    (int)(_sizeOfTile.Y * AdamGame.Tilesize));
        }

        /// <summary>
        ///     This updates the animation of the tile.
        /// </summary>
        public void Update()
        {
            OnTileUpdate?.Invoke(this);
            if (!_isSampleTile)
                Interactable?.Update(this);
            Animate();
            ChangeOpacity();
        }

        private void Animate()
        {
            if (_frameCount.X > 1 && !AnimationStopped)
            {
                switch (Id)
                {
                    case TileType.Metal: //Metal
                        animationSpeed = 100;
                        _restartWait = 2000;
                        _frameTimer += AdamGame.GameTime.ElapsedGameTime.TotalMilliseconds;
                        _restartTimer += AdamGame.GameTime.ElapsedGameTime.TotalMilliseconds;

                        if (_restartTimer < _restartWait)
                            break;
                        if (_frameTimer >= animationSpeed)
                        {
                            if (_frameCount.X != 0)
                            {
                                _frameTimer = 0;
                                CurrentFrame++;
                            }
                        }

                        if (CurrentFrame >= _frameCount.X)
                        {
                            CurrentFrame = 0;
                            _restartTimer = 0;
                        }
                        SourceRectangle.X = _startingRectangle.X + _startingRectangle.Width * CurrentFrame;
                        break;
                    default:
                        DefaultAnimation();
                        break;
                }
            }
        }

        private void DefaultAnimation()
        {
            var gameTime = AdamGame.GameTime;

            if (animationSpeed == 0) animationSpeed = 130;
            _frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_frameTimer >= animationSpeed)
            {
                if (_frameCount.X != 0)
                {
                    _frameTimer = 0;
                    CurrentFrame++;
                }
            }

            if (CurrentFrame >= _frameCount.X)
            {
                if (animationPlaysOnce)
                {
                    if (animationResets)
                    {
                        CurrentFrame = 0;
                        AnimationStopped = true;
                    }
                    else
                    {
                        CurrentFrame--;
                    }
                }
                else
                {
                    CurrentFrame = 0;
                }
            }
            SourceRectangle.X = _startingRectangle.X + _startingRectangle.Width * CurrentFrame;

        }

        private void ChangeOpacity()
        {
            if (AdamGame.CurrentGameMode == GameMode.Edit)
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

        public void ResetToDefault()
        {
            OnTileDestroyed?.Invoke(this);
            Interactable?.OnTileDestroyed(this);
            GameWorld.WorldData.MetaData.Remove(TileIndex);
            GameWorld.RemoveEntityAt(TileIndex);


            Id = 0;
            Interactable = null;
            _hasTopAndBottomPattern = false;
            IsSolid = false;
            SubId = 0;
            animationPlaysOnce = false;
            animationResets = false;
            animationSpeed = DefaultAnimationSpeed;
            DrawRectangle = _defaultDrawRectangle;
            _frameCount = Vector2.Zero;
            _wasInitialized = false;
            LetsLightThrough = false;
            _sizeOfTile = new Vector2(1, 1);
            DefineDrawRectangle();
            DefineSourceRectangle();
            SetToDefaultSourceRect();
            CurrentCollisionType = CollisionType.All;


            //DefineTexture();
            //LightingEngine.UpdateLightAt(TileIndex);
        }

        public void DrawRipples(SpriteBatch spriteBatch)
        {
            if (Id == TileType.Water && Texture != null)
                spriteBatch.Draw(Texture, DrawRectangle, new Rectangle(SourceRectangle.X, SourceRectangle.Y + 16, 16, 16), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //DrawShadowVersion(spriteBatch);

            if (Texture != null)
            {
                if (_isInvisibleInPlayMode && AdamGame.CurrentGameMode == GameMode.Play)
                    return;
                if (_isInvisibleInEditMode && AdamGame.CurrentGameMode == GameMode.Edit)
                    return;
                else
                {
                    Interactable?.Draw(spriteBatch, this);
                    spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color * _opacity);
                }
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
            if (AdamGame.CurrentGameMode == GameMode.Edit && IsWall)
            {
                spriteBatch.Draw(GameWorld.SpriteSheet, new Rectangle(_originalPosition.X, _originalPosition.Y, AdamGame.Tilesize, AdamGame.Tilesize), _gridSourceRectangle, Color.CornflowerBlue * .5f);
            }
        }

        public void DrawByForce(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color);
        }

        public void DrawShadowVersion(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, new Rectangle(DrawRectangle.X + 4, DrawRectangle.Y + 4, DrawRectangle.Width, DrawRectangle.Height), SourceRectangle, Color.Black * .4f);
        }

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.SpriteSheet, DrawRectangle, Color.Red * .5f);
        }

        /// <summary>
        ///     This is used for the tiles that have special textures for corners. In the spritesheet they are arranged in the same
        ///     way. This includes grass, sand, stone, and mesa.
        /// </summary>
        /// <param name="ids">The tile array that will be analyzed.</param>
        /// <param name="mapWidth">The width of the map in tiles.</param>
        public void FindConnectedTextures(TileType[] ids, int mapWidth)
        {
            _cornerPieces = new List<Tile>();

            // Wallpaper.
            if (_hasTopAndBottomPattern)
            {
                var indexAbove = TileIndex - mapWidth;
                var indexBelow = TileIndex + mapWidth;
                if (ids[indexAbove] != Id)
                {
                    SubId = 1;
                }
                else if (ids[indexBelow] != Id)
                {
                    SubId = 2;
                }
                else SubId = 0;
            }

            //Marble columns
            if (Id == TileType.MarbleColumn)
            {
                var indexAbove = TileIndex - mapWidth;
                var indexBelow = TileIndex + mapWidth;
                if (ids[indexAbove] != TileType.MarbleColumn)
                {
                    SubId = 1;
                }
                else if (ids[indexBelow] != TileType.MarbleColumn)
                {
                    SubId = 2;
                }
                else SubId = 0;
            }

            //Marble Floor
            else if (Id == TileType.MarbleFloor)
            {
                if (ids[TileIndex - 1] != TileType.MarbleFloor)
                    SubId = 2;
                else if (ids[TileIndex + 1] != TileType.MarbleFloor)
                    SubId = 1;
                else SubId = 0;
            }


            //Marble Ceiling
            else if (Id == TileType.MarbleCeiling)
            {
                if (ids[TileIndex + 1] != TileType.MarbleCeiling)
                    SubId = 1;
                else if (ids[TileIndex - 1] != TileType.MarbleCeiling)
                    SubId = 2;
                else SubId = 0;
            }

            //Fences
            else if (Id == TileType.Fence)
            {
                if (ids[TileIndex - mapWidth] != TileType.Fence)
                    SubId = 1;
                else SubId = 0;
            }

            // Water.
            else if (Id == TileType.Water)
            {
                if (ids[TileIndex - mapWidth] != TileType.Water)
                    SubId = 1;
                else SubId = 0;
            }

            // Lava.
            else if (Id == TileType.Lava)
            {
                if (ids[TileIndex - mapWidth] != TileType.Lava)
                    SubId = 1;
                else SubId = 0;
            }

            GiveSubIdsToConnectPatterns();
           
        }

        private void GiveSubIdsToConnectPatterns()
        {
            //Default Connected Textures Pattern
            //"Please don't change this was a headache to make." -Lucas 2015

            if (!_hasConnectPattern)
                return;

            int mapWidth = GameWorld.WorldData.LevelWidth;

            var m = TileIndex;
            var t = m - mapWidth;
            var b = m + mapWidth;
            var tl = t - 1;
            var tr = t + 1;
            var ml = m - 1;
            var mr = m + 1;
            var bl = b - 1;
            var br = b + 1;

            var topLeft = GameWorld.GetTile(tl, IsWall);
            var top = GameWorld.GetTile(t, IsWall);
            var topRight = GameWorld.GetTile(tr, IsWall);
            var midLeft = GameWorld.GetTile(ml, IsWall);
            var mid = GameWorld.GetTile(m, IsWall);
            var midRight = GameWorld.GetTile(mr, IsWall);
            var botLeft = GameWorld.GetTile(bl, IsWall);
            var bot = GameWorld.GetTile(b, IsWall);
            var botRight = GameWorld.GetTile(br, IsWall);

            if (topLeft.Equals(mid) &&
                top.Equals(mid) &&
                topRight.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                botLeft.Equals(mid) &&
                bot.Equals(mid) &&
                botRight.Equals(mid))
                SubId = 0;

            if (topLeft.Equals(mid) &&
                top.Equals(mid) &&
                topRight.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                botLeft.Equals(mid) &&
                bot.Equals(mid) &&
                !botRight.Equals(mid))
                SubId = 0;

            if (topLeft.Equals(mid) &&
                top.Equals(mid) &&
                topRight.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                !botLeft.Equals(mid) &&
                bot.Equals(mid) &&
                botRight.Equals(mid))
                SubId = 0;

            if (!topLeft.Equals(mid) &&
                top.Equals(mid) &&
                topRight.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                botLeft.Equals(mid) &&
                bot.Equals(mid) &&
                botRight.Equals(mid))
                SubId = 0;

            if (!top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 4;

            if (!top.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 5;

            if (!top.Equals(mid) &&
                midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 6;

            if (topLeft.Equals(mid) &&
                top.Equals(mid) &&
                !topRight.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                botLeft.Equals(mid) &&
                bot.Equals(mid) &&
                botRight.Equals(mid))
                SubId = 0;

            if (top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 8;

            if (!top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 9;

            if (top.Equals(mid) &&
                midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 10;

            if (!top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 11;

            if (top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 12;

            if (top.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 13;

            if (top.Equals(mid) &&
                midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 14;

            if (top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                bot.Equals(mid))
                SubId = 15;

            if (!top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 16;

            if (!top.Equals(mid) &&
                midLeft.Equals(mid) &&
                midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 17;

            if (!top.Equals(mid) &&
                midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 18;

            if (top.Equals(mid) &&
                !midLeft.Equals(mid) &&
                !midRight.Equals(mid) &&
                !bot.Equals(mid))
                SubId = 19;

            //Special
            if (!botRight.Equals(mid) &&
                midRight.Equals(mid) &&
                bot.Equals(mid))
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 1;
                _cornerPieces.Add(corner);
            }

            if (!botLeft.Equals(mid) &&
                midLeft.Equals(mid) &&
                bot.Equals(mid))
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 2;
                _cornerPieces.Add(corner);
            }

            if (!topLeft.Equals(mid) &&
                midLeft.Equals(mid) &&
                top.Equals(mid))
            {
                var corner = new Tile();
                corner.Id = mid.Id;
                corner.DrawRectangle = DrawRectangle;
                corner.Texture = Texture;
                corner.SubId = 3;
                _cornerPieces.Add(corner);
            }

            if (!topRight.Equals(mid) &&
                midRight.Equals(mid) &&
                top.Equals(mid))
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
            // Add snow cover if it is snowing, otherwise add flowers and grass.
            if (GameWorld.WorldData.IsSnowing)
            {
                if (!IsWall)
                {
                    if (IsSolid)
                    {
                        var indexAbove = TileIndex - mapWidth;
                        if (array[indexAbove].Id == TileType.Air)
                        {
                            array[indexAbove].Id = TileType.SnowCover;
                            array[indexAbove].DefineTexture();
                        }
                    }
                }
            }
            else
            {

                //Add decoration on top of grass tile.
                if (Id == TileType.Grass && SubId == 5)
                {
                    var indexAbove = TileIndex - mapWidth;
                    if (array[indexAbove].Id == 0)
                    {
                        var rand = AdamGame.Random.Next(0, 10);
                        if (rand == 0) //flower
                        {
                            array[indexAbove].Id = TileType.Flower;
                        }
                        else if (rand == 1 || rand == 2) //tall grass
                        {
                            array[indexAbove].Id = TileType.TallGrass;
                        }
                        else //short grass
                        {
                            array[indexAbove].Id = TileType.ShortGrass;
                        }

                        array[indexAbove].DefineTexture();
                    }
                }
            }

            // Random decorations for mud.
            if (Id == TileType.Mud && SubId == 5)
            {
                var indexAbove = TileIndex - mapWidth ;
                if (array[indexAbove].Id == 0)
                {
                    var rand = AdamGame.Random.Next(0, 100);
                    if (rand > 90)
                        array[indexAbove].Id = TileType.MushroomDecor;

                    array[indexAbove].DefineTexture();
                }
            }

            // Random decorations for sand.
            if (Id == TileType.Sand && SubId == 5)
            {
                var indexAbove = TileIndex - mapWidth * 2;
                var indexToRight = TileIndex - mapWidth + 1;
                var indexTopRight = indexAbove + 1;
                if (array[indexAbove].Id == 0 && array[indexToRight].Id == 0 && array[indexTopRight].Id == 0)
                {
                    var rand = AdamGame.Random.Next(0, 100);
                    if (rand > 80)
                        array[indexAbove].Id = TileType.Cactus;

                    array[indexAbove].DefineTexture();
                }
            }

            // Random decoration for hellstone.
            if (Id == TileType.Hellrock && SubId == 5)
            {
                var indexAbove = TileIndex - mapWidth;
                if (array[indexAbove].Id == 0)
                {
                    var rand = AdamGame.Random.Next(0, 10);

                    // Skull.
                    if (rand == 0)
                    {
                        array[indexAbove].Id = TileType.Skull;
                    }
                    array[indexAbove].DefineTexture();
                }
            }

            // Hellstone stalagmmite.
            if (Id == TileType.Hellrock && SubId == 13)
            {
                if (AdamGame.Random.Next(0, 5) == 1)
                {
                    var indexBelow = TileIndex + mapWidth;
                    var indexTwoBelow = indexBelow + mapWidth;
                    if (array[indexBelow].Id == 0 && array[indexTwoBelow].Id == 0)
                    {
                        array[indexBelow].Id = TileType.Stalagmite;
                        array[indexBelow].DefineTexture();
                    }
                }
            }

            // Randomly generate different plain textures for certain tiles.
            // Grass
            if (Id == TileType.Grass && SubId == 0 && AdamGame.Random.Next(0, 100) > 80)
            {
                switch (AdamGame.Random.Next(0, 4))
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
            // Sample tiles such as the ones in the tileholders have the same sub id, but the brush tiles do not.
            if (_isSampleTile && !IsBrushTile) SubId = 5;
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
            SourceRectangle = new Rectangle(12 * SmallTileSize, 8 * SmallTileSize, SmallTileSize, SmallTileSize);
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
                    return .94f;
                default:
                    return .94f;
            }
        }

        /// <summary>
        /// Returns true if light can pass through it.
        /// </summary>
        public bool LetsLightThrough { get; set; }

        /// <summary>
        /// Called when the player clicks on the tile in edit mode.
        /// </summary>
        public void InteractInEditMode()
        {
            Interactable?.OnPlayerClickInEditMode(this);
        }


        /// <summary>
        /// Returns true if there is an interactable component to this tile.
        /// </summary>
        /// <returns></returns>
        public bool HasInteractable()
        {
            if (Interactable == null) return false;
            return Interactable.CanBeLinkedByOtherInteractables;
        }

        public void ConnectToInteractable(Interactable interactable)
        {
            if (Interactable == null)
            {
                Console.WriteLine("Could not find interactable!");
                return;
            }
            interactable.OnActivation += Interactable.OnPlayerAction;
        }

        public void OnEntityTouch(Entity entity)
        {
            Interactable?.OnEntityTouch(this, entity);
        }

        /// <summary>
        /// Returns true if both tiles have the same ID.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Tile)
            {
                Tile other = (Tile)obj;
                return (other.Id == Id);
            }
            return false;
        }

    }
}