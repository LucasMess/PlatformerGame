using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

namespace Adam.UI
{
    /// <summary>
    /// Used to select, play, edit, create and delete levels in the main menu screen.
    /// </summary>
    public class LevelSelection
    {
        private Rectangle _scissorRectangle;
        private Rectangle _boundsDrawRectangle;
        private Rectangle _boundsSourceRectangle;

        private BitmapFont _headerFont;
        private Vector2 _headerPos;
        private string _headerText;

        private Rectangle _functionButtonContainer;

        private IconButton _playButton;
        private IconButton _editButton;
        private IconButton _renameButton;
        private IconButton _deleteButton;
        private IconButton _newButton;
        private IconButton _backButton;
        private List<IconButton> _buttons;

        private List<LevelInfo> _levelInfos;
        private LevelInfo _selectedLevel;
        private int _selectedLevelIndex = -1;
        private int _levelCount;

        public static int WidthOfBounds;
        public static int HeightOfBounds;

        private int _lastScrollWheel;
        private int _startingY;

        public LevelSelection()
        {
            // Defines the bounding box for the Level Info List.
            int widthOfBounds = 300 * 2;
            int heightOfBounds = 150 * 2;
            _boundsDrawRectangle = new Rectangle(AdamGame.DefaultUiWidth / 2, AdamGame.DefaultUiHeight / 2, widthOfBounds, heightOfBounds);
            _boundsDrawRectangle.X -= widthOfBounds / 2;
            _boundsDrawRectangle.Y -= heightOfBounds / 2;
            _boundsSourceRectangle = new Rectangle(376, 48, 300, 150);

            WidthOfBounds = widthOfBounds;
            HeightOfBounds = heightOfBounds;

            // Defines how big the level selection box will be based on the game's resolution.
            _scissorRectangle = new Rectangle(_boundsDrawRectangle.X + 6, _boundsDrawRectangle.Y + 6, _boundsDrawRectangle.Width - 12, _boundsDrawRectangle.Height - 12);

            // Defines where the function buttons will be.
            _functionButtonContainer = new Rectangle(_scissorRectangle.X + _scissorRectangle.Width - (4 + 128 + 16 + 32), _scissorRectangle.Y + _scissorRectangle.Height + 8, 184, 40);
            _playButton = new IconButton(new Vector2(4 + IconButton.Size * 0, 4), _functionButtonContainer, "Play level", ButtonImage.Play);
            _editButton = new IconButton(new Vector2(4 + IconButton.Size * 1 + 4, 4), _functionButtonContainer, "Edit level", ButtonImage.Edit);
            _renameButton = new IconButton(new Vector2(4 + IconButton.Size * 2 + 8, 4), _functionButtonContainer, "Rename level", ButtonImage.Rename);
            _deleteButton = new IconButton(new Vector2(4 + IconButton.Size * 3 + 12, 4), _functionButtonContainer, "Delete level", ButtonImage.Delete);
            _newButton = new IconButton(new Vector2(4 + IconButton.Size * 4 + 16, 4), _functionButtonContainer, "Create a new level", ButtonImage.New);
            _backButton = new IconButton(new Vector2(-500 + IconButton.Size * 3 + 4, 4), _functionButtonContainer, "Return", ButtonImage.Back);

            _buttons = new List<IconButton>();
            _buttons.Add(_playButton);
            _buttons.Add(_editButton);
            _buttons.Add(_renameButton);
            _buttons.Add(_deleteButton);
            _buttons.Add(_newButton);
            _buttons.Add(_backButton);

            _playButton.MouseClicked += PlayButton_MouseClicked;
            _editButton.MouseClicked += EditButton_MouseClicked;
            _renameButton.MouseClicked += RenameButton_MouseClicked;
            _deleteButton.MouseClicked += DeleteButton_MouseClicked;
            _newButton.MouseClicked += NewButton_MouseClicked;
            _backButton.MouseClicked += BackButton_MouseClicked;

            // Define the BitmapFont for the "Select Level" header and its position.
            _headerFont = ContentHelper.LoadFont("Fonts/x32");
            _headerText = "Select a Level:";
            _headerPos = new Vector2(_scissorRectangle.X, _scissorRectangle.Y - _headerFont.LineHeight - (10));
        }

        private void NewButton_MouseClicked(Button button)
        {
            AdamGame.TextInputBox.Show("Please enter the name for your new level:");
            AdamGame.TextInputBox.OnInputEntered += NewLevel_OnTextEntered;
        }

