using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam
{
    public class GameDebug
    {
        Texture2D _black;
        Rectangle _rect;
        Keys[] _lastPressedKeys;
        public bool IsWritingCommand;
        KeyboardState _oldKeyboardState, _currentKeyboardState;
        string _textString;
        SpriteFont _font;
        Vector2 _monitorRes;
        Vector2 _position;
        Main _game1;
        Player.Player _player;
        bool _definitionFound;

        public GameDebug(SpriteFont font, Vector2 monitorRes, Texture2D black)
        {
            this._monitorRes = monitorRes;
            this._font = font;
            this._black = black;
            _lastPressedKeys = new Keys[0];
            _position = new Vector2(10, monitorRes.Y - font.LineSpacing - 40);
            _rect = new Rectangle(0, (int)(monitorRes.Y - font.LineSpacing - 40), (int)monitorRes.X, (int)font.LineSpacing);
        }

        public void Update(Main game1, Player.Player player, GameWorld map, bool isOnDebug)
        {
            if (!isOnDebug)
            {
                _textString = "";
                IsWritingCommand = false;
                map.IsOnDebug = false;
                return;
            }

            if (!IsWritingCommand)
            {
                map.IsOnDebug = false;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)
                    && Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    && Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    IsWritingCommand = true;
                    _textString = "";
                    return;
                }
                else return;
            }
            else
            {
                map.IsOnDebug = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)
                    && Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    && Keyboard.GetState().IsKeyDown(Keys.C))
            {
                _textString = "";
            }

            if (_textString == "No command found" && Keyboard.GetState().IsKeyDown(Keys.Back))
                _textString = "";

            this._game1 = game1;
            this._player = player;

            _oldKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            InputHelper.TryLinkToKeyboardInput(ref _textString, _currentKeyboardState, _oldKeyboardState);
            if (InputHelper.IsKeyDown(Keys.Enter) &&!_definitionFound)
            {
                AnalyzeText();
            }

        }

        public void AnalyzeText()
        {
            switch (_textString)
            {
                case "is op true":
                    _player.CanFly = true;
                    _player.IsInvulnerable = true;
                    _definitionFound = true;
                    break;
                case "is op false":
                    _player.CanFly = false;
                    _player.IsInvulnerable = false;
                    _definitionFound = true;
                    break;
                case "is ghost true":
                    _player.IsGhost = true;
                    _player.CanFly = true;
                    _player.IsInvulnerable = true;
                    _definitionFound = true;
                    break;
                case "is ghost false":
                    _player.IsGhost = false;
                    _player.CanFly = false;
                    _player.IsInvulnerable = false;
                    _definitionFound = true;
                    break;
                case "set level":
                    break;
            }

            String text = _textString;
            string keyword = "set background ";
            if (text.StartsWith(keyword))
            {
                string newString = text.Remove(0, keyword.Length);
                int number;
                Int32.TryParse(newString, out number);
                GameWorld.Instance.WorldData.BackgroundId = (byte)number;
                _definitionFound = true;
            }
            keyword = "set soundtrack ";
            if (text.StartsWith(keyword))
            {
                string newString = text.Remove(0, keyword.Length);
                int number;
                Int32.TryParse(newString, out number);
                GameWorld.Instance.WorldData.SoundtrackId = (byte)number;
                _definitionFound = true;
            }

            //keyword = "set ambience ";
            //if (text.StartsWith(keyword))
            //{
            //    string newString = text.Remove(0, keyword.Length);
            //    int number;
            //    Int32.TryParse(newString, out number);
            //    GameWorld.Instance.worldData.SoundtrackID = (byte)number;
            //    definitionFound = true;
            //}

            if (_definitionFound)
            {
                _textString = "";
                _definitionFound = false;
                IsWritingCommand = false;
            }
            else _textString = "No command found";

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsWritingCommand)
                spriteBatch.Draw(_black, _rect, Color.White * .3f);
            if (_textString != null)
                spriteBatch.DrawString(_font, _textString, _position, Color.White);
        }
    }
}
