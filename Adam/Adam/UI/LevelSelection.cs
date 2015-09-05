using Adam.Misc.Helpers;
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
        private List<LevelInfo> levelInfos;
        private int levelCount;

        public static int WidthOfBounds;
        public static int HeightOfBounds;

        private int lastScrollWheel;

        public LevelSelection()
        {
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
            int startingY = scissorRectangle.Y + 2;
            for (int i = 0; i < levelCount; i++)
            {
                levelInfos[i].SetPosition(startingY, i);
            }
        }

        public void Update()
        {
            int scrollWheel = Mouse.GetState().ScrollWheelValue;
            Rectangle mouseRectangle = InputHelper.MouseRectangle;

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

            foreach (LevelInfo l in levelInfos)
            {
                l.Update();
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
            drawRectangle.Y = initialY + drawRectangle.Height * orderInList + 5 * orderInList;
        }

        public void Update()
        {
            drawRectangle.Y += (int)VelocityY;
            VelocityY *= .92f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boxTexture, drawRectangle, sourceRectangle, Color.White);

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

    }

}