        private void NewLevel_OnTextEntered(TextInputArgs e)
        {
            string newPath;
            AdamGame.TextInputBox.OnInputEntered -= NewLevel_OnTextEntered;

            try
            {
                newPath = DataFolder.CreateNewLevel(e.Input, 256, 256);
            }
            catch (Exception ex)
            {
                AdamGame.MessageBox.Show(ex.Message);
                return;
            }

            DataFolder.EditLevel(newPath);
        }

        private void BackButton_MouseClicked(Button button)
        {
            Menu.CurrentMenuState = Menu.MenuState.Main;
        }

        private void DeleteButton_MouseClicked(Button button)
        {
            if (_selectedLevel == null)
            {
                AdamGame.MessageBox.Show("Please select a level.");
                return;
            }
            DataFolder.DeleteFile(_selectedLevel.FilePath);
            LoadLevels();
        }

        private void RenameButton_MouseClicked(Button button)
        {
            if (_selectedLevel == null)
            {
                AdamGame.MessageBox.Show("Please select a level.");
                return;
            }
            AdamGame.TextInputBox.Show("What would you like to rename this level to?");
            AdamGame.TextInputBox.SetTextTo(_selectedLevel.Name);
            AdamGame.TextInputBox.OnInputEntered += RenameButton_OnInputEntered;
        }

        private void RenameButton_OnInputEntered(TextInputArgs e)
        {
            DataFolder.RenameFile(_selectedLevel.FilePath, _selectedLevel.Name, e.Input);
            AdamGame.TextInputBox.OnInputEntered -= RenameButton_OnInputEntered;
            LoadLevels();
        }

        private void EditButton_MouseClicked(Button button)
        {
            if (_selectedLevel == null)
            {
                AdamGame.MessageBox.Show("Please select a level.");
                return;
            }
            DataFolder.EditLevel(_selectedLevel.FilePath);
        }

        private void PlayButton_MouseClicked(Button button)
        {
            try
            {
                DataFolder.PlayLevel(_selectedLevel.FilePath);
            }
            catch (Exception e)
            {
                AdamGame.MessageBox.Show(e.Message);
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
            _levelCount = filePaths.Count;
            _levelInfos = new List<LevelInfo>();
            for (int i = 0; i < _levelCount; i++)
            {
                LevelInfo lev = new LevelInfo(fileNames[i], filePaths[i], fileModifiedDates[i]);
                _levelInfos.Add(lev);
            }

            // Arranges the levels in an alphabetical list and sets their position.
            _startingY = _scissorRectangle.Y + (4 * 2);
            for (int i = 0; i < _levelCount; i++)
            {
                _levelInfos[i].SetPosition(_startingY, i);
            }
        }

        public void Update()
        {
            // Checks if there is scrolling.
            int scrollWheel = Mouse.GetState().ScrollWheelValue;
            Rectangle mouse = InputHelper.GetMouseInUi();


            if (_levelCount > 5)
            {
                if (mouse.Intersects(_scissorRectangle))
                {
                    if (_lastScrollWheel != scrollWheel)
                    {
                        foreach (LevelInfo l in _levelInfos)
                        {
                            l.VelocityY = (scrollWheel - _lastScrollWheel) / 5;
                        }
                    }
                }

                _lastScrollWheel = scrollWheel;

                float velocityY = 0;

                // If first element is below its starting point, make them all go back, but only if the user is not making them go back already.
                if (_levelInfos[0].GetY() > _startingY && _levelInfos[0].VelocityY > -1)
                    velocityY = -1f;

                // If last element is above bottom of box, make them go back, but only if the user is not making them go back already.
                if (_levelInfos[_levelCount - 1].GetY() < _scissorRectangle.Y + _scissorRectangle.Height - _levelInfos[0].GetHeight() - (3) && _levelInfos[0].VelocityY < 1)
                    velocityY = 1f;

                if (velocityY != 0)
                    foreach (LevelInfo l in _levelInfos)
                    {
                        l.VelocityY = velocityY;
                    }
            }

            // Makes the level infos scroll.
            foreach (LevelInfo l in _levelInfos)
            {
                l.Update();
            }

            // Check to see if a level info is being selected.
            if (InputHelper.IsLeftMousePressed() && mouse.Intersects(_scissorRectangle))
            {
                foreach (LevelInfo level in _levelInfos)
                {
                    if (level.IsBeingClickedOn())
                    {
                        _selectedLevelIndex = _levelInfos.IndexOf(level);
                        break;
                    }
                }
            }
            if (_selectedLevelIndex > -1 && _selectedLevelIndex < _levelCount)
                _selectedLevel = _levelInfos[_selectedLevelIndex];
            else _selectedLevel = null;

            // Update buttons.
            foreach (IconButton b in _buttons)
            {
                b.Update();
            }

            // Make the selected level know about it.
            foreach (LevelInfo level in _levelInfos)
            {
                if (level == _selectedLevel)
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
            spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), new Rectangle(0, 0, AdamGame.DefaultUiWidth, AdamGame.DefaultUiHeight), Color.Black * .7f);
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _boundsDrawRectangle, _boundsSourceRectangle, Color.White);

