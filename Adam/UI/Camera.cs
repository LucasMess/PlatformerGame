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

        public Matrix HalfTranslate;

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
        private Timer _shakeTimer = new Timer(true);
        public Vector3 Velocity;
        public Vector3 LastCameraLeftCorner;
        public Vector2 CenterGameCoords;
        public Vector2 LeftTopGameCoords;
        public Vector2 InvertedCoordsBeforeShake;

        public Camera(Viewport newViewport)
        {
            _viewport = newViewport;
            _defRes = new Vector2(AdamGame.DefaultResWidth, AdamGame.DefaultResHeight);
            _prefRes = new Vector2(AdamGame.UserResWidth, AdamGame.UserResHeight);
            Velocity = new Vector3(0, 0, 0);
            TileIndex = 100;
        }

        public override Vector2 GetPosition()
        {
            return LeftTopGameCoords;
        }

        public void UpdateSmoothly(Rectangle rectangle, int width, int height, bool active)
        {
            Vector2 playerPos = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
            Vector3 currentLeftCorner = new Vector3(-playerPos.X + _defRes.X / _zoom / 2, -playerPos.Y + (3 * _defRes.Y / _zoom / 5), 0);

            if (currentLeftCorner.X > 0)
                currentLeftCorner.X = 0;
            if (currentLeftCorner.X < -(width * AdamGame.Tilesize - _defRes.X))
                currentLeftCorner.X = -(width * AdamGame.Tilesize - _defRes.X);
            if (currentLeftCorner.Y > 0)
                currentLeftCorner.Y = 0;
            if (currentLeftCorner.Y < -(height * AdamGame.Tilesize - _defRes.Y))
                currentLeftCorner.Y = -(height * AdamGame.Tilesize - _defRes.Y);

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


            //MoveTo(new Vector2(currentLeftCorner.X, currentLeftCorner.Y), 2000);


            Velocity = (currentLeftCorner - LastCameraLeftCorner) / 5;
            Vector3 cameraLeftCorner = LastCameraLeftCorner;
            cameraLeftCorner += Velocity;


            // Make sure mult of 2.
            //double mult = .5;
            //if (cameraLeftCorner.X % mult != 0)
            //{
            //    cameraLeftCorner.X = (int)(cameraLeftCorner.X*2)/ 2;
            //}
            //if (cameraLeftCorner.Y % mult != 0)
            //{
            //    cameraLeftCorner.Y = (int)(cameraLeftCorner.Y * 2) / 2;
            //}
            //if (cameraLeftCorner.Z % mult != 0)
            //{
            //    cameraLeftCorner.Z++;
            //}

            CenterGameCoords = new Vector2(-currentLeftCorner.X, -currentLeftCorner.Y);

            LeftTopGameCoords = CenterGameCoords;
            //LeftTopGameCoords.X -= Main.DefaultResWidth;
            //LeftTopGameCoords.Y -= Main.DefaultResHeight;

            CenterGameCoords.X += AdamGame.DefaultResWidth / 2;
            CenterGameCoords.Y += AdamGame.DefaultResHeight * 2 / 3;
            TileIndex = (int)((int)CenterGameCoords.Y / AdamGame.Tilesize * GameWorld.WorldData.LevelWidth) + (int)((int)CenterGameCoords.X / AdamGame.Tilesize);


            LastCameraLeftCorner = cameraLeftCorner;
            _lastVelocity = Velocity;

            int shakeOffset = 1;
            if (_shakeTimer.TimeElapsedInMilliSeconds < 100)
            {
                switch (AdamGame.Random.Next(0, 5))
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


            //cameraLeftCorner = new Vector3(DrawRectangle.X, DrawRectangle.Y, 0);
            cameraLeftCorner = new Vector3((int)cameraLeftCorner.X, (int)cameraLeftCorner.Y, 0);
            _translation = Matrix.CreateTranslation(cameraLeftCorner) * Matrix.CreateScale(new Vector3(_zoom, _zoom, 0));
            HalfTranslate = Matrix.CreateTranslation(cameraLeftCorner * 2);
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
