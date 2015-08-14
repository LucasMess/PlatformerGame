using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class PopUp
    {
        Texture2D fill;
        Texture2D outline;

        Rectangle fillRect;
        Rectangle outlineRect;

        float fillRotation, outlineRotation;
        Vector2 fillOrigin, outlineOrigin;
        public bool isVisible;
        string[] texts;

        double clickTimer;

        Texture2D obj;
        Rectangle objRect, objSourceRect;
        Vector2 objOrigin;

        Rectangle dialogRect;

        ContentManager Content;

        public PopUp()
        {
            texts = new string[Player.MAX_LEVEL + 2];
            texts[1] = "A leaf! The best protection in the world!";
            texts[2] = "A stick! If only you had marshmallows.";
            texts[3] = "Leather shoes! Show those ants who is boss!";
            texts[4] = "Bear skin! Just don't smell it.";
            texts[5] = "A... piece of scrap metal?";
            texts[6] = "A bow! Arrows not included.";
            texts[7] = "A toga! Hey, it's better than nothing.";
            texts[8] = "Sandals! Stylish and aerodynamic.";
            texts[9] = "A razor! CAUTION: Use it above the waist.";
            texts[10] = "A diadem! Sorry, no refunds.";
            texts[11] = "An iron sword! Useful for spreading butter!";
            texts[12] = "An iron chestplate! Not recommended for females.";
            texts[13] = "Iron leggings! Protect the children.";
            texts[14] = "An iron helmet!";
            texts[15] = "";
            texts[16] = "A shotgun! Get these kids off my lawn!";
            texts[17] = "A fancy jacket! At least it has pockets.";
            texts[18] = "A wig! Hide that male pattern baldness!";
            texts[19] = "Fancy pants! ";
            texts[20] = "";
            texts[21] = "Sunglasses! Do not look directly at the Sun.";
            texts[22] = "A hoodie! ";
            texts[23] = "Jeans!";
            texts[24] = "Athletic shoes!";
            texts[25] = "";
            texts[26] = "A laser gun! Built-in mp3 player.";
            texts[27] = "An astronaut suit! It's cut in half.";
            texts[28] = "The rest of the astronaut suit! Goodbye freedom.";
            texts[29] = "A helmet. Fish not included.";
            texts[30] = "A jetpack! To infinity and beyond!";
            texts[31] = "Untapped power. Using it voids the warranty.";

            objSourceRect = new Rectangle(0, 0, 16, 16);
        }

        public void Load(ContentManager Content)
        {
            this.Content = Content;

            Vector2 monitorResolution = new Vector2(Main.UserResWidth, Main.UserResHeight);
            dialogRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y * 4 / 5, (int)monitorResolution.X * 2 / 3, (int)monitorResolution.Y * 1 / 6);

            fill = Content.Load<Texture2D>("Menu/Star Fill");
            outline = Content.Load<Texture2D>("Menu/Star Outline");

            fillRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y / 2 - 100, 300, 300);
            fillOrigin = new Vector2(fill.Width / 2, fill.Height / 2);

            outlineRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y / 2 - 100, 300, 300);
            outlineOrigin = new Vector2(outline.Width / 2, outline.Height / 2);

            obj = Content.Load<Texture2D>("Objects/tech_items");
            objRect = new Rectangle((int)monitorResolution.X / 2, (int)monitorResolution.Y / 2 - 100, 64, 64);
            objOrigin = new Vector2(8, 8);
        }

        public void Update(GameTime gameTime, Player player)
        {

            if (!isVisible)
                return;

            fillRotation += .01f;
            outlineRotation -= .01f;

            objSourceRect.X = (0) % 5 * 16;
            objSourceRect.Y = (0) / 5 * 16;

            clickTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (clickTimer > 1 && Mouse.GetState().LeftButton == ButtonState.Pressed && !player.manual_hasControl)
            {
                player.manual_hasControl = true;
                clickTimer = 0;
                isVisible = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                spriteBatch.Draw(fill, fillRect, null, Color.White, fillRotation, fillOrigin, SpriteEffects.None, 0);
                spriteBatch.Draw(outline, outlineRect, null, Color.White, outlineRotation, outlineOrigin, SpriteEffects.None, 0);
                spriteBatch.Draw(obj, objRect, objSourceRect, Color.White, 0, objOrigin, SpriteEffects.None, 0);
            }
        }


    }
}
