﻿using ThereMustBeAnotherWay.Characters;
using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Interactables;
using ThereMustBeAnotherWay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static ThereMustBeAnotherWay.TMBAW_Game;
using ThereMustBeAnotherWay.PlayerCharacter;

namespace ThereMustBeAnotherWay
{
    public sealed partial class Tile
    {
        public enum BorderType
        {
            None, BorderAlways, BorderNonSolid, StarboundLike
        }
        public BorderType CurrentBorderType = BorderType.None;


        public TileProperties GetProperties()
        {
            return Properties[Id];
        }

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
        private List<Tile> _shadowCornerPieces = new List<Tile>();
        public int CurrentFrame;
        private Vector2 _frameCount;
        private double _frameTimer;
        private bool _hasAddedEntity;
        private bool _hasRandomStartingPoint;
        private bool _isInvisibleInPlayMode;
        private bool _isInvisibleInEditMode;
        private float _opacity = 1;
        private Rectangle _originalPosition;
        private Vector2 _positionInSpriteSheet;
        private double _restartTimer;
        private double _restartWait;
        private Point _sizeOfTile = new Point(Tilesize, Tilesize);
        private Rectangle _startingRectangle;
        private const int DefaultAnimationSpeed = 125;
        private int animationSpeed = DefaultAnimationSpeed;
        /// <summary>
        /// Set to true if the tile should not be shown in the gameworld anymore.
        /// </summary>
        public bool IsHidden { get; set; } = false;
        private bool _hasTopAndBottomPattern;
        private bool _hasLeftAndRightPattern;
        public Color Color = Color.White;
        public Rectangle DrawRectangle;
        private Rectangle _defaultDrawRectangle;
        public Interactable Interactable { get; private set; }
        public bool IsClimbable;
        public bool IsSolid;
        public bool IsSolidTopDown = false;
        public bool IsWall;
        public Rectangle SourceRectangle;
        public byte SubId;
        private byte shadowSubId;
        public Texture2D Texture;
        public Light Light;
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
            SetOriginalPosition(x, y);
            DrawRectangle = new Rectangle(x, y, TMBAW_Game.Tilesize, TMBAW_Game.Tilesize);
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
        /// Sets the position this tile should return to whenever it is reset.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetOriginalPosition(int x, int y)
        {
            _originalPosition = new Rectangle(x, y, 0, 0);
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
            // Destroy current interactable to p
            Interactable?.OnTileDestroyed(this);

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

            DrawRectangle = _originalPosition;

            #region DefiningTextures

            switch (Id)
            {
                case TileType.Grass: //Grass
                    CurrentBorderType = BorderType.BorderNonSolid;
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
                    CurrentBorderType = BorderType.BorderNonSolid;
                    IsSolid = true;
                    startingPoint = new Vector2(4, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.MarbleFloor: //Marble Floor
                    IsSolid = true;
                    LetsLightThrough = true;
                    _hasLeftAndRightPattern = true;
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
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(4, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Sand: //Sand
                    CurrentBorderType = BorderType.BorderNonSolid;
                    IsSolid = true;
                    startingPoint = new Vector2(8, 0);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Mesa: //Mesa
                    CurrentBorderType = BorderType.BorderNonSolid;
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
                    CurrentBorderType = BorderType.BorderAlways;
                    IsSolid = true;
                    startingPoint = new Vector2(0, 5);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Torch: // Torch.
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(12, 0);
                    Interactable = new Torch();
                    LetsLightThrough = true;
                    Light = new Light(Position + new Vector2(8,11) * 2, new Color(255, 154, 34), 300);
                    break;
                case TileType.GreenTorch:
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(480 / 16, 448 / 16);
                    Interactable = new Torch();
                    LetsLightThrough = true;
                    break;
                case TileType.Chandelier: //Chandelier
                    _frameCount = new Vector2(4, 0);
                    _sizeOfTile.X = 64;
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
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(12, 10 + TMBAW_Game.Random.Next(0, 3) * 2);
                    DrawRectangle.Y = _originalPosition.Y - TMBAW_Game.Tilesize;
                    _hasRandomStartingPoint = true;
                    LetsLightThrough = true;
                    break;
                case TileType.MarbleColumn: //Marble Column
                    LetsLightThrough = true;
                    _hasTopAndBottomPattern = true;
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
                    _sizeOfTile.X = 48;
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(15, 30);
                    animationPlaysOnce = true;
                    DrawRectangle.X = _originalPosition.X + TMBAW_Game.Tilesize / 4;
                    DrawRectangle.Y = _originalPosition.Y - TMBAW_Game.Tilesize;
                    Interactable = new Chest(this);
                    LetsLightThrough = true;
                    break;
                case TileType.MarbleBrick: // Marble Brick
                    CurrentBorderType = BorderType.BorderAlways;
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
                    Interactable = new Spikes();
                    LetsLightThrough = true;
                    break;
                case TileType.Water: //water
                    _hasTopAndBottomPattern = true;
                    IsSolidTopDown = true;
                    if (GameWorld.WorldData.IsTopDown)
                    {
                        _positionInSpriteSheet = new Vector2(4, 16);
                        if (SubId == 1)
                        {
                            _positionInSpriteSheet = new Vector2(4, 15);
                            DrawRectangle.Y = _originalPosition.Y - 32;
                            _sizeOfTile.Y = 64;
                        }
                    }
                    else
                    {
                        _frameCount = new Vector2(4, 0);
                        _hasRandomStartingPoint = true;
                        Interactable = new Water();
                        _positionInSpriteSheet = new Vector2(4, 15);
                        if (SubId == 1)
                            _positionInSpriteSheet = new Vector2(17, 24);
                    }

                    break;
                case TileType.Lava: //lava
                    Interactable = new Lava();
                    animationSpeed = 1000;
                    _hasTopAndBottomPattern = true;
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
                    _hasLeftAndRightPattern = true;
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
                    _sizeOfTile.X = 32 * 4;
                    _sizeOfTile.Y = 32 * 6;

                    DrawRectangle.Y = _originalPosition.Y - _sizeOfTile.Y + 32;
                    DrawRectangle.X = _originalPosition.X - _sizeOfTile.X / 2;
                    _positionInSpriteSheet = new Vector2(16, 0);
                    LetsLightThrough = true;
                    break;
                case TileType.SmallRock: //Small Rock
                    _positionInSpriteSheet = new Vector2(13, 18);
                    LetsLightThrough = true;
                    break;
                case TileType.BigRock: //Big Rock
                    _frameCount = new Vector2(0, 0);
                    _sizeOfTile.X = 64;
                    _sizeOfTile.Y = 64;
                    DrawRectangle.Y = _originalPosition.Y - 32;
                    _positionInSpriteSheet = new Vector2(14, 17);
                    LetsLightThrough = true;
                    break;
                case TileType.MediumRock: //Medium Rock
                    _positionInSpriteSheet = new Vector2(11, 18);
                    _sizeOfTile.X = 64;
                    LetsLightThrough = true;
                    break;
                case TileType.Sign: //Sign
                    _positionInSpriteSheet = new Vector2(12, 4);
                    LetsLightThrough = true;
                    break;
                case TileType.Checkpoint: //Checkpoint
                    LetsLightThrough = true;
                    _sizeOfTile.Y = 32 * 3;
                    _sizeOfTile.X = 48;
                    DrawRectangle.X -= 16;
                    _frameCount = new Vector2(15, 0);
                    animationPlaysOnce = true;
                    _positionInSpriteSheet = new Vector2(22, 30);
                    Interactable = new CheckPoint(this);
                    _hasAddedEntity = true;
                    break;
                case TileType.StoneBrick: //Stone Brick
                    IsSolid = true;
                    startingPoint = new Vector2(0, 10);
                    CurrentBorderType = BorderType.BorderAlways;
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Snow: //Ice
                    IsSolid = true;
                    CurrentBorderType = BorderType.BorderAlways;
                    startingPoint = new Vector2(4, 10);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.SnowyGrass: //Snow Covered Grass
                    IsSolid = true;
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(8, 10);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.CompressedVoid: //Void tile
                    IsSolid = true;
                    CurrentBorderType = BorderType.BorderAlways;
                    startingPoint = new Vector2(16, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.FlameSpitter: // Flamespitter
                    _frameCount = new Vector2(8, 0);
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(12, 29);
                    LetsLightThrough = true;
                    Interactable = new FlameSpitter(this);
                    animationResets = true;
                    animationPlaysOnce = true;
                    break;
                case TileType.MachineGun: // Machine Gun
                    _frameCount = new Vector2(8, 0);
                    IsSolid = true;
                    _positionInSpriteSheet = new Vector2(12, 28);
                    LetsLightThrough = true;
                    break;
                case TileType.Cactus: // Cacti
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.X = 64;
                    _sizeOfTile.Y = 64;
                    switch (TMBAW_Game.Random.Next(0, 4))
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
                    _sizeOfTile.X = 64;
                    DrawRectangle.Width = _sizeOfTile.X;
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
                    Interactable = new Crystal(this, 3);
                    LetsLightThrough = true;
                    break;
                case TileType.HeliauraCrystal: // Yellow crystal.
                    _frameCount = new Vector2(4, 0);
                    _positionInSpriteSheet = new Vector2(20, 29);
                    Interactable = new Crystal(this, 1);
                    LetsLightThrough = true;
                    break;
                case TileType.SentistractSludge: // Green sludge.
                    _frameCount = new Vector2(6, 0);
                    _positionInSpriteSheet = new Vector2(14, 27);
                    Interactable = new Crystal(this, 2);
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
                    Interactable = new Crystal(this, 3);
                    LetsLightThrough = true;
                    break;
                case TileType.RubyCrystal: // Ruby Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(22, 25);
                    Interactable = new Crystal(this, 4);
                    LetsLightThrough = true;
                    break;
                case TileType.EmeraldCrystal: // Emerald Crystal.
                    _frameCount = new Vector2(1, 0);
                    _positionInSpriteSheet = new Vector2(21, 25);
                    Interactable = new Crystal(this, 2);
                    LetsLightThrough = true;
                    break;
                case TileType.Skull: // Skull.
                    _positionInSpriteSheet = new Vector2(22, 24);
                    LetsLightThrough = true;
                    break;
                case TileType.Stalagmite: // Stalagmite
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(23, 24);
                    LetsLightThrough = true;
                    break;
                case TileType.Mud: // Mud.
                    CurrentBorderType = BorderType.BorderNonSolid;
                    IsSolid = true;
                    startingPoint = new Vector2(4, 29);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.MarbleDoor: // Portal.
                    _positionInSpriteSheet = new Vector2(128, 480) / 16;
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 3 * 32;
                    _sizeOfTile.X = 64;
                    Interactable = new BackgroundDoor(this);
                    LetsLightThrough = true;
                    break;
                case TileType.Bed: // Bed.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 64;
                    _sizeOfTile.X = 3 * 32;
                    _positionInSpriteSheet = new Vector2(10, 30);
                    LetsLightThrough = true;
                    break;
                case TileType.Bookshelf: // Bookshelf.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 3 * 32;
                    _sizeOfTile.X = 64;
                    _positionInSpriteSheet = new Vector2(13, 30);
                    LetsLightThrough = true;
                    break;
                case TileType.Painting: // Painting.
                    _frameCount = new Vector2(1, 0);
                    _sizeOfTile.Y = 64;
                    _sizeOfTile.X = 64;
                    _positionInSpriteSheet = new Vector2(10, 32);
                    LetsLightThrough = true;
                    break;
                case TileType.TreeofKnowledge: // Tree of Knowledge
                    _sizeOfTile.X = 50 * 2;
                    _sizeOfTile.Y = 25 * 32;
                    //Texture = ContentHelper.LoadTexture("Tiles/tree of knowledge big");
                    _positionInSpriteSheet = new Vector2(0, 0);

                    DrawRectangle.Y = _originalPosition.Y - _sizeOfTile.Y;
                    DrawRectangle.X = _originalPosition.X - _sizeOfTile.X / 2;
                    LetsLightThrough = true;
                    break;
                case TileType.TreeBark: // Tree Bark
                    CurrentBorderType = BorderType.BorderAlways;
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
                    CurrentBorderType = BorderType.BorderAlways;
                    startingPoint = new Vector2(28, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.Glass:
                    IsSolid = true;
                    LetsLightThrough = true;
                    CurrentBorderType = BorderType.BorderAlways; ;
                    startingPoint = new Vector2(32, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.SteelBeam:
                    IsSolid = true;
                    switch (TMBAW_Game.Random.Next(0, 5))
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
                    switch (TMBAW_Game.Random.Next(0, 5))
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
                    CurrentBorderType = BorderType.BorderAlways;
                    IsSolid = true;
                    startingPoint = new Vector2(24, 14);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.EmeraldVase:
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(25, 24);
                    LetsLightThrough = true;
                    DrawRectangle.Y -= TMBAW_Game.Tilesize;
                    break;
                case TileType.RubyVase:
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(24, 24);
                    LetsLightThrough = true;
                    DrawRectangle.Y -= TMBAW_Game.Tilesize;
                    break;
                case TileType.SapphireVase:
                    _sizeOfTile.Y = 64;
                    _positionInSpriteSheet = new Vector2(26, 24);
                    LetsLightThrough = true;
                    DrawRectangle.Y -= TMBAW_Game.Tilesize;
                    break;
                case TileType.MushroomDecor:
                    _positionInSpriteSheet = new Vector2(8, 17);
                    LetsLightThrough = true;
                    break;
                case TileType.ReinforcedWood:
                    CurrentBorderType = BorderType.BorderAlways;
                    IsSolid = true;
                    startingPoint = new Vector2(32, 14);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.FutureBrick:
                    CurrentBorderType = BorderType.BorderAlways;
                    IsSolid = true;
                    startingPoint = new Vector2(36, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.FireHydrant:
                    _positionInSpriteSheet = new Vector2(16, 17);
                    _sizeOfTile.Y = 64;
                    LetsLightThrough = true;
                    break;
                case TileType.LampPost:
                    _positionInSpriteSheet = new Vector2(554 / 16, 400 / 16);
                    LetsLightThrough = true;
                    _sizeOfTile.Y = 32 * 5;
                    break;
                case TileType.WallLamp:
                    _positionInSpriteSheet = new Vector2(22, 9);
                    LetsLightThrough = true;
                    break;
                case TileType.BillBoard:
                    _positionInSpriteSheet = new Vector2(464 / 16, 384 / 16);
                    _sizeOfTile.X = 80 * 2;
                    _sizeOfTile.Y = 64 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.PalmTree:
                    _positionInSpriteSheet = new Vector2(560 / 16, 384 / 16);
                    _sizeOfTile.X = 64 * 2;
                    _sizeOfTile.Y = 96 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.ReinforcedWoodPlatform:
                    _hasLeftAndRightPattern = true;
                    switch (SubId)
                    {
                        case 0: // Base
                            _positionInSpriteSheet = new Vector2(256 / 16, 512 / 16);
                            break;
                        case 1: // Right
                            _positionInSpriteSheet = new Vector2(256 / 16 + 1, 512 / 16);
                            break;
                        case 2: // Left
                            _positionInSpriteSheet = new Vector2(256 / 16 - 1, 512 / 16);
                            break;
                    }
                    IsSolid = true;
                    LetsLightThrough = true;
                    CurrentCollisionType = CollisionType.FromAbove;
                    break;
                case TileType.SingleCrate:
                    _positionInSpriteSheet = new Vector2(208 / 16, 528 / 16);
                    LetsLightThrough = true;
                    break;
                case TileType.MultipleCrates:
                    _positionInSpriteSheet = new Vector2(288 / 16, 512 / 16);
                    _sizeOfTile.X = 32 * 2;
                    _sizeOfTile.Y = 32 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.WoodenChair:
                    _positionInSpriteSheet = new Vector2(192 / 16, 512 / 16);
                    _sizeOfTile.Y = 32 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.PressurePlate:
                    _positionInSpriteSheet = new Vector2(224 / 16, 528 / 16);
                    LetsLightThrough = true;
                    break;
                case TileType.CobWebSmall:
                    _positionInSpriteSheet = new Vector2(240 / 16, 528 / 16);
                    LetsLightThrough = true;
                    break;
                case TileType.CobWebMedium:
                    _positionInSpriteSheet = new Vector2(256 / 16, 528 / 16);
                    _sizeOfTile.X = 32 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.CobWebLarge:
                    _positionInSpriteSheet = new Vector2(432 / 16, 384 / 16);
                    _sizeOfTile.X = 32 * 2;
                    _sizeOfTile.Y = 32 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.Minecart:
                    _positionInSpriteSheet = new Vector2(448 / 16, 448 / 16);
                    _sizeOfTile.X = 32 * 2;
                    _sizeOfTile.Y = 32 * 2;
                    LetsLightThrough = true;
                    break;
                case TileType.SittingSkeleton:
                    _positionInSpriteSheet = new Vector2(384 / 16, 448 / 16);
                    _sizeOfTile.Y = 32 * 2;
                    _sizeOfTile.X = (int)(32 * 1.5);
                    LetsLightThrough = true;
                    break;
                case TileType.ClosedMineshaftDoor:
                    _positionInSpriteSheet = new Vector2(512 / 16, 176 / 16);
                    _sizeOfTile.X = 32 * 3;
                    _sizeOfTile.Y = 32 * 3;
                    LetsLightThrough = true;
                    break;
                case TileType.EnemySpawner:
                    _positionInSpriteSheet = new Vector2(224 / 16, 144 / 16);
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    break;
                case TileType.WoodenWindow:
                    _positionInSpriteSheet = new Vector2(560 / 16, 176 / 16);
                    _sizeOfTile.X = 32 * 2;
                    _sizeOfTile.Y = 32 * 3;
                    LetsLightThrough = true;
                    break;
                case TileType.WhitePaintedWindow:
                    _positionInSpriteSheet = new Vector2(560 / 16, 128 / 16);
                    _sizeOfTile.X = 32 * 2;
                    _sizeOfTile.Y = 32 * 3;
                    LetsLightThrough = true;
                    break;
                case TileType.Shelf:
                    _positionInSpriteSheet = new Vector2(336, 528) / 16;
                    _sizeOfTile.X = 32 * 3;
                    _sizeOfTile.Y = 32 * 1;
                    LetsLightThrough = true;
                    break;
                case TileType.Nightstand:
                    _positionInSpriteSheet = new Vector2(416, 432) / 16;
                    _sizeOfTile.X = 32 * 2;
                    _sizeOfTile.Y = 32 * 3;
                    LetsLightThrough = true;
                    break;

                #region Wall Textures

                case TileType.GoldBrickWall: //Gold Brick Wall
                    CurrentBorderType = BorderType.BorderAlways;
                    startingPoint = new Vector2(4, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    IsSolid = true;
                    break;
                case TileType.StoneWall: //Stone Wall
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(20, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    IsSolid = true;
                    break;
                case TileType.DirtWall: //Dirt Wall
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(0, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    IsSolid = true;
                    break;
                case TileType.Fence: //Fences
                    _hasTopAndBottomPattern = true;
                    switch (SubId)
                    {
                        case 0: //Plain
                            _positionInSpriteSheet = new Vector2(12, 7);
                            break;
                        case 1: //Top point
                            _positionInSpriteSheet = new Vector2(12, 6);
                            break;
                        case 2: goto case 0;
                    }
                    LetsLightThrough = true;
                    break;
                case TileType.MarbleWall: //Marble wall
                    CurrentBorderType = BorderType.BorderAlways;
                    startingPoint = new Vector2(12, 19);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    IsSolid = true;
                    break;
                case TileType.SandWall: // Sand Wall
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(4, 24);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    IsSolid = true;
                    break;
                case TileType.HellstoneWall: //Hellstone Wall
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(0, 24);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    IsSolid = true;
                    break;
                case TileType.StoneBrickWall: //Stone Brick Wall
                    CurrentBorderType = BorderType.BorderAlways;
                    startingPoint = new Vector2(8, 19);
                    IsSolid = true;
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.MesaWall: // Mesa Wall
                    CurrentBorderType = BorderType.BorderNonSolid;
                    startingPoint = new Vector2(0, 29);
                    IsSolid = true;
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
                    _sizeOfTile.X = 10 * 32;
                    _sizeOfTile.Y = 10 * 32;
                    _positionInSpriteSheet = new Vector2(24, 5);

                    DrawRectangle.Y = _originalPosition.Y - _sizeOfTile.Y;
                    DrawRectangle.X = _originalPosition.X - _sizeOfTile.X / 2;
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
                case TileType.MosaicWall:
                    CurrentBorderType = BorderType.BorderAlways;
                    IsSolid = true;
                    startingPoint = new Vector2(28, 14);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.ReinforcedWoodWall:
                    CurrentBorderType = BorderType.BorderAlways;
                    IsSolid = true;
                    startingPoint = new Vector2(36, 14);
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(startingPoint);
                    break;
                case TileType.DialogueActivator:
                    _positionInSpriteSheet = new Vector2(12, 9);
                    Interactable = new DialogueActivator(this);
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    break;
                case TileType.ReinforcedWoodDoor:
                    _positionInSpriteSheet = new Vector2(272 / 16, 240 / 16);
                    Interactable = new BackgroundDoor(this);
                    LetsLightThrough = true;
                    _sizeOfTile.X = 32 * 3;
                    _sizeOfTile.Y = 32 * 4;
                    break;

                #endregion

                case TileType.Player: //Player
                    if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
                    {
                        _positionInSpriteSheet = new Vector2(17, 12);
                        foreach (Player player in GameWorld.GetPlayers())
                        {
                            player.RespawnPos = new Vector2(DrawRectangle.X, DrawRectangle.Y);
                        }
                    }
                    else if (GameWorld.IsTestingLevel)
                    {

                    }
                    else
                    {
                        if (!_hasAddedEntity)
                        {
                            List<Player> players = GameWorld.GetPlayers();
                            for (int i = 0; i < players.Count; i++)
                            {
                                players[i].Initialize(DrawRectangle.X + i * 10, DrawRectangle.Y);
                            }
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
                    _positionInSpriteSheet = new Vector2(21, 12);
                    break;
                case TileType.NPC: // NPC
                    if (!_isSampleTile)
                        new NonPlayableCharacter(this);
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(288 / 16, 208 / 16);
                    break;
                case TileType.Lost: //Lost
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new Lost(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(304, 192) / 16;
                    break;
                case TileType.Hellboar: //Hellboar
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new Hellboar(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(320, 192) / 16;
                    break;
                case TileType.StoneGolem: //Hellboar
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new StoneGolem(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(18, 12);
                    break;
                case TileType.FallingBoulder: //Falling Boulder
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new FallingBoulder(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(19, 13);
                    break;
                case TileType.SwingingAxe:
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new SwingingAxe(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(592 / 16, 80 / 16);
                    _sizeOfTile.X = 32 * 3;
                    _sizeOfTile.Y = 32 * 7;
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
                case TileType.GunCommonEnemy:
                    if (!_isSampleTile)
                        //GameWorld.AddEntityAt(TileIndex, new GunCommon(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(21, 12);
                    break;
                case TileType.TheIllusionist:
                    if (!_isSampleTile)
                        GameWorld.AddEntityAt(TileIndex, new TheIllusionist(DrawRectangle.X, DrawRectangle.Y));
                    LetsLightThrough = true;
                    _isInvisibleInPlayMode = true;
                    _isInvisibleInEditMode = true;
                    _positionInSpriteSheet = new Vector2(21, 12);
                    break;

                case TileType.Shadow:
                    _positionInSpriteSheet = GetPositionInSpriteSheetOfConnectedTextures(new Vector2(640, 304) / 16);
                    break;
            }

            #endregion

            DefineSourceRectangle();
            DefineDrawRectangle();
            _startingRectangle = SourceRectangle;

            if (_hasRandomStartingPoint)
            {
                var randX = TMBAW_Game.Random.Next(0, (int)_frameCount.X);
                SourceRectangle.X += randX * SmallTileSize;
                CurrentFrame += randX;
            }
        }

        public void ReadMetaData()
        {
            if (IsWall) return;
            Interactable?.ReadMetaData(this);
        }

        /// <summary>
        ///     Takes all the variables given in DefineTexture method and returns the appropriate source rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineSourceRectangle()
        {
            //return new Rectangle((int)(startingPosition.X * SmallTileSize), (int)(startingPosition.Y * SmallTileSize), (int)(SmallTileSize * sizeOfTile.X), (int)(SmallTileSize * sizeOfTile.Y));
            SourceRectangle = new Rectangle((int)(_positionInSpriteSheet.X * SmallTileSize),
                (int)(_positionInSpriteSheet.Y * SmallTileSize), _sizeOfTile.X / 2,
                _sizeOfTile.Y / 2);
        }

        /// <summary>
        ///     Takes all the variables given in DefineTexture method and returns the appropriate draw rectangle.
        /// </summary>
        /// <returns></returns>
        private void DefineDrawRectangle()
        {
            if (_isSampleTile && !IsBrushTile)
            {
                var width = _sizeOfTile.X;
                var height = _sizeOfTile.Y;

                if (width > height)
                {
                    width = TMBAW_Game.Tilesize;
                    height = (int)(TMBAW_Game.Tilesize / _sizeOfTile.X);
                }
                if (height > width)
                {
                    width = (int)(TMBAW_Game.Tilesize / _sizeOfTile.Y);
                    height = TMBAW_Game.Tilesize;
                }
                if (height == width)
                {
                    width = TMBAW_Game.Tilesize;
                    height = TMBAW_Game.Tilesize;
                }
                // Console.WriteLine("Name:{0}, Width:{1}, Height:{2}", name, width, height);
                DrawRectangle = new Rectangle(DrawRectangle.X, DrawRectangle.Y, width, height);
            }
            else
                DrawRectangle = new Rectangle(DrawRectangle.X, DrawRectangle.Y, _sizeOfTile.X,
                    _sizeOfTile.Y);
        }

        /// <summary>
        ///     This updates the animation of the tile.
        /// </summary>
        public void Update()
        {
            OnTileUpdate?.Invoke(this);
            if (!_isSampleTile && TMBAW_Game.CurrentGameMode == GameMode.Play)
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
                        _frameTimer += TMBAW_Game.GameTime.ElapsedGameTime.TotalMilliseconds;
                        _restartTimer += TMBAW_Game.GameTime.ElapsedGameTime.TotalMilliseconds;

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
            var gameTime = TMBAW_Game.GameTime;

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
            if (TMBAW_Game.CurrentGameMode == GameMode.Edit)
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
            _hasLeftAndRightPattern = false;
            IsSolid = false;
            IsSolidTopDown = false;
            SubId = 0;
            animationPlaysOnce = false;
            animationResets = false;
            animationSpeed = DefaultAnimationSpeed;
            DrawRectangle = _defaultDrawRectangle;
            _frameCount = Vector2.Zero;
            IsHidden = false;
            LetsLightThrough = false;
            _sizeOfTile = new Point(32, 32);
            DefineDrawRectangle();
            DefineSourceRectangle();
            SetToDefaultSourceRect();
            CurrentCollisionType = CollisionType.All;
            Light = null;

            //DefineTexture();
            //LightingEngine.UpdateLightAt(TileIndex);
        }

        public void DrawRipples(SpriteBatch spriteBatch)
        {
            if (Id == TileType.Water && Texture != null && !GameWorld.WorldData.IsTopDown)
                spriteBatch.Draw(Texture, DrawRectangle, new Rectangle(SourceRectangle.X, SourceRectangle.Y + 16, 16, 16), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                if (_isInvisibleInPlayMode && TMBAW_Game.CurrentGameMode == GameMode.Play)
                    return;
                if (_isInvisibleInEditMode && TMBAW_Game.CurrentGameMode == GameMode.Edit)
                    return;
                else if (!IsHidden)
                {
                    Interactable?.Draw(spriteBatch, this);
                    spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color * _opacity);

                }
            }
            if (_cornerPieces.Count != 0)
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
            if (TMBAW_Game.CurrentGameMode == GameMode.Edit && IsWall)
            {
                spriteBatch.Draw(GameWorld.SpriteSheet, new Rectangle(_originalPosition.X, _originalPosition.Y, TMBAW_Game.Tilesize, TMBAW_Game.Tilesize), _gridSourceRectangle, Color.CornflowerBlue * .5f);
            }
        }

        public void DrawWallShadow(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                Vector2 start = GetPositionInSpriteSheetOfShadowTextures(new Vector2(640, 304) / 16);
                Rectangle sourceRect = new Rectangle((int)start.X * 16, (int)start.Y * 16, 16, 16);
                spriteBatch.Draw(Texture, DrawRectangle, sourceRect, Color.White);
                foreach (var tileShadow in _shadowCornerPieces)
                {
                    tileShadow.Draw(spriteBatch);
                }
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
                spriteBatch.Draw(Texture, new Rectangle(DrawRectangle.X + 2, DrawRectangle.Y + 2, DrawRectangle.Width, DrawRectangle.Height), SourceRectangle, Color.Black * .4f);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            Light?.Draw(spriteBatch);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            Light?.DrawGlow(spriteBatch);
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
            _cornerPieces.Clear();

            // i.e Wallpaper.
            if (_hasTopAndBottomPattern)
            {
                var tileAbove = GameWorld.GetTileAbove(TileIndex, IsWall);
                var tileBelow = GameWorld.GetTileBelow(TileIndex, IsWall);
                if (tileAbove.Id != Id)
                {
                    SubId = 1;
                }
                else if (tileBelow.Id != Id)
                {
                    SubId = 2;
                }
                else SubId = 0;
            }

            // i.e Marble Floor
            else if (_hasLeftAndRightPattern)
            {
                if (GameWorld.GetTile(TileIndex + 1, IsWall).Id != Id)
                    SubId = 1;
                else if (GameWorld.GetTile(TileIndex - 1, IsWall).Id != Id)
                    SubId = 2;
            }

            switch (CurrentBorderType)
            {
                case BorderType.None:
                    break;
                case BorderType.BorderAlways:
                    ConnectWithSelf();
                    break;
                case BorderType.BorderNonSolid:
                    ConnectWithSolids();
                    break;
                case BorderType.StarboundLike:
                    break;
                default:
                    break;
            }

            if (IsWall)
            {
                ConnectShadows();
            }

        }

        private void ConnectShadows()
        {
            _shadowCornerPieces.Clear();


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

            var topLeft = GameWorld.GetTile(tl, false);
            var top = GameWorld.GetTile(t, false);
            var topRight = GameWorld.GetTile(tr, false);
            var midLeft = GameWorld.GetTile(ml, false);
            var mid = GameWorld.GetTile(m, true);
            var midRight = GameWorld.GetTile(mr, false);
            var botLeft = GameWorld.GetTile(bl, false);
            var bot = GameWorld.GetTile(b, false);
            var botRight = GameWorld.GetTile(br, false);

            if (topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                shadowSubId = 9;

            if (topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                !botRight.IsSolid)
                shadowSubId = 9;

            if (topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                !botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                shadowSubId = 0;

            if (!topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                shadowSubId = 0;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 14;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 19;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 12;

            if (topLeft.IsSolid &&
                top.IsSolid &&
                !topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                shadowSubId = 0;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 18;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 0;

            if (top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 16;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 13;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 6;

            if (top.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 11;

            if (top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 4;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                shadowSubId = 17;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 10;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 15;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 8;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                shadowSubId = 5;

            // ALL BLACK BEHIND TILES
            if (GameWorld.GetTile(TileIndex).IsSolid)
                shadowSubId = 20;


            if (topLeft.IsSolid &&
                !top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = TileType.Shadow,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 3
                };
                _shadowCornerPieces.Add(corner);
            }

            if (topRight.IsSolid &&
                !top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = TileType.Shadow,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 7
                };
                _shadowCornerPieces.Add(corner);
            }

            if (botLeft.IsSolid &&
                !top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = TileType.Shadow,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 2
                };
                _shadowCornerPieces.Add(corner);
            }

            if (botRight.IsSolid &&
               !top.IsSolid &&
               !midLeft.IsSolid &&
               !midRight.IsSolid &&
               !bot.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = TileType.Shadow,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 1
                };
                _shadowCornerPieces.Add(corner);
            }

            foreach (var corners in _shadowCornerPieces)
            {
                corners.DefineTexture();
            }
        }

        private void ConnectWithSolids()
        {
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

            if (topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                SubId = 0;

            if (topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                !botRight.IsSolid)
                SubId = 0;

            if (topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                !botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                SubId = 0;

            if (!topLeft.IsSolid &&
                top.IsSolid &&
                topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                SubId = 0;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
                SubId = 4;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
                SubId = 5;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                SubId = 6;

            if (topLeft.IsSolid &&
                top.IsSolid &&
                !topRight.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                botLeft.IsSolid &&
                bot.IsSolid &&
                botRight.IsSolid)
                SubId = 0;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
                SubId = 8;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 9;

            if (top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                SubId = 10;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                SubId = 11;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 12;

            if (top.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 13;

            if (top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 14;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                bot.IsSolid)
                SubId = 15;

            if (!top.IsSolid &&
                !midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 16;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 17;

            if (!top.IsSolid &&
                midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 18;

            if (top.IsSolid &&
                !midLeft.IsSolid &&
                !midRight.IsSolid &&
                !bot.IsSolid)
                SubId = 19;

            //Special
            if (!botRight.IsSolid &&
                midRight.IsSolid &&
                bot.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    Texture = Texture,
                    SubId = 1
                };
                _cornerPieces.Add(corner);
            }

            if (!botLeft.IsSolid &&
                midLeft.IsSolid &&
                bot.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 2
                };
                _cornerPieces.Add(corner);
            }

            if (!topLeft.IsSolid &&
                midLeft.IsSolid &&
                top.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 3
                };
                _cornerPieces.Add(corner);
            }

            if (!topRight.IsSolid &&
                midRight.IsSolid &&
                top.IsSolid)
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 7
                };
                _cornerPieces.Add(corner);
            }

            foreach (var corners in _cornerPieces)
            {
                corners.DefineTexture();
            }
        }

        private void ConnectWithSelf()
        {
            //Default Connected Textures Pattern
            //"Please don't change this was a headache to make." -Lucas 2015

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
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 1
                };
                _cornerPieces.Add(corner);
            }

            if (!botLeft.Equals(mid) &&
                midLeft.Equals(mid) &&
                bot.Equals(mid))
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 2
                };
                _cornerPieces.Add(corner);
            }

            if (!topLeft.Equals(mid) &&
                midLeft.Equals(mid) &&
                top.Equals(mid))
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 3
                };
                _cornerPieces.Add(corner);
            }

            if (!topRight.Equals(mid) &&
                midRight.Equals(mid) &&
                top.Equals(mid))
            {
                var corner = new Tile(DrawRectangle.X, DrawRectangle.Y)
                {
                    Id = mid.Id,
                    DrawRectangle = DrawRectangle,
                    Texture = Texture,
                    SubId = 7
                };
                _cornerPieces.Add(corner);
            }

            foreach (var corners in _cornerPieces)
            {
                corners.DefineTexture();
            }
        }

        public void AddRandomlyGeneratedDecoration(Tile[] array, int mapWidth)
        {
            // Ignore decoration when on top down mode.
            if (GameWorld.WorldData.IsTopDown)
                return;

            // Add snow cover if it is snowing, otherwise add flowers and grass.
            if (GameWorld.WorldData.IsSnowing)
            {
                if (!IsWall)
                {
                    if (IsSolid)
                    {
                        var tileAbove = GameWorld.GetTileAbove(TileIndex);
                        if (tileAbove.Id == TileType.Air)
                        {
                            tileAbove.Id = TileType.SnowCover;
                            tileAbove.DefineTexture();
                        }
                    }
                }
            }
            else
            {

                //Add decoration on top of grass tile.
                if (Id == TileType.Grass && SubId == 5)
                {
                    var tileAbove = GameWorld.GetTileAbove(TileIndex, IsWall);
                    if (tileAbove.Id == 0)
                    {
                        var rand = TMBAW_Game.Random.Next(0, 10);
                        if (rand == 0) //flower
                        {
                            tileAbove.Id = TileType.Flower;
                        }
                        else if (rand == 1 || rand == 2) //tall grass
                        {
                            tileAbove.Id = TileType.TallGrass;
                        }
                        else //short grass
                        {
                            tileAbove.Id = TileType.ShortGrass;
                        }

                        tileAbove.DefineTexture();
                    }
                }
            }

            // Random decorations for mud.
            if (Id == TileType.Mud && SubId == 5)
            {
                var tileAbove = GameWorld.GetTileAbove(TileIndex, IsWall);
                if (tileAbove.Id == 0)
                {
                    var rand = TMBAW_Game.Random.Next(0, 100);
                    if (rand > 90)
                        tileAbove.Id = TileType.MushroomDecor;

                    tileAbove.DefineTexture();
                }
            }

            // Random decorations for sand.
            if (Id == TileType.Sand && SubId == 5)
            {
                var tileTwoAbove = GameWorld.GetTile(TileIndex - mapWidth * 2, IsWall);
                var tileTopRight = GameWorld.GetTile(TileIndex - mapWidth + 1, IsWall);
                var tileTwoAboveRight = GameWorld.GetTile(TileIndex - mapWidth * 2 + 1, IsWall);
                if (tileTwoAbove.Id == 0 && tileTopRight.Id == 0 && tileTwoAboveRight.Id == 0)
                {
                    var rand = TMBAW_Game.Random.Next(0, 100);
                    if (rand > 80)
                        tileTwoAbove.Id = TileType.Cactus;

                    tileTwoAbove.DefineTexture();
                }
            }

            // Random decoration for hellstone.
            if (Id == TileType.Hellrock && SubId == 5)
            {
                var tileAbove = GameWorld.GetTileAbove(TileIndex, IsWall);
                if (tileAbove.Id == 0)
                {
                    var rand = TMBAW_Game.Random.Next(0, 10);

                    // Skull.
                    if (rand == 0)
                    {
                        tileAbove.Id = TileType.Skull;
                    }
                    tileAbove.DefineTexture();
                }
            }

            // Hellstone stalagmmite.
            if (Id == TileType.Hellrock && SubId == 13)
            {
                if (TMBAW_Game.Random.Next(0, 5) == 1)
                {
                    var tileBelow = GameWorld.GetTileBelow(TileIndex, IsWall);
                    var tileTwoBelow = GameWorld.GetTile(TileIndex + mapWidth * 2);
                    if (tileBelow.Id == 0 && tileTwoBelow.Id == 0)
                    {
                        tileBelow.Id = TileType.Stalagmite;
                        tileBelow.DefineTexture();
                    }
                }
            }

            // Randomly generate different plain textures for certain tiles.
            // Grass
            if (Id == TileType.Grass && SubId == 0 && TMBAW_Game.Random.Next(0, 100) > 80)
            {
                switch (TMBAW_Game.Random.Next(0, 4))
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

        private Vector2 GetPositionInSpriteSheetOfShadowTextures(Vector2 startingPoint)
        {
            // Sample tiles such as the ones in the tileholders have the same sub id, but the brush tiles do not.
            var position = new Vector2();
            switch (shadowSubId)
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
                case 20: // All black
                    position = startingPoint + new Vector2(4, 4);
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
        public Point GetSize()
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
                    return .90f;
                case TileType.Snow:
                    return .95f;
                default:
                    return .90f;
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
            if (obj is Tile other)
            {
                return (other.Id == Id);
            }
            return false;
        }

        public Vector2 Center => new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y);
        public Vector2 Position => new Vector2(DrawRectangle.X, DrawRectangle.Y);
    }
}