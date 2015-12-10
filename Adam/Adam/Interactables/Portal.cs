using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    /// <summary>
    /// Used to teleport the player to different areas of the map.
    /// </summary>
    public class Portal
    {
        Vector2 position;
        Portal linkedPortal;
        bool locked = true;
        int tileIndex;

        public Portal(int x, int y, int tileIndex)
        {
            position = new Vector2(x, y);
            this.tileIndex = tileIndex;
        }

        /// <summary>
        /// Returns the id of the portal, which is based on the tile index where it is positioned.
        /// </summary>
        public int PortalID
        {
            get { return tileIndex; }
        }

        /// <summary>
        /// Connects this portal to the specified portal and unlocks both of them.
        /// </summary>
        /// <param name="p"></param>
        public void LinkTo(Portal p)
        {
            if (linkedPortal != null)
            {
                linkedPortal.Locked = true;
            }

            linkedPortal = p;
            linkedPortal.Locked = false;
            locked = false;
        }

        /// <summary>
        /// Returns true if portal is locked.
        /// </summary>
        public bool Locked
        {
            get; set;
        }

        /// <summary>
        /// Returns the destination coordinates of portal, unless it is locked or there is not linked portal.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool TryGetDestination(out Vector2 destination)
        {
            if (locked)
            {
                destination = Vector2.Zero;
                return false;
            }

            destination = linkedPortal.Position;
            return true;
        }

        /// <summary>
        /// returns the position of the portal.
        /// </summary>
        public Vector2 Position
        {
            get;
        }
    }
}
