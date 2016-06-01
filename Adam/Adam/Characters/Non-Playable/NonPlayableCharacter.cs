using System;
using Adam.Levels;
using Adam.Noobs;
using Adam.PlayerCharacter;
using Adam.UI;
using Microsoft.Xna.Framework;

namespace Adam.Characters
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

        protected NonPlayableCharacter()
        {
        }

        public NonPlayableCharacter(Tile sourceTile)
        {
            _xCoord = sourceTile.GetDrawRectangle().X;
            _yCoord = sourceTile.GetDrawRectangle().Y;
            _sourceTileIndex = sourceTile.TileIndex;

            if (GameWorld.Instance.WorldData.MetaData[sourceTile.TileIndex] != null)
            {
                string metadata = GameWorld.Instance.WorldData.MetaData[sourceTile.TileIndex];
                if (metadata.StartsWith("npc:"))
                {
                    var npcName = metadata.Substring(4);
                    Console.WriteLine("Creating NPC with name {0}", npcName);
                    CreateNpc(npcName);
                }
            }
            else
            {
                Main.TextInputBox.Show("Please enter the name of the NPC you would like to put here.");
                Main.TextInputBox.OnInputEntered += OnNpcNameEntered;
                Console.WriteLine("Creating brand new NPC");
            }
        }

        private void OnNpcNameEntered(TextInputArgs e)
        {
            Main.TextInputBox.OnInputEntered -= OnNpcNameEntered;
            string npcName = e.Input.ToLower();
            GameWorld.Instance.WorldData.MetaData[_sourceTileIndex] = "npc:" + npcName;
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
                default:
                    Main.MessageBox.Show("This NPC does not exist.");
                    Main.TextInputBox.ShowSameMessage();
                    return;
            }
            GameWorld.Instance.Entities.Add(_npc);
            GameWorld.Instance.Player.InteractAction += Player_InteractAction;
            Console.WriteLine("NPC created successfully.");
        }

        /// <summary>
        /// If the player sends an interact event and is colliding with the NPC, the NPC will talk.
        /// </summary>
        private void Player_InteractAction()
        {
            Player player = GameWorld.Instance.Player;
            if (player.GetCollRectangle().Intersects(_npc.GetCollRectangle()))
            {
                _npc.ShowDialog();
            }
        }

        /// <summary>
        /// Makes the NPC say whatever it has to say and plays talk aniamtion.
        /// </summary>
        protected virtual void ShowDialog()
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
            Main.Dialog.Say(text, nextDialogCode, options);
        }

        /// <summary>
        /// Returns complex animation draw rectangle because all NPCs have complex anims.
        /// </summary>
        protected override Rectangle DrawRectangle => ComplexAnim.GetDrawRectangle();
    }
}
