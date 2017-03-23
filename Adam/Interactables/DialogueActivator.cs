using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Interactables
{
    /// <summary>
    /// Sends a trigger word to the story tracker.
    /// </summary>
    class DialogueActivator : Interactable
    {
        public DialogueActivator(Tile tile)
        {
            CanBeLinkedByOtherInteractables = true;
        }


    }
}
