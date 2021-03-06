﻿using ThereMustBeAnotherWay.Characters;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Noobs
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
            _complexAnimation = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/god");
            CollRectangle = new Rectangle(x, y, 48, 80);
            SetPosition(new Vector2(x, y));
            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 24, 40), 0, 24, 40, 500, 4, true));
            AddAnimationToQueue("still");
            TMBAW_Game.Dialog.NextDialog += Dialog_NextDialog;
        }

        public override void ShowDialog(string code, int optionChosen)
        {
            /*
             * How dialog formatting is going to work:
             * 
             * Every level has a folder with a .lvl file and a .dlg file.
             * At level load, the .dlg file is read and each entry is put into a dictionary.
             * This means that each dialogue text has a unique key.
             * 
             * [key] : [Dialogue]
             * 
             * Inside the Dialogue there will be the following attributes:
             *      [Character] : the character that is speaking.
             *      [text] : the text to be shown.
             *      [next] : the next dialogue to be shown, if there are options, null.
             *      [DialogOptions] : a class for the options.
             *      
             *      DialogOptions:
             *          [option-n] : text for option n (n can be up to 4?)
             *          [next-n] : the next key if option n is chosen.
             *          
             * Example of a .dlg file:
             * 
             * "2999" : {
	                "character" : "god",
	                "text" : "Hello! How are you doing?",
	                "next" : 2922,
                }

              "2922" : {
	                "character" : "god",
	                "text" : "I don't care, let's get started.",
	                "next" : null,
	                "options" : {
		                "1" : {
			                "text" : "Why are you so rude?",
			                "next" : "277",
		                },
		                "2" : {
			                "text" : "lmao",
			                "next" : "255"
		                }
	                }
                }
             * 
             * 
             */
            
            
            if (!StoryTracker.Profile.HasStartedMainQuest)
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
                    StoryTracker.Profile.HasStartedMainQuest = true;
                    break;
            }
        }
    }
}
