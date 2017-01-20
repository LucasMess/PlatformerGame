using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
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
                spriteBatch.Draw(_white, new Rectangle(0, 0, AdamGame.UserResWidth, AdamGame.UserResHeight), Color.White * _whiteOpacity);
            }
        }
        /// <summary>
        /// Used to fade in and out to black.
        /// </summary>
        private static class BlackFade
        {
            private static bool _isActive = false;
            private static bool _isFadingIn = false;
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
        }

        private static class Heart
        {
            static ComplexAnimation animation;
            static Vector2 position;
            static int health;
            static int maxHealth;

            /// <summary>
            /// Initializes animation components and resets variables.
            /// </summary>
            public static void Initialize()
            {
                position = new Vector2(5, 5);
                position *= CalcHelper.GetScreenScale();
                animation = new ComplexAnimation();
                animation.DoubleSpriteSize = false;
                ComplexAnimData normal = new ComplexAnimData(1, GameWorld.UiSpriteSheet, new Rectangle(), 80, 16, 16, 125, 4, true);
                ComplexAnimData dead = new ComplexAnimData(1000, GameWorld.UiSpriteSheet, new Rectangle(), 96, 16, 16, 125, 4, true);
                ComplexAnimData poison = new ComplexAnimData(100, GameWorld.UiSpriteSheet, new Rectangle(), 64, 16, 16, 125, 4, true);
                animation.AddAnimationData("normal", normal);
                animation.AddAnimationData("dead", dead);
                animation.AddAnimationData("poison", poison);
                animation.AddToQueue("normal");
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
                maxHealth = player.MaxHealth;

            }

            public static void Draw(SpriteBatch spriteBatch)
            {
                animation.Draw(spriteBatch, position);
                float x = position.X + 16;
                FontHelper.DrawWithOutline(spriteBatch, FontHelper.ChooseBestFont(20), health + "/" + maxHealth, new Vector2(x, position.Y), 2, Color.White, Color.Black);
            }
        }

        public static void Initialize()
        {
            Heart.Initialize();
        }

        public static void Update()
        {
            WhiteFlash.Update();

            Heart.Update(GameWorld.GetPlayer());
        }

        public static void FlashWhite()
        {
            WhiteFlash.Start();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            WhiteFlash.Draw(spriteBatch);
            if (AdamGame.CurrentGameMode == GameMode.Play)
                Heart.Draw(spriteBatch);
        }
    }
}
