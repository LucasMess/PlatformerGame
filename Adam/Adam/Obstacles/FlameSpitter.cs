using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;

namespace Adam.Obstacles
{
    class FlameSpitter : Obstacle
    {
        Timer firingTimer = new Timer();
        Timer particleTimer = new Timer();
        bool isFlaming;
        Rectangle sourceRectangleOfParticle;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public FlameSpitter(Tile sourceTile)
        {
            
            collRectangle = sourceTile.drawRectangle;
        }

        public override void Update(GameTime gameTime, Player player, GameWorld map)
        {
            CheckIfChangingState();

            attackBox = new Rectangle(collRectangle.X, collRectangle.Y - (Main.Tilesize * 4), Main.Tilesize, Main.Tilesize * 4);

            if (isFlaming)
            {
                particleTimer.Increment();
                if (particleTimer.TimeElapsedInMilliSeconds > 100)
                {
                    FlameParticle par = new FlameParticle(this, Color.LightBlue);
                    FlameParticle par2 = new FlameParticle(this, Color.Red);
                    FlameParticle par3 = new FlameParticle(this, Color.Yellow);
                    GameWorld.Instance.particles.Add(par);
                    GameWorld.Instance.particles.Add(par2);
                    GameWorld.Instance.particles.Add(par3);
                    particleTimer.Reset();
                }
            }

            if (isFlaming && attackBox.Intersects(player.GetCollRectangle()))
            {
                player.TakeDamageAndKnockBack(EnemyDB.FlameSpitter_TouchDamage);
                player.IsOnFire = true;
            }


            base.Update(gameTime, player, map);
        }

        /// <summary>
        /// Updates timer and determines whether the flamespitter is firing or not.
        /// </summary>
        private void CheckIfChangingState()
        {
            firingTimer.Increment();

            if (!isFlaming)
            {
                if (firingTimer.TimeElapsedInMilliSeconds > 3000)
                {
                    isFlaming = true;
                    firingTimer.Reset();
                }
            }
            else
            {
                if (firingTimer.TimeElapsedInMilliSeconds > 3000)
                {
                    isFlaming = false;
                    firingTimer.Reset();
                }
            }
        }


    }
}
