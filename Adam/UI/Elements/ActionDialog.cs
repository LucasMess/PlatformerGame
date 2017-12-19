using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.UI.Elements
{
    /// <summary>
    /// UI Dialog that requires immediate action from the user.
    /// </summary>
    class ActionDialog
    {
        private UiWindow _window;

        public event EventHandler OnPositiveButtonClick;
        public event EventHandler OnNegativeButtonClick;
        public event EventHandler OnDialogDismissed;

        public void Show()
        {
            UiController.EnqueueActionDialog(this);
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
