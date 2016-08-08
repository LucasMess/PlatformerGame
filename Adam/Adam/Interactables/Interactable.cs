using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    class Interactable
    {

        public virtual void OnTileUpdate(Tile tile)
        {
            
        }

        public virtual void OnPlayerTouch()
        {
            
        }

        public virtual void OnPlayerAction()
        {
            
        }

        public virtual void OnTileDestroyed(Tile tile)
        {
            
        }
    }
}
