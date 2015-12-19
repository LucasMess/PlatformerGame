using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class SplashNumber : Particle
    {
        SpriteFont _font;
        string _text;
        bool _isNegative;
        bool _hasExpanded;
        int _number;
        float _scale, _normScale;

        public SplashNumber(Entity entity, int number, Color color)
        {
            this._number = number;
            _text = this._number.ToString();
            Position = new Vector2(entity.GetCollRectangle().Right + 20, entity.GetCollRectangle().Y - 20);
            CollRectangle = new Rectangle((int)Position.X, (int)Position.Y, 50, 50);
            Color = color;

            if (this._number < 0)
                _isNegative = true;

            int absDamage = Math.Abs(this._number);
            _scale = .1f;
            if (absDamage > 10)
                _scale = .15f;
            if (absDamage > 20)
                _scale = .2f;
            if (absDamage > 40)
                _scale = .3f;
            if (absDamage > 80)
                _scale = .5f;

            _font = ContentHelper.LoadFont("Fonts/x32");
            Velocity = new Vector2(0, -5);

            Origin = _font.MeasureString(_text) / 2;
            _normScale = _scale;
            Opacity = 2;
        }


        public override void Update(GameTime gameTime)
        {
            Opacity -= .05f;
            Position += Velocity;
            Velocity.Y = Velocity.Y * 0.95f;

            if (_scale > _normScale * 2)
            {
                _hasExpanded = true;
            }

            if (!_hasExpanded)
            {
                _scale += .00005f;
            }
            else
            {
                _scale -= .01f;
            }

            if (Opacity <= 0)
                ToDelete = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isNegative)
            {
                FontHelper.DrawWithOutline(spriteBatch, _font, _number.ToString(), Position, 2, Color * Opacity, Color.Black * Opacity);
            }
            else FontHelper.DrawWithOutline(spriteBatch, _font, "+" + _number, Position, 2, Color * Opacity, Color.Black * Opacity);
        }
    }
}
