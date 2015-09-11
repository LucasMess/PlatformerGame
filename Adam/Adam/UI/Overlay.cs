using Adam;
using Adam.Misc.Helpers;
using Adam.UI;
using Adam.UI.Overlay_Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class Overlay
    {
        private static Overlay instance;
        public static Overlay Instance { get { return instance; } }

        public static SpriteFont Font;

        Heart heart;
        Coin coin;
        Image blackCorners = new Image();
        Image blackScreen = new Image();

        bool fadeIn;
        bool fadeOut;
        float blackOpacity;
        double fadingTimer;

        public Overlay()
        {
            instance = this;
            Font = ContentHelper.LoadFont("Fonts/overlay");

            heart = new Heart(new Vector2(40,40));
            coin = new Coin(new Vector2(40, 120));

            //Black corners of the screen
            blackCorners.Texture = ContentHelper.LoadTexture("Backgrounds/blackCorners");
            blackCorners.Rectangle = new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight);

            blackScreen.Texture = ContentHelper.LoadTexture("Tiles/black");
            blackScreen.Rectangle = new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight);
        }

        public void Update(GameTime gameTime, Player player, GameWorld map)
        {
            heart.Update(gameTime, player, map);
            coin.Update(player, gameTime);

            if (fadeIn)
            {
                blackOpacity -= .03f;
                if (blackOpacity <= 0)
                    fadeIn = false;
            }
            if (fadeOut)
            {
                blackOpacity += .03f;
                if (blackOpacity >= 1)
                    fadeOut = false;
            }
        }

        public void FadeIn()
        {
            fadeIn = true;
            fadeOut = false;
            blackOpacity = 2;
        }

        public void FadeOut()
        {
            blackOpacity = 0;
            fadeOut = true;
            fadeIn = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                goto Fade;

            spriteBatch.Draw(blackCorners.Texture, blackCorners.Rectangle, Color.White * .6f);

            heart.Draw(spriteBatch);
            coin.Draw(spriteBatch);

        Fade:
            if (fadeIn || fadeOut)
            spriteBatch.Draw(blackScreen.Texture, blackScreen.Rectangle, Color.White * blackOpacity);
        }
    }



}
