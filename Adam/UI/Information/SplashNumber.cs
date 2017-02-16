using Adam.Misc.Helpers;
using Adam.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Adam.UI
{
    public class SplashNumber : Particle
    {
        BitmapFont _font;
        string _text;
        bool _isNegative;
        bool _hasExpanded;
        int _number;
        private int _offset;
        float _scale, _normScale;
        private Color _borderColor = Microsoft.Xna.Framework.Color.White;

        public SplashNumber(Entity entity, int number, Color color)
        {
            this._number = number;
            _text = this._number.ToString();
            Position = new Vector2(entity.GetCollRectangle().Right + 20, entity.GetCollRectangle().Y - 20);
            Color = color;
            const int offset = 150;
            //_borderColor = new Color(color.R - offset, color.G - offset, color.B - offset);
            _borderColor = Color.White;
            if (this._number < 0)
                _isNegative = true;

            int absDamage = Math.Abs(this._number);
            _scale = .4f;
            if (absDamage > 10)
                _scale = .6f;
            if (absDamage > 20)
                _scale = .7f;
            if (absDamage > 40)
                _scale = .8f;
            if (absDamage > 80)
                _scale = 1f;

            _font = ContentHelper.LoadFont("Fonts/x16");
            Velocity = new Vector2(0, AdamGame.Random.Next(-5, -3));

            _normScale = _scale;
            _scale = .01f;
            _offset = AdamGame.Random.Next(0, 10);
            Opacity = 2;
        }


        public override void Update()
        {
            Opacity -= .01f;
            Position += Velocity;
            Velocity = new Vector2((float)Math.Cos(_offset + AdamGame.GameTime.TotalGameTime.TotalSeconds * 10), Velocity.Y * .95f);

            if (_scale > _normScale * 2)
            {
                _hasExpanded = true;
            }

            if (!_hasExpanded)
            {
                _scale += .6f;
            }
            else
            {
                _scale -= .03f;
            }
            if (Velocity.Y > -60)
            {
                _scale -= .03f;
               // Velocity = new Vector2(0, 6);
            }

            if (_scale < 0)
                _scale = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isNegative)
            {
                FontHelper.DrawWithOutline(spriteBatch, _font, _number.ToString(), Position, 2, Color * Opacity, _borderColor * Opacity, _scale);
            }
            else FontHelper.DrawWithOutline(spriteBatch, _font, "+" + _number, Position, 2, Color * Opacity, _borderColor * Opacity, _scale);
        }
    }
}
