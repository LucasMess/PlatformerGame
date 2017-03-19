using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.AdamGame;

namespace Adam.Levels
{
    public class Elektran
    {
        private Rectangle _drawRectangle = new Rectangle(0, 0, 32, 32);
        private Rectangle _sourceRectangle;

        public bool IsOn { get; set; }

        public TileType Id { get; set; }

        public byte SubId { get; set; }

        public int TileIndex { get; set; }

        public Elektran(int x, int y)
        {
            _drawRectangle.X = x;
            _drawRectangle.Y = y;
            Id = AdamGame.TileType.ElektranWireOFF;
        }

        public void DefineTexture()
        {
            Point startingPoint;
            switch (Id)
            {
                case TileType.ElektranWireOFF:
                    startingPoint = new Point(384, 114);
                    FindSubIdFromStartingPoint(startingPoint);
                    GetSourceRectangleOfConnectedTexture(startingPoint);
                    break;
                case TileType.ElektranWireON:
                    startingPoint = new Point(448, 114);
                    FindSubIdFromStartingPoint(startingPoint);
                    GetSourceRectangleOfConnectedTexture(startingPoint);
                    break;
                default:
                    break;
            }
        }

        private Rectangle GetSourceRectangleOfConnectedTexture(Point startingPoint)
        {
            // Sample tiles such as the ones in the tileholders have the same sub id, but the brush tiles do not.
            var position = new Point();
            switch (SubId)
            {
                case 0: //Dirt
                    position = startingPoint + new Point(0, 0);
                    break;
                case 1: //Inner bot right corner
                    position = startingPoint + new Point(1, 0);
                    break;
                case 2: //Inner bot left corner
                    position = startingPoint + new Point(2, 0);
                    break;
                case 3: //Inner top left corner
                    position = startingPoint + new Point(3, 0);
                    break;
                case 4: //Top left corner
                    position = startingPoint + new Point(0, 1);
                    break;
                case 5: //Top
                    position = startingPoint + new Point(1, 1);
                    break;
                case 6: //Top right corner
                    position = startingPoint + new Point(2, 1);
                    break;
                case 7: //Inner top right corner
                    position = startingPoint + new Point(3, 1);
                    break;
                case 8: //Left
                    position = startingPoint + new Point(0, 2);
                    break;
                case 9: //Middle
                    position = startingPoint + new Point(1, 2);
                    break;
                case 10: //Right
                    position = startingPoint + new Point(2, 2);
                    break;
                case 11: //Top vertical
                    position = startingPoint + new Point(3, 2);
                    break;
                case 12: //Bot left corner
                    position = startingPoint + new Point(0, 3);
                    break;
                case 13: //Bot
                    position = startingPoint + new Point(1, 3);
                    break;
                case 14: //Bot right corner
                    position = startingPoint + new Point(2, 3);
                    break;
                case 15: //Middle vertical
                    position = startingPoint + new Point(3, 3);
                    break;
                case 16: //Left horizontal
                    position = startingPoint + new Point(0, 4);
                    break;
                case 17: //Middle horizontal
                    position = startingPoint + new Point(1, 4);
                    break;
                case 18: //Right horizontal
                    position = startingPoint + new Point(2, 4);
                    break;
                case 19: //Bot vertical
                    position = startingPoint + new Point(3, 4);
                    break;
            }
            return new Rectangle(position.X * 16, position.Y * 16, 16, 16);
        }

