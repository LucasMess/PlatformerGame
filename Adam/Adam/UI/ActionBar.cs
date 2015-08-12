using Adam.Levels;
using Adam.Network;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    class ActionBar
    {
        Rectangle box;
        PlayButton playButton;
        OpenButton openButton;
        SaveButton saveButton;
        List<FunctionButton> buttons = new List<FunctionButton>();

        LevelEditor levelEditor;
        GameWorld gameWorld;

        float velocityY;
        int originalY;

        public ActionBar()
        {
            box = new Rectangle((int)((Game1.DefaultResWidth / 2) / Game1.WidthRatio), (int)(Game1.DefaultResHeight / Game1.HeightRatio), (int)(184 / Game1.WidthRatio), (int)(40 / Game1.HeightRatio));
            box.X -= box.Width / 2;
            box.Y -= box.Height;
            originalY = box.Y;

            playButton = new PlayButton(new Vector2(12 + 64, 4), box);
            openButton = new OpenButton(new Vector2(8 + 32, 4),box);
            saveButton = new SaveButton(new Vector2(16 + 96, 4),box);

            playButton.MouseClicked += PlayButton_MouseClicked;
            openButton.MouseClicked += OpenButton_MouseClicked;
            saveButton.MouseClicked += SaveButton_MouseClicked;

            buttons.Add(playButton);
            buttons.Add(openButton);
            buttons.Add(saveButton);
        }

        private void SaveButton_MouseClicked()
        {
            gameWorld.worldData.SaveLevelLocally();
        }

        private void OpenButton_MouseClicked()
        {
            gameWorld.worldData.OpenLevelLocally();
        }

        private void PlayButton_MouseClicked()
        {
            WorldConfigFile data = new WorldConfigFile(GameWorld.Instance);
            data.LoadIntoPlay();
        }

        public void Update()
        {
            gameWorld = GameWorld.Instance;
            levelEditor = gameWorld.levelEditor;

            if (levelEditor.onInventory)
            {
                velocityY = (originalY - box.Y)/5;
                if (box.Y < originalY)
                {
                    box.Y = originalY;
                    velocityY = 0;
                }
            }
            else
            {
                int hidingPlace = Game1.UserResHeight + 200;
                velocityY += .3f;
                if (box.Y > hidingPlace)
                {
                    box.Y = hidingPlace;
                    velocityY = 0;
                }
            }

            box.Y += (int)velocityY;

            foreach (FunctionButton b in buttons)
            {
                b.Update(box);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UI_SpriteSheet, box, new Rectangle(0,48,184,40),Color.White * .5f);

            foreach(FunctionButton b in buttons)
            {
                b.Draw(spriteBatch);
            }
        }

    }
}
