using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ThereMustBeAnotherWay.UI.MainMenu
{
    public static class SaveSelector
    {
        static TextButton save1;
        static TextButton save2;
        static TextButton save3;

        static List<TextButton> buttons;

        public static int ActiveSave = -1;

        public static void Initialize()
        {
            save1 = new TextButton(new Vector2(0, 0), "Save 1", false);
            save1.MouseClicked += Save1_MouseClicked;

            save2 = new TextButton(new Vector2(0, 0), "Save 2", false);
            save2.MouseClicked += Save2_MouseClicked;

            save3 = new TextButton(new Vector2(0, 0), "Save 3", false);
            save3.MouseClicked += Save3_MouseClicked;

            var back = new TextButton(new Vector2(0, 0), "Back", false);
            back.MouseClicked += Back_MouseClicked;

            buttons = new List<TextButton>
            {
                save1,
                save2,
                save3,
                back
            };
            foreach (var button in buttons)
            {
                button.ChangeDimensions(new Vector2(TextButton.Width * 2, TextButton.Height * 2));
                button.Color = new Color(196, 69, 69);
            }

            int startingY = 200;
            int x = TMBAW_Game.DefaultUiWidth / 2 - TextButton.Width;
            for(int i = 0; i < buttons.Count; i++)
            {
                buttons[i].SetPosition(new Vector2(x, startingY + (TextButton.Height * 2 + 10) * i));
            }
        }

        private static void Save3_MouseClicked(Button button)
        {
            ActiveSave = 3;
            StoryTracker.Profile = DataFolder.GetPlayerProfile(ActiveSave);
            StoryTracker.ResumeFromSavePoint();
        }

        private static void Save2_MouseClicked(Button button)
        {
            ActiveSave = 2;
            StoryTracker.Profile = DataFolder.GetPlayerProfile(ActiveSave);
            StoryTracker.ResumeFromSavePoint();
        }

        private static void Back_MouseClicked(Button button)
        {
            TMBAW_Game.ChangeState(GameState.MainMenu, GameMode.None, false);
        }

        private static void Save1_MouseClicked(Button button)
        {
            ActiveSave = 1;
            StoryTracker.Profile = DataFolder.GetPlayerProfile(ActiveSave);
            StoryTracker.ResumeFromSavePoint();
        }

        public static void Update()
        {
            foreach (var button in buttons)
            {
                button.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in buttons)
            {
                button.Draw(spriteBatch);
            }
        }
    }
}
