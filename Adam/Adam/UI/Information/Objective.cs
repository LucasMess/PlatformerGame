using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Information
{
    public class Objective
    {
        SpriteFont _textFont, _headFont;
        Texture2D _texture;
        Rectangle _drawRectangle;
        float _opacity = 0;
        public bool IsActive;
        public bool IsComplete;
        bool _completeAnimationDone;
        int _newY;
        int _goalX;
        public int Id;
        double _lifespan;
        Color _color = Color.Black;

        string _text = "";

        public Objective()
        {
            _texture = ContentHelper.LoadTexture("Tiles/white");
            _textFont = ContentHelper.LoadFont("Fonts/x32");
            _headFont = ContentHelper.LoadFont("Fonts/x64");
            _drawRectangle = new Rectangle(Main.UserResWidth, 0, 300, 125);
            _goalX = Main.UserResWidth - _drawRectangle.Width;
        }

        public void Create(string text, int id)
        {
            this._text = FontHelper.WrapText(_textFont, text, _drawRectangle.Width - 20);
            this.Id = id;
            IsActive = true;
            _drawRectangle.Y = (30);
            _newY = _drawRectangle.Y;
        }

        public void SetPosition(int index)
        {
            _drawRectangle.Y = (30 + (130 * index));
            _newY = _drawRectangle.Y;
        }

        public void TransitionIntoNewPosition(int index)
        {
            _newY = (30 + (155 * index));
        }

        public void Update(GameTime gameTime)
        {
            float deltaOpacity = .03f;
            if (IsActive)
            {
                if (!IsComplete)
                {
                    _color = Color.White;
                    _opacity += deltaOpacity;
                    _drawRectangle.Y += (_newY - _drawRectangle.Y) / 10;
                    _drawRectangle.X += (_goalX - _drawRectangle.X) / 10;
                }
                else
                {
                    _drawRectangle.Width = 400;

                    if (_lifespan > 2)
                    {
                        int completeX = Main.UserResWidth + 100;
                        _drawRectangle.X += (completeX - _drawRectangle.X) / 10;
                        if (_drawRectangle.X >= Main.UserResWidth)
                            IsActive = false;
                    }
                    else
                    {
                        int completeX = _goalX - 100;
                        _drawRectangle.X += (completeX - _drawRectangle.X) / 10;
                        _color = Color.Green;
                        _lifespan += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }

                if (_opacity > .8f)
                    _opacity = .8f;
                if (_opacity < 0)
                    _opacity = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, drawRectangle, color * opacity);
            //spriteBatch.DrawString(font, "Objective:", new Vector2(drawRectangle.X + 5, drawRectangle.Y + 5), Color.Yellow * opacity);
            string headText = "Objective:";
            if (IsComplete) headText = "Objective Complete!";
            FontHelper.DrawWithOutline(spriteBatch, _headFont, headText, new Vector2(_drawRectangle.X + 5, _drawRectangle.Y + 5), 2, Color.Yellow, Color.Black);
            //spriteBatch.DrawString(textFont, text, new Vector2(drawRectangle.X + 5, drawRectangle.Y + 60), Color.Black * opacity, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            FontHelper.DrawWithOutline(spriteBatch, _textFont, _text, new Vector2(_drawRectangle.X + 5, _drawRectangle.Y + 60), 2, new Color(240,240,240), Color.Black);
        }
    }
}
