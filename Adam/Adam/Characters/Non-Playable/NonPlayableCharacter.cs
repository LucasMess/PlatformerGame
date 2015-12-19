using Adam.Network;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Characters.Non_Playable
{
    public abstract class NonPlayableCharacter : Entity
    {
        bool _destinationFound;
        bool _toTheRight;
        double _destinationTimer;
        int _destinationX;
        GameTime _gameTime;
        Player.Player _player;
        protected bool CanTalk;
        protected bool IsTalking;

        KeyPopUp _key;
        public NonPlayableCharacter()
        {
            Texture = Main.DefaultTexture;

            _key = new KeyPopUp();
        }

        public virtual void Update(GameTime gameTime, Player.Player player)
        {
            _key.Update(CollRectangle);

            CollRectangle.X += (int)Velocity.X;
            CollRectangle.Y += (int)Velocity.Y;

            this._player = player;
            this._gameTime = gameTime;
            base.Update();

            if (CanTalk)
                CheckForPlayer();
        }

        private void CheckForPlayer()
        {
            if (CollRectangle.Intersects(_player.GetCollRectangle()))
            {
                if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) && !IsTalking)
                {
                    IsTalking = true;
                    ShowMessage();
                }
            }
        }

        protected virtual void ShowMessage()
        {

        }

        protected void WalkAroundSpawnPoint(int spawnX)
        {
            if (Session.IsActive && !Session.IsHost)
                return;

            int speed = 1;
            if (IsTalking)
            {
                Velocity.X = 0;
                return;
            }
            if (!_destinationFound)
            {
                Velocity.X = 0;
                _destinationTimer += _gameTime.ElapsedGameTime.TotalSeconds;
                if (_destinationTimer > 2)
                {
                    _destinationTimer = 0;
                    _destinationX = GameWorld.RandGen.Next(-3, 4);
                    _destinationX *= Main.Tilesize;
                    _destinationX += spawnX;
                    _destinationFound = true;

                    if (_destinationX > CollRectangle.X)
                    {
                        _toTheRight = true;
                    }
                    else _toTheRight = false;
                }
            }

            if (_destinationFound)
            {
                if (_toTheRight)
                {
                    Velocity.X = speed;
                    if (CollRectangle.X > _destinationX)
                    {
                        _destinationFound = false;
                    }
                }
                else
                {
                    Velocity.X = -speed;
                    if (CollRectangle.X < _destinationX)
                    {
                        _destinationFound = false;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsTalking)
                _key.Draw(spriteBatch);
        }
    }
}
