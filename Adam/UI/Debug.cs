using Adam.Levels;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

namespace Adam
{
    public static class GameDebug
    {
        //Rectangle _rect;
        //Keys[] _lastPressedKeys;
        //public bool IsWritingCommand;
        //KeyboardState _oldKeyboardState, _currentKeyboardState;
        //string _textString;
        //bool _definitionFound;

        private static List<string> _infos = new List<string>();
        private static BitmapFont _font = FontHelper.Fonts[1];

        static bool _debugKeyReleased;

        /// <summary>
        /// Returns true if the user requested debug mode to be on.
        /// </summary>
        public static bool IsDebugOn { get; set; }

        /// <summary>
        /// Checks if user is pressing the debug key and turns it on or off.
        /// </summary>
        private static void CheckIfDebugIsOn()
        {
            if (InputHelper.IsKeyUp(Keys.F3))
            {
                _debugKeyReleased = true;
            }
            else
            {
                if (_debugKeyReleased)
                {
                    IsDebugOn = !IsDebugOn;
                    _debugKeyReleased = false;
                }
            }
        }

        /// <summary>
        /// Checks for input.
        /// </summary>
        public static void Update()
        {
            CheckIfDebugIsOn();
        }

        /// <summary>
        /// Runs the appropriate command if one is found.
        /// </summary>
        public static void RunCommand(string command)
        {
            //switch (_textString)
            //{
            //    case "is op true":
            //        _player.CanFly = true;
            //        _player.IsInvulnerable = true;
            //        _definitionFound = true;
            //        break;
            //    case "is op false":
            //        _player.CanFly = false;
            //        _player.IsInvulnerable = false;
            //        _definitionFound = true;
            //        break;
            //    case "is ghost true":
            //        _player.IsGhost = true;
            //        _player.CanFly = true;
            //        _player.IsInvulnerable = true;
            //        _definitionFound = true;
            //        break;
            //    case "is ghost false":
            //        _player.IsGhost = false;
            //        _player.CanFly = false;
            //        _player.IsInvulnerable = false;
            //        _definitionFound = true;
            //        break;
            //    case "set level":
            //        break;
            //    case "has clouds true":
            //        GameWorld.WorldData.HasClouds = true;
            //        _definitionFound = true;
            //        break;
            //    case "has clouds false":
            //        GameWorld.WorldData.HasClouds = false;
            //        _definitionFound = true;
            //        break;
            //}

            //String text = _textString;
            //string keyword = "set background ";
            //if (text.StartsWith(keyword))
            //{
            //    string newString = text.Remove(0, keyword.Length);
            //    int number;
            //    Int32.TryParse(newString, out number);
            //    GameWorld.WorldData.BackgroundId = (byte)number;
            //    _definitionFound = true;
            //}
            //keyword = "set soundtrack ";
            //if (text.StartsWith(keyword))
            //{
            //    string newString = text.Remove(0, keyword.Length);
            //    int number;
            //    Int32.TryParse(newString, out number);
            //    GameWorld.WorldData.SoundtrackId = (byte)number;
            //    _definitionFound = true;
            //}

            ////keyword = "set ambience ";
            ////if (text.StartsWith(keyword))
            ////{
            ////    string newString = text.Remove(0, keyword.Length);
            ////    int number;
            ////    Int32.TryParse(newString, out number);
            ////    GameWorld.worldData.SoundtrackID = (byte)number;
            ////    definitionFound = true;
            ////}

            //if (_definitionFound)
            //{
            //    _textString = "";
            //    _definitionFound = false;
            //    IsWritingCommand = false;
            //}
            //else _textString = "No command found";

        }

        /// <summary>
        /// Draws the debug information to the screen if debug is on.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (IsDebugOn)
            {
                _infos = new List<string>();
                _infos.Add(AdamGame.Producers + " (" + AdamGame.Version + ")");
                _infos.Add("FPS: " + AdamGame.FPS);
                _infos.Add("Gamestate: " + AdamGame.CurrentGameState);
                _infos.Add("Gamemode: " + AdamGame.CurrentGameMode);
                _infos.Add("Camera Position: " + AdamGame.Camera.GetPosition().X + "," + AdamGame.Camera.GetPosition().Y);
                _infos.Add("Mouse (Game): " + InputHelper.GetMouseRectGameWorld().X + "," + InputHelper.GetMouseRectGameWorld().Y);
                _infos.Add("Index of mouse: " + LevelEditor.IndexOfMouse);

                for (int i = 0; i < _infos.Count; i++)
                {
                    Point pos = new Point(0, i * _font.LineHeight);
                    FontHelper.DrawWithOutline(spriteBatch, _font, _infos[i], pos.ToVector2(), 1, new Color(220, 220, 220), Color.Black);
                }
            }
        }
    }
}
