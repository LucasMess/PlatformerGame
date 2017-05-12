using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace ThereMustBeAnotherWay.Characters.Non_Playable
{
    class COM : NonPlayableCharacter
    {
        public COM(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            _complexAnimation = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/charlie");
            CollRectangle = new Rectangle(0, 0, 24, 48);
            SetPosition(new Vector2(x, y));
            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 24, 40), 0, 24, 40, 500, 1, true));
            AddAnimationToQueue("still");
            TMBAW_Game.Dialog.NextDialog += ShowDialog;
        }

        public override void ShowDialog(string code, int optionChosen)
        {
            switch (code)
            {
                case "greenhills01-wakeup":
                    TMBAW_Game.Dialog.Say("Dot....", "greenhills01-wakeup2", null);
                    break;
                case "greenhills01-wakeup2":
                    TMBAW_Game.Dialog.Say("Are you ok?\n\n Can you hear me? DOT!!!!!!!!!!!!", "greenhills01-fadein", null);
                    break;
                case "greenhills01-fadein":
                    Overlay.FadeIn();
                    StoryTracker.AddTrigger("hasWokenUp");
                    break;
                case "greenhills01-whathappened":
                    TMBAW_Game.Dialog.Say("Oh thank the creator! I thought I would never hear from you again... \n...\n Where are you?", "greenhills01-whathappened2", new[] { "I have no idea...", "I might be in danger", "There are green things all around me!" });
                    break;
                case "greenhills01-whathappened2":
                    TMBAW_Game.Dialog.Say("I am still at the spaceship, master, but it appears to be completely unoperational. The blast right before the time jump seems to have thrown us off course by a couple thousand years, which is not something the spaceship was ever designed for.", "greenhills01-whathappened3", null);
                    break;
                case "greenhills01-whathappened3":
                    TMBAW_Game.Dialog.Say("I'll send you the location of the ship. If you come back quickly we might be able to find a way out of this time period!", "greenhills01-whathappened4" ,null);
                    break;
                case "greenhills01-whathappened4":
                    StoryTracker.AddTrigger("sentCoordinates");
                    break;
                case "greenHills01-cave":
                    TMBAW_Game.Dialog.Say("There seems to be a light source in this cave. Dot, I think we are not alone on this planet.", null, null);
                    break;
                case "greenHills01-door":
                    TMBAW_Game.Dialog.Say("My sensors are reading multiple life signatures behind this door. I cannot determine their friendliness, and I cannot communicate with you if you proceed into the depths of the cave. I advise you to find another way around.", null, null);
                    break;
                default:
                    break;
            }
        }
    }
}
