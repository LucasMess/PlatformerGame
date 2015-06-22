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
        bool isOnTop;

        public Lava(int x, int y)
        {
            collRectangle = new Rectangle(x, y, Game1.Tilesize, Game1.Tilesize);
        }

        public override void Update(GameTime gameTime, Player player, Map map)
        {
            base.Update(gameTime, player, map);

            if (IsTouching)
            {
                player.KillAndRespawn();
                //TODO: Add player.PlayBurningSound();
            }

            particleTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (particleTimer > Map.randGen.Next(3,8) && map.particles.Count < 1000)
            {
                Particle par = new Particle();
                par.CreateLavaParticle(this, map);
                map.particles.Add(par);
                particleTimer = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Blank because it is in the animated tile.
        }
    }
}
