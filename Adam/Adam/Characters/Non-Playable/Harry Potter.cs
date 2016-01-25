using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.Characters
{
    class HarryPotter : NonPlayableCharacter
    {
        public HarryPotter(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            ComplexAnim = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/harrypotter");
            CollRectangle = new Rectangle(x, y, 48, 80);
            ComplexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 26, 40), 0, 26, 40, 500, 1, true));
            AddAnimationToQueue("still");
            Main.Dialog.NextDialog += Dialog_NextDialog;
        }

        protected override void ShowDialog()
        {
            Say("Is this Diagon Alley?",null,null);
            base.ShowDialog();
        }

        private void Dialog_NextDialog(string code, int optionChosen)
        {

        }
    }
}
