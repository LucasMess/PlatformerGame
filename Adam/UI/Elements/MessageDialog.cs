using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.UI.Elements
{
    /// <summary>
    /// Used to display messages that only have an OK button. Requires no other input from user.
    /// </summary>
    class MessageDialog : ActionDialog
    {
        private UiWindow _window;
        private string _message;

        public MessageDialog(string message)
        {
            _message = message;

            _window.AddElement(new Message(_message));

        }

    }
}
