﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace ThereMustBeAnotherWay.UI
{

    /// <summary>
    /// Used to display information about the player on screen and also to make transitions such as screen flashes and fade to black.
    /// </summary>
    public static class Overlay
    {
        /// <summary>
        /// Used to briefly do a camera flash on the screen.
        /// </summary>
        private static class WhiteFlash
        {
            private const float OpacityDelta = .01f;
            private static float _whiteOpacity;
            private static bool _isFlashingWhite;
            private static bool _hasReachedMaxWhite;
            private static Texture2D _white = ContentHelper.LoadTexture("Tiles/white");

            public static void Update()
            {
                if (_isFlashingWhite)
                {
                    if (!_hasReachedMaxWhite)
                    {
                        _whiteOpacity += OpacityDelta * 5;
                        if (_whiteOpacity > 1f)
                        {
                            _hasReachedMaxWhite = true;
                        }
                    }
                    else
                    {
                        _whiteOpacity -= OpacityDelta;
                        if (_whiteOpacity < 0)
                        {
                            _whiteOpacity = 0;
                            _hasReachedMaxWhite = false;
                            _isFlashingWhite = false;
                        }
                    }
                }
            }

            public static void Start()
            {
                _isFlashingWhite = true;
                _hasReachedMaxWhite = false;
                _whiteOpacity = 1f;
            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(_white, new Rectangle(0, 0, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight), Color.White * _whiteOpacity);
            }
        }
        /// <summary>
        /// Used to fade in and out to black.
        /// </summary>
        private static class BlackFade
        {
            private static bool _isFadingIn = true;
            private static float _opacity = 0f;
            private const float MaxOpacity = 1f;
            private const float MinOpacity = 0f;

            private static float GetOpacityDelta(int speed)
            {
                switch (speed)
                {
                    case 1:
                        return .05f;

                    case 2:
                        return .06f;

                    case 3:
                        return .07f;

                    case 4:
                        return .08f;

                    case 5:
                        return .09f;
                }

                return .05f;
            }
            private static int _speed;

            public static void Update()
            {
                if (_isFadingIn)
                {
                    _opacity -= GetOpacityDelta(_speed);

                    if (_opacity < MinOpacity)
                    {
                        _opacity = MinOpacity;
                    }
                }
                else
                {
                    _opacity += GetOpacityDelta(_speed);

                    if (_opacity > MaxOpacity)
                    {
                        _opacity = MaxOpacity;
                    }
                }
            }

            /// <summary>
            /// Fades in using the given speed.
            /// </summary>
            /// <param name="speed">1 = slowest, 5 = fastest.</param>
            public static void FadeIn(int speed = 1)
            {
                _opacity = 1;
                _speed = speed;
                _isFadingIn = true;
            }

            /// <summary>
            /// Fades in using the given speed.
            /// </summary>
            /// <param name="speed">1 = slowest, 5 = fastest.</param>
            public static void FadeOut(int speed = 1)
            {
                _speed = speed;
                _isFadingIn = false;
            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/black"), new Rectangle(0, 0, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight), Color.Black * _opacity);
            }
        }

        public static class BlackBars
        {
            static bool _isActive;
            static Texture2D _texture = ContentHelper.LoadTexture("Tiles/black");
            static int _height;
            public static void Show()
            {
                _height = (int)(TMBAW_Game.UserResHeight * .2f);
                _isActive = true;
            }

            public static void Hide()
            {
                _isActive = false;
            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                if (_isActive)
                {
                    spriteBatch.Draw(_texture, new Rectangle(0, 0, TMBAW_Game.UserResWidth, (int)(_height)), Color.Black);
                    spriteBatch.Draw(_texture, new Rectangle(0, (TMBAW_Game.UserResHeight - (int)(_height)) , TMBAW_Game.UserResWidth, (int)(_height * TMBAW_Game.HeightRatio)), Color.Black);
                }
            }
        }

        /// <summary>
        /// Does all the effects for when the player rewinds.
        /// </summary>
        private static class RewindEffect
        {
            static Texture2D _cornersTexture = ContentHelper.LoadTexture("Overlay/rewind_corners");
            static Texture2D _rippleTexture = ContentHelper.LoadTexture("Overlay/rewind_ripple");

            static float rotation = 0;
            static Vector2 center = new Vector2(_rippleTexture.Width / 2, _rippleTexture.Height / 2);

            static bool _active;
            static float _opacity;

            public static void Update()
            {
                rotation -= .5f;
            }

            public static void Activate()
            {
                _active = true;
                _opacity = 1;
            }

            public static void Deactivate()
            {
                _active = false;
                _opacity = 0;
            }

            public static void SetOpacity(float opacity)
            {
                _opacity = opacity;
            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                if (_active)
                    spriteBatch.Draw(_cornersTexture, new Vector2(0, 0), Color.White * _opacity);
            }

            public static void DrawRipples(SpriteBatch spriteBatch)
            {
                if (_active)
                    spriteBatch.Draw(_rippleTexture, new Vector2(TMBAW_Game.DefaultUiWidth / 2, (int)(TMBAW_Game.DefaultUiHeight * 3 / 5f)), new Rectangle(0, 0, _rippleTexture.Width, _rippleTexture.Height), Color.White * _opacity, rotation, center, 1, SpriteEffects.None, 0);
            }

        }

        public static class ColoredCorners
        {
            static Texture2D texture = ContentHelper.LoadTexture("Overlay/ui_whiteCorners");
            static Color color = Color.Black;
            static Color changeColor;
            static GameTimer timer = new GameTimer(true);

            public static void FlashColor(Color color)
            {
                timer.Reset();
                changeColor = color;
            }

            internal static void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, new Rectangle(0, 0, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight), Color.Black);
                color = changeColor * (float)((500 - timer.TimeElapsedInMilliSeconds) / 500);
                spriteBatch.Draw(texture, new Rectangle(0, 0, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight), color);
            }
        }


        private static class Heart
        {
            static ComplexAnimation animation;
            static Vector2 position;
            static SpriteFont fontBig;
            static SpriteFont fontSmall;
            static int health;
            static int maxHealth;

            /// <summary>
            /// Initializes animation components and resets variables.
            /// </summary>
            public static void Initialize()
            {
                position = new Vector2(30, 30);
                animation = new ComplexAnimation()
                {
                    Scale = 4
                };
                ComplexAnimData normal = new ComplexAnimData(1, GameWorld.UiSpriteSheet, new Rectangle(), 80, 16, 16, 125, 4, true);
                ComplexAnimData dead = new ComplexAnimData(1000, GameWorld.UiSpriteSheet, new Rectangle(), 96, 16, 16, 125, 4, true);
                ComplexAnimData poison = new ComplexAnimData(100, GameWorld.UiSpriteSheet, new Rectangle(), 64, 16, 16, 125, 4, true);
                animation.AddAnimationData("normal", normal);
                animation.AddAnimationData("dead", dead);
                animation.AddAnimationData("poison", poison);
                animation.AddToQueue("normal");
                fontBig = ContentHelper.LoadFont("Fonts/x32");
                fontSmall = ContentHelper.LoadFont("Fonts/x16");
            }

            public static void Update(Player player)
            {
                //animation.RemoveAllFromQueue();

                //if (player.Health <= 0)
                //{
                //    animation.AddToQueue("dead");
                //}
                //else if (player.IsPoisoned)
                //{
                //    animation.AddToQueue("poison");
                //}
                //else
                //{
                //    animation.AddToQueue("normal");
                //}

                animation.Update();

                health = player.Health;

                if (health < 0) health = 0;

                maxHealth = player.MaxHealth;

            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                animation.Draw(spriteBatch, position);
                float x = position.X + 80;
                float y = position.Y + 15;
                FontHelper.DrawWithOutline(spriteBatch, fontBig, health.ToString(), new Vector2(x, y), 1, Color.White, Color.Black);

                float widthHealth = fontBig.MeasureString(health.ToString()).X;
                FontHelper.DrawWithOutline(spriteBatch, fontSmall, "/" + maxHealth.ToString(), new Vector2(x + widthHealth + 5, y + 20), 1, Color.White, Color.Black);
            }
        }

        private static class Coin
        {
            static ComplexAnimation animation;
            static Vector2 position;
            static SpriteFont fontBig;
            static SpriteFont fontSmall;
            static int score;

            /// <summary>
            /// Initializes animation components and resets variables.
            /// </summary>
            public static void Initialize()
            {
                position = new Vector2(250, 30);
                animation = new ComplexAnimation()
                {
                    Scale = 4
                };
                ComplexAnimData normal = new ComplexAnimData(1, GameWorld.UiSpriteSheet, new Rectangle(), 112, 16, 16, 125, 8, true);
                animation.AddAnimationData("normal", normal);
                animation.AddToQueue("normal");
                fontBig = ContentHelper.LoadFont("Fonts/x32");
                fontSmall = ContentHelper.LoadFont("Fonts/x16");
            }

            public static void Update(Player player)
            {
                animation.Update();

                score = player.Score;

            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                animation.Draw(spriteBatch, position);
                float x = position.X + 80;
                float y = position.Y + 20;
                FontHelper.DrawWithOutline(spriteBatch, fontBig, score.ToString(), new Vector2(x, y), 1, Color.White, Color.Black);

            }
        }

        /// <summary>
        /// The semi-transparent black texture behind active UI elements that require immediate attention.
        /// </summary>
        public static class DarkBackground
        {
            static bool active;
            public static void Show()
            {
                active = true;
            }
            public static void Hide()
            {
                active = false;
            }
            internal static void Draw(SpriteBatch spriteBatch)
            {
                if (active)
                    spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), new Rectangle(0, 0, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight), Color.Black * .7f);
                Hide();
            }
        }

        public static void Initialize()
        {
            Heart.Initialize();
            Coin.Initialize();
        }

        public static void Update()
        {
            WhiteFlash.Update();
            RewindEffect.Update();
            BlackFade.Update();
            Heart.Update(GameWorld.GetPlayers()[0]);
            Coin.Update(GameWorld.GetPlayers()[0]);
        }

        public static void FlashWhite()
        {
            WhiteFlash.Start();
        }

        public static void FadeToBlack()
        {
            BlackFade.FadeOut();
        }

        public static void FadeIn()
        {
            BlackFade.FadeIn();
        }

        public static void ActivateRewindEffect()
        {
            RewindEffect.Activate();
        }

        public static void DeactivateRewindEffect()
        {
            RewindEffect.Deactivate();
        }

        public static void RewindEffectSetOpacity(float opacity)
        {
            RewindEffect.SetOpacity(opacity);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            WhiteFlash.Draw(spriteBatch);
            RewindEffect.Draw(spriteBatch);
            ColoredCorners.Draw(spriteBatch);
            if (TMBAW_Game.CurrentGameMode == GameMode.Play && !StoryTracker.InCutscene)
            {
                Heart.Draw(spriteBatch);
                Coin.Draw(spriteBatch);
            }
            DarkBackground.Draw(spriteBatch);
            BlackFade.Draw(spriteBatch);
            BlackBars.Draw(spriteBatch);
        }

        public static void DrawRipples(SpriteBatch spriteBatch)
        {
            RewindEffect.DrawRipples(spriteBatch);
        }
    }
}
