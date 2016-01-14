using Adam.Characters.Non_Playable;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.UI.Information;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Noobs
{
    public class God : NonPlayableCharacter, ITalkable, IAnimated
    {
        GameTime _gameTime;
        int _spawnPoint;

        public God(int x, int y)
        {
            CanTalk = true;
            Texture = ContentHelper.LoadTexture("Characters/NPCs/god");
            CollRectangle = new Rectangle(x, y, 48, 80);
            SourceRectangle = new Rectangle(0, 0, 24, 40);

            _spawnPoint = CollRectangle.X;

            _animation = new Animation(Texture, DrawRectangle, SourceRectangle);
        }

        public override void Update(GameTime gameTime, Player.Player player)
        {
            this._gameTime = gameTime;
            base.Update(gameTime, player);

            WalkAroundSpawnPoint(_spawnPoint);

            if (Velocity.X != 0)
            {
                CurrentAnimationState = AnimationState.Walking;
            }
            else CurrentAnimationState = AnimationState.Still;

            if (Velocity.X > 0)
                _animation.IsFlipped = false;
            if (Velocity.X < 0)
                _animation.IsFlipped = true;

        }

        protected override void ShowMessage()
        {
            SoundFx fx;
            Main.Dialog.Say("What are you up to Adam? I hope you got your shit together now, because Eve needs you to save her before something really, really bad happens to her. I don't want you to waster your time doing nothing. GO!");
            EndConversation = true;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _animation.Draw(spriteBatch);
        }

        public int StartingConversation { get; set; }

        public int CurrentConversation { get; set; }


        public void OnNextDialog()
        {
            if (EndConversation)
            {
                //No more messages to show.
                EndConversation = false;
                IsTalking = false;
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
                    _animation.Update(gameTime, DrawRectangle, _animationData[0]);
                    break;
                case AnimationState.Walking:
                    _animation.Update(gameTime, DrawRectangle, _animationData[1]);
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

        Animation _animation;
        public Animation Animation
        {
            get
            {
                if (_animation == null)
                    _animation = new Animation(Texture, DrawRectangle, SourceRectangle);
                return _animation;
            }
        }

        AnimationData[] _animationData;
        public AnimationData[] AnimationData
        {
            get
            {
                if (_animationData == null)
                    _animationData = new Adam.AnimationData[]
                    {
                        new Adam.AnimationData(250,4,0,AnimationType.Loop),
                        new Adam.AnimationData(250,4,1,AnimationType.Loop),
                    };
                return _animationData;
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
                return new Rectangle(CollRectangle.X - 8, CollRectangle.Y - 16, 48, 80);
            }
        }
    }
}
