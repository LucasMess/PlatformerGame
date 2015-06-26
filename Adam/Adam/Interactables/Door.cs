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
    class Door : Entity
    {
        Rectangle radiusRect;
        public bool isOpen;
        int secret;
        SoundEffect lockedSound;
        SoundEffect openSound;
        bool soundPlayed;
        public int tilePos;
        Dialog dialog;
        Vector2 monitorRes;
        double dialogTimer;

        public Door(int x, int y, ContentManager Content, int secret, int tilePos)
        {
            this.secret = secret;
            this.tilePos = tilePos;
            this.Content = Content;
            this.monitorRes = new Vector2(Game1.DefaultResWidth,Game1.DefaultResHeight);

            dialog = new Dialog(Content, Dialog.Type.Notification);
            animation = new Animation(Content.Load<Texture2D>("Objects/door"), new Rectangle(x, y, Game1.Tilesize, Game1.Tilesize * 2), 10, 0, AnimationType.SlowPanVertical);
            lockedSound = Content.Load<SoundEffect>("Sounds/lock_closed");
            openSound = Content.Load<SoundEffect>("Sounds/lock_open");
            radiusRect = new Rectangle(x - 100, y - 100, 200, 200);
        }

        public void Update(GameTime gameTime, Player player, Tile[] tileArray)
        {
            animation.Update(gameTime);

            if (player.collRectangle.Intersects(radiusRect) && Keyboard.GetState().IsKeyDown(Keys.W) && !isOpen)
            {
                foreach (int s in player.keySecrets)
                {
                    if (s == secret)
                    {
                        isOpen = true;
                        animation.canStart = true;
                        player.keySecrets.Remove(s);
                        openSound.Play();
                        tileArray[tilePos].isSolid = false;
                        dialog.AddText("The key fit!");
                        dialog.isVisible = true;
                        return;
                    }
                }
                if (!soundPlayed)
                {
                    lockedSound.Play();
                    soundPlayed = true;
                    dialog.AddText("It's locked.");
                    dialog.isVisible = true;
                }
            }
            if (soundPlayed && (Keyboard.GetState().IsKeyUp(Keys.W)))
                soundPlayed = false;
            if (dialog.isVisible)
            {
                dialogTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (dialogTimer > Dialog.ExpirationTime)
                {
                    dialog.isVisible = false;
                    dialogTimer = 0;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            dialog.Draw(spriteBatch);
        }

    }
}
