using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class NonPlayableCharacter : Entity
    {
        public enum NpcType
        {
            God,
        }

        public Rectangle radiusRectangle;
        public Rectangle topMidBound, botMidBound;
        int destination;
        Random randGen;
        Dialog dialog;
        public Vector2 velocity;
        GameTime gameTime;
        Vector2 monitorRes;
        SoundEffect still1, still2, still3, inter1, inter2, inter3;

        public int ID;
        int currentText;
        double startWalkTimer, cancelTimer;
        public double jumpStartTimer;
        int sleepTime, cancelTime;
        int switchFrame;
        int currentFrame;
        int speed;
        int maxX, minX, deltaX, startingX;
        Vector2 frameCount;
        double frameTimer;
        bool isKeyPressed, isMousePressed;
        bool isFacingRight = true;
        public bool needsToJump;
        public bool isFlying;
        bool isInteracting, goingToDestination;

        public const int Villager_SleepTime = 5000;
        public const int Villager_CancelWalkTime = 10000;
        public const int Villager_Speed = 4;

        enum AnimationState { still, walking, jumping, falling, jumpWalking, sleeping }
        AnimationState CurrentAnimation = AnimationState.still;

        public enum ChatComplexity { Simple, Story }
        ChatComplexity complexity;

        public NonPlayableCharacter()
        {

        }

        public virtual void TalkTo()
        {

        }

        public NonPlayableCharacter(int x, int y, int ID, ContentManager Content, int seed, Vector2 monitorRes)
        {
            this.ID = ID;
            this.complexity = ChatComplexity.Simple;
            this.Content = Content;
            this.monitorRes = monitorRes;
            randGen = new Random(seed);
            DefineStats();
            Load();

            deltaX = 50;
            minX = x - 50;
            maxX = x + 50;
            startingX = x;
            collRectangle = new Rectangle(x, y, 32, 64);
            sourceRectangle = new Rectangle(0, 0, 48, 80);
            drawRectangle = new Rectangle(0, 0, 48, 80);
            radiusRectangle = new Rectangle(0,0,0,0);
            frameCount = new Vector2(4, 6);
        }

        public void Load()
        {
            switch (ID)
            {
                case 1:
                    texture = Content.Load<Texture2D>("Characters/adam_prehistoric_s1");

                    still1 = Content.Load<SoundEffect>("Sounds/Villagers/villager_still_1");
                    still2 = Content.Load<SoundEffect>("Sounds/Villagers/villager_still_2");
                    still3 = Content.Load<SoundEffect>("Sounds/Villagers/villager_still_3");
                    inter1 = Content.Load<SoundEffect>("Sounds/Villagers/villager_interact_1");
                    inter2 = Content.Load<SoundEffect>("Sounds/Villagers/villager_interact_2");
                    inter3 = Content.Load<SoundEffect>("Sounds/Villagers/villager_interact_3");

                    break;
            }
        }

        public void Update(GameTime gameTime, Player player)
        {
            this.gameTime = gameTime;
            Pathfinding();
            Animate();

            //Update the drawRectangle
            drawRectangle.X = collRectangle.X - 8;
            drawRectangle.Y = collRectangle.Y - 16;

            //Update the main collision rectangle;
            collRectangle.X += (int)velocity.X;
            collRectangle.Y += (int)velocity.Y;

            //Update the rectangles that define the player's position
            topMidBound = new Rectangle(collRectangle.X + 16, collRectangle.Y + 8, 1, 1);
            botMidBound = new Rectangle(collRectangle.X + 16, collRectangle.Y + (3 * 64 / 4), 1, 1);

            radiusRectangle = new Rectangle(collRectangle.X - 50, collRectangle.Y - 50, 100, 100);

            //Update the rectangles that define the player collision
            xRect = new Rectangle(collRectangle.X, collRectangle.Y + 15, 32, 64 - 20);
            yRect = new Rectangle(collRectangle.X + 10, collRectangle.Y, 32 - 20, 64);

            //If player is in range and wants to talk
            if (InputHelper.IsKeyDown(Keys.W) && player.collRectangle.Intersects(radiusRectangle) && !isInteracting && !isKeyPressed)
            {
                dialog = new Dialog(Content, Dialog.Type.Notification);
                isInteracting = true;
                isKeyPressed = true;
                PlaySound("interact");
            }
            //make dialog appear
            if (isInteracting)
            {
                dialog.isVisible = true;
                player.hasControl = false;
                Interacting();
            }
            //See if needs to progress or quit
            if (InputHelper.IsLeftMousePressed() && isInteracting && !isMousePressed)
            {
                if (complexity == ChatComplexity.Simple)
                {
                    isInteracting = false;
                    dialog.isVisible = false;
                    player.hasControl = true;
                }
                if (complexity == ChatComplexity.Story)
                {
                    currentText++;
                    isMousePressed = true;
                    if (GetVillagerText() == null)
                    {
                        isInteracting = false;
                        dialog.isVisible = false;
                        player.hasControl = true;
                        currentText = 0;
                    }
                }
            }
            //If player presses any key stop dialog
            if (isInteracting)
            {
                if (InputHelper.IsAnyInputPressed() && !isKeyPressed)
                {
                    isInteracting = false;
                    dialog.isVisible = false;
                    player.hasControl = true;
                    isKeyPressed = true;
                }
            }
            //If the w key is released let player initiate dialog again, otherwise infinite loop
            if (InputHelper.IsKeyUp(Keys.W))
            {
                isKeyPressed = false;
            }
            //If mouse was released, let player press mouse again to progress
            if (InputHelper.IsLeftMouseReleased())
            {
                isMousePressed = false;
            }
            if (randGen.Next(0, 2000) == 1)
            {
                PlaySound("still");
            }
    

        }

        void Pathfinding()
        {
            if (isInteracting)
                return;
            if (!goingToDestination)
                startWalkTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (startWalkTimer > sleepTime && !goingToDestination)
            {
                if (randGen.Next(0, 1000) < 10)
                {
                    goingToDestination = true;
                    startWalkTimer = 0;
                    //destination = randGen.Next(minX - rectangle.X, maxX - rectangle.X) + rectangle.X;
                    destination = randGen.Next(-200, 200) + collRectangle.X;
                }
            }
            if (goingToDestination)
            {
                cancelTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (collRectangle.X < destination)
                {
                    if (!isFlying)
                        CurrentAnimation = AnimationState.walking;
                    velocity.X = Villager_Speed;
                    isFacingRight = true;
                }
                if (collRectangle.X > destination)
                {
                    if (!isFlying)
                        CurrentAnimation = AnimationState.walking;
                    velocity.X = -Villager_Speed;
                    isFacingRight = false;
                }
                if (collRectangle.X < destination + 10 && collRectangle.X > destination - 10)
                {
                    goingToDestination = false;
                }
            }
            if (cancelTimer > cancelTime && goingToDestination)
            {
                goingToDestination = false;
                cancelTimer = 0;
            }
            if (needsToJump && !isFlying)
            {
                velocity.Y = -5f;
                CurrentAnimation = AnimationState.jumping;
                if (velocity.X != 0)
                    CurrentAnimation = AnimationState.jumpWalking;
                needsToJump = false;
                isFlying = true;
            }
            if (Math.Abs(velocity.X) < 1)
            {
                CurrentAnimation = AnimationState.still;
            }
            if (velocity.Y > 2f)
                isFlying = true;

            velocity.X = velocity.X * 0.9f;

            //If player reaches speed limit, reduce his speed
            if (velocity.Y > 8f)
                velocity.Y = 8f;
            velocity.Y += .3f;

        }

        public void Interacting()
        {
            switch (ID)
            {
                case 1:
                    dialog.AddText(GetSimpleText());
                    break;
                case 2:
                    dialog.AddText(GetVillagerText());
                    break;
            }
        }

        string GetSimpleText()
        {
            switch (ID)
            {
                case 1:
                    return "The quick brown fox jumps over the lazy water.";
                case 2:
                    return "banana banana banana.";
            }
            return "Text not available.";
        }

        string GetVillagerText()
        {
            switch (currentText)
            {
                case 0:
                    return "There is water!";
                case 1:
                    return "Ouch!";
            }

            return null;
        }

        void DefineStats()
        {
            sleepTime = Villager_SleepTime;
            cancelTime = Villager_CancelWalkTime;
            speed = Villager_Speed;
        }

        public void Animate()
        {
            switch (CurrentAnimation)
            {
                #region Still Animation
                case AnimationState.still:
                    //define where in the spritesheet the still sequence is
                    sourceRectangle.Y = 0;
                    //defines the speed of the animation
                    switchFrame = 500;
                    //starts timer
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    //if the time is up, moves on to the next frame
                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += sourceRectangle.Width;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                #endregion
                #region Walking Animation
                case AnimationState.walking:
                    //define where in the spritesheet the still sequence is
                    sourceRectangle.Y = sourceRectangle.Height;
                    //defines the speed of the animation
                    switchFrame = 100;
                    //starts timer
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    //if the time is up, moves on to the next frame
                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += sourceRectangle.Width;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                #endregion
                #region Jump Animation
                case AnimationState.jumping:

                    if (velocity.X != 0)
                    {
                        CurrentAnimation = AnimationState.jumpWalking;
                        break;
                    }
                    //define where in the spritesheet the still sequence is
                    sourceRectangle.Y = sourceRectangle.Height * 2;
                    sourceRectangle.X = 0;
                    currentFrame = 0;
                    if (velocity.Y > -4)
                    {
                        sourceRectangle.X = sourceRectangle.Width;
                        currentFrame = 1;

                        if (velocity.Y > 7)
                        {
                            sourceRectangle.X = sourceRectangle.Width * 2;
                            currentFrame = 2;
                        }
                    }

                    break;
                #endregion
                #region Falling Animation
                case AnimationState.falling:
                    //define where in the spritesheet the still sequence is
                    sourceRectangle.Y = sourceRectangle.Height * 3;
                    //defines the speed of the animation
                    switchFrame = 100;
                    //starts timer
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    //if the time is up, moves on to the next frame
                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += sourceRectangle.Width;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }
                    break;
                #endregion
                #region Jump and Walking Animation
                case AnimationState.jumpWalking:
                    if (velocity.X == 0)
                    {
                        CurrentAnimation = AnimationState.jumping;
                        break;
                    }
                    //define where in the spritesheet the still sequence is
                    sourceRectangle.Y = sourceRectangle.Height * 4;
                    sourceRectangle.X = 0;
                    currentFrame = 0;
                    if (velocity.Y > -4)
                    {
                        sourceRectangle.X = sourceRectangle.Width;
                        currentFrame = 1;

                        if (velocity.Y > 7)
                        {
                            sourceRectangle.X = sourceRectangle.Width * 2;
                            currentFrame = 2;
                        }
                    }
                    break;
                #endregion
                #region Sleeping Animation
                case AnimationState.sleeping:
                    //define where in the spritesheet the still sequence is
                    sourceRectangle.Y = sourceRectangle.Height * 5;
                    //defines the speed of the animation
                    switchFrame = 600;
                    //starts timer
                    frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    //if the time is up, moves on to the next frame
                    if (frameTimer >= switchFrame)
                    {
                        if (frameCount.X != 0)
                        {
                            frameTimer = 0;
                            sourceRectangle.X += sourceRectangle.Width;
                            currentFrame++;
                        }
                    }

                    if (currentFrame >= frameCount.X)
                    {
                        currentFrame = 0;
                        sourceRectangle.X = 0;
                    }

                    break;
                #endregion
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isFacingRight)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White);
            else spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            if (dialog != null)
                dialog.Draw(spriteBatch);
        }

        void PlaySound(string soundType)
        {
            switch (soundType)
            {
                case "still":
                    {
                        switch (randGen.Next(1,4))
                        {
                            case 1:
                                still1.Play();
                                break;
                            case 2:
                                still2.Play();
                                break;
                            case 3:
                                still3.Play();
                                break;
                        }
                    }
                    break;
                case "interact":
                    {
                        switch (randGen.Next(1, 4))
                        {
                            case 1:
                                inter1.Play();
                                break;
                            case 2:
                                inter2.Play();
                                break;
                            case 3:
                                inter3.Play();
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
