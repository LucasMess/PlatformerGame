using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    class Brush
    {
        const int MaxSize = 6;
        const int MinSize = 1;

        public int size = 1;
        int index;
        Image[] selectionSquares = new Image[1];
        public int[] selectedIndexes;

        bool increased, decreased;

        public void Update()
        {
            CreateBrush();
            CheckIfSizeChanged();
        }

        private void CheckIfSizeChanged()
        {
            if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemPlus) && !increased)
            {
                increased = true;               
                size++;
                selectionSquares = new Image[size * size];
            }
            if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemMinus) && !decreased)
            {
                decreased = true;               
                size--;
                selectionSquares = new Image[size * size];
            }

            if (InputHelper.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.OemPlus))
            {
                increased = false;
            }
            if (InputHelper.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.OemMinus))
            {
                decreased = false;
            }

            if (size > MaxSize) size = MaxSize;
            if (size < MinSize) size = MinSize;


        }

        private void CreateBrush()
        {
            GameWorld gameWorld = GameWorld.Instance;
            selectedIndexes = GetTilesCoveredByBrush();
            
            for (int i = 0; i < selectionSquares.Length; i++)
            {
                if (selectedIndexes[i] >= 0 && selectedIndexes[i] < gameWorld.tileArray.Length)
                {
                    selectionSquares[i] = new Image();
                    Tile hovered = gameWorld.tileArray[selectedIndexes[i]];
                    selectionSquares[i].Rectangle = hovered.drawRectangle;
                    selectionSquares[i].SourceRectangle = new Microsoft.Xna.Framework.Rectangle(21 * 16, 7 * 16, 16, 16);
                    selectionSquares[i].Texture = GameWorld.SpriteSheet;
                }
            }

        }

        private int[] GetTilesCoveredByBrush()
        {
            index = GameWorld.Instance.levelEditor.IndexOfMouse;

            GameWorld gameWorld = GameWorld.Instance;

            //Get indexes around brush
            List<int> indexes = new List<int>();
            for (int h = 0; h < size; h++)
            {
                for (int w = 0; w < size; w++)
                {
                    int startingIndex = index - (int)(Math.Truncate((double)(size / 2))) - (int)(Math.Truncate((double)(size / 2)) * gameWorld.worldData.width);
                    int i = startingIndex + (h * gameWorld.worldData.height) + w;
                    indexes.Add(i);
                }
            }

            return indexes.ToArray();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (selectionSquares == null) return;
            foreach(Image i in selectionSquares)
            {
                if (i.Texture != null)
                spriteBatch.Draw(i.Texture, i.Rectangle, i.SourceRectangle, Color.White);
            }
        }
    }

}
