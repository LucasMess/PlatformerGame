using Adam.UI;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Interactables
{
    /// <summary>
    /// Used to teleport the player to different areas of the map.
    /// </summary>
    [Serializable]
    public class Portal
    {
        Portal _linkedPortal;
        Line _line;
        bool _locked = true;
        int _tileIndex;
        int _linkedPortalIndex;
        string _linkedLevelName = "Tutorial";

        public Portal(Tile sourceTile)
        {
            _tileIndex = sourceTile.TileIndex;
            sourceTile.OnTileDestroyed += SourceTile_OnTileDestroyed;
            sourceTile.OnPlayerInteraction += SourceTile_OnPlayerInteraction;

            if (GameWorld.Instance.WorldData.MetaData[_tileIndex] != null)
            {
                string metadata = GameWorld.Instance.WorldData.MetaData[_tileIndex];
                if (metadata.StartsWith("pl:nl:"))
                {
                    //linkedLevelName = metadata.Substring(7);
                }
            }
            Console.WriteLine("Creating a Portal");
        }

        private void SourceTile_OnPlayerInteraction(Tile t)
        {
            t.OnPlayerInteraction -= SourceTile_OnPlayerInteraction;
            DataFolder.PlayLevel(DataFolder.LevelDirectory + "/" + _linkedLevelName + ".lvl");
        }

        private void SourceTile_OnTileDestroyed(Tile t)
        {
            t.OnTileDestroyed -= SourceTile_OnTileDestroyed;
            t.OnPlayerInteraction -= SourceTile_OnPlayerInteraction;
            GameWorld.Instance.WorldData.MetaData[_tileIndex] = null;
        }

        /// <summary>
        /// The line connecting the two portals in the level editor.
        /// </summary>
        public Line ConnectingLine
        {
            get; set;
        }

        /// <summary>
        /// Returns the id of the portal, which is based on the tile index where it is positioned.
        /// </summary>
        public int PortalId
        {
            get { return _tileIndex; }
        }

        /// <summary>
        /// Connects this portal to the specified portal and unlocks both of them.
        /// </summary>
        /// <param name="p"></param>
        public void LinkTo(Portal p)
        {
            if (_linkedPortal != null)
            {
                _linkedPortal.Locked = true;
            }

            _linkedPortal = p;
            _linkedPortal.Locked = false;
            _locked = false;
        }

        /// <summary>
        /// Returns true if portal is locked.
        /// </summary>
        public bool Locked
        {
            get; set;
        }
    }
}
