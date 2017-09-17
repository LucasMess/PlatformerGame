using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.UI.Elements;

namespace ThereMustBeAnotherWay.UI
{
    /// <summary>
    /// Decides when and in what order to show UI elements.
    /// </summary>
    static class UiController
    {
        static Queue<ActionDialog> actionDialogQueue;
        static bool hasActiveItem = false;
        static ActionDialog activeItem;

        public static void Initialize()
        {
            actionDialogQueue = new Queue<ActionDialog>();
        }

        public static void Update()
        {
            if (!hasActiveItem)
            {
                DequeueActionDialog();
            }
            else
            {
                activeItem?.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (hasActiveItem)
            {
                if (activeItem == null)
                {

                }
                else
                {
                    activeItem.Draw(spriteBatch);
                }
            }
        }

        public static void EnqueueActionDialog(ActionDialog actionDialog)
        {
            actionDialogQueue.Enqueue(actionDialog);
        }

        private static void DequeueActionDialog()
        {
            if (actionDialogQueue.Count == 0)
            {
                // TODO: Throw error.
            }
            else if (IsAnyUiElementActive())
            {
                // TODO: Throw error.
            }
            else
            {
                activeItem = actionDialogQueue.Dequeue();
                activeItem.OnDialogDismissed += OnActionDialogDismissed;
            }
        }

        private static void OnActionDialogDismissed(object sender, EventArgs e)
        {
            hasActiveItem = false;
            activeItem = null;
        }

        public static bool IsAnyUiElementActive()
        {
            if (actionDialogQueue.Count == 0)
                return false;
            return true;
        }
    }
}
