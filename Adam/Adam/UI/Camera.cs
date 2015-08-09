using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    class Camera
    {
        private Matrix translation;
        public Matrix Translate
        {
            get { return translation; }
        }

        //leave center for later
        public Vector2 centerPos;
        private Vector2 lastCenterPos;
        Vector3 lastVelocity;
        private Viewport viewport;
        public int tileIndex;

        float zoom = 1;
        int lastScrollWheel;

        Vector2 prefRes;
        Vector2 defRes;
        public Vector3 velocity;
        public Vector3 lastCameraLeftCorner;
        public Vector2 inverted;

        public Camera(Viewport newViewport, Vector2 prefferedResolution, Vector2 defaultResolution)
        {
            viewport = newViewport;
            defRes = defaultResolution;
            prefRes = prefferedResolution;
            velocity = new Vector3(0, 0, 0);
            tileIndex = 100;
        }

        public void Update(Vector2 position, int newWidth, int newHeight, Player player)
        {
            centerPos = new Vector2(position.X, position.Y);


            if (player.isDead == false)
            {
                lastCenterPos = centerPos;
                translation = Matrix.CreateTranslation(new Vector3(-centerPos.X + ((int)defRes.X / 2), -centerPos.Y + (3 * (int)defRes.Y / 5), 0));
            }
            else
                translation = Matrix.CreateTranslation(new Vector3(-lastCenterPos.X + ((int)defRes.X / 2), -lastCenterPos.Y + (3 * (int)defRes.Y / 5), 0));
        }

        public void UpdateSmoothly(Rectangle rectangle, GameWorld map)
        {
            Vector2 playerPos = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
            Vector3 currentLeftCorner = new Vector3(-playerPos.X + defRes.X / 2, -playerPos.Y + (3 * defRes.Y / 5), 0);

            if (currentLeftCorner.X > 0)
                currentLeftCorner.X = 0;
            if (currentLeftCorner.X < -(map.worldData.mainMap.Width * Game1.Tilesize - defRes.X))
                currentLeftCorner.X = -(map.worldData.mainMap.Width * Game1.Tilesize - defRes.X);
            if (currentLeftCorner.Y > 0)
                currentLeftCorner.Y = 0;
            if (currentLeftCorner.Y < -(map.worldData.mainMap.Height * Game1.Tilesize - defRes.Y))
                currentLeftCorner.Y = -(map.worldData.mainMap.Height * Game1.Tilesize - defRes.Y);

            velocity = (currentLeftCorner - lastCameraLeftCorner) / 10;
            Vector3 cameraLeftCorner = lastCameraLeftCorner;
            cameraLeftCorner += velocity;
            cameraLeftCorner = new Vector3((int)cameraLeftCorner.X, (int)cameraLeftCorner.Y, 0);

            inverted = new Vector2(-currentLeftCorner.X,- currentLeftCorner.Y);
            inverted.X += Game1.DefaultResWidth / 2;
            inverted.Y += Game1.DefaultResHeight * 2 / 3;
            tileIndex = (int)((int)inverted.Y / Game1.Tilesize * map.worldData.mainMap.Width) + (int)((int)inverted.X / Game1.Tilesize);
           // tileIndex = player.playerTileIndex;

            //if (player.isDead == false)
            //{
               lastCameraLeftCorner = cameraLeftCorner;
                lastVelocity = velocity;
                translation = Matrix.CreateTranslation(cameraLeftCorner);
            //}
            //else
            //{
            //    lastCameraLeftCorner.Y += lastVelocity.Y /3;
            //    lastCameraLeftCorner = new Vector3((int)lastCameraLeftCorner.X, (int)lastCameraLeftCorner.Y, 0);
            //    translation = Matrix.CreateTranslation(lastCameraLeftCorner);
            //}

        }

        public void UpdateWithZoom(Vector2 position)
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            if (scrollWheel < lastScrollWheel)
            {
                zoom += .05f;
            }
            if (scrollWheel > lastScrollWheel)
            {
                zoom -= .05f;
            }
            lastScrollWheel = scrollWheel;

            centerPos = new Vector2(position.X, position.Y);
            translation = Matrix.CreateTranslation(new Vector3(-centerPos.X + ((int)defRes.X / 2), -centerPos.Y + (3 * (int)defRes.Y / 5), 0))
                    * Matrix.CreateScale(new Vector3(zoom, zoom, 0));
        }

    }
}
