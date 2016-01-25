using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.UI.Information;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Characters;
using Adam.Levels;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Noobs
{
    public class God : NonPlayableCharacter
    {
        /// <summary>
        /// God NPC. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public God(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            ComplexAnim = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/god");
            CollRectangle = new Rectangle(x, y, 48, 80);
            ComplexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 24, 40), 0, 24, 40, 500, 4, true));
            AddAnimationToQueue("still");
            Main.Dialog.NextDialog += Dialog_NextDialog;
        }

        protected override void ShowDialog()
        {
            if (!Main.LevelProgression.HasMetGod)
            {
                Say("Hello Adam. You know who I am.", "god-background-info-1", new[] { "What are you talking about?", "Pizza!" });
                Main.LevelProgression.HasMetGod = true;
            }
            else
            {
                Dialog_NextDialog("god-background-info-1", 0);
            }
        }

        private void Dialog_NextDialog(string code, int selectedOption)
        {
            switch (code)
            {
                case "god-background-info-1":
                    Say("Yesterday I was going to the bathroom and got lost. I found a very strange room full of pictures, but they I don't recall every buying those. Well I never found the place again.", "god-background-info-2", new[] { "Do you know if this room still exists?", "I don't care, what am I supposed to do now?", "Ok, bye." });
                    break;
                case "god-background-info-2":
                    switch (selectedOption)
                    {
                        case 0:
                            Say("What room?", null, null);
                            break;
                        case 1:
                            Say("I don't know. I haven't made up the future just yet and you have free will. That reminds me! I have something in the oven. See you later, Adam.",null, null);
                            break;
                        case 2:
                            Say("Farewell.", null, null);
                            break;
                    }
                    break;
            }
        }
    }
}
