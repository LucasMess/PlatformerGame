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
        Texture2D texture;
        Rectangle rectangle;
        public bool isPickedUp;
        public bool toDelete;
        public int secret;
        SoundEffect pickSound;

        public Key(int x, int y, ContentManager Content, int secret)
        {
            this.secret = secret;
            texture = ContentHelper.LoadTexture("Objects/key");
            rectangle = new Rectangle(x, y, Main.Tilesize, Main.Tilesize);
            pickSound = Content.Load<SoundEffect>("Sounds/key_get");
        }

        public void Update(Player player)
        {
            if (player.GetCollRectangle().Intersects(rectangle) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                isPickedUp = true;
                pickSound.Play();
                toDelete = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
