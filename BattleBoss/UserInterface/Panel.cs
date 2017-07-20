using System;
using System.Collections.Generic;
using System.Text;

namespace BattleBoss.UserInterface
{
    public class Panel
    {

        /// <summary>
        /// The elements, in order, of the panel.
        /// </summary>
        private List<Element> _elements;

        /// <summary>
        /// The text to be written at the top of the panel.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Adds an element to the panel right after the last inserted element.
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(Element element)
        {

        }

        internal void Update()
        {

        }
    }
}
