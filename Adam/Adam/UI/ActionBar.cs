using Adam.GameData;
using Adam.Levels;
using Adam.Misc.Errors;
using Adam.Network;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Adam.UI
{
    public class ActionBar
    {
        Rectangle _box;
        PlayButton _playButton;
        WallButton _wallButton;
        List<FunctionButton> _buttons = new List<FunctionButton>();

        LevelEditor _levelEditor;
        GameWorld _gameWorld;

        float _velocityY;
        int _originalY;

        WorldProperties _properties;

        public ActionBar()
        {
            _box = new Rectangle((int)((Main.DefaultResWidth / 2) / Main.WidthRatio), (int)(Main.DefaultResHeight / Main.HeightRatio), (int)(184 / Main.WidthRatio), (int)(40 / Main.HeightRatio));
            _box.X -= _box.Width / 2;
            _box.Y -= _box.Height;
            _originalY = _box.Y;
            _box.Y = Main.UserResHeight + 300;

            _playButton = new PlayButton(new Vector2(12 + 64, 4), _box);
           
            _wallButton = new WallButton(new Vector2(20 + 128, 4), _box);

            _playButton.MouseClicked += PlayButton_MouseClicked;
            _wallButton.MouseClicked += WallButton_MouseClicked;

            _buttons.Add(_playButton);
            _buttons.Add(_wallButton);

            _properties = new WorldProperties();
        }

        private void WallButton_MouseClicked()
        {
            _gameWorld.LevelEditor.ChangeToWallMode();
        }

        private void NewButton_MouseClicked()
        {
            Thread thread = new Thread(new ThreadStart(ShowProperties));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ShowProperties()
        {
            _properties.Show();
        }

        private void OpenButton_MouseClicked()
        {

        }

        private bool IsWorldEmpty()
        {
            foreach (Tile t in _gameWorld.TileArray)
            {
                if (t.Id != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPlayerInWorld()
        {
            foreach (Tile t in _gameWorld.TileArray)
            {
                //check if there is a player tile
                if (t.Id == 200)
                {
                    return true;
                }
            }
            return false;
        }

        private void PlayButton_MouseClicked()
        {
            DataFolder.SaveLevel();
            GameWorld.Instance.LevelEditor.OnWallMode = false;
            TestLevel();
        }

        private void TestLevel()
        {
            _gameWorld.DebuggingMode = true;
            try
            {
                DataFolder.PlayLevel(DataFolder.CurrentLevelFilePath);
            }
            catch (Exception e)
            {
                Main.MessageBox.Show(e.Message);
            }
        }

        public void Update()
        {
            _gameWorld = GameWorld.Instance;
            _levelEditor = _gameWorld.LevelEditor;

            if (_box.Y < _originalY)
            {
                _box.Y = _originalY;
                _velocityY = 0;
            }

            if (_levelEditor.OnInventory)
            {
                _velocityY = (_originalY - _box.Y) / 5;

            }
            else
            {
                int hidingPlace = Main.UserResHeight + 200;
                _velocityY += .3f;
                if (_box.Y > hidingPlace)
                {
                    _box.Y = hidingPlace;
                    _velocityY = 0;
                }
            }

            _box.Y += (int)_velocityY;

            foreach (FunctionButton b in _buttons)
            {
                b.Update(_box);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _box, new Rectangle(0, 48, 184, 40), Color.White * .5f);

            if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                PlayButton_MouseClicked();
            }

            foreach (FunctionButton b in _buttons)
            {
                b.Draw(spriteBatch);
            }

            foreach (FunctionButton b in _buttons)
            {
                b.DrawOnTop(spriteBatch);
            }
        }

    }
}
