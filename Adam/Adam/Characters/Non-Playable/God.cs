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
using Adam.Misc.Helpers;
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
            if (!Main.LevelProgression.HasStartedMainQuest)
            {
                Say("You are finally awake!.", "god-mainstory-1", new[] { "What are you talking about?", "What are you cooking there?" });
            }
            else
            {
                Dialog_NextDialog("god-mainstory-1", 0);
            }
        }

        private void Dialog_NextDialog(string code, int selectedOption)
        {
            switch (code)
            {
                case "god-mainstory-1":
                    switch (selectedOption)
                    {
                        case 0:
                            Say("You have been asleep for nearly two days now. Ever since Eve was kidnapped you have not left you room for anything!", "god-mainstory-2", new []{"Eve was... kidnapped?", "NO! HOW COULD THIS BE?"});
                            break;
                        case 1:
                            Say("I'm baking this awesome brand new recipe that will revolutionize the cake industry. I will call it \"The Cake\" and everyone will love it. Thanks for asking. Now, aren't you going to do anything about Eve? ", "god-mainstory-1.5", new [] { "Yes, we are going to meet this afternoon.", "Do I have to?"});
                            break;
                    }
                    break;
                case "god-mainstory-1.5":
                    switch (selectedOption)
                    {
                        default:
                            Say("What do you mean?!\nEve has been kidnapped!\nDid you forget already?", "god-mainstory-2", new []{"Eve was taken hostage? NOOOOOOO!", "Why can I not remember this?"});
                            break;
                    }
                    break;
                case "god-mainstory-2":
                    switch (selectedOption)
                    {
                        case 0:
                            Say("Yes! You need to go find her before it is too late!", "god-mainstory-3", new [] {"This is so cliche.", "I'm on it, just tell me where to go."});
                            break;
                        case 1:
                            Say("You... had too much honey yesterday. But the point is, you need to rescue her!","god-mainstory-3", new []{"This is so cliche.","I'm on it just tell me where to go."});
                            break;
                    }
                    break;
                case "god-mainstory-3":
                    switch (selectedOption)
                    {
                        default:
                            Say("Eve was captured by Satan! This means you must find him in his lair: \nPANDEMONIUM!\nIt will be a difficult journey, so take this.","god-mainstory-4",null);
                            break;
                    }
                    break;
                case "god-mainstory-4":
                    Say("God gives you a toothpick.","god-mainstory-5",null);
                    break;
                case "god-mainstory-5":
                    Say("Be very careful with it.\nNow Go!",null,null);
                    Main.LevelProgression.HasStartedMainQuest = true;
                    break;
            }
        }
    }
}
