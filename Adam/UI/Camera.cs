using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ThereMustBeAnotherWay.UI.Level_Editor;

namespace ThereMustBeAnotherWay
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
        Vector3 _lastVelocity;
        private Viewport _viewport;
        public int TileIndex;

        float _zoom = 1;
        bool _updateInstantly = false;
        int _lastScrollWheel;

        Vector2 _prefRes;
        Vector2 _defRes;
        private GameTimer _shakeTimer = new GameTimer(true);
        public Vector3 Velocity;
        public Vector3 LastCameraLeftCorner;
        public Vector2 CenterGameCoords;
        public Vector2 LeftTopGameCoords;
        public Vector2 InvertedCoordsBeforeShake;

        /// <summary>
        /// Set to true if the camera cannot move in the x-direction from its set position.
        /// </summary>
        public bool LockedX { get; set; } = false;
        /// <summary>
        /// Set to true if the camera cannot move in the y-direction from its set position.
        /// </summary>
        public bool LockedY { get; set; } = false;
        /// <summary>
        /// Returns true if the camera is currently zoomed out from its default zoom.
        /// </summary>
        /// <returns></returns>
        public bool IsZoomedOut()
        {
            return _zoom < 1;
        }
        public bool RestricedToGameWorld { get; set; } = true;

        public Camera(Viewport newViewport)
        {
            _viewport = newViewport;
            _defRes = new Vector2(TMBAW_Game.DefaultResWidth, TMBAW_Game.DefaultResHeight);
            _prefRes = new Vector2(TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight);
            Velocity = new Vector3(0, 0, 0);
            TileIndex = 100;
        }

        public override Vector2 GetPosition()
        {
            return LeftTopGameCoords;
        }

        public override void SetPosition(Vector2 position)
        {
            LastCameraLeftCorner = new Vector3(-position.X, -position.Y, 0);
            base.SetPosition(position);
        }

        /// <summary>
        /// Updates the camera by following the rectangle given in a smooth manner.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="active">Should the camera follow the rectangle.</param>
        public void UpdateSmoothly(Rectangle rectangle, int width, int height, bool active)
        {
            Vector3 currentLeftCorner;
            // If the camera is not active, which is used to show the player is dead, the camera stops moving.
            if (active)
            {
                Vector2 playerPos = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
                currentLeftCorner = new Vector3(-playerPos.X + _defRes.X / _zoom / 2, -playerPos.Y + (3 * _defRes.Y / _zoom / 5), 0);
            }
            else
            {
                currentLeftCorner = LastCameraLeftCorner;
            }

            if (LockedX)
            {
                currentLeftCorner.X = LastCameraLeftCorner.X;
            }
            if (LockedY)
            {
                currentLeftCorner.Y = LastCameraLeftCorner.Y;
            }
            if (RestricedToGameWorld)
            {
                if (currentLeftCorner.X > 0)
                    currentLeftCorner.X = 0;
                if (currentLeftCorner.X < -(width * TMBAW_Game.Tilesize - _defRes.X))
                    currentLeftCorner.X = -(width * TMBAW_Game.Tilesize - _defRes.X);
                if (currentLeftCorner.Y > 0)
                    currentLeftCorner.Y = 0;
                if (currentLeftCorner.Y < -(height * TMBAW_Game.Tilesize - _defRes.Y))
                    currentLeftCorner.Y = -(height * TMBAW_Game.Tilesize - _defRes.Y);
            }

            if (InputSystem.IsKeyDown(Keys.OemMinus))
                ButtonBar.MinusButton_MouseClicked(null);
            if (InputSystem.IsKeyDown(Keys.OemPlus))
                ButtonBar.PlusButton_MouseClicked(null) ;
            if (InputSystem.IsKeyDown(Keys.R))
                ResetZoom();

            Velocity = (currentLeftCorner - LastCameraLeftCorner) / 5;
            Vector3 cameraLeftCorner = LastCameraLeftCorner;

            // Used if the camera zoom changed.
            if (_updateInstantly)
            {
                cameraLeftCorner = currentLeftCorner;
                _updateInstantly = false;
            }
            else
            {
                cameraLeftCorner += Velocity;
            }

            CenterGameCoords = new Vector2(-currentLeftCorner.X, -currentLeftCorner.Y);

            LeftTopGameCoords = CenterGameCoords;

            CenterGameCoords.X += TMBAW_Game.DefaultResWidth / 2;
            CenterGameCoords.Y += TMBAW_Game.DefaultResHeight * 2 / 3;
            TileIndex = (int)((int)CenterGameCoords.Y / TMBAW_Game.Tilesize * GameWorld.WorldData.LevelWidth) + (int)((int)CenterGameCoords.X / TMBAW_Game.Tilesize);


            LastCameraLeftCorner = cameraLeftCorner;
            _lastVelocity = Velocity;

            int shakeOffset = 1;
            if (_shakeTimer.TimeElapsedInMilliSeconds < 100)
            {
                switch (TMBAW_Game.Random.Next(0, 5))
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
            SetZoomTo(_zoom + .005f);
        }

        //Resets the zoom to its default value.
        public void ResetZoom()
        {
            SetZoomTo(1);
        }

        /// <summary>
        /// Zoom Out slowly.
        /// </summary>
        public void ZoomOut()
        {
            SetZoomTo(_zoom - .005f);
        }

        /// <summary>
        /// Changes the current zoom to a new zoom.
        /// </summary>
        /// <param name="newZoom"></param>
        public void SetZoomTo(float newZoom)
        {
            if (newZoom < .25)
                newZoom = .25f;
            if (newZoom > 2)
                newZoom = 2;
            _updateInstantly = true;
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
