using Adam.Misc.Errors;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    /// <summary>
    /// Used to select, play, edit, create and delete levels in the main menu screen.
    /// </summary>
    public class LevelSelection
    {
        private Rectangle scissorRectangle;
        private Rectangle boundsDrawRectangle;
        private Rectangle boundsSourceRectangle;
        private Texture2D boundsTexture;

        private SpriteFont headerFont;
        private Vector2 headerPos;
        private string headerText;

        private Rectangle functionButtonContainer;

        private PlayButton playButton;
        private EditButton editButton;
        private RenameButton renameButton;
        private DeleteButton deleteButton;
        private NewButton newButton;
        private BackButton backButton;
        private List<FunctionButton> buttons;

        private List<LevelInfo> levelInfos;
        private LevelInfo selectedLevel;
        private int selectedLevelIndex = -1;
        private int levelCount;

        public static int WidthOfBounds;
        public static int HeightOfBounds;

        private int lastScrollWheel;
        private int startingY;

        public LevelSelection()
        {
            // Defines the bounding box for the Level Info List.
            int widthOfBounds = (int)(600 / Main.WidthRatio);
            int heightOfBounds = (int)(300 / Main.HeightRatio);
            boundsTexture = ContentHelper.LoadTexture("Tiles/ui_spritemap");
            boundsDrawRectangle = new Rectangle(Main.UserResWidth / 2, Main.UserResHeight / 2, widthOfBounds, heightOfBounds);
            boundsDrawRectangle.X -= widthOfBounds / 2;
            boundsDrawRectangle.Y -= heightOfBounds / 2;
            boundsSourceRectangle = new Rectangle(376, 48, 300, 150);

            WidthOfBounds = widthOfBounds;
            HeightOfBounds = heightOfBounds;

            // Defines how big the level selection box will be based on the game's resolution.
            scissorRectangle = new Rectangle(boundsDrawRectangle.X + (int)(12 / Main.WidthRatio), boundsDrawRectangle.Y + (int)(12 / Main.HeightRatio), boundsDrawRectangle.Width - (int)(24 / Main.WidthRatio), boundsDrawRectangle.Height - (int)(24 / Main.HeightRatio));

            // Defines where the function buttons will be.
            functionButtonContainer = new Rectangle(scissorRectangle.X + scissorRectangle.Width - CalcHelper.ApplyHeightRatio(4 + 128 + 16 + 32), scissorRectangle.Y + scissorRectangle.Height + (int)(8 / Main.HeightRatio), (int)(184 / Main.WidthRatio), (int)(40 / Main.HeightRatio));
            playButton = new PlayButton(new Vector2(4, 4), functionButtonContainer);
            editButton = new EditButton(new Vector2(4 + 32 + 4, 4), functionButtonContainer);
            renameButton = new RenameButton(new Vector2(4 + 64 + 8, 4), functionButtonContainer);
            deleteButton = new DeleteButton(new Vector2(4 + 96 + 12, 4), functionButtonContainer);
            newButton = new NewButton(new Vector2(4 + 128 + 16, 4), functionButtonContainer);
            backButton = new BackButton(new Vector2(-500 + 96 + 4, 4), functionButtonContainer);

            buttons = new List<FunctionButton>();
            buttons.Add(playButton);
            buttons.Add(editButton);
            buttons.Add(renameButton);
            buttons.Add(deleteButton);
            buttons.Add(newButton);
            buttons.Add(backButton);

            playButton.MouseClicked += PlayButton_MouseClicked;
            editButton.MouseClicked += EditButton_MouseClicked;
            renameButton.MouseClicked += RenameButton_MouseClicked;
            deleteButton.MouseClicked += DeleteButton_MouseClicked;
            newButton.MouseClicked += NewButton_MouseClicked;
            backButton.MouseClicked += BackButton_MouseClicked;

            // Define the spritefont for the "Select Level" header and its position.
            headerFont = ContentHelper.LoadFont("Fonts/x64");
            headerText = "Select a Level:";
            headerPos = new Vector2(Main.UserResWidth / 2 - headerFont.MeasureString(headerText).X / 2, scissorRectangle.Y - headerFont.LineSpacing - CalcHelper.ApplyHeightRatio(10));
        }

        private void NewButton_MouseClicked()
        {
            Main.TextInputBox.Show("Please enter the name for your new level:");
            Main.TextInputBox.OnInputEntered += NewLevel_OnTextEntered;
        }

        private void NewLevel_OnTextEntered(TextInputArgs e)
        {
            string newPath = DataFolder.CreateNewLevel(e.Input, 256, 256);
            Main.TextInputBox.OnInputEntered -= NewLevel_OnTextEntered;
            DataFolder.EditLevel(newPath);
        }

        private void BackButton_MouseClicked()
        {
            Menu.CurrentMenuState = Menu.MenuState.Main;
        }

        private void DeleteButton_MouseClicked()
        {
            if (selectedLevel == null)
            {
                Main.MessageBox.Show("Please select a level.");
                return;
            }
            DataFolder.DeleteFile(selectedLevel.FilePath);
            LoadLevels();
        }

        private void RenameButton_MouseClicked()
        {
            if (selectedLevel == null)
            {
                Main.MessageBox.Show("Please select a level.");
                return;
            }
            Main.TextInputBox.Show("What would you like to rename this level to?");
            Main.TextInputBox.SetTextTo(selectedLevel.Name);
            Main.TextInputBox.OnInputEntered += RenameButton_OnInputEntered;
        }

        private void RenameButton_OnInputEntered(TextInputArgs e)
        {
            DataFolder.RenameFile(selectedLevel.FilePath, selectedLevel.Name, e.Input);
            Main.TextInputBox.OnInputEntered -= RenameButton_OnInputEntered;
            LoadLevels();
        }

        private void EditButton_MouseClicked()
        {
            if (selectedLevel == null)
            {
                Main.MessageBox.Show("Please select a level.");
                return;
            }
            DataFolder.EditLevel(selectedLevel.FilePath);
        }

        private void PlayButton_MouseClicked()
        {
            try
            {
                DataFolder.PlayLevel(selectedLevel.FilePath);

            }
            catch (Exception e)
            {
                Main.MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Checks the Level directory and load all levels.
        /// </summary>
        public void LoadLevels()
        {
            // Retrieves the information of the levels.
            List<string> filePaths = DataFolder.GetLevelPaths();
            List<string> fileNames = DataFolder.GetLevelNames();
            List<DateTime> fileModifiedDates = DataFolder.GetLevelLastModifiedDates();

            // Stores all this inforamtion into separate LevelInfo classes.
            levelCount = filePaths.Count;
            levelInfos = new List<LevelInfo>();
            for (int i = 0; i < levelCount; i++)
            {
                LevelInfo lev = new LevelInfo(fileNames[i], filePaths[i], fileModifiedDates[i]);
                levelInfos.Add(lev);
            }

            // Arranges the levels in an alphabetical list and sets their position.
            startingY = scissorRectangle.Y + CalcHelper.ApplyHeightRatio(3);
            for (int i = 0; i < levelCount; i++)
            {
                levelInfos[i].SetPosition(startingY, i);
            }
        }

        public void Update()
        {
            // Checks if there is scrolling.
            int scrollWheel = Mouse.GetState().ScrollWheelValue;
            Rectangle mouseRectangle = InputHelper.MouseRectangle;

            if (levelCount > 5)
            {
                if (mouseRectangle.Intersects(scissorRectangle))
                {
                    if (lastScrollWheel != scrollWheel)
                    {
                        foreach (LevelInfo l in levelInfos)
                        {
                            l.VelocityY = (scrollWheel - lastScrollWheel) / 5;
                        }
                    }
                }

                lastScrollWheel = scrollWheel;

                float velocityY = 0;

                // If first element is below its starting point, make them all go back, but only if the user is not making them go back already.
                if (levelInfos[0].GetY() > startingY && levelInfos[0].VelocityY > -1)
                    velocityY = -1f;

                // If last element is above bottom of box, make them go back, but only if the user is not making them go back already.
                if (levelInfos[levelCount - 1].GetY() < scissorRectangle.Y + scissorRectangle.Height - levelInfos[0].GetHeight() - CalcHelper.ApplyHeightRatio(3) && levelInfos[0].VelocityY < 1)
                    velocityY = 1f;

                if (velocityY != 0)
                    foreach (LevelInfo l in levelInfos)
                    {
                        l.VelocityY = velocityY;
                    }
            }

            // Makes the level infos scroll.
            foreach (LevelInfo l in levelInfos)
            {
                l.Update();
            }

            // Check to see if a level info is being selected.
            if (InputHelper.IsLeftMousePressed() && InputHelper.MouseRectangle.Intersects(scissorRectangle))
            {
                foreach (LevelInfo level in levelInfos)
                {
                    if (level.IsBeingClickedOn())
                    {
                        selectedLevelIndex = levelInfos.IndexOf(level);
                        break;
                    }
                }
            }
            if (selectedLevelIndex > -1 && selectedLevelIndex < levelCount)
                selectedLevel = levelInfos[selectedLevelIndex];
            else selectedLevel = null;

            // Update buttons.
            foreach (FunctionButton b in buttons)
            {
                b.Update(functionButtonContainer);
            }

            // Make the selected level know about it.
            foreach (LevelInfo level in levelInfos)
            {
                if (level == selectedLevel)
                {
                    level.IsSelected = true;
                }
                else
                {
                    level.IsSelected = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.Black * .7f);
            spriteBatch.Draw(boundsTexture, boundsDrawRectangle, boundsSourceRectangle, Color.White);

            // Sets the scrolling levels to disappear if they are not inside of this bounding box.
            Rectangle originalScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

            foreach (LevelInfo level in levelInfos)
            {
                level.Draw(spriteBatch);
            }

            // Returns the scissor rectangle to original.
            spriteBatch.GraphicsDevice.ScissorRectangle = originalScissorRectangle;

            // Draw buttons.
            foreach (FunctionButton b in buttons)
            {
                b.Draw(spriteBatch);
            }

            // Draw header text.
            FontHelper.DrawWithOutline(spriteBatch, headerFont, headerText, headerPos, 2, Color.Yellow, Color.Black);
        }
    }

    /// <summary>
    /// Used to contain all the information on a level kept inside the Level directory.
    /// </summary>
    public class LevelInfo
    {
        Rectangle drawRectangle;
        Rectangle sourceRectangle;
        Texture2D boxTexture;
        SpriteFont infoFont;
        SpriteFont nameFont;

        private static int Spacing;

        public float VelocityY { get; set; }

        public LevelInfo(string name, string filePath, DateTime lastModDate)
        {
            // Store information of the level.
            Name = name;
            FilePath = filePath;
            LastModifiedDate = lastModDate;

            // Define how big each info section will be.
            nameFont = ContentHelper.LoadFont("Fonts/objectiveHead");
            infoFont = ContentHelper.LoadFont("Fonts/objectiveText");
            Spacing = infoFont.LineSpacing + 4;
            drawRectangle = new Rectangle(Main.DefaultResWidth - LevelSelection.WidthOfBounds / 2 + (int)(16 / Main.WidthRatio), 0, LevelSelection.WidthOfBounds - (int)(32 / Main.WidthRatio), (int)(50 / Main.HeightRatio));
            sourceRectangle = new Rectangle(128, 0, 284, 25);
            boxTexture = ContentHelper.LoadTexture("Tiles/ui_spritemap");
        }

        public string Name
        {
            get; set;
        }

        public string FilePath
        {
            get; set;
        }

        public DateTime LastModifiedDate
        {
            get; set;
        }

        /// <summary>
        /// Used once to define where the information should be when compared to the other elements in the list.
        /// </summary>
        /// <param name="initialY">The Y-Coordinate of the first item in the list.</param>
        /// <param name="orderInList">The rank of this item in the list.</param>
        public void SetPosition(int initialY, int orderInList)
        {
            drawRectangle.Y = initialY + drawRectangle.Height * orderInList + CalcHelper.ApplyHeightRatio(3) * orderInList;
        }

        public void Update()
        {
            drawRectangle.Y += (int)VelocityY;
            VelocityY *= .92f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Change color if it is selected.
            Color color;
            if (IsSelected)
            {
                color = Color.LightPink;
            }
            else color = Color.White;

            spriteBatch.Draw(boxTexture, drawRectangle, sourceRectangle, color);

            Vector2 namePos = new Vector2(drawRectangle.X + Spacing, drawRectangle.Y + drawRectangle.Height / 2 - nameFont.LineSpacing / 2);
            FontHelper.DrawWithOutline(spriteBatch, nameFont, Name, namePos, 2, Color.LightGray, Color.Black);
            spriteBatch.DrawString(infoFont, GetDate(LastModifiedDate), new Vector2(drawRectangle.X + drawRectangle.Width - 20 - infoFont.MeasureString(GetDate(LastModifiedDate)).X, drawRectangle.Y + drawRectangle.Height / 2 - infoFont.LineSpacing / 2), Color.Black);
        }

        /// <summary>
        /// Converts DateTime into a string.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string GetDate(DateTime dt)
        {
            string seconds = dt.Second.ToString();
            if (dt.Second < 10)
                seconds = "0" + seconds;

            string date = "Last modified: " + dt.Year + "/" + dt.Month + "/" + dt.Day + " at " + dt.Hour + ":" + seconds;
            return date;
        }

        /// <summary>
        /// Checks if the Level Info box is being clicked on.
        /// </summary>
        /// <returns></returns>
        public bool IsBeingClickedOn()
        {
            Rectangle mouse = InputHelper.MouseRectangle;
            return (mouse.Intersects(drawRectangle));
        }

        /// <summary>
        /// Determines whether this is the level info clicked on by the user.
        /// </summary>
        public bool IsSelected
        {
            get; set;
        }

        /// <summary>
        /// Get the Y-coordinate of this level info box.
        /// </summary>
        /// <returns></returns>
        public int GetY()
        {
            return drawRectangle.Y;
        }

        /// <summary>
        /// Get the height of the level info box.
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return drawRectangle.Height;
        }
    }

}
