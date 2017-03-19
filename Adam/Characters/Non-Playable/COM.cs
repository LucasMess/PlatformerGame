using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Adam.Characters.Non_Playable
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
            AdamGame.Dialog.NextDialog += ShowDialog;
        }

        public override void ShowDialog(string code, int optionChosen)
        {
            switch (code)
            {
                case "greenhills01-wakeup":
                    MediaPlayer.Pause();
                    AdamGame.Dialog.Say("Dot....", "greenhills01-wakeup2", null);
                    break;
                case "greenhills01-wakeup2":
                    AdamGame.Dialog.Say("Are you ok?\n\n Can you hear me? DOT!!!!!!!!!!!!", "greenhills01-fadein", null);
                    break;
                case "greenhills01-fadein":
                    Overlay.FadeIn();
                    StoryTracker.AddTrigger("hasWokenUp");
                    break;
                case "greenhills01-whathappened":
                    AdamGame.Dialog.Say("Oh thank the creator! I thought I would never hear from you again... \n...\n Where are you?", "greenhills01-whathappened2", new[] { "I have no idea...", "I might be in danger", "There are green things all around me!" });
                    break;
                case "greenhills01-whathappened2":
                    AdamGame.Dialog.Say("I am still at the spaceship, master, but it appears to be completely unoperational. The blast right before the time jump seems to have thrown us off course by a couple thousand years, which is not something the spaceship was ever designed for.", "greenhills01-whathappened3", null);
                    break;
                case "greenhills01-whathappened3":
                    AdamGame.Dialog.Say("I'll send you the location of the ship. If you come back quickly we might be able to find a way out of this time period!", null,null);
                    StoryTracker.AddTrigger("sentCoordinates");
                    break;
                default:
                    break;
            }
        }
    }
}