        private void FindSubIdFromStartingPoint(Point startingPoint)
        {
            int mapWidth = GameWorld.WorldData.LevelWidth;

            var m = TileIndex;
            var t = m - mapWidth;
            var b = m + mapWidth;
            var tl = t - 1;
            var tr = t + 1;
            var ml = m - 1;
            var mr = m + 1;
            var bl = b - 1;
            var br = b + 1;

            var topLeft = GameWorld.GetElektran(tl);
            var top = GameWorld.GetElektran(t);
            var topRight = GameWorld.GetElektran(tr);
            var midLeft = GameWorld.GetElektran(ml);
            var mid = GameWorld.GetElektran(m);
            var midRight = GameWorld.GetElektran(mr);
            var botLeft = GameWorld.GetElektran(bl);
            var bot = GameWorld.GetElektran(b);
            var botRight = GameWorld.GetElektran(br);

            if (topLeft != null &&
                top != null &&
                topRight != null &&
                midLeft != null &&
                midRight != null &&
                botLeft != null &&
                bot != null &&
                botRight != null)
                SubId = 0;

            if (topLeft != null &&
                top != null &&
                topRight != null &&
                midLeft != null &&
                midRight != null &&
                botLeft != null &&
                bot != null &&
                botRight == null)
                SubId = 0;

            if (topLeft != null &&
                top != null &&
                topRight != null &&
                midLeft != null &&
                midRight != null &&
                botLeft == null &&
                bot != null &&
                botRight != null)
                SubId = 0;

            if (topLeft == null &&
                top != null &&
                topRight != null &&
                midLeft != null &&
                midRight != null &&
                botLeft != null &&
                bot != null &&
                botRight != null)
                SubId = 0;

            if (top == null &&
                midLeft == null &&
                midRight != null &&
                bot != null)
                SubId = 4;

            if (top == null &&
                midLeft != null &&
                midRight != null &&
                bot != null)
                SubId = 5;

            if (top == null &&
                midLeft != null &&
                midRight == null &&
                bot != null)
                SubId = 6;

            if (topLeft != null &&
                top != null &&
                topRight == null &&
                midLeft != null &&
                midRight != null &&
                botLeft != null &&
                bot != null &&
                botRight != null)
                SubId = 0;

            if (top != null &&
                midLeft == null &&
                midRight != null &&
                bot != null)
                SubId = 8;

            if (top == null &&
                midLeft == null &&
                midRight == null &&
                bot == null)
                SubId = 9;

            if (top != null &&
                midLeft != null &&
                midRight == null &&
                bot != null)
                SubId = 10;

            if (top == null &&
                midLeft == null &&
                midRight == null &&
                bot != null)
                SubId = 11;

            if (top != null &&
                midLeft == null &&
                midRight != null &&
                bot == null)
                SubId = 12;

            if (top != null &&
                midLeft != null &&
                midRight != null &&
                bot == null)
                SubId = 13;

            if (top != null &&
                midLeft != null &&
                midRight == null &&
                bot == null)
                SubId = 14;

            if (top != null &&
                midLeft == null &&
                midRight == null &&
                bot != null)
                SubId = 15;

            if (top == null &&
                midLeft == null &&
                midRight != null &&
                bot == null)
                SubId = 16;

            if (top == null &&
                midLeft != null &&
                midRight != null &&
               bot == null)
                SubId = 17;

            if (top == null &&
                midLeft != null &&
                midRight == null &&
                bot == null)
                SubId = 18;

            if (top != null &&
                midLeft == null &&
                midRight == null &&
                bot == null)
                SubId = 19;
        }

        public void Activate()
        {
            ActivateRecursive(0);
        }

        private void ActivateRecursive(int distance)
        {
            if (IsOn) return;
            IsOn = true;

            Elektran above = GameWorld.GetElektran(TileIndex - GameWorld.WorldData.LevelWidth);
            above?.ActivateRecursive(distance++);
            Elektran below = GameWorld.GetElektran(TileIndex + GameWorld.WorldData.LevelWidth);
            below?.ActivateRecursive(distance++);
            Elektran left = GameWorld.GetElektran(TileIndex - 1);
            left?.ActivateRecursive(distance++);
            Elektran right = GameWorld.GetElektran(TileIndex + 1);
            above?.ActivateRecursive(distance++);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.SpriteSheet, _drawRectangle, _sourceRectangle, Color.White);
        }
    }
}
