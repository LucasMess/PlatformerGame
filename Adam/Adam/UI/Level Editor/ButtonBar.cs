using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// The UI element at the top-center of the screen in the level editor.
    /// </summary>
    class ButtonBar
    {
        private Container _container;
        List<FunctionButton> buttons = new List<FunctionButton>();


        private Texture2D _texture;
        private Rectangle _drawRectangle;
        private Rectangle _sourceRectangle;

        public ButtonBar()
        {
            _texture = GameWorld.UiSpriteSheet;
            _sourceRectangle = new Rectangle(0, 212, 382, 40);
            _drawRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(_sourceRectangle.Width), CalcHelper.ApplyHeightRatio(_sourceRectangle.Height));

            int x = Main.UserResWidth / 2 - _drawRectangle.Width / 2;
            _drawRectangle.X = x;


            var wallButton = new WallButton(new Vector2(100, 8), _drawRectangle);
            var playButton = new PlayButton(new Vector2(346, 11), _drawRectangle);
            var deleteButton = new DeleteButton(new Vector2(364, 11), _drawRectangle);
            var expandButton = new ExpandButton(new Vector2(322, 11), _drawRectangle);

            buttons.Add(wallButton);
            buttons.Add(playButton);
            buttons.Add(deleteButton);
            buttons.Add(expandButton);

            _container = new Container(0, 0, 100, 200);
        }

        public void Update()
        {
            foreach (var button in buttons)
            {
                button.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);

            foreach (var button in buttons)
            {
                button.Draw(spriteBatch);
            }
            foreach (var button in buttons)
            {
                button.DrawOnTop(spriteBatch);
            }
        }
    }
}
