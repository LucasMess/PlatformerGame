﻿using ThereMustBeAnotherWay.Characters.Non_Playable;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Noobs;
using ThereMustBeAnotherWay.PlayerCharacter;
using ThereMustBeAnotherWay.UI;
using Microsoft.Xna.Framework;
using System;
using ThereMustBeAnotherWay.UI;

namespace ThereMustBeAnotherWay.Characters
{
    /// <summary>
    /// Subset of characters that cannot be played and are not enemies. These characters can sometimes be talked to.
    /// 
    /// Created by Lucas Message a long time ago.
    /// Cleaned up on 1/24/2016
    /// </summary>
    public class NonPlayableCharacter : Character
    {
        public string CharacterId { get; set; }
        private readonly int _xCoord;
        private readonly int _yCoord;
        private readonly int _sourceTileIndex;
        private NonPlayableCharacter _npc;

        public Type CurrentType;

        public enum Type
        {
            None, God, Charlie, HarryPotter, Rose, Scooter, Vladimir, Will,
            COM,
        }

        protected NonPlayableCharacter()
        {
        }

        public NonPlayableCharacter(Tile sourceTile)
        {
            _xCoord = sourceTile.GetDrawRectangle().X;
            _yCoord = sourceTile.GetDrawRectangle().Y;
            _sourceTileIndex = sourceTile.TileIndex;

            if (GameWorld.WorldData.MetaData.ContainsKey(sourceTile.TileIndex))
            {
                string metadata = GameWorld.WorldData.MetaData[sourceTile.TileIndex];
                if (metadata.StartsWith("npc:"))
                {
                    var npcName = metadata.Substring(4);
                    Console.WriteLine("Creating NPC with name {0}", npcName);
                    CreateNpc(npcName);
                }
            }
            else
            {
                TMBAW_Game.TextInputBox.Show("Please enter the name of the NPC you would like to put here.");
                TMBAW_Game.TextInputBox.OnInputEntered += OnNpcNameEntered;
                Console.WriteLine("Creating brand new NPC");
            }
        }

        private void OnNpcNameEntered(TextInputArgs e)
        {
            TMBAW_Game.TextInputBox.OnInputEntered -= OnNpcNameEntered;
            string npcName = e.Input.ToLower();
            GameWorld.WorldData.MetaData[_sourceTileIndex] = "npc:" + npcName;
            CreateNpc(npcName);
        }

        private void CreateNpc(string npcName)
        {
            switch (npcName)
            {
                case "god":
                    _npc = new God(_xCoord, _yCoord);
                    break;
                case "charlie":
                    _npc = new Charlie(_xCoord, _yCoord);
                    break;
                case "harrypotter":
                    _npc = new HarryPotter(_xCoord, _yCoord);
                    break;
                case "rose":
                    _npc = new Rose(_xCoord, _yCoord);
                    break;
                case "scooter":
                    _npc = new Scooter(_xCoord, _yCoord);
                    break;
                case "vladimir":
                    _npc = new Vladimir(_xCoord, _yCoord);
                    break;
                case "will":
                    _npc = new Will(_xCoord, _yCoord);
                    break;
                case "COM":
                    _npc = new COM(_xCoord, _yCoord);
                    break;
                default:
                    TMBAW_Game.MessageBox.Show("This NPC does not exist.");
                    return;
            }
            GameWorld.AddEntityAt(_sourceTileIndex, _npc);
            foreach (Player player in GameWorld.GetPlayers())
                player.InteractAction += Player_InteractAction;
            Console.WriteLine("NPC created successfully.");
        }

        /// <summary>
        /// If the player sends an interact event and is colliding with the NPC, the NPC will talk.
        /// </summary>
        private void Player_InteractAction()
        {
            foreach (Player player in GameWorld.GetPlayers())
                if (player.GetCollRectangle().Intersects(_npc.GetCollRectangle()))
                {
                    _npc.ShowDialog(null, 0);
                }
        }

        /// <summary>
        /// Makes the NPC say whatever it has to say and plays talk aniamtion.
        /// </summary>
        public virtual void ShowDialog(string code, int optionChosen)
        {
            //TODO: Add talking animation.
            //ComplexAnim.AddToQueue("talk");
        }

        /// <summary>
        /// Shortcut to dialog method.
        /// </summary>
        /// <param name="text">The text the character will say.</param>
        /// <param name="nextDialogCode">The code that defines what the next dialog should be.</param>
        /// <param name="options">The options the player has to choose from.</param>
        protected void Say(string text, string nextDialogCode, string[] options)
        {
            TMBAW_Game.Dialog.Say(text, nextDialogCode, options);
        }

        /// <summary>
        /// Returns complex animation draw rectangle because all NPCs have complex anims.
        /// </summary>
        protected override Rectangle DrawRectangle => _complexAnimation.GetDrawRectangle();
    }
}
