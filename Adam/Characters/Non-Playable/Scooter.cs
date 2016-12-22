using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;

namespace Adam.Characters
{
    class Scooter : NonPlayableCharacter
    {
        public Scooter(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            ComplexAnim = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/scooter");
            CollRectangle = new Rectangle(x, y, 48, 80);
            ComplexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 26, 40), 0, 26, 40, 500, 1, true));
            AddAnimationToQueue("still");
            AdamGame.Dialog.NextDialog += Dialog_NextDialog;
        }
        protected override void ShowDialog()
        {
            Say("You see thosee clouds over there? I will be the first person to ever reach them! Momma will let me do anything I want after that.", null, null);
            base.ShowDialog();
        }

        private void Dialog_NextDialog(string code, int optionChosen)
        {
        }
    }
}
