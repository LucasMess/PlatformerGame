﻿using Adam.Levels;
using Adam.Misc;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    class CategorySelector : UiElement
    {
        private Rectangle _sourceRectangle = new Rectangle(148, 180, 57, 32);
        private SoundFx _swooshSound = new SoundFx("Sounds/Level Editor/switch_category");
        private Vector2 _oldPosition;

        public CategorySelector()
        {
            DrawRectangle = new Rectangle(0, 0, _sourceRectangle.Width
            , _sourceRectangle.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, DrawRectangle, _sourceRectangle, Color.White);
        }

        public override void MoveTo(Vector2 position, int duration)
        {
            if (_oldPosition != position)
            {
                _swooshSound.Play();
                _oldPosition = position;
            }
            position.X -= 6;
            position.Y -= 9;
            base.MoveTo(position, duration);
        }
    }
}
