using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.UI;
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
        static Textbox chatBox = new Textbox(0, AdamGame.DefaultUiHeight - 60, AdamGame.DefaultUiWidth - 10, 60);

        static bool _debugKeyReleased;
        static bool _chatKeyReleased;

        /// <summary>
        /// Returns true if the user requested debug mode to be on.
        /// </summary>
        public static bool IsDebugOn { get; set; }

        public static bool IsTyping { get; set; }

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

            // Chat box.
            if (!GameWorld.GetPlayer().IsEnterCommandPressed())
            {
                _chatKeyReleased = true;
            }

            if (GameWorld.GetPlayer().IsEnterCommandPressed() && _chatKeyReleased && !IsTyping)
            {
                _chatKeyReleased = false;
                IsTyping = true;
                chatBox.Reset();
            }
        }

        /// <summary>
        /// Checks for input.
        /// </summary>
        public static void Update()
        {
            CheckIfDebugIsOn();

            if (IsTyping && _chatKeyReleased)
            {
                chatBox.IsSelected = true;
                chatBox.Update(new Rectangle());
                if (GameWorld.GetPlayer().IsEnterCommandPressed())
                {
                    _chatKeyReleased = false;
                    IsTyping = false;
                    string text = chatBox.Text;
                    if (text == null || text.Length == 0)
                        return;
                    if (text[0] == '/')
                    {
                        RunCommand(text);
                    }
                }
            }
        }

        /// <summary>
        /// Runs the appropriate command if one is found.
        /// </summary>
        public static void RunCommand(string command)
        {
            string[] commands = command.Split(' ');

            commands[0] = commands[0].Substring(1);

            bool value;

            switch (commands[0])
            {
                case "set":
                    switch (commands[1])
                    {
                        case "background":
                            int number;
                            if (int.TryParse(commands[2], out number))
                                GameWorld.WorldData.BackgroundId = (byte)number;
                            break;
                        case "snow":
                            if (bool.TryParse(commands[2], out value))
                                GameWorld.WorldData.IsSnowing = value;
                            break;
                        case "rain":
                            if (bool.TryParse(commands[2], out value))
                                GameWorld.WorldData.IsRaining = value;
                            break;
                    }

                    break;
            }

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
                _infos.Add("Tile Type: " + GameWorld.GetTile(LevelEditor.IndexOfMouse)?.Id.ToString());
                _infos.Add("Particle iteration: " + GameWorld.ParticleSystem?.GetIteration());
                _infos.Add("Is Sprinting: " + GameWorld.GetPlayer().IsRunningFast);
                _infos.Add("Steam Name: " + AdamGame.UserName);

                spriteBatch.Draw(GameWorld.SpriteSheet, new Rectangle(0, 0, AdamGame.UserResWidth, (_infos.Count + 1) * _font.LineHeight), new Rectangle(304, 224, 8, 8), Color.White * .6f);

                for (int i = 0; i < _infos.Count; i++)
                {
                    Point pos = new Point(0, i * _font.LineHeight);
                    FontHelper.DrawWithOutline(spriteBatch, _font, _infos[i], pos.ToVector2(), 1, new Color(220, 220, 220), Color.Black);
                }
            }

            if (IsTyping)
            {
                spriteBatch.Draw(GameWorld.SpriteSheet, new Rectangle(0, AdamGame.UserResHeight - 30, AdamGame.UserResWidth, 30), new Rectangle(304, 224, 8, 8), Color.White * .6f);
                chatBox.Draw(spriteBatch);
            }
        }
    }
}
