using Adam.Characters.Non_Playable;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Noobs
{
    class God : NonPlayableCharacter
    {
        enum AnimationState
        {
            Still,
            Walking,
            Sleeping,
            Talking,
        }
        AnimationState CurrentAnimationState = AnimationState.Still;
        AnimationData walking;
        AnimationData still;
        GameTime gameTime;
        int spawnPoint;
        int conversationProgress;
        int conversationSave;
        bool endOfStory;

        public God(int x, int y)
        {
            canTalk = true;
            texture = ContentHelper.LoadTexture("Characters/NPCs/god");
            collRectangle = new Rectangle(x, y, 48, 80);
            drawRectangle = collRectangle;
            sourceRectangle = new Rectangle(0, 0, 24, 40);

            spawnPoint = collRectangle.X;
            drawRectangle.X = collRectangle.X - 8;
            drawRectangle.Y = collRectangle.Y - 16;

            still = new AnimationData(300, 4, 0);
            walking = new AnimationData(150, 4, 1);

            animation = new Animation(texture, drawRectangle, sourceRectangle);

            Game1.Dialog.NextDialog += Dialog_NextDialog;
            Game1.Dialog.CancelDialog += Dialog_CancelDialog;
        }

        void Dialog_CancelDialog()
        {
            conversationProgress = conversationSave;
            isTalking = false;
            endOfStory = false;
        }

        void Dialog_NextDialog()
        {
            if (endOfStory)
            {
                Game1.Dialog.Cancel();
                Dialog_CancelDialog();
            }
            else ShowMessage();
        }

        public override void Update(GameTime gameTime, Player player)
        {
            drawRectangle.X = collRectangle.X - 8;
            drawRectangle.Y = collRectangle.Y - 16;

            this.gameTime = gameTime;
            base.Update(gameTime, player);

            WalkAroundSpawnPoint(spawnPoint);
            Animate();

            if (velocity.X != 0)
            {
                CurrentAnimationState = AnimationState.Walking;
            }
            else CurrentAnimationState = AnimationState.Still;

            if (velocity.X > 0)
                animation.isFlipped = false;
            if (velocity.X < 0)
                animation.isFlipped = true;

        }

        private void Animate()
        {
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    animation.Update(gameTime, drawRectangle, still);
                    break;
                case AnimationState.Walking:
                    animation.Update(gameTime, drawRectangle, walking);
                    break;
                case AnimationState.Sleeping:
                    break;
                case AnimationState.Talking:
                    break;
                default:
                    break;
            }
        }

        protected override void ShowMessage()
        {
            SoundFx fx;
            switch (conversationProgress)
            {
                case 0:
                    fx = new SoundFx("Voices/God/conversation1");
                    fx.PlayOnce();
                    Game1.Dialog.Say("Adam! You have been asleep for nearly 7 hours. You must rescue Eve from the Devil before it is too late.");
                    conversationProgress++;
                    break;
                case 1:
                    fx = new SoundFx("Voices/God/conversation2");
                    fx.PlayOnce();
                    Game1.Dialog.Say("At the top of the Tree of Knowledge there is a map of the world. You must find it if you wish to rescue Eve.");
                    conversationProgress++;
                    break;
                case 2:
                    fx = new SoundFx("Voices/God/happy");
                    fx.PlayOnce();
                     Game1.Dialog.Say("You did it! Well done. Now give it to me.");
                    endOfStory = true;
                    break;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            animation.Draw(spriteBatch);
        }
    }
}
