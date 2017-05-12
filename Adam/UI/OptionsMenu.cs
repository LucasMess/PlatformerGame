using ThereMustBeAnotherWay.GameData;
using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ThereMustBeAnotherWay.UI
{
    public static class OptionsMenu
    {

        private static List<TextButton> _buttons;


        public static bool IsActive { get; private set; }
        private static bool _buttonReleased;

        public static void Initialize()
        {
            _buttons = new List<TextButton>();

            SettingsFile settings = DataFolder.GetSettingsFile();

            TextButton fullscreen = new TextButton(new Vector2(), "Fullscreen: " + settings.IsFullscreen, false);
            fullscreen.MouseClicked += Fullscreen_MouseClicked;

            TextButton resolution = new TextButton(new Vector2(), "Resolution: " + settings.ResolutionWidth + "x" + settings.ResolutionHeight, false);
            resolution.MouseClicked += Resolution_MouseClicked;

            TextButton musicVolume = new TextButton(new Vector2(), "Music Volume: " + settings.MusicVolume * 100 + "%", false);
            musicVolume.MouseClicked += MusicVolume_MouseClicked;

            TextButton soundVolume = new TextButton(new Vector2(), "Sound Volume: " + settings.SoundVolume * 100 + "%", false);
            soundVolume.MouseClicked += SoundVolume_MouseClicked;

            TextButton back = new TextButton(new Vector2(), "Back", false);
            back.MouseClicked += Back_MouseClicked;

            _buttons.Add(fullscreen);
            _buttons.Add(resolution);
            _buttons.Add(musicVolume);
            _buttons.Add(soundVolume);
            _buttons.Add(back);

            foreach (var button in _buttons)
            {
                button.ChangeDimensions(new Vector2(TextButton.Width * 2, TextButton.Height * 2));
                button.Color = new Color(196, 69, 69);
            }

            int startingY = 200;
            int x = TMBAW_Game.DefaultUiWidth / 2 - TextButton.Width;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].SetPosition(new Vector2(x, startingY + (TextButton.Height * 2 + 10) * i));
            }
        }

        private static void Back_MouseClicked(Button button)
        {
            IsActive = false;
        }

        private static void SoundVolume_MouseClicked(Button button)
        {
            SettingsFile settings = DataFolder.GetSettingsFile();
            settings.SoundVolume = (settings.SoundVolume + .25f);
            if (settings.SoundVolume > 1)
            {
                settings.SoundVolume = settings.SoundVolume % 1 - .25f;
            }
            TMBAW_Game.MaxVolume = settings.SoundVolume;
            DataFolder.SaveSettingsFile(settings);

            button.Text = "Sound Volume: " + settings.SoundVolume * 100 + "%";
        }

        private static void MusicVolume_MouseClicked(Button button)
        {
            SettingsFile settings = DataFolder.GetSettingsFile();
            settings.MusicVolume = (settings.MusicVolume + .25f);
            if (settings.MusicVolume > 1)
            {
                settings.MusicVolume = settings.MusicVolume % 1 - .25f;
            }
            MediaPlayer.Volume = settings.MusicVolume;
            DataFolder.SaveSettingsFile(settings);

            button.Text = "Music Volume: " + settings.MusicVolume * 100 + "%";
        }

        private static void Resolution_MouseClicked(Button button)
        {
            TMBAW_Game.MessageBox.Show("Not yet implemented!");
        }

        private static void Fullscreen_MouseClicked(Button button)
        {
            SettingsFile settings = DataFolder.GetSettingsFile();
            settings.IsFullscreen = !settings.IsFullscreen;
            GraphicsRenderer.SetFullscreen(settings.IsFullscreen);
            DataFolder.SaveSettingsFile(settings);

            button.Text = "Fullscreen: " + settings.IsFullscreen;
        }

        /// <summary>
        /// Shows the options menu.
        /// </summary>
        public static void Show()
        {
            IsActive = true;
            _buttonReleased = false;
        }

        public static void Update()
        {
            if (IsActive)
            {
                if (!InputHelper.IsLeftMousePressed())
                {
                    _buttonReleased = true;
                }

                if (_buttonReleased)
                {
                    foreach (var button in _buttons)
                    {
                        button.Update();
                    }
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                foreach (var button in _buttons)
                {
                    button.Draw(spriteBatch);
                }
            }
        }
    }
}
