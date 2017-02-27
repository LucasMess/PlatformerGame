using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;

namespace Adam.Characters
{
    class Rose : NonPlayableCharacter
    {
        public Rose(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            _complexAnimation = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/rose");
            CollRectangle = new Rectangle(x, y, 48, 80);
            SetPosition(new Vector2(x, y));
            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 26, 40), 0, 26, 40, 500, 1, true));
            AddAnimationToQueue("still");
            AdamGame.Dialog.NextDialog += Dialog_NextDialog;
        }

        private void Dialog_NextDialog(string code, int optionChosen)
        {

        }
    }
}
