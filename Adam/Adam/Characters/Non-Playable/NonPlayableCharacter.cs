using Adam.Levels;
using Microsoft.Xna.Framework;

namespace Adam.Characters
{
    /// <summary>
    /// Subset of characters that cannot be played and are not enemies. These characters can sometimes be talked to.
    /// </summary>
    public abstract class NonPlayableCharacter : Character
    {
        public string CharacterId { get; set; }

        protected NonPlayableCharacter()
        {
            GameWorld.Instance.Player.InteractAction += Player_InteractAction;
        }

        /// <summary>
        /// If the player sends an interact event and is colliding with the NPC, the NPC will talk.
        /// </summary>
        private void Player_InteractAction()
        {
            Player.Player player = GameWorld.Instance.Player;
            if (player.GetCollRectangle().Intersects(GetCollRectangle()))
            {
                ShowDialog();
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
        /// <param name="s"></param>
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
