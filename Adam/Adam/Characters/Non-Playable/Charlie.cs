using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.Characters
{
    class Charlie : NonPlayableCharacter
    {
        public Charlie(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            ComplexAnim = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/charlie");
            CollRectangle = new Rectangle(x, y, 48, 80);
            ComplexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 24, 40), 0, 24, 40, 500, 1, true));
            AddAnimationToQueue("still");
            Main.Dialog.NextDialog += Dialog_NextDialog;
        }

        private void Dialog_NextDialog(string code, int optionChosen)
        {
            
        }
    }
}
