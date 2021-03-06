﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Characters
{
    class Charlie : NonPlayableCharacter
    {
        public Charlie(int x, int y)
        {
            ObeysGravity = true;
            IsCollidable = true;
            _complexAnimation = new ComplexAnimation();
            Texture = ContentHelper.LoadTexture("Characters/NPCs/charlie");
            CollRectangle = new Rectangle(x, y, 48, 80);
            SetPosition(new Vector2(x, y));
            _complexAnimation.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 24, 40), 0, 24, 40, 500, 1, true));
            AddAnimationToQueue("still");
            TMBAW_Game.Dialog.NextDialog += Dialog_NextDialog;
        }

        private void Dialog_NextDialog(string code, int optionChosen)
        {
            switch (code)
            {
                case "charlie-honeyquest-1":
                    switch (optionChosen)
                    {
                        case 0:

                            break;
                    }
                    break;
            }
        }
    }
}
