﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.UI.Level_Editor
{
    class CategorySelector : UiElement
    {
        private Rectangle _sourceRectangle = new Rectangle(148, 180, 57, 32);
        private SoundFx _swooshSound = new SoundFx("Sounds/Level Editor/switch_category");
        private Vector2 _oldPosition;
        private Button _followingButton;
        private bool _playSound = true;

        public CategorySelector()
        {
            DrawRectangle = new Rectangle(0, 0, _sourceRectangle.Width * 2
            , _sourceRectangle.Height * 2);
        }

        public void Update()
        {
            _playSound = false;
            MoveTo(_followingButton.GetPosition(), 100);
            _playSound = true;
        }

        public void BindToButton(Button button)
        {
            _followingButton = button;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, DrawRectangle, _sourceRectangle, Color.White);
        }

        public override void MoveTo(Vector2 position, int duration)
        {
            if (_oldPosition != position && _playSound)
            {
                _swooshSound.Play();
                _oldPosition = position;
            }
            position.X -= 12;
            position.Y -= 16;
            base.MoveTo(position, duration);
        }


    }
}
