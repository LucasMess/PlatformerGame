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
    public class GameDebug
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
        Main game1;
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

        public void Update(Main game1, Player player, GameWorld map, bool isOnDebug)
        {
            if (!isOnDebug)
            {
                textString = "";
                isWritingCommand = false;
                map.isOnDebug = false;
                return;
            }

            if (!isWritingCommand)
            {
                map.isOnDebug = false;
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
                map.isOnDebug = true;
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

            InputHelper.TryLinkToKeyboardInput(ref textString, currentKeyboardState, oldKeyboardState);
            if (InputHelper.IsKeyDown(Keys.Enter) &&!definitionFound)
            {
                AnalyzeText();
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

            String text = textString;
            string keyword = "set background ";
            if (text.StartsWith(keyword))
            {
                string newString = text.Remove(0, keyword.Length);
                int number;
                Int32.TryParse(newString, out number);
                GameWorld.Instance.worldData.BackgroundID = (byte)number;
                definitionFound = true;
            }
            keyword = "set soundtrack ";
            if (text.StartsWith(keyword))
            {
                string newString = text.Remove(0, keyword.Length);
                int number;
                Int32.TryParse(newString, out number);
                GameWorld.Instance.worldData.SoundtrackID = (byte)number;
                definitionFound = true;
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
