using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    public class Camera
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

        public Camera(Viewport newViewport)
        {
            viewport = newViewport;
            defRes = new Vector2(Main.DefaultResWidth,Main.DefaultResHeight);
            prefRes = new Vector2(Main.UserResWidth, Main.UserResHeight) ;
            velocity = new Vector3(0, 0, 0);
            tileIndex = 100;
        }

        public void UpdateSmoothly(Rectangle rectangle, int width, int height, bool active)
        {
            Vector2 playerPos = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
            Vector3 currentLeftCorner = new Vector3(-playerPos.X + defRes.X / zoom / 2, -playerPos.Y + (3 * defRes.Y / zoom / 5), 0);

            if (currentLeftCorner.X > 0)
                currentLeftCorner.X = 0;
            if (currentLeftCorner.X < -(width * Main.Tilesize - defRes.X))
                currentLeftCorner.X = -(width * Main.Tilesize - defRes.X);
            if (currentLeftCorner.Y > 0)
                currentLeftCorner.Y = 0;
            if (currentLeftCorner.Y < -(height * Main.Tilesize - defRes.Y))
                currentLeftCorner.Y = -(height * Main.Tilesize - defRes.Y);

            //if (zoom > 1)
            //{
            //    translation = Matrix.CreateTranslation(new Vector3(-playerPos.X + ((int)defRes.X /zoom/ 2), -playerPos.Y + (3 * (int)defRes.Y /zoom/ 5), 0))
            //        * Matrix.CreateScale(new Vector3(zoom, zoom, 0));

            //    return;
            //}
            if (InputHelper.IsKeyDown(Keys.OemPlus))
                SetZoomTo(.5f);
            if (InputHelper.IsKeyDown(Keys.OemMinus))
                ResetZoom();
            if (InputHelper.IsKeyDown(Keys.R))
                ResetZoom();

            velocity = (currentLeftCorner - lastCameraLeftCorner) / 8;
            Vector3 cameraLeftCorner = lastCameraLeftCorner;
            cameraLeftCorner += velocity;
            cameraLeftCorner = new Vector3((int)cameraLeftCorner.X, (int)cameraLeftCorner.Y, 0);

            // Make sure mult of 2.
            if (cameraLeftCorner.X % 2 != 0)
            {
                cameraLeftCorner.X++;
            }
            if (cameraLeftCorner.Y % 2!= 0)
            {
                cameraLeftCorner.Y++;
            }
            if (cameraLeftCorner.Z % 2 != 0)
            {
                cameraLeftCorner.Z++;
            }

            inverted = new Vector2(-currentLeftCorner.X, -currentLeftCorner.Y);
            inverted.X += Main.DefaultResWidth / 2;
            inverted.Y += Main.DefaultResHeight * 2 / 3;
            tileIndex = (int)((int)inverted.Y / Main.Tilesize * GameWorld.Instance.worldData.LevelWidth) + (int)((int)inverted.X / Main.Tilesize);

            lastCameraLeftCorner = cameraLeftCorner;
            lastVelocity = velocity;


            translation = Matrix.CreateTranslation(cameraLeftCorner) * Matrix.CreateScale(new Vector3(zoom, zoom, 0));
        }


        //Slowly zooom in.
        public void ZoomIn()
        {
            zoom += .005f;
            if (zoom > 2)
                zoom = 2;
        }

        //Resets the zoom to its default value.
        public void ResetZoom()
        {
            zoom = 1;
        }

        /// <summary>
        /// Zoom Out slowly.
        /// </summary>
        public void ZoomOut()
        {
            zoom -= .05f;
            if (zoom < 0.25)
                zoom = 0.25f;
        }

        /// <summary>
        /// Changes the current zoom to a new zoom.
        /// </summary>
        /// <param name="newZoom"></param>
        public void SetZoomTo(float newZoom)
        {
            zoom = newZoom;
        }

        /// <summary>
        /// Returns the current zoom.
        /// </summary>
        /// <returns></returns>
        public float GetZoom()
        {
            return zoom;
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
