using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.UI.Dialogue
{
    /// <summary>
    /// Stores the data that will be showed by the dialogue system.
    /// </summary>
    public class DialogueData
    {
        public string CharacterName { get; set; } = "Unnamed Character";
        public string Text { get; set; } = "Text is missing.";
        public string[] Options { get; set; }
    }
}
