using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.UI;
using ThereMustBeAnotherWay.UI.Dialogue;

namespace ThereMustBeAnotherWay.Scripting
{
    public class ScriptFunctions
    {
        /// <summary>
        /// Activates the cutscene black bars in the overlay and deactivates player input.
        /// </summary>
        public void ActivateCutsceneMode()
        {
            Overlay.BlackBars.Show();
            GameWorld.IsInCutsceneMode = true;
        }

        /// <summary>
        /// Hides the cutscene black bar in the overlay and reactivates player input.
        /// </summary>
        public void DeactivateCutsceneMode()
        {
            Overlay.BlackBars.Hide();
            GameWorld.IsInCutsceneMode = false;
        }

        /// <summary>
        /// Waits the given amount of time in milliseconds before proceeding.
        /// </summary>
        public void Wait(double time)
        {
            Thread.Sleep((int)time);
        }

        public int ShowDialogue(string characterName, string text, params string[] options)
        {
            DialogueSystem.Enqueue(new DialogueData()
            {
                CharacterName = characterName,
                Text = text,
                Options = options,
            });

            bool dialogueIsDone = false;
            int optionChosen = -1;
            DialogueSystem.OnDialogueEnd += delegate(int code){
                optionChosen = code;
                dialogueIsDone = true;
            };

            while (!dialogueIsDone)
            {
                Thread.Sleep(100);
            }

            return optionChosen;
        }
    }
}
