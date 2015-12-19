using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class PopUp
    {
        Texture2D _fill;
        Texture2D _outline;

        Rectangle _fillRect;
        Rectangle _outlineRect;

        float _fillRotation, _outlineRotation;
        Vector2 _fillOrigin, _outlineOrigin;
        public bool IsVisible;
        string[] _texts;

        double _clickTimer;

        Texture2D _obj;
        Rectangle _objRect, _objSourceRect;
        Vector2 _objOrigin;

        Rectangle _dialogRect;

        ContentManager _content;

        public PopUp()
        {
            _texts = new string[32];
            _texts[1] = "A leaf! The best protection in the world!";
            _texts[2] = "A stick! If only you had marshmallows.";
            _texts[3] = "Leather shoes! Show those ants who is boss!";
            _texts[4] = "Bear skin! Just don't smell it.";
            _texts[5] = "A... piece of scrap metal?";
            _texts[6] = "A bow! Arrows not included.";
            _texts[7] = "A toga! Hey, it's better than nothing.";
            _texts[8] = "Sandals! Stylish and aerodynamic.";
            _texts[9] = "A razor! CAUTION: Use it above the waist.";
            _texts[10] = "A diadem! Sorry, no refunds.";
            _texts[11] = "An iron sword! Useful for spreading butter!";
            _texts[12] = "An iron chestplate! Not recommended for females.";
            _texts[13] = "Iron leggings! Protect the children.";
            _texts[14] = "An iron helmet!";
            _texts[15] = "";
            _texts[16] = "A shotgun! Get these kids off my lawn!";
            _texts[17] = "A fancy jacket! At least it has pockets.";
            _texts[18] = "A wig! Hide that male pattern baldness!";
            _texts[19] = "Fancy pants! ";
            _texts[20] = "";
            _texts[21] = "Sunglasses! Do not look directly at the Sun.";
            _texts[22] = "A hoodie! ";
            _texts[23] = "Jeans!";
            _texts[24] = "Athletic shoes!";
            _texts[25] = "";
            _texts[26] = "A laser gun! Built-in mp3 player.";
            _texts[27] = "An astronaut suit! It's cut in half.";
            _texts[28] = "The rest of the astronaut suit! Goodbye freedom.";
            _texts[29] = "A helmet. Fish not included.";
            _texts[30] = "A jetpack! To infinity and beyond!";
            _texts[31] = "Untapped power. Using it voids the warranty.";

            _objSourceRect = new Rectangle(0, 0, 16, 16);
        }

        public void Load(ContentManager content)
        {
            this._content = content;

            Vector2 monitorResolution = new Vector2(Main.UserResWidth, Main.UserResHeight);
            _dialogRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y * 4 / 5, (int)monitorResolution.X * 2 / 3, (int)monitorResolution.Y * 1 / 6);

            _fill = ContentHelper.LoadTexture("Menu/Star Fill");
            _outline = ContentHelper.LoadTexture("Menu/Star Outline");

            _fillRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y / 2 - 100, 300, 300);
            _fillOrigin = new Vector2(_fill.Width / 2, _fill.Height / 2);

            _outlineRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y / 2 - 100, 300, 300);
            _outlineOrigin = new Vector2(_outline.Width / 2, _outline.Height / 2);

            _obj = ContentHelper.LoadTexture("Objects/tech_items");
            _objRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y / 2 - 100, 64, 64);
            _objOrigin = new Vector2(8, 8);
        }

        public void Update(GameTime gameTime, Player player)
        {

            if (!IsVisible)
                return;

            _fillRotation += .01f;
            _outlineRotation -= .01f;

            _objSourceRect.X = (0) % 5 * 16;
            _objSourceRect.Y = (0) / 5 * 16;

            _clickTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_clickTimer > 1 && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _clickTimer = 0;
                IsVisible = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.Draw(_fill, _fillRect, null, Color.White, _fillRotation, _fillOrigin, SpriteEffects.None, 0);
                spriteBatch.Draw(_outline, _outlineRect, null, Color.White, _outlineRotation, _outlineOrigin, SpriteEffects.None, 0);
                spriteBatch.Draw(_obj, _objRect, _objSourceRect, Color.White, 0, _objOrigin, SpriteEffects.None, 0);
            }
        }


    }
}
