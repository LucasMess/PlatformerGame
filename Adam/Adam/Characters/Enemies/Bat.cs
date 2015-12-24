using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;

namespace Adam.Characters.Enemies
{
    internal class Bat : Enemy, IAnimated
    {
        private Animation _animation;
        private AnimationData[] _animationData;
        private bool _isLookingForRefuge;
        private bool _isSleeping;
        private Vector2 _maxVelocity;
        private Rectangle _rangeRect;
        private Rectangle _respawnRect;

        public Bat(int x, int y)
        {
            CollRectangle = new Rectangle(x, y, 32, 32);
            SourceRectangle = new Rectangle(0, 0, 32, 32);
            _maxVelocity = new Vector2(2, 2);
            Texture = ContentHelper.LoadTexture("Enemies/bat");

            CollidedWithTileAbove += OnCollisionWithTerrainAbove;
        }

        public override byte Id => 207;

        public override int MaxHealth => EnemyDb.BatMaxHealth;
        protected override SoundFx MeanSound => null;
        protected override SoundFx AttackSound => null;
        protected override SoundFx DeathSound => null;
        protected override Rectangle DrawRectangle => new Rectangle(CollRectangle.X - 16, CollRectangle.Y, 64, 64);

        public Animation Animation
            => _animation ?? (_animation = new Animation(Texture, DrawRectangle, SourceRectangle));

        public AnimationData[] AnimationData => _animationData ?? (_animationData = new[]
        {
            new AnimationData(200, 5, 0, AnimationType.Loop),
            new AnimationData(85, 5, 1, AnimationType.Loop)
        });

        public AnimationState CurrentAnimationState { get; set; }

        public void Animate()
        {
            switch (CurrentAnimationState)
            {
                case AnimationState.Still:
                    break;
                case AnimationState.Walking:
                    break;
                case AnimationState.Jumping:
                    break;
                case AnimationState.Charging:
                    break;
                case AnimationState.Talking:
                    break;
                case AnimationState.Sleeping:
                    Animation.Update(Main.GameTime, DrawRectangle, AnimationData[0]);
                    break;
                case AnimationState.Flying:
                    Animation.Update(Main.GameTime, DrawRectangle, AnimationData[1]);
                    break;
                case AnimationState.Transforming:
                    break;
            }
        }

        public void OnCollisionWithTerrainAbove(Entity entity, Tile tile)
        {
            if (_isLookingForRefuge)
            {
                _isSleeping = true;
            }
            else
            {
                Velocity.Y = 0;
            }
        }

        public override void Update()
        {
            var player = GameWorld.Instance.GetPlayer();

            _rangeRect = new Rectangle(CollRectangle.X - 100, CollRectangle.Y - 100, CollRectangle.Width + 200,
                CollRectangle.Height + 200);

            if (player.GetCollRectangle().Intersects(_rangeRect))
            {
                _isSleeping = false;
                _isLookingForRefuge = false;
            }
            else
            {
                _isLookingForRefuge = true;
            }

            if (!_isLookingForRefuge)
            {
                const int buffer = 5;
                if (CollRectangle.Y < player.GetCollRectangle().Y - buffer)
                {
                    Velocity.Y = _maxVelocity.Y;
                }
                else if (CollRectangle.Y > player.GetCollRectangle().Y + buffer)
                {
                    Velocity.Y = -_maxVelocity.Y;
                }
                else
                {
                    Velocity.Y = 0;
                }

                if (CollRectangle.X < player.GetCollRectangle().X - buffer)
                {
                    Velocity.X = _maxVelocity.X;
                }
                else if (CollRectangle.X > player.GetCollRectangle().X + buffer)
                {
                    Velocity.X = -_maxVelocity.X;
                }
                else
                {
                    Velocity.X = 0;
                }
            }
            else
            {
                Velocity.X = 0;
                Velocity.Y = -_maxVelocity.Y;
            }

            if (_isSleeping)
            {
                CurrentAnimationState = AnimationState.Sleeping;
                Velocity = Vector2.Zero;
            }
            else
            {
                CurrentAnimationState = AnimationState.Flying;
            }

            base.Update();
        }
    }
}