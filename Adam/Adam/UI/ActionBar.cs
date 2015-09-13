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
        Rectangle box;
        PlayButton playButton;
        WallButton wallButton;
        List<FunctionButton> buttons = new List<FunctionButton>();

        LevelEditor levelEditor;
        GameWorld gameWorld;

        float velocityY;
        int originalY;

        WorldProperties properties;

        public ActionBar()
        {
            box = new Rectangle((int)((Main.DefaultResWidth / 2) / Main.WidthRatio), (int)(Main.DefaultResHeight / Main.HeightRatio), (int)(184 / Main.WidthRatio), (int)(40 / Main.HeightRatio));
            box.X -= box.Width / 2;
            box.Y -= box.Height;
            originalY = box.Y;
            box.Y = Main.UserResHeight + 300;

            playButton = new PlayButton(new Vector2(12 + 64, 4), box);
           
            wallButton = new WallButton(new Vector2(20 + 128, 4), box);

            playButton.MouseClicked += PlayButton_MouseClicked;
            wallButton.MouseClicked += WallButton_MouseClicked;

            buttons.Add(playButton);
            buttons.Add(wallButton);

            properties = new WorldProperties();
        }

        private void WallButton_MouseClicked()
        {
            gameWorld.levelEditor.ChangeToWallMode();
        }

        private void NewButton_MouseClicked()
        {
            Thread thread = new Thread(new ThreadStart(ShowProperties));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ShowProperties()
        {
            properties.Show();
        }

        public void SaveButton_MouseClicked()
        {
            gameWorld.worldData.SaveLevelLocally();
        }

        private void OpenButton_MouseClicked()
        {

        }

        private bool IsWorldEmpty()
        {
            foreach (Tile t in gameWorld.tileArray)
            {
                if (t.ID != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPlayerInWorld()
        {
            foreach (Tile t in gameWorld.tileArray)
            {
                //check if there is a player tile
                if (t.ID == 200)
                {
                    return true;
                }
            }
            return false;
        }

        private void PlayButton_MouseClicked()
        {
            GameWorld.Instance.levelEditor.onWallMode = false;
            TestLevel();
        }

        private void TestLevel()
        {
            gameWorld.debuggingMode = true;
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
            gameWorld = GameWorld.Instance;
            levelEditor = gameWorld.levelEditor;

            if (box.Y < originalY)
            {
                box.Y = originalY;
                velocityY = 0;
            }

            if (levelEditor.onInventory)
            {
                velocityY = (originalY - box.Y) / 5;

            }
            else
            {
                int hidingPlace = Main.UserResHeight + 200;
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
            spriteBatch.Draw(GameWorld.UI_SpriteSheet, box, new Rectangle(0, 48, 184, 40), Color.White * .5f);

            if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                PlayButton_MouseClicked();
            }

            foreach (FunctionButton b in buttons)
            {
                b.Draw(spriteBatch);
            }
        }

    }
}
