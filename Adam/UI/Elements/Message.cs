using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.UI.Elements
{
    class Message : UiElement
    {
        private string _message = "";
        public Message(string message)
        {
            _message = message;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
