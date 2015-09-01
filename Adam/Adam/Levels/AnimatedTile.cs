using Adam;
using Adam.Lights;
using Adam.Obstacles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{

    public class SpecialTile : Tile
    {
        double frameTimer, restartTimer;
        int restartWait;
        int currentFrame, switchFrame;
        bool hasRandomStartingPoint;
        Vector2 frameCount;
        Vector2 startingPosition;
        Vector2 size;
        Rectangle startingRectangle;
        Rectangle originalPosition;
        Liquid liquid;
        Chest chest;
        FlameSpitter flameSpitter;
        MachineGun machineGun;
        Tile sourceTile;


        /// <summary>
        /// For tiles that are not 16x16, have animations, or interact differently with the player.
        /// </summary>
        /// <param name="sourceTile"></param>
        public SpecialTile(Tile sourceTile)
        {
            texture = GameWorld.SpriteSheet;
            this.sourceTile = sourceTile;
            ID = sourceTile.ID;
            drawRectangle = sourceTile.drawRectangle;
            originalPosition = base.drawRectangle;
            TileIndex = sourceTile.TileIndex;
            DefineTexture();
        }

        public override void DefineTexture()
        {
            startingPosition = new Vector2(0, 0);
            size = new Vector2(1, 1);

            switch (ID)
            {
                case 7: //short grass
                    frameCount = new Vector2(4, 0);
                    sunlightPassesThrough = true;
                    break;
                case 9: //tall Grass
                    frameCount = new Vector2(12, 0);
                    sunlightPassesThrough = true;
                    break;
                case 8: //Metal
                    frameCount = new Vector2(4, 0);
                    break;
                case 11: //Torch
                    frameCount = new Vector2(4, 0);
                    size.Y = 2;
                    drawRectangle.Height = Main.Tilesize * 2;
                    break;
                case 12: //Chandelier
                    frameCount = new Vector2(4, 0);
                    size.X = 2;
                    drawRectangle.Width = Main.Tilesize * 2;
                    break;
                case 17: //Daffodyls
                    frameCount = new Vector2(4, 0);
                    size.Y = 2;
                    drawRectangle.Height = Main.Tilesize * 2;
                    drawRectangle.Y = originalPosition.Y - Main.Tilesize;
                    sunlightPassesThrough = true;
                    break;
                case 19: //Chest
                    chest = new Chest(this);
                    frameCount = new Vector2(6, 0);
                    size.X = 1.5f;
                    drawRectangle.Width = (int)(Main.Tilesize * size.X);
                    break;
                case 23: //Water
                    liquid = new Liquid(drawRectangle.X, drawRectangle.Y, Liquid.Type.Water);
                    frameCount = new Vector2(4, 0);
                    hasRandomStartingPoint = true;
                    break;
                case 24: //Lava
                    liquid = new Liquid(drawRectangle.X, drawRectangle.Y, Liquid.Type.Lava);
                    frameCount = new Vector2(4, 0);
                    hasRandomStartingPoint = true;
                    sunlightPassesThrough = true;
                    break;
                case 25: //Poison
                    liquid = new Liquid(drawRectangle.X, drawRectangle.Y, Liquid.Type.Poison);
                    frameCount = new Vector2(4, 0);
                    hasRandomStartingPoint = true;
                    break;
                case 31: //Tree
                    frameCount = new Vector2(0, 0);
                    size.X = 6;
                    size.Y = 7;
                    drawRectangle.Height = Main.Tilesize * 7;
                    drawRectangle.Width = Main.Tilesize * 6;
                    drawRectangle.Y = originalPosition.Y - (32 * 6);
                    drawRectangle.X = originalPosition.X - (16 * 5);
                    sunlightPassesThrough = true;
                    break;
                case 33: //Big Rock
                    frameCount = new Vector2(0, 0);
                    size.X = 2;
                    size.Y = 2;
                    drawRectangle.Height = Main.Tilesize * 2;
                    drawRectangle.Width = Main.Tilesize * 2;
                    drawRectangle.Y = originalPosition.Y - 32;
                    break;
                case 34: //Small Rock
                    frameCount = new Vector2(0, 0);
                    size.X = 2;
                    drawRectangle.Width = Main.Tilesize * 2;
                    break;
                case 42: // Flame Spitter.
                    frameCount = new Vector2(8, 0);
                    flameSpitter = new FlameSpitter(sourceTile);
                    break;
                case 43: // Machine Gun.
                    frameCount = new Vector2(8, 0);
                    machineGun = new MachineGun(sourceTile);
                    break;
                case 44: // Cactus.
                    frameCount = new Vector2(1, 0);
                    size.X = 2;
                    size.Y = 2;
                    drawRectangle.Width = 2 * Main.Tilesize;
                    drawRectangle.Height = 2 * Main.Tilesize;
                    break;
                case 48: // Blue Crystal.
                    frameCount = new Vector2(2, 0);
                    break;
                case 49: // Yellow Crystal.
                    frameCount = new Vector2(4, 0);
                    break;
                case 50: // Green Sludge.
                    frameCount = new Vector2(6, 0);
                    break;
                case 51: // Void Fire Spitter.
                    frameCount = new Vector2(4, 0);
                    break;

            }
            startingPosition = sourceTile.GetPositionInSpriteSheet();
            sourceRectangle = DefineSourceRectangle();

            // The rectangle the source rectangle returns to after the animation is complete.
            startingRectangle = sourceRectangle;

            // Defines a starting point for the animation if you do not want to have all tiles synchronized.
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
        private Rectangle DefineSourceRectangle()
        {
            return new Rectangle((int)(startingPosition.X * SmallTileSize), (int)(startingPosition.Y * SmallTileSize), (int)(SmallTileSize * size.X), (int)(SmallTileSize * size.Y));
        }

        /// <summary>
        /// Takes all the variables given in DefineTexture method and returns the appropriate draw rectangle.
        /// </summary>
        /// <returns></returns>
        private Rectangle DefineDrawRectangle()
        {
            return new Rectangle(drawRectangle.X, drawRectangle.Y, (int)(drawRectangle.Width * size.X * Main.Tilesize), (int)(drawRectangle.Height * size.Y * Main.Tilesize));
        }

        public void Animate(GameTime gameTime)
        {
            liquid?.Update(gameTime);
            chest?.Update();
            flameSpitter?.Update(gameTime, GameWorld.Instance.GetPlayer(), GameWorld.Instance);
            machineGun?.Update(gameTime, GameWorld.Instance.GetPlayer(), GameWorld.Instance);

            switch (ID)
            {
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
                    sourceRectangle.X += SmallTileSize;
                    currentFrame++;
                }
            }

            if (currentFrame >= frameCount.X)
            {
                currentFrame = 0;
                sourceRectangle.X = startingRectangle.X;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White);
        }
    }
}
