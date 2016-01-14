using Adam.UI;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private string _linkedPortalIndex;
        private string _linkedLevelName;

        /// <summary>
        /// Creates blank Portal for UI and other stuff.
        /// </summary>
        public Portal()
        {

        }

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
                    _linkedLevelName = metadata.Substring(6);
                    if (_linkedLevelName.Contains("/"))
                    {
                        string[] data = _linkedLevelName.Split('/');
                        _linkedLevelName = data[0];
                        _linkedPortalIndex = data[1];
                    }
                }
                Console.WriteLine("Creating a Portal that was in the level already. Index:" + _tileIndex);
            }
            else
            {
                Main.TextInputBox.Show("Please enter the name of the level this portal links to");
                Main.TextInputBox.OnInputEntered += OnLevelNameEntered;
                Console.WriteLine("Creating a brand new Portal.Index:" + _tileIndex);
            }

        }

        /// <summary>
        /// Checks if level exists and creates links.
        /// </summary>
        /// <param name="e"></param>
        private void OnLevelNameEntered(TextInputArgs e)
        {
            // If the input has an index to teleport to, then include it.
            if (e.Input.Contains("/"))
            {
                string[] data = e.Input.Split('/');
                _linkedLevelName = data[0];
                _linkedPortalIndex = data[1];
            }
            else
            {
                _linkedLevelName = e.Input;
                _linkedPortalIndex = null;
            }

            if (DataFolder.LevelExists(_linkedLevelName))
            {
                GameWorld.Instance.WorldData.MetaData[_tileIndex] = "pl:nl:" + e.Input;
                Main.TextInputBox.OnInputEntered -= OnLevelNameEntered;
                string message = "Portal link created to level: " + _linkedLevelName + " on index: " +
                                 (_linkedPortalIndex ?? "SPAWN") +
                                 ". This portal index is: " + _tileIndex;
                Main.MessageBox.Show(message);
                Console.WriteLine(message);
            }
            else
            {
                Main.MessageBox.Show("Level does not exist. Try again.");
                Main.TextInputBox.ShowSameMessage();
            }
        }

        private void SourceTile_OnPlayerInteraction(Tile t)
        {
            if (_linkedLevelName == null)
            {
                Main.MessageBox.Show("There is no level linked to this portal. Try placing it again.");
                return;
            }

            GameWorld.Instance.Player.SetSpawnPointForNextLevel(_linkedPortalIndex);
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
