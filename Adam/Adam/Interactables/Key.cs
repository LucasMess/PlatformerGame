using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class Key
    {
        Texture2D _texture;
        Rectangle _rectangle;
        public bool IsPickedUp;
        public bool ToDelete;
        public int Secret;
        SoundEffect _pickSound;

        public Key(int x, int y, ContentManager content, int secret)
        {
            this.Secret = secret;
            _texture = ContentHelper.LoadTexture("Objects/key");
            _rectangle = new Rectangle(x, y, Main.Tilesize, Main.Tilesize);
            _pickSound = content.Load<SoundEffect>("Sounds/key_get");
        }

        public void Update(Player.Player player)
        {
            if (player.GetCollRectangle().Intersects(_rectangle) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                IsPickedUp = true;
                _pickSound.Play();
                ToDelete = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _rectangle, Color.White);
        }
    }
}
