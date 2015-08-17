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
    public class PowerUp : Entity
    {
        public bool wasPickedUp;
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
            //xRect = new Rectangle(drawRectangle.X, drawRectangle.Y + 5, drawRectangle.Width, drawRectangle.Height - 10);
            //yRect = new Rectangle(drawRectangle.X + 10, drawRectangle.Y, drawRectangle.Width - 20, drawRectangle.Height);
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

            base.Update();
        }

        private void CheckCollisionWithTerrain()
        {
            //Simple collision check with terrain that I am too lazy to put into a collision public class
            if (velocity.X == 0 && velocity.Y == 0) { }
            else
            {
                tileIndex = (int)(topMidBound.Y / Main.Tilesize * gameWorld.worldData.LevelWidth) + (int)(topMidBound.X / Main.Tilesize);

                int[] q = new int[9];
                q[0] = tileIndex - gameWorld.worldData.LevelWidth - 1;
                q[1] = tileIndex - gameWorld.worldData.LevelWidth;
                q[2] = tileIndex - gameWorld.worldData.LevelWidth + 1;
                q[3] = tileIndex - 1;
                q[4] = tileIndex;
                q[5] = tileIndex + 1;
                q[6] = tileIndex + gameWorld.worldData.LevelWidth - 1;
                q[7] = tileIndex + gameWorld.worldData.LevelWidth;
                q[8] = tileIndex + gameWorld.worldData.LevelWidth + 1;

                //test = q;

                //check the tiles around the goldOre for collision
                foreach (int quadrant in q)
                {
                    if (quadrant >= 0 && quadrant <= gameWorld.tileArray.Length - 1 && gameWorld.tileArray[quadrant].isSolid == true)
                    {
                        if (yRect.Intersects(gameWorld.tileArray[quadrant].drawRectangle))
                        {
                            if (drawRectangle.Y < gameWorld.tileArray[quadrant].drawRectangle.Y) //hits bot
                            {
                                drawRectangle.Y = gameWorld.tileArray[quadrant].drawRectangle.Y - drawRectangle.Height;
                                velocity.Y = -velocity.Y * .9f;
                                velocity.X = 0;
                            }
                            if (drawRectangle.Y > gameWorld.tileArray[quadrant].drawRectangle.Y) //hits top
                            {
                                velocity.Y = -velocity.Y * .9f;
                                velocity.X = 0;
                                drawRectangle.Y = gameWorld.tileArray[quadrant].drawRectangle.Y + drawRectangle.Height + 1;
                            }
                        }
                        if (xRect.Intersects(gameWorld.tileArray[quadrant].drawRectangle))
                        {
                            if (drawRectangle.X < gameWorld.tileArray[quadrant].drawRectangle.X) //hits right
                            {
                                velocity.X = -velocity.X * .9f;
                                velocity.Y = velocity.Y * .9f;
                                drawRectangle.X = gameWorld.tileArray[quadrant].drawRectangle.X - drawRectangle.Width - 1;
                            }
                            if (drawRectangle.X > gameWorld.tileArray[quadrant].drawRectangle.X) //hits left
                            {
                                velocity.X = -velocity.X * .9f;
                                velocity.Y = velocity.Y * .9f;
                                drawRectangle.X = gameWorld.tileArray[quadrant].drawRectangle.X + gameWorld.tileArray[quadrant].drawRectangle.Width + 1;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) texture = Main.DefaultTexture;
            spriteBatch.Draw(texture, drawRectangle, Color.White);
        }
    }
}
