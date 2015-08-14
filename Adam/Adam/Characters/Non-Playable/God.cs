using Adam.Characters.Non_Playable;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.UI.Information;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Noobs
{
    class God : NonPlayableCharacter, ITalkable
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

            still = new AnimationData(300, 4, 0, AnimationType.Loop);
            walking = new AnimationData(150, 4, 1, AnimationType.Loop);

            animation = new Animation(texture, drawRectangle, sourceRectangle);
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
            switch (GameWorld.Instance.CurrentLevel)
            {
                case Level.Level1and1:
                    switch (CurrentConversation)
                    {
                        case 0:
                            fx = new SoundFx("Voices/God/conversation1");
                            fx.PlayOnce();
                            Game1.Dialog.Say("Adam! You have been asleep for nearly 7 hours. Tell me, what were you thinking?", this);
                            break;
                        case 1:
                            fx = new SoundFx("Voices/God/conversation2");
                            fx.PlayOnce();
                            Game1.Dialog.Say("Pff... I should have known. Why did I make you sleep every day is the biggest question I suppose. Now all that I get is a sleepwalking, lazy, moronic, insensitive, piece of...", this);
                            break;
                        case 2:
                            fx = new SoundFx("Voices/God/conversation1");
                            fx.PlayOnce();
                            Game1.Dialog.Say("Erm... I digress, where were we? Oh Yes, saving Eve.", this);
                            break;
                        case 3:
                            fx = new SoundFx("Voices/God/conversation1");
                            fx.PlayOnce();
                            Game1.Dialog.Say("No, Adam. You cannot just walk into Satan's Palace and get her.", this);
                            break;
                        case 4:
                            fx = new SoundFx("Voices/God/conversation2");
                            fx.PlayOnce();
                            Objective ob = new Objective();
                            ob.Create("Find the golden apple in God's Floating Island Zoo", 1);
                            Game1.ObjectiveTracker.AddObjective(ob);
                            Game1.Dialog.Say("You need a golden apple! I think I have one, but I left it in my Floating Island Zoo. You should go get it, It's not too far.", this);
                            EndConversation = true;
                            StartingConversation = 5;
                            break;
                        case 5:
                            fx = new SoundFx("Voices/God/conversation2");
                            fx.PlayOnce();
                            Game1.Dialog.Say("What are you waiting for? Go find the apple!", this);
                            EndConversation = true;                            
                            break;
                    }
                    break;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            animation.Draw(spriteBatch);
        }

        public int StartingConversation { get; set; }

        public int CurrentConversation { get; set; }


        public void OnNextDialog()
        {
            if (EndConversation)
            {
                //No more messages to show.
                EndConversation = false;
                isTalking = false;
                CurrentConversation = StartingConversation;
            }
            else
            {
                //The dialog is not over and there is more.
                ShowMessage();
            }
        }


        public bool EndConversation { get; set; }
    }
}
