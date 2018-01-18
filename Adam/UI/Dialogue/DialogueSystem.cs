using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;

namespace ThereMustBeAnotherWay.UI.Dialogue
{
    public static class DialogueSystem
    {
        private static Queue<DialogueData> _queue;
        private static DialogueBox _dialogueBox;

        private static bool _skipDialogueButtonDown = false;

        /// <summary>
        /// Returns true if a dialogue box is currently being shown to the player.
        /// </summary>
        public static bool IsShowingDialogue => _dialogueBox == null ? false : _dialogueBox.IsActive;

        public delegate void DialogueHandler(int optionChosen);
        public static event DialogueHandler OnDialogueEnd;

        public static void Initialize()
        {
            _queue = new Queue<DialogueData>();
            _dialogueBox = new DialogueBox();
        }

        /// <summary>
        /// Adds the given dialogue data to the queue to be shown when possible.
        /// </summary>
        /// <param name="data"></param>
        public static void Enqueue(DialogueData data)
        {
            _queue.Enqueue(data);
        }

        public static void Update()
        {
            if (_queue.Count != 0)
            {
                ShowNextDialogue();
            }

            if (IsShowingDialogue)
            {
                _dialogueBox.Update();
                if (GameWorld.GetPlayers()[0].IsSkipDialoguePressed() && !_skipDialogueButtonDown)
                {
                    _skipDialogueButtonDown = true;
                    Console.WriteLine("User is trying to skip dialogue.");
                    _dialogueBox.SkipDialogue();
                    // If skypping the dialogue caused it to end.
                    if (!_dialogueBox.IsActive)
                    {
                        OnDialogueEnd?.Invoke(-1);
                        Update();
                    }
                }
                if (!GameWorld.GetPlayers()[0].IsSkipDialoguePressed())
                {
                    _skipDialogueButtonDown = false;
                }
            }
        }

        private static void ShowNextDialogue()
        {
            DialogueData data = _queue.Dequeue();
            _dialogueBox = new DialogueBox();
            _dialogueBox.Show(data.CharacterName, data.Text);
            Console.WriteLine("Showing dialogue with text: " + data.Text);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            _dialogueBox.Draw(spriteBatch);
        }

    }
}
