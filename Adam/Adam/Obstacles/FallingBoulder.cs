using Adam;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Obstacles
{
    public class FallingBoulder : Obstacle, INewtonian
    {
        bool _hasFallen;
        int _originalY;

        SoundFx _fallingSound;

        public float GravityStrength { get; set; }

        public bool IsFlying { get; set; }

        public bool IsAboveTile
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public FallingBoulder(int x, int y)
        {
            GravityStrength = Main.Gravity ;
            Texture = GameWorld.SpriteSheet;
            _fallingSound = new SoundFx("Sounds/Boulder/boulder_fall", this);   

            CollRectangle = new Rectangle(x, y, Main.Tilesize * 2, Main.Tilesize * 2);
            SourceRectangle = new Rectangle(12 * 16, 26 * 16, 32, 32);
            CurrentDamageType = DamageType.Bottom;
            IsCollidable = true;
            _originalY = DrawRectangle.Y;

            CollidedWithTileBelow += OnCollisionWithTerrainBelow;
        }

        public override void Update()
        {
            base.Update();

            if (IsTouchingPlayer)
            {
                Player player = GameWorld.Instance.Player;
                player.KillAndRespawn();
            }

            // If hit ground go back up slowly.
            if (_hasFallen)
            {
                Velocity.Y = -1f;
            }
            else
            {
                GravityStrength = Main.Gravity;
            }

            if (CollRectangle.Y <= _originalY && _hasFallen)
            {
                _fallingSound.Reset();
                _hasFallen = false;
                Velocity.Y = 0;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White);

            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), attackBox, Color.Red);
        }

        public void OnCollisionWithTerrainBelow(Entity entity, Tile tile)
        {
            Velocity.Y = 0;
            GravityStrength = 0;
            _hasFallen = true;

            StompSmokeParticle.Generate(10, this);
            _fallingSound.PlayNewInstanceOnce();
        }
    }
}
