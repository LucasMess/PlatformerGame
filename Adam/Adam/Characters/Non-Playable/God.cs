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
    public class God : NonPlayableCharacter, ITalkable, IAnimated
    {
        GameTime gameTime;
        int spawnPoint;

        public God(int x, int y)
        {
            canTalk = true;
            Texture = ContentHelper.LoadTexture("Characters/NPCs/god");
            collRectangle = new Rectangle(x, y, 48, 80);
            sourceRectangle = new Rectangle(0, 0, 24, 40);

            spawnPoint = collRectangle.X;

            animation = new Animation(Texture, DrawRectangle, sourceRectangle);
        }

        public override void Update(GameTime gameTime, Player player)
        {
            this.gameTime = gameTime;
            base.Update(gameTime, player);

            WalkAroundSpawnPoint(spawnPoint);

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

        protected override void ShowMessage()
        {
            SoundFx fx;
            Main.Dialog.Say("What are you up to Adam?", this);
            EndConversation = true;
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

        void IAnimated.Animate()
        {
            GameTime gameTime = GameWorld.Instance.GetGameTime();
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    animation.Update(gameTime, DrawRectangle, animationData[0]);
                    break;
                case AnimationState.Walking:
                    animation.Update(gameTime, DrawRectangle, animationData[1]);
                    break;
                case AnimationState.Sleeping:
                    break;
                case AnimationState.Talking:
                    break;
                default:
                    break;
            }
        }

        public bool EndConversation { get; set; }

        Animation animation;
        public Animation Animation
        {
            get
            {
                if (animation == null)
                    animation = new Animation(Texture, DrawRectangle, sourceRectangle);
                return animation;
            }
        }

        AnimationData[] animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (animationData == null)
                    animationData = new Adam.AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                        new Adam.AnimationData(250,4,1,AnimationType.Loop),
                    };
                return animationData;
            }
        }

        public Adam.Misc.Interfaces.AnimationState CurrentAnimationState
        {
            get; set;
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(collRectangle.X - 8, collRectangle.Y - 16, 48, 80);
            }
        }
    }
}
