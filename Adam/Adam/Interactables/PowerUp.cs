using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    class PowerUp : Entity
    {
        public bool wasPickedUp;
        public bool toDelete;
        protected Vector2 velocity;
        protected Rectangle topMidBound;
        protected double elapsedTime;
        protected SoundEffect loop;
        protected SoundEffectInstance loopInstance;
        int tileIndex;
        public bool IsCollidable { get; set; }
        protected double effectTimer;
        protected List<Particle> effects = new List<Particle>();

        public PowerUp() { }

        public virtual void Update(GameTime gameTime, Player player, GameWorld map)
        {
            this.gameWorld = map;
            drawRectangle.X += (int)velocity.X;
            drawRectangle.Y += (int)velocity.Y;

            //They are supposed to bounce up an down but it is not working right....

            topMidBound = new Rectangle(drawRectangle.X + drawRectangle.Width / 2, drawRectangle.Y + drawRectangle.Height / 2, 1, 1);
            xRect = new Rectangle(drawRectangle.X, drawRectangle.Y + 5, drawRectangle.Width, drawRectangle.Height - 10);
            yRect = new Rectangle(drawRectangle.X + 10, drawRectangle.Y, drawRectangle.Width - 20, drawRectangle.Height);
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (IsCollidable)
                CheckCollisionWithTerrain();

            if (player.collRectangle.Intersects(drawRectangle) && elapsedTime > 500)
            {
                wasPickedUp = true;
                toDelete = true;
                if (loopInstance != null)
                    loopInstance.Stop();
            }

            float xDist = player.collRectangle.Center.X - drawRectangle.Center.X;
            float yDist = player.collRectangle.Center.Y - drawRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);
            if (loopInstance != null)
            {
                if (loopInstance.State == SoundState.Stopped)
                    loopInstance.Play();

                if (distanceTo > 1000)
                    loopInstance.Volume = 0;
                else loopInstance.Volume = .5f - (distanceTo / 1000) / 2;
            }

            foreach (var eff in effects)
            {
                eff.Update(gameTime);
                if (eff.ToDelete())
                {
                    effects.Remove(eff);
                    break;
                }
            }
        }

        private void CheckCollisionWithTerrain()
        {
            //Simple collision check with terrain that I am too lazy to put into a collision class
            if (velocity.X == 0 && velocity.Y == 0) { }
            else
            {
                tileIndex = (int)(topMidBound.Y / Game1.Tilesize * gameWorld.worldData.mainMap.Width) + (int)(topMidBound.X / Game1.Tilesize);

                int[] q = new int[9];
                q[0] = tileIndex - gameWorld.worldData.mainMap.Width - 1;
                q[1] = tileIndex - gameWorld.worldData.mainMap.Width;
                q[2] = tileIndex - gameWorld.worldData.mainMap.Width + 1;
                q[3] = tileIndex - 1;
                q[4] = tileIndex;
                q[5] = tileIndex + 1;
                q[6] = tileIndex + gameWorld.worldData.mainMap.Width - 1;
                q[7] = tileIndex + gameWorld.worldData.mainMap.Width;
                q[8] = tileIndex + gameWorld.worldData.mainMap.Width + 1;

                //test = q;

                //check the tiles around the goldOre for collision
                foreach (int quadrant in q)
                {
                    if (quadrant >= 0 && quadrant <= gameWorld.tileArray.Length - 1 && gameWorld.tileArray[quadrant].isSolid == true)
                    {
                        if (yRect.Intersects(gameWorld.tileArray[quadrant].rectangle))
                        {
                            if (drawRectangle.Y < gameWorld.tileArray[quadrant].rectangle.Y) //hits bot
                            {
                                drawRectangle.Y = gameWorld.tileArray[quadrant].rectangle.Y - drawRectangle.Height;
                                velocity.Y = -velocity.Y * .9f;
                                velocity.X = 0;
                            }
                            if (drawRectangle.Y > gameWorld.tileArray[quadrant].rectangle.Y) //hits top
                            {
                                velocity.Y = -velocity.Y * .9f;
                                velocity.X = 0;
                                drawRectangle.Y = gameWorld.tileArray[quadrant].rectangle.Y + drawRectangle.Height + 1;
                            }
                        }
                        if (xRect.Intersects(gameWorld.tileArray[quadrant].rectangle))
                        {
                            if (drawRectangle.X < gameWorld.tileArray[quadrant].rectangle.X) //hits right
                            {
                                velocity.X = -velocity.X * .9f;
                                velocity.Y = velocity.Y * .9f;
                                drawRectangle.X = gameWorld.tileArray[quadrant].rectangle.X - drawRectangle.Width - 1;
                            }
                            if (drawRectangle.X > gameWorld.tileArray[quadrant].rectangle.X) //hits left
                            {
                                velocity.X = -velocity.X * .9f;
                                velocity.Y = velocity.Y * .9f;
                                drawRectangle.X = gameWorld.tileArray[quadrant].rectangle.X + gameWorld.tileArray[quadrant].rectangle.Width + 1;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) texture = Game1.DefaultTexture;
            spriteBatch.Draw(texture, drawRectangle, Color.White);
        }
    }
}
