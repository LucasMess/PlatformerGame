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
using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam
{
    public class Overlay
    {
        private static Overlay _instance;
        public static Overlay Instance { get { return _instance; } }

        public static SpriteFont Font;

        Heart _heart;
        Coin _coin;
        Image _blackCorners = new Image();
        Image _blackScreen = new Image();

        bool _fadeIn;
        bool _fadeOut;
        float _blackOpacity;
        double _fadingTimer;

        public static Color CornerColor = Color.Black;

        public Overlay()
        {
            _instance = this;
            Font = ContentHelper.LoadFont("Fonts/splashNumber");

            _heart = new Heart(new Vector2(40, 40));
            _coin = new Coin(new Vector2(40, 120));

            //Black corners of the screen
            _blackCorners.Texture = ContentHelper.LoadTexture("Backgrounds/ui_whiteCorners");
            _blackCorners.Rectangle = new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight);

            _blackScreen.Texture = ContentHelper.LoadTexture("Tiles/black");
            _blackScreen.Rectangle = new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight);
        }

        public void Update()
        {
            _heart.Update(Main.GameTime, GameWorld.GetPlayer());
            _coin.Update(GameWorld.GetPlayer(), Main.GameTime);

            if (_fadeIn)
            {
                _blackOpacity -= .03f;
                if (_blackOpacity <= 0)
                    _fadeIn = false;
            }
            if (_fadeOut)
            {
                _blackOpacity += .03f;
                if (_blackOpacity >= 1)
                    _fadeOut = false;
            }
        }

        public void FadeIn()
        {
            _fadeIn = true;
            _fadeOut = false;
            _blackOpacity = 2;
        }

        public void FadeOut()
        {
            _blackOpacity = 0;
            _fadeOut = true;
            _fadeIn = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.CurrentGameMode == GameMode.Edit)
                goto Fade;

            spriteBatch.Draw(_blackCorners.Texture, _blackCorners.Rectangle, CornerColor * .6f);

            _heart.Draw(spriteBatch);
            _coin.Draw(spriteBatch);

            Fade:

            spriteBatch.Draw(_blackScreen.Texture, _blackScreen.Rectangle, Color.White * _blackOpacity);
        }
    }



}
