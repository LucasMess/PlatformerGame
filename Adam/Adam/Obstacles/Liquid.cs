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

        public Liquid(int x, int y, Type type)
        {
            collRectangle = new Rectangle(x + 8, y + 8, Main.Tilesize / 2, Main.Tilesize / 2);
            particleTimer = GameWorld.RandGen.Next(0, 8) * GameWorld.RandGen.NextDouble();
            restartTime = GameWorld.RandGen.Next(5, 8) * GameWorld.RandGen.NextDouble();
            if (restartTime < 1)
                restartTime = 1;
            CurrentType = type;
        }

        public void Update(GameTime gameTime)
        {
            this.gameWorld = GameWorld.Instance;
            this.player = gameWorld.player;

            if (collRectangle.Intersects(GameWorld.Instance.player.collRectangle))
            {
                player.KillAndRespawn();
            }

            //if (!isOnTop)
            //    return;

            if (CurrentType == Type.Lava)
            {
                if (GameWorld.Instance.tileArray[GetTileIndex() - GameWorld.Instance.worldData.LevelWidth].ID == 0)
                {
                    particleTimer += gameTime.ElapsedGameTime.TotalSeconds;
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
            //    this.gameWorld = gameWorld;
            //    int indexAbove = TileIndex - gameWorld.worldData.LevelWidth;
            //    if (array[indexAbove].ID == 0)
            //    {
            //        this.isOnTop = true;
            //    }
        }
    }
}
