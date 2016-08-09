using Adam.Levels;
using Adam.Misc;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    public class Camera : UiElement
    {
        private Matrix _translation;
        public Matrix Translate
        {
            get { return _translation; }
        }

        //leave center for later
        public Vector2 CenterPos;
        private Vector2 _lastCenterPos;
        Vector3 _lastVelocity;
        private Viewport _viewport;
        public int TileIndex;

        float _zoom = 1;
        int _lastScrollWheel;

        Vector2 _prefRes;
        Vector2 _defRes;
        private Timer _shakeTimer = new Timer();
        public Vector3 Velocity;
        public Vector3 LastCameraLeftCorner;
        public Vector2 CenterGameCoords;
        public Vector2 LeftTopGameCoords;
        public Vector2 InvertedCoordsBeforeShake;

        public Camera(Viewport newViewport)
        {
            _viewport = newViewport;
            _defRes = new Vector2(Main.DefaultResWidth, Main.DefaultResHeight);
            _prefRes = new Vector2(Main.UserResWidth, Main.UserResHeight);
            Velocity = new Vector3(0, 0, 0);
            TileIndex = 100;
        }

        public void UpdateSmoothly(Rectangle rectangle, int width, int height, bool active)
        {
            Vector2 playerPos = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
            Vector3 currentLeftCorner = new Vector3(-playerPos.X + _defRes.X / _zoom / 2, -playerPos.Y + (3 * _defRes.Y / _zoom / 5), 0);

            if (currentLeftCorner.X > 0)
                currentLeftCorner.X = 0;
            if (currentLeftCorner.X < -(width * Main.Tilesize - _defRes.X))
                currentLeftCorner.X = -(width * Main.Tilesize - _defRes.X);
            if (currentLeftCorner.Y > 0)
                currentLeftCorner.Y = 0;
            if (currentLeftCorner.Y < -(height * Main.Tilesize - _defRes.Y))
                currentLeftCorner.Y = -(height * Main.Tilesize - _defRes.Y);

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

            Velocity = (currentLeftCorner - LastCameraLeftCorner) * 50;
            Vector3 cameraLeftCorner = LastCameraLeftCorner;
            cameraLeftCorner += Velocity * Main.TimeSinceLastUpdate;
            cameraLeftCorner = new Vector3((int)cameraLeftCorner.X, (int)cameraLeftCorner.Y, 0);

            // Make sure mult of 2.
            if (cameraLeftCorner.X % 2 != 0)
            {
                cameraLeftCorner.X++;
            }
            if (cameraLeftCorner.Y % 2 != 0)
            {
                cameraLeftCorner.Y++;
            }
            if (cameraLeftCorner.Z % 2 != 0)
            {
                cameraLeftCorner.Z++;
            }

            CenterGameCoords = new Vector2(-currentLeftCorner.X, -currentLeftCorner.Y);

            LeftTopGameCoords = CenterGameCoords;
            //LeftTopGameCoords.X -= Main.DefaultResWidth;
            //LeftTopGameCoords.Y -= Main.DefaultResHeight;

            CenterGameCoords.X += Main.DefaultResWidth / 2;
            CenterGameCoords.Y += Main.DefaultResHeight * 2 / 3;
            TileIndex = (int)((int)CenterGameCoords.Y / Main.Tilesize * GameWorld.WorldData.LevelWidth) + (int)((int)CenterGameCoords.X / Main.Tilesize);


            LastCameraLeftCorner = cameraLeftCorner;
            _lastVelocity = Velocity;

            int shakeOffset = 1;
            if (_shakeTimer.TimeElapsedInMilliSeconds < 100)
            {
                switch (Main.Random.Next(0, 5))
                {
                    case 0:
                        cameraLeftCorner.X += shakeOffset;
                        break;
                    case 1:
                        cameraLeftCorner.X -= shakeOffset;
                        break;
                    case 2:
                        cameraLeftCorner.Y += shakeOffset;
                        break;
                    case 3:
                        cameraLeftCorner.Y -= shakeOffset;
                        break;
                }
            }




            _translation = Matrix.CreateTranslation(cameraLeftCorner) * Matrix.CreateScale(new Vector3(_zoom, _zoom, 0));
        }


        //Slowly zooom in.
        public void ZoomIn()
        {
            _zoom += .005f;
            if (_zoom > 2)
                _zoom = 2;
        }

        //Resets the zoom to its default value.
        public void ResetZoom()
        {
            _zoom = 1;
        }

        /// <summary>
        /// Zoom Out slowly.
        /// </summary>
        public void ZoomOut()
        {
            _zoom -= .05f;
            if (_zoom < 0.25)
                _zoom = 0.25f;
        }

        /// <summary>
        /// Changes the current zoom to a new zoom.
        /// </summary>
        /// <param name="newZoom"></param>
        public void SetZoomTo(float newZoom)
        {
            _zoom = newZoom;
        }

        /// <summary>
        /// Returns the current zoom.
        /// </summary>
        /// <returns></returns>
        public float GetZoom()
        {
            return _zoom;
        }

        public void Shake()
        {
            _shakeTimer.Reset();
        }

        public void UpdateWithZoom(Vector2 position)
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            if (scrollWheel < _lastScrollWheel)
            {
                _zoom += .05f;
            }
            if (scrollWheel > _lastScrollWheel)
            {
                _zoom -= .05f;
            }
            _lastScrollWheel = scrollWheel;

            CenterPos = new Vector2(position.X, position.Y);
            _translation = Matrix.CreateTranslation(new Vector3(-CenterPos.X + ((int)_defRes.X / 2), -CenterPos.Y + (3 * (int)_defRes.Y / 5), 0))
                    * Matrix.CreateScale(new Vector3(_zoom, _zoom, 0));
        }

    }
}
