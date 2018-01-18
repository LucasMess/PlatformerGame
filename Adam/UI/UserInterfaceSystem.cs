using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.UI.Elements;

namespace ThereMustBeAnotherWay.UI
{
    public static class UserInterfaceSystem
    {
        private static List<UiElement> _elements;
        private static Thread _thread;

        public static void Initialize()
        {
            _elements = new List<UiElement>();
            _thread = new Thread(new ThreadStart(Update));
            _thread.Start();
        }

        private static void Update()
        {
            while (true)
            {

            }
        }


    }
}
