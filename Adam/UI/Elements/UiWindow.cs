using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.UI.Elements
{
    /// <summary>
    /// Can add elements to it and it will automatically resize and place the elements.
    /// </summary>
    class UiWindow
    {
        private List<UiElement> _elements = new List<UiElement>();
        private Container _container;
        private const int Padding = 10;

        public UiWindow()
        {

        }

        public void AddElement(UiElement element)
        {
            _elements.Add(element);
            CalculatePositions();
            Resize();
        }

        private void CalculatePositions()
        {

        }

        private void Resize()
        {
            int width = 0;
            int height = 0;
            foreach (var elem in _elements)
            {
                width += elem.Width;
                height += elem.Height + Padding;
            }

            _container = new Container(width + Padding * 2, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _container.Draw(spriteBatch);

            foreach (var elem in _elements)
            {
                elem.Draw(spriteBatch);
            }
        }
    }
}
