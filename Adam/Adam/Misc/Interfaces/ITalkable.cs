using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Interfaces
{
    public interface ITalkable
    {
        int StartingConversation { get; set; }
        int CurrentConversation { get; set; }
        bool EndConversation { get; set; }

        void OnNextDialog();
    }

}
