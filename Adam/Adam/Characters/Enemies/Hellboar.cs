using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    class Hellboar : Enemy , ICollidable
    {

        public Hellboar(int x, int y)
        {
            health = 100;
            collRectangle = new Rectangle(x, y, 50 * 2, 38 * 2);
            drawRectangle = collRectangle;
            sourceRectangle = new Rectangle(0, 0, 50, 38);
            texture = ContentHelper.LoadTexture("Enemies/hellboar");


            base.Initialize();
        }

        public override void Update(Player player, GameTime gameTime, List<Entity> entities, Map map)
        {
            drawRectangle = collRectangle;
            //damageBox = new Rectangle(collRectangle.X - 5, collRectangle.Y - 10, collRectangle.Width + 10, collRectangle.Height / 2);

            velocity.Y += .3f;

            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 15, collRectangle.Width, collRectangle.Height - 20);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, collRectangle.Width - 20, collRectangle.Height);

            //CheckTerrainCollision(map);

            base.Update(player, gameTime, entities, map);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }


        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.rectangle.Y - collRectangle.Height;
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            collRectangle.Y = e.Tile.rectangle.Y - collRectangle.Height;
            velocity.Y = 0;
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
        }
    }
}
