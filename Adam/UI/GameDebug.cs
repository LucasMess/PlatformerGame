﻿using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using ThereMustBeAnotherWay.Particles;
using ThereMustBeAnotherWay.GameData;

namespace ThereMustBeAnotherWay
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
        private static SpriteFont _font = FontHelper.Fonts[0];
        static Textbox chatBox = new Textbox(0, TMBAW_Game.DefaultUiHeight - 60, TMBAW_Game.DefaultUiWidth - 10, 60);

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
            if (InputSystem.IsKeyUp(Keys.F3))
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
            if (TMBAW_Game.CurrentGameState == GameState.GameWorld)
            {
                if (!GameWorld.GetPlayers()[0].IsEnterCommandPressed())
                {
                    _chatKeyReleased = true;
                }

                if (GameWorld.GetPlayers()[0].IsEnterCommandPressed() && _chatKeyReleased && !IsTyping)
                {
                    _chatKeyReleased = false;
                    IsTyping = true;
                    chatBox.Reset();
                }
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
                if (GameWorld.GetPlayers()[0].IsEnterCommandPressed())
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
            try
            {
                string[] commands = command.Split(' ');

                commands[0] = commands[0].Substring(1);

                bool value;
                int number;

                switch (commands[0])
                {
                    case "set":
                        switch (commands[1])
                        {
                            case "background":
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
                            case "clouds":
                                if (bool.TryParse(commands[2], out value))
                                    GameWorld.WorldData.HasClouds = value;
                                break;
                            case "sun":
                                if (bool.TryParse(commands[2], out value))
                                    GameWorld.WorldData.HasSun = value;
                                break;
                            case "soundtrack":
                                if (int.TryParse(commands[2], out number))
                                    GameWorld.WorldData.SoundtrackId = (byte)number;
                                break;
                            case "sunlight":
                                int r, g, b;
                                if (int.TryParse(commands[2], out r))
                                    if (int.TryParse(commands[3], out g))
                                        if (int.TryParse(commands[4], out b))
                                        {
                                            GameWorld.WorldData.SunLightColor = new Color(r, g, b);
                                            LevelEditor.SaveLevel();
                                            DataFolder.EditLevel(DataFolder.CurrentLevelFilePath);
                                        }
                                break;
                        }

                        break;
                }

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Command not found.");
                return;
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
                SettingsFile settings = DataFolder.GetSettingsFile();

                _infos = new List<string>
                {
                    "There Must Be Another Way - Version " + TMBAW_Game.Version,
                    "FPS: " + TMBAW_Game.FPS + " Resolution: " + settings.ResolutionWidth + "x" + settings.ResolutionHeight,
                    "Gamestate: " + TMBAW_Game.CurrentGameState,
                    "Gamemode: " + TMBAW_Game.CurrentGameMode,
                    "Camera Position: " + TMBAW_Game.Camera.GetPosition().X + "," + TMBAW_Game.Camera.GetPosition().Y,
                    "Mouse (Game): " + InputSystem.GetMouseRectGameWorld().X + "," + InputSystem.GetMouseRectGameWorld().Y,
                    "Index of mouse: " + LevelEditor.IndexOfMouse,
                    "Tile Type: " + GameWorld.GetTile(LevelEditor.IndexOfMouse)?.Id.ToString(),
                    "Particles in use: " + ParticleSystem.GetInUseParticleCount(),
                    "Is Sprinting: " + GameWorld.GetPlayers()[0].IsRunningFast,
                    "Steam Name: " + TMBAW_Game.UserName + " ID: " + TMBAW_Game.SteamID.m_SteamID,
                    "Particle update time: " + ParticleSystem.UpdateTime + " " + String.Format("{0:0.00}", ParticleSystem.UpdateTime * 100 / GameWorld.TotalUpdateTimer.GetAverage()) + "% of update time",
                    "Particle draw time: " + ParticleSystem.DrawTime + " " + String.Format("{0:0.00}", ParticleSystem.DrawTime * 100 / GameWorld.TotalDrawTimer.GetAverage()) + "% of draw time",
                    "Tile draw time: " + GraphicsRenderer.TileDrawTimer.GetAverage() + " " + String.Format("{0:0.00}", GraphicsRenderer.TileDrawTimer.GetAverage() * 100 / GameWorld.TotalDrawTimer.GetAverage() ) + "% of draw time",
                    "UI draw time: " + GraphicsRenderer.UserInterfaceDrawTimer.GetAverage() + " " + String.Format("{0:0.00}",  GraphicsRenderer.UserInterfaceDrawTimer.GetAverage() * 100 / GameWorld.TotalDrawTimer.GetAverage() ) + "% of draw time",
                    "Light draw time: " + GraphicsRenderer.LightDrawTimer.GetAverage() + " " + String.Format("{0:0.00}", GraphicsRenderer.LightDrawTimer.GetAverage() * 100 / GameWorld.TotalDrawTimer.GetAverage()) + "% of draw time",
                    "Total update time: " + GameWorld.TotalUpdateTimer.GetAverage(),
                    "Total draw time: " + GameWorld.TotalDrawTimer.GetAverage(),
                    "Visible tiles: " + GameWorld.ChunkManager?.GetVisibleIndexes()?.Length,
                    "Ambient Color: " + GameWorld.WorldData.SunLightColor,
                };
                spriteBatch.Draw(GameWorld.SpriteSheet, new Rectangle(0, 0, TMBAW_Game.UserResWidth, (_infos.Count) * _font.LineSpacing), new Rectangle(304, 224, 8, 8), Color.White * .6f);

                for (int i = 0; i < _infos.Count; i++)
                {
                    Point pos = new Point(0, i * _font.LineSpacing);
                    FontHelper.DrawWithOutline(spriteBatch, _font, _infos[i], pos.ToVector2(), 1, new Color(220, 220, 220), Color.Black);
                }
            }

            if (IsTyping)
            {
                spriteBatch.Draw(GameWorld.SpriteSheet, new Rectangle(0, TMBAW_Game.UserResHeight - 30, TMBAW_Game.UserResWidth, 30), new Rectangle(304, 224, 8, 8), Color.White * .6f);
                chatBox.Draw(spriteBatch);
            }
        }
    }
}
