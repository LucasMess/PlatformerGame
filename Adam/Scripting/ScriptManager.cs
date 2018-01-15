using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay.Scripting
{
    class ScriptManager
    {

        public ScriptManager(string filename)
        {
            Lua lua = new Lua();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Scripts", filename + ".lua");
            lua.DoFile(path);

            string b = (string)lua["name"];
            Console.WriteLine("Name:" + b);
            Console.ReadLine();
        }

        //public Entity GetEntityByName()
        //{

        //}

        //public void FocusCameraOn(Entity entity)
        //{

        //}

    }
}
