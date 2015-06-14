using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodenameAdam
{
    class LoadingScreen
    {
        Texture2D background, customObject, circle;
        Rectangle rectangle, customRect, circleRect;
        GameTime gameTime;
        Vector2 monitorRes;
        SpriteFont font;
        Random randGen = new Random();

        bool hasAnimated, hasGoneUp;
        bool textChosen;
        public bool isReady;

        string loadingText, randomText, normalLoadingText;

        double bufferTimer, textTimer, loadingTextTimer;
        float velocity;
        float rotation;
        int count = 0, maxCount;

        public LoadingScreen(Vector2 monitorRes, ContentManager Content)
        {
            background = Content.Load<Texture2D>("Backgrounds/Loading Screen/loading_background");
            customObject = Content.Load<Texture2D>("Backgrounds/Loading Screen/loading_tree");
            circle = Content.Load<Texture2D>("Backgrounds/Loading Screen/loading_circle");
            font = Content.Load<SpriteFont>("Fonts/loading_screen");

            randomText = "";
            normalLoadingText = "";
            loadingText = "";

            this.monitorRes = monitorRes;

            rectangle = new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y);
            customRect = new Rectangle(0, (int)monitorRes.Y, (int)monitorRes.X, (int)monitorRes.Y);
            circleRect = new Rectangle((int)monitorRes.X / 2, (int)monitorRes.Y / 2 - 100, circle.Width, circle.Height);
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            UpdateTimers();
            LoadingDots();

            rotation += .2f;

            if (!textChosen)
                ChooseText();

            if (!hasAnimated)
            {
                if (!hasGoneUp)
                {
                    customRect.Y += (int)velocity;
                    velocity = -10f;
                    if (customRect.Y < 10)
                    {
                        hasGoneUp = true;
                        velocity = 0;
                    }
                }
                else
                {
                    customRect.Y += (int)velocity;
                    velocity = 1f;
                    if (customRect.Y >= 30)
                    {
                        hasAnimated = true;
                        customRect.Y = 30;
                    }
                }
            }

            if (velocity < -10)
                velocity = -10;
            if (velocity > 5)
                velocity = 5;

        }

        public void UpdateTimers()
        {
            bufferTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (bufferTimer > 0)
                isReady = true;

            textTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (textTimer > 3)
            {
                textTimer = 0;
                textChosen = false;
            }

            loadingTextTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (loadingTextTimer > 250)
            {
                count++;
                loadingTextTimer = 0;
                if (count > maxCount)
                    count = 0;
            }

        }

        public void LoadingDots()
        {
            switch (count)
            {
                case 0:
                    loadingText = "Loading";
                    break;
                case 1:
                    loadingText = "Loading.";
                    break;
                case 2:
                    loadingText = "Loading..";
                    break;
                case 3:
                    loadingText = "Loading...";
                    break;
            }
            maxCount = 3;
            normalLoadingText = "Loading...";
        }

        public void Restart()
        {
            isReady = false;
            hasAnimated = false;
            hasGoneUp = false;
            rectangle = new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y);
            customRect = new Rectangle(0, (int)monitorRes.Y, (int)monitorRes.X, (int)monitorRes.Y);
            bufferTimer = 0;
            textTimer = 10000;
        }

        public void ChooseText()
        {
            switch (randGen.Next(0, 36))
            {
                case 0:
                    randomText = "Hello there.";
                    break;
                case 1:
                    randomText = "Walnuts are yummy.";
                    break;
                case 2:
                    randomText = "Stephen smells like roses.";
                    break;
                case 3:
                    randomText = "Salmon is a good source of protein.";
                    break;
                case 4:
                    randomText = "Indeeeeeeeeeed.";
                    break;
                case 5:
                    randomText = "Loading the loading screen.";
                    break;
                case 6:
                    randomText = "Reticulating spines.";
                    break;
                case 7:
                    randomText = "Error 404: Level not found.";
                    break;
                case 8:
                    randomText = "You are beautiful.";
                    break;
                case 9:
                    randomText = "Trying to find lost keys.";
                    break;
                case 10:
                    randomText = "Deleting system memory.";
                    break;
                case 11:
                    randomText = "Windows update incoming.";
                    break;
                case 12:
                    randomText = "You have lost connection to the internet.";
                    break;
                case 13:
                    randomText = "Lighting the darkness.";
                    break;
                case 14:
                    randomText = "Moving immovable objects.";
                    break;
                case 15:
                    randomText = "Stopping unstoppable force.";
                    break;
                case 16:
                    randomText = "Nerfing Irelia.";
                    break;
                case 17:
                    randomText = "Meow.";
                    break;
                case 18:
                    randomText = "Upgrading antiviruses.";
                    break;
                case 19:
                    randomText = "Opening Internet Explorer.";
                    break;
                case 20:
                    randomText = "Putting out the firewall.";
                    break;
                case 21:
                    randomText = "Giving Satan a massage.";
                    break;
                case 22:
                    randomText = "Doing Satan's pedicure.";
                    break;
                case 23:
                    randomText = "Far Lands or Bust!";
                    break;
                case 24:
                    randomText = "Shaving bears.";
                    break;
                case 25:
                    randomText = "Drinking tea.";
                    break;
                case 26:
                    randomText = "Starting pillow fight.";
                    break;
                case 27:
                    randomText = "Reloading the unloadable.";
                    break;
                case 28:
                    randomText = "Checking out pictures folder.";
                    break;
                case 29:
                    randomText = "Taking a break.";
                    break;
                case 30:
                    randomText = "Loading assets.";
                    break;
                case 31:
                    randomText = "Googling for solution.";
                    break;
                case 32:
                    randomText = "Oh oh, we might blue screen!";
                    break;
                case 33:
                    randomText = "Deleting user files.";
                    break;
                case 34:
                    randomText = "Drinking water.";
                    break;
                case 35:
                    randomText = "Pretending to load.";
                    break;
            }
            textChosen = true;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, rectangle, Color.White);
            spriteBatch.Draw(customObject, customRect, Color.White);

            Vector2 randomTextOrigin = new Vector2(font.MeasureString(randomText).X / 2, font.LineSpacing / 2);
            Vector2 loadingTextOrigin = new Vector2(font.MeasureString(normalLoadingText).X / 2, font.LineSpacing / 2);
            Vector2 circleOrigin = new Vector2(circle.Width / 2, circle.Height / 2);

            spriteBatch.DrawString(font, randomText, monitorRes / 2, new Color(51, 63, 80), 0, randomTextOrigin, .2f, SpriteEffects.None, 0);
            spriteBatch.Draw(circle, new Vector2(circleRect.X, circleRect.Y), null, Color.White, rotation, circleOrigin, .08f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, loadingText, new Vector2(monitorRes.X / 2, monitorRes.Y / 2 - 200), new Color(51, 63, 80), 0, loadingTextOrigin, .5f, SpriteEffects.None, 0);
        }

    }
}
