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
        private Container container;

        public event EventHandler OnPositiveButtonClick;
        public event EventHandler OnNegativeButtonClick;
        public event EventHandler OnDialogDismissed;

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
