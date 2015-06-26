using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    class Lava : Obstacle
    {
        double particleTimer;
        double restartTime;
        bool isOnTop;


        public Lava(int x, int y)
        {
            collRectangle = new Rectangle(x, y, Game1.Tilesize, Game1.Tilesize);
            particleTimer = GameWorld.RandGen.Next(0, 8);
            restartTime = GameWorld.RandGen.Next(5, 8);

        }

        public override void Update(GameTime gameTime, Player player, GameWorld map)
        {
            base.Update(gameTime, player, map);

            if (IsTouching)
            {
                player.KillAndRespawn();
            }

            if (!isOnTop)
                return;

            particleTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (particleTimer > restartTime)
            {
                Particle par = new Particle();
                par.CreateLavaParticle(this, map);
                map.particles.Add(par);
                particleTimer = 0;
            }
        }

        public void CheckOnTop(Tile[] array, GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
            int indexAbove = TileIndex - gameWorld.worldData.mainMap.Width;
            if (array[indexAbove].ID == 0)
            {
                this.isOnTop = true;
                Console.WriteLine("Tile above: " + array[indexAbove].ID +" "+array[TileIndex].ID+ " "+isOnTop );
            }
        }
    }
}
