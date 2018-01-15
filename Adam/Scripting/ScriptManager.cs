using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay.Scripting
{
    class ScriptManager
    {
        private Thread _interpreterThread;
        private Lua _lua;
        private String _path;

        public ScriptManager()
        {

        }

        /// <summary>
        /// Sets the script file name that should be read for commands. Do not include file extension.
        /// </summary>
        /// <param name="filename"></param>
        public void SetFilename(string filename)
        {
            // Get the script in the content directory.
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Scripts", filename + ".lua");

            if (File.Exists(_path))
                Console.WriteLine("Script found at: " + _path);
            else
            {
                Console.WriteLine("No script found with path: " + _path);
                _path = null;
            }
        }

        /// <summary>
        /// Creates the lua interpreter and registers functions.
        /// </summary>
        public void Initialize()
        {
            _lua = new Lua();
            RegisterFunctions();
        }

        /// <summary>
        /// Binds the methods in this class to lua functions.
        /// </summary>
        private void RegisterFunctions()
        {
            ScriptFunctions functions = new ScriptFunctions();

            var methods = typeof(ScriptFunctions).GetMethods();
            foreach (var method in methods)
            {
                String name = method.Name;
                // Converts name to camel case for lua functions.
                String luaCaseName = name.Substring(0, 1).ToLower() + name.Substring(1);
                _lua.RegisterFunction(luaCaseName, functions, method);
                Console.WriteLine("Registered function: " + name);
            }
        }

        /// <summary>
        /// Starts the separate thread where the script commands are read.
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Starting interpreter thread...");
            _interpreterThread = new Thread(new ThreadStart(Run));
            _interpreterThread.Start();
        }

        // Interprets each command one by one.
        private void Run()
        {
            if (_path == null)
            {
                Console.WriteLine("Not running script because no script was found with this level name");
            }
            else
            {
                _lua.DoFile(_path);
            }
        }

    }
}
