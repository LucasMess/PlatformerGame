using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    public class Liquid : Tile
    {
        double particleTimer;
        double restartTime;
        bool isOnTop;
        public Rectangle collRectangle;

        GameWorld gameWorld;
        Player player;
        

        public enum Type
        {
            Water, Poison, Lava
        }
        Type CurrentType;

        public Liquid(int x, int y, Type type)
        {
            collRectangle = new Rectangle(x + 8, y + 8, Main.Tilesize / 2, Main.Tilesize / 2);
            particleTimer = GameWorld.RandGen.Next(0, 8);
            restartTime = GameWorld.RandGen.Next(5, 8);
            CurrentType = type;
        }

        public override void Update(GameTime gameTime)
        {
            this.gameWorld = GameWorld.Instance;
            this.player = gameWorld.player;

            if (collRectangle.Intersects(GameWorld.Instance.player.collRectangle))
            {
                player.KillAndRespawn();
            }

            if (!isOnTop)
                return;

            if (CurrentType == Type.Lava)
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

        public void CheckOnTop(Tile[] array, GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
            int indexAbove = TileIndex - gameWorld.worldData.width;
            if (array[indexAbove].ID == 0)
            {
                this.isOnTop = true;
            }
        }
    }
}
