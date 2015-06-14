using CodenameAdam;
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
    class Key
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
            texture = Content.Load<Texture2D>("Objects/key");
            rectangle = new Rectangle(x, y, Game1.TILESIZE, Game1.TILESIZE);
            pickSound = Content.Load<SoundEffect>("Sounds/key_get");
        }

        public void Update(Player player)
        {
            if (player.collRectangle.Intersects(rectangle) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                isPickedUp = true;
                player.keySecrets.Add(secret);
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
