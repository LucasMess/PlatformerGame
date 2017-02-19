using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;

namespace Adam.Characters
{
    class Will : NonPlayableCharacter
    {
        public Will(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            _complexAnimation = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/will");
            CollRectangle = new Rectangle(x, y, 48, 68);
            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 28, 34), 0, 28, 34, 500, 1, true));
            AddAnimationToQueue("still");
            AdamGame.Dialog.NextDialog += Dialog_NextDialog;
        }

        protected override void ShowDialog()
        {
            Say("Hey..... can you leave.... please?", "will-whatiswrong-1", new[] { "Is there something wrong?", "Ok..." });
            base.ShowDialog();
        }

        private void Dialog_NextDialog(string code, int optionChosen)
        {
            switch (code)
            {
                case "will-whatiswrong-1":
                    switch (optionChosen)
                    {
                        case 0:
                            Say("Well, I don't want to talk about it. If you could just leave me alone then maybe... Sorry. I didn't want to be rude. It's just that no one was ever nice to me when I was alive, and I was never good at anything. I guess I just don't expect much from others now.", "will-whatiswrong-2", new[] { "Sorry about that.", "I have better things to do." });
                            break;
                        case 1:
                            Say("...", null, null);
                            break;
                    }
                    break;
                case "will-whatiswrong-2":
                    switch (optionChosen)
                    {
                        case 0:
                            Say("That does not make me feel any better...",null,null);
                            break;
                        case 1:
                            Say("...", null, null);
                            break;
                    }
                    break;
            }
        }
    }
}
