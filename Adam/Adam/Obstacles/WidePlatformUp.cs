using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    public class WidePlatformUp : Obstacle
    {
        bool _hasStartedMoving;
        bool _hasReachedDestination;
        Vector2 _velocity;
        Rectangle _original;

        Vector2 _endPoint;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public WidePlatformUp(Player.Player player)
        {            
            player.PlayerRespawned += player_PlayerRespawned;
            Texture = ContentHelper.LoadTexture("Tiles/Obstacles/goldbrick_platform_wide");

            IsCollidable = true;
        }

        /// <summary>
        /// For when the platform location is found before the endpoint of the platform.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetStartPoint(int x, int y)
        {
            CollRectangle = new Rectangle(x, y, Main.Tilesize * 12, Main.Tilesize);
            _original = DrawRectangle;
        }

        /// <summary>
        /// For when the end of the platform is found before the platform.
        /// </summary>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        public void SetEndPoint(int endX, int endY)
        {
            _endPoint = new Vector2(endX, endY);
        }

        void player_PlayerRespawned()
        {
            _hasStartedMoving = false;
            _hasReachedDestination = false;
            CollRectangle = _original;
        }

        public override void Update()
        {
            base.Update();

            CollRectangle.Y += (int)_velocity.Y;

            if (!_hasStartedMoving)
            {
                if (CollRectangle.Intersects(Player.GetCollRectangle()))
                    _hasStartedMoving = true;
            }

            if (_hasStartedMoving)
            {
                if (_hasReachedDestination)
                {
                    _velocity.Y = 0;
                    CollRectangle.Y = (int)_endPoint.Y;
                }
                else
                {
                    _velocity.Y = -1f;

                    if (CollRectangle.Y <= (int)_endPoint.Y)
                    {
                        _hasReachedDestination = true;
                    }
                }
            }
        }

    }
}
