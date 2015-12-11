//using Adam;
//using Adam.Interactables;
//using Adam.Lights;
//using Adam.Obstacles;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Adam
//{

//    public class SpecialTile : Tile
//    {
//        double frameTimer, restartTimer;
//        int restartWait;
//        int currentFrame, switchFrame;
//        bool hasRandomStartingPoint;
//        Vector2 frameCount;
//        Vector2 startingPosition;
//        Vector2 size;
//        Rectangle startingRectangle;
//        Rectangle originalPosition;
//        Liquid liquid;
//        Chest chest;
//        FlameSpitter flameSpitter;
//        MachineGun machineGun;
//        Crystal crystal;
//        Apple apple;
//        Tile sourceTile;
//        public Portal portal;



//        /// <summary>
//        /// For tiles that are not 16x16, have animations, or interact differently with the player.
//        /// </summary>
//        /// <param name="sourceTile"></param>
//        public SpecialTile(Tile sourceTile)
//        {
//            texture = GameWorld.SpriteSheet;
//            this.sourceTile = sourceTile;
//            ID = sourceTile.ID;
//            drawRectangle = sourceTile.drawRectangle;
//            originalPosition = base.drawRectangle;
//            TileIndex = sourceTile.TileIndex;
//            DefineTexture();
//        }

//        public override void DefineTexture()
//        {
//            startingPosition = new Vector2(0, 0);
//            size = new Vector2(1, 1);

//            switch (ID)
//            {
//                case 7: //short grass
                    
//                    sunlightPassesThrough = true;
//                    break;
//                case 9: //tall Grass
                    
//                    sunlightPassesThrough = true;
//                    break;
//                case 8: //Metal
                    
//                    break;
//                case 11: //Torch
                    
//                    drawRectangle.Height = Main.Tilesize * 2;
//                    break;
//                case 12: //Chandelier
                    
//                    drawRectangle.Width = Main.Tilesize * 2;
//                    sunlightPassesThrough = true;
//                    break;
//                case 17: //Daffodyls
                    
//                    drawRectangle.Height = Main.Tilesize * 2;
                    
//                    sunlightPassesThrough = true;
//                    break;
//                case 19: //Chest
                    
//                    drawRectangle.Width = (int)(Main.Tilesize * size.X);
//                    break;
//                case 23: //Water
                    
//                    break;
//                case 24: //Lava
                    
//                    sunlightPassesThrough = true;
//                    break;
//                case 25: //Poison
                   
//                    break;
//                case 26: // Golden Apple.
                   
//                    break;
//                case 31: //Tree
                    
//                    sunlightPassesThrough = true;
//                    break;
//                case 33: //Big Rock
                    
//                    break;
//                case 34: //Small Rock
//                    frameCount = new Vector2(0, 0);
//                    size.X = 2;
//                    drawRectangle.Width = Main.Tilesize * 2;
//                    break;
//                case 42: // Flame Spitter.
                    
//                    break;
//                case 43: // Machine Gun.
                    
//                    break;
//                case 44: // Cactus.
                    
//                    drawRectangle.Width = (int)size.X * Main.Tilesize;
//                    drawRectangle.Height = (int)size.Y * Main.Tilesize;
//                    break;
//                case 48: // Blue Crystal.
                    
//                    break;
//                case 49: // Yellow Crystal.
                    
//                    break;
//                case 50: // Green Sludge.
                   
//                    break;
//                case 51: // Void Fire Spitter.
                    
//                    break;
//                case 52: // Sapphire Crystal.
                    
//                    break;
//                case 53: // Ruby Crystal.
                    
//                    break;
//                case 54: // Emerald Crystal.
                    
//                    break;
//                case 56: // Stalagmite.
                    
//                    drawRectangle.Height = (int)size.Y * Main.Tilesize;
//                    break;
//                case 58: // Portal.
                    
//                    break;
//                case 59: // Bed.
                    
//                    drawRectangle.Height = (int)size.Y * Main.Tilesize;
//                    drawRectangle.Width = (int)size.X * Main.Tilesize;
//                    break;
//                case 60: // Bookshelf.
                   
//                    drawRectangle.Height = (int)size.Y * Main.Tilesize;
//                    drawRectangle.Width = (int)size.X * Main.Tilesize;
//                    break;
//                case 61: // Paintings.
                   
//                    drawRectangle.Height = (int)size.Y * Main.Tilesize;
//                    drawRectangle.Width = (int)size.X * Main.Tilesize;
//                    break;

//            }
//            startingPosition = sourceTile.GetPositionInSpriteSheet();
//            sourceRectangle = DefineSourceRectangle();

//            // The rectangle the source rectangle returns to after the animation is complete.
//            startingRectangle = sourceRectangle;

//            // Defines a starting point for the animation if you do not want to have all tiles synchronized.
//            if (hasRandomStartingPoint)
//            {
//                int randX = GameWorld.RandGen.Next(0, (int)frameCount.X);
//                sourceRectangle.X += randX * SmallTileSize;
//                currentFrame += randX;
//            }

//        }
//    }
//}
