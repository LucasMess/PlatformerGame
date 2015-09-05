using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Rectangle scrollingFileBounds;
        private List<LevelInfo> levelInfos;
        private int levelCount;

        public static int WidthOfBounds;
        public static int HeightOfBounds;

        public LevelSelection()
        {
            // Defines how big the level selection box will be based on the game's resolution.
            int widthOfBounds = (int)(600 / Main.WidthRatio);
            int heightOfBounds = (int)(300 / Main.HeightRatio);
            scrollingFileBounds = new Rectangle(Main.UserResWidth / 2, Main.UserResHeight / 2, widthOfBounds, heightOfBounds);
            scrollingFileBounds.X -= widthOfBounds / 2;
            scrollingFileBounds.Y -= heightOfBounds / 2;

            WidthOfBounds = widthOfBounds;
            HeightOfBounds = heightOfBounds;
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
            int startingY = scrollingFileBounds.Y + 2;
            for (int i = 0; i < levelCount; i++)
            {
                levelInfos[i].SetPosition(startingY, i);
            }
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.DefaultTexture, new Vector2(Main.UserResWidth, Main.UserResHeight), Color.White);

            // Sets the scrolling levels to disappear if they are not inside of this bounding box.
            Rectangle originalScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = scrollingFileBounds;

            foreach (LevelInfo level in levelInfos)
            {
                level.Draw(spriteBatch);
            }

            // Returns the scissor rectangle to original.
            spriteBatch.GraphicsDevice.ScissorRectangle = originalScissorRectangle;
        }
    }

    /// <summary>
    /// Used to contain all the information on a level kept inside the Level directory.
    /// </summary>
    public class LevelInfo
    {
        Rectangle drawRectangle;
        SpriteFont font;

        private static int Spacing;

        public LevelInfo(string name, string filePath, DateTime lastModDate)
        {
            // Store information of the level.
            Name = name;
            FilePath = filePath;
            LastModifiedDate = lastModDate;

            // Define how big each info section will be.
            Spacing = font.LineSpacing + 4;
            drawRectangle = new Rectangle(Main.UserResWidth - LevelSelection.WidthOfBounds / 2, 0, LevelSelection.WidthOfBounds, 50);
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
            drawRectangle.Y = initialY + drawRectangle.Height * orderInList;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentHelper.LoadTexture("tiles/white"), drawRectangle, Color.Black);
            spriteBatch.DrawString(font, Name, new Vector2(drawRectangle.X + Spacing, drawRectangle.Y + 2), Color.White);
            spriteBatch.DrawString(font, GetDate(LastModifiedDate), new Vector2(drawRectangle.X + Spacing, drawRectangle.Y + 2 + Spacing), Color.White);
        }

        /// <summary>
        /// Converts DateTime into a string.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string GetDate(DateTime dt)
        {
            string date = "Last modified: " + dt.Year + "/" + dt.Month + "/" + dt.Day + " at " + dt.Hour + ":" + dt.Second;
            return date;
        }

    }

}