            // Sets the scrolling levels to disappear if they are not inside of this bounding box.
            Rectangle originalScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = _scissorRectangle;

            foreach (LevelInfo level in _levelInfos)
            {
                level.Draw(spriteBatch);
            }

            // Returns the scissor rectangle to original.
            spriteBatch.GraphicsDevice.ScissorRectangle = originalScissorRectangle;

            // Draw buttons.
            foreach (IconButton b in _buttons)
            {
                b.Draw(spriteBatch);
            }
            foreach (IconButton b in _buttons)
            {
                b.DrawTooltip(spriteBatch);
            }

            // Draw header text.
            FontHelper.DrawWithOutline(spriteBatch, _headerFont, _headerText, _headerPos, 2, Color.White, Color.Black);
        }
    }

    /// <summary>
    /// Used to contain all the information on a level kept inside the Level directory.
    /// </summary>
    public class LevelInfo
    {
        Rectangle _drawRectangle;
        Rectangle _sourceRectangle;
        BitmapFont _infoFont;
        BitmapFont _nameFont;

        private static int _spacing;

        public float VelocityY { get; set; }

        public LevelInfo(string name, string filePath, DateTime lastModDate)
        {
            // Store information of the level.
            Name = name;
            FilePath = filePath;
            LastModifiedDate = lastModDate;

            // Define how big each info section will be.
            _nameFont = ContentHelper.LoadFont("Fonts/x16");
            _infoFont = ContentHelper.LoadFont("Fonts/x8");

            _drawRectangle = new Rectangle(AdamGame.DefaultUiWidth / 2 - LevelSelection.WidthOfBounds / 2 + 8 * 2, 8, LevelSelection.WidthOfBounds - 16 * 2, 25 * 2);

            _spacing = _infoFont.LineHeight + 4;

            _sourceRectangle = new Rectangle(128, 0, 284, 25);
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
            _drawRectangle.Y = initialY + _drawRectangle.Height * orderInList + (3 * 2) * orderInList;
        }

        public void Update()
        {
            _drawRectangle.Y += (int)VelocityY;
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

            spriteBatch.Draw(GameWorld.UiSpriteSheet, _drawRectangle, _sourceRectangle, color);

            Vector2 namePos = new Vector2(_drawRectangle.X + _spacing, _drawRectangle.Y + _drawRectangle.Height / 2 - _nameFont.LineHeight / 2);
            FontHelper.DrawWithOutline(spriteBatch, _nameFont, Name, namePos, 2, Color.LightGray, Color.Black);
            spriteBatch.DrawString(_infoFont, GetDate(LastModifiedDate), new Vector2(_drawRectangle.X + _drawRectangle.Width - 20 - _infoFont.MeasureString(GetDate(LastModifiedDate)).X, _drawRectangle.Y + _drawRectangle.Height / 2 - _infoFont.LineHeight / 2), Color.Black);
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

            string date = dt.Year + "/" + dt.Month + "/" + dt.Day + " at " + dt.Hour + ":" + seconds;
            return date;
        }

        /// <summary>
        /// Checks if the Level Info box is being clicked on.
        /// </summary>
        /// <returns></returns>
        public bool IsBeingClickedOn()
        {
            Rectangle mouse = InputHelper.GetMouseInUi();
            return (mouse.Intersects(_drawRectangle));
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
            return _drawRectangle.Y;
        }

        /// <summary>
        /// Get the height of the level info box.
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return _drawRectangle.Height;
        }
    }

}
