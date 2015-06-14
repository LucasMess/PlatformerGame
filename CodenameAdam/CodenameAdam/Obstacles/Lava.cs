using CodenameAdam;
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Blank because it is in the animated tile.
        }
    }
}
