using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    public class Liquid : Obstacle
    {
        double particleTimer;
        double restartTime;
        bool isOnTop;

        public enum Type
        {
            Water, Poison, Lava
        }
        Type CurrentType;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(collRectangle.X + 8, collRectangle.Y + 8, 32, 32);
            }
        }

        public Liquid(Tile sourceTile, Type type)
        {
            collRectangle = new Rectangle(sourceTile.drawRectangle.X + 8, sourceTile.drawRectangle.Y + 8, Main.Tilesize / 2, Main.Tilesize / 2);
            particleTimer = GameWorld.RandGen.Next(0, 8) * GameWorld.RandGen.NextDouble();
            restartTime = GameWorld.RandGen.Next(5, 8) * GameWorld.RandGen.NextDouble();
            if (restartTime < 1)
                restartTime = 1;
            CurrentType = type;

            sourceTile.OnTileUpdate += Update;
            sourceTile.OnTileDestroyed += SourceTile_OnTileDestroyed;
        }

        private void SourceTile_OnTileDestroyed(Tile t)
        {
            t.OnTileUpdate -= Update;
            t.OnTileDestroyed -= SourceTile_OnTileDestroyed;
        }

        public void Update(Tile t)
        {
            GameWorld gameWorld = GameWorld.Instance;
            this.player = GameWorld.Instance.player;

            if (collRectangle.Intersects(GameWorld.Instance.player.GetCollRectangle()))
            {
                player.KillAndRespawn();
            }

            //if (!isOnTop)
            //    return;

            if (CurrentType == Type.Lava)
            {
                if (GameWorld.Instance.tileArray[GetTileIndex() - GameWorld.Instance.worldData.LevelWidth].ID == 0)
                {
                    particleTimer += GameWorld.Instance.GetGameTime().ElapsedGameTime.TotalSeconds;
                    if (particleTimer > restartTime)
                    {
                        Particle par = new Particle();
                        par.CreateLavaParticle(this, gameWorld);
                        gameWorld.particles.Add(par);
                        particleTimer = 0;
                    }
                }
            }
            

        }

        public void CheckOnTop(Tile[] array, GameWorld gameWorld)
        {
            //this.gameWorld = gameWorld;
            //int indexAbove = TileIndex - gameWorld.worldData.LevelWidth;
            //if (array[indexAbove].ID == 0)
            //{
            //    this.isOnTop = true;
            //}
        }
    }
}
