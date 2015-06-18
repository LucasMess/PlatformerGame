using Adam.Misc.Interfaces;
using Adam.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    class Hellboar : Enemy , ICollidable, INewtonian
    {
        bool isAngry;
        List<Rectangle> rects;

        public Hellboar(int x, int y)
        {
            health = 100;
            collRectangle = new Rectangle(x, y, 50 * 2, 38 * 2);
            drawRectangle = collRectangle;
            sourceRectangle = new Rectangle(0, 0, 50, 38);
            texture = ContentHelper.LoadTexture("Enemies/hellboar");
            singleTexture = texture;


            base.Initialize();
        }

        public override void Update(Player player, GameTime gameTime, List<Entity> entities, Map map)
        {
            this.player = player;
            base.Update(player, gameTime, entities, map);

            drawRectangle.X = collRectangle.X;
            drawRectangle.Y = collRectangle.Y;
            damageBox = new Rectangle(collRectangle.X - 5, collRectangle.Y - 10, collRectangle.Width + 10, collRectangle.Height / 2);

            int t = GetTileIndex();
            int[] i = GetNearbyTileIndexes(map);

            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 15, collRectangle.Width, collRectangle.Height - 20);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, collRectangle.Width - 20, collRectangle.Height);

            CheckForPlayer();
        }

        private void CheckForPlayer()
        {            
            if (CollisionRay.IsPlayerInSight(this, player, map, out rects))
            {
                isAngry = true;
            }
            else isAngry = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color;
            if (isAngry) color = Color.Blue; else color = Color.White;
            spriteBatch.Draw(texture, drawRectangle, color);

            if (rects != null)
            foreach (var rect in rects)
                spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), new Rectangle(rect.X,rect.Y,16,16), Color.Black);

            DrawSurroundIndexes(spriteBatch);
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


        public float GravityStrength
        {
            get { return 0; }
        }
    }
}
