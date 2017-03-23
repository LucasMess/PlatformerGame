﻿using Adam.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adam.PlayerCharacter;

namespace Adam.Interactables
{
    /// <summary>
    /// Sends a trigger word to the story tracker.
    /// </summary>
    class DialogueActivator : Interactable
    {
        Tile source;
        public DialogueActivator(Tile tile)
        {
            source = tile;
            CanBeLinkedByOtherInteractables = true;
        }

        public override void OnPlayerAction(Tile tile, Player player)
        {
            string[] commands = GetCommands(source);
            if (commands != null && commands.Length > 1)
            {
                if (commands[0] == "trigger")
                {
                    StoryTracker.AddTrigger(commands[1]);
                }
            }

            base.OnPlayerAction(tile, player);
        }

        public override void OnPlayerClickInEditMode(Tile tile)
        {
            AdamGame.TextInputBox.Show("What trigger would you like to set?");
            AdamGame.TextInputBox.OnInputEntered += TextInputBox_OnInputEntered;

            string[] commands = GetCommands(tile);
            if (commands != null)
            {
                if (commands.Length > 1)
                {
                    if (commands[0] == "trigger")
                    {
                        AdamGame.TextInputBox.SetTextTo(commands[1]);
                    }
                }
            }

            base.OnPlayerClickInEditMode(tile);
        }

        private void TextInputBox_OnInputEntered(UI.TextInputArgs e)
        {
            AdamGame.TextInputBox.OnInputEntered -= TextInputBox_OnInputEntered;
            string value = "trigger:" + e.Input;
            if (GameWorld.WorldData.MetaData.ContainsKey(source.TileIndex))
            {
                GameWorld.WorldData.MetaData[source.TileIndex] = value;
            }
            else
            {
                GameWorld.WorldData.MetaData.Add(source.TileIndex, value);
            }
        }
    }
}
