using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class GameDebug
    {
        Texture2D black;
        Rectangle rect;
        Keys[] lastPressedKeys;
        public bool isWritingCommand;
        KeyboardState oldKeyboardState, currentKeyboardState;
        string textString;
        SpriteFont font;
        Vector2 monitorRes;
        Vector2 position;
        Game1 game1;
        Player player;
        bool definitionFound;

        public GameDebug(SpriteFont font, Vector2 monitorRes, Texture2D black)
        {
            this.monitorRes = monitorRes;
            this.font = font;
            this.black = black;
            lastPressedKeys = new Keys[0];
            position = new Vector2(10, monitorRes.Y - font.LineSpacing - 40);
            rect = new Rectangle(0, (int)(monitorRes.Y - font.LineSpacing - 40), (int)monitorRes.X, (int)font.LineSpacing);
        }

        public void Update(Game1 game1, Player player, Map map, bool isOnDebug)
        {
            if (!isOnDebug)
            {
                textString = "";
                isWritingCommand = false;
                map.isPaused = false;
                return;
            }

            if (!isWritingCommand)
            {
                map.isPaused = false;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)
                    && Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    && Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    isWritingCommand = true;
                    textString = "";
                    return;
                }
                else return;
            }
            else
            {
                map.isPaused = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl)
                    && Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    && Keyboard.GetState().IsKeyDown(Keys.C))
            {
                textString = "";
            }

            if (textString == "No command found" && Keyboard.GetState().IsKeyDown(Keys.Back))
                textString = "";

            this.game1 = game1;
            this.player = player;
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            Keys[] pressedKeys;
            pressedKeys = currentKeyboardState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (oldKeyboardState.IsKeyUp(key))
                {
                    string keyString = key.ToString().ToLower();

                    switch (key)
                    {
                        case Keys.Back:
                            if (textString.Length != 0)
                                textString = textString.Remove(textString.Length - 1, 1);
                            return;
                        case Keys.A:
                            textString += keyString;
                            return;
                        case Keys.B:
                            textString += keyString;
                            return;
                        case Keys.C:
                            textString += keyString;
                            return;
                        case Keys.D:
                            textString += keyString;
                            return;
                        case Keys.E:
                            textString += keyString;
                            return;
                        case Keys.F:
                            textString += keyString;
                            return;
                        case Keys.G:
                            textString += keyString;
                            return;
                        case Keys.H:
                            textString += keyString;
                            return;
                        case Keys.I:
                            textString += keyString;
                            return;
                        case Keys.J:
                            textString += keyString;
                            return;
                        case Keys.K:
                            textString += keyString;
                            return;
                        case Keys.L:
                            textString += keyString;
                            return;
                        case Keys.M:
                            textString += keyString;
                            return;
                        case Keys.N:
                            textString += keyString;
                            return;
                        case Keys.O:
                            textString += keyString;
                            return;
                        case Keys.P:
                            textString += keyString;
                            return;
                        case Keys.Q:
                            textString += keyString;
                            return;
                        case Keys.R:
                            textString += keyString;
                            return;
                        case Keys.S:
                            textString += keyString;
                            return;
                        case Keys.T:
                            textString += keyString;
                            return;
                        case Keys.U:
                            textString += keyString;
                            return;
                        case Keys.V:
                            textString += keyString;
                            return;
                        case Keys.W:
                            textString += keyString;
                            return;
                        case Keys.X:
                            textString += keyString;
                            return;
                        case Keys.Y:
                            textString += keyString;
                            return;
                        case Keys.Z:
                            textString += keyString;
                            return;
                        case Keys.Space:
                            textString = textString.Insert(textString.Length, " ");
                            return;
                        case Keys.Enter:
                            AnalyzeText();
                            return;
                    }
                }
            }
        }

        public void AnalyzeText()
        {
            switch (textString)
            {
                case "is op true":
                    player.canFly = true;
                    player.isInvulnerable = true;
                    definitionFound = true;
                    break;
                case "is op false":
                    player.canFly = false;
                    player.isInvulnerable = false;
                    definitionFound = true;
                    break;
                case "is ghost true":
                    player.isGhost = true;
                    player.canFly = true;
                    player.isInvulnerable = true;
                    definitionFound = true;
                    break;
                case "is ghost false":
                    player.isGhost = false;
                    player.canFly = false;
                    player.isInvulnerable = false;
                    definitionFound = true;
                    break;
                case "set level":
                    break;
            }

            if (definitionFound)
            {
                textString = "";
                definitionFound = false;
                isWritingCommand = false;
            }
            else textString = "No command found";

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isWritingCommand)
                spriteBatch.Draw(black, rect, Color.White * .3f);
            if (textString != null)
                spriteBatch.DrawString(font, textString, position, Color.White);
        }
    }
}
