using Adam.Characters;
using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Adam.PlayerCharacter
{
    public partial class Player : Character
    {
        public delegate void DamageHandler(Rectangle damageArea, int damage);
        public delegate void Eventhandler();
        public delegate void PlayerHandler(Player player);

        private readonly PlayerScript script = new PlayerScript();
        public Rectangle AttackBox;
        public SoundFx AttackSound;
        //Debug
        public bool CanFly;
        public bool IsGhost;
        public bool IsInvulnerable;
        public bool IsClimbing { get; set; }
        public bool IsVisible { get; set; } = true;
        private string _spawnPointNextLevel;

        public RewindTracker rewindTracker = new RewindTracker();
        public Timer rewindTimer = new Timer();

        public Player()
        {
            script.Initialize(this);

            var edenTexture = ContentHelper.LoadTexture("Characters/new_player");
            var idlePoop = ContentHelper.LoadTexture("Characters/adam_poop");
            var ninjaDash = ContentHelper.LoadTexture("Characters/adam_ninja");
            var fallStandTexture = ContentHelper.LoadTexture("Characters/adam_fall");
            var fightTexture = ContentHelper.LoadTexture("Characters/adam_punch");

            AttackSound = new SoundFx("Player/attackSound");

            _complexAnimation.AnimationEnded += ComplexAnim_AnimationEnded;
            _complexAnimation.AnimationStateChanged += ComplexAnim_AnimationStateChanged;
            _complexAnimation.FrameChanged += ComplexAnim_FrameChanged;

            // Animation textures.
            _complexAnimation.AddAnimationData("editMode",
            new ComplexAnimData(9999, edenTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 400, 4, true));
            _complexAnimation.AddAnimationData("idle",
            new ComplexAnimData(0, edenTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 400, 4, true));
            _complexAnimation.AddAnimationData("smellPoop",
                new ComplexAnimData(1, idlePoop, new Rectangle(6, 7, 12, 66), 0, 24, 40, 125, 21, false));
            _complexAnimation.AddAnimationData("sleep",
                new ComplexAnimData(1, edenTexture, new Rectangle(6, 7, 12, 66), 200, 24, 40, 125, 4, true));
            //ComplexAnim.AddAnimationData("idle",
            //    new ComplexAnimData(0, edenTexture, new Rectangle(6, 7, 12, 66), 400, 24, 40, 125, 4, true));
            _complexAnimation.AddAnimationData("oldWalk",
                new ComplexAnimData(100, edenTexture, new Rectangle(6, 7, 12, 66), 40, 24, 40, 25, 4, true));
            _complexAnimation.AddAnimationData("walk",
                new ComplexAnimData(150, edenTexture, new Rectangle(6, 7, 12, 66), 240, 24, 40, 125, 4, true));
            _complexAnimation.AddAnimationData("slide",
               new ComplexAnimData(153, edenTexture, new Rectangle(6, 7, 12, 66), 280, 24, 40, 125, 4, true));
            _complexAnimation.AddAnimationData("standup",
                new ComplexAnimData(155, fallStandTexture, new Rectangle(15, 7, 12, 66), 0, 45, 40, 125, 3, false));
            _complexAnimation.AddAnimationData("duck",
                new ComplexAnimData(156, fallStandTexture, new Rectangle(15, 7, 12, 66), 40, 45, 40, 125, 3, false));
            _complexAnimation.AddAnimationData("jump",
                new ComplexAnimData(200, edenTexture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 125, 4, false));
            _complexAnimation.AddAnimationData("climb",
                new ComplexAnimData(900, edenTexture, new Rectangle(6, 7, 12, 66), 160, 24, 40, 75, 4, true));
            _complexAnimation.AddAnimationData("fall",
                new ComplexAnimData(1000, edenTexture, new Rectangle(6, 7, 12, 66), 120, 24, 40, 125, 4, true));
            _complexAnimation.AddAnimationData("ninjaDash",
                new ComplexAnimData(1100, ninjaDash, new Rectangle(19, 8, 12, 66), 0, 48, 40, 200, 1, false));
            _complexAnimation.AddAnimationData("punch",
                new ComplexAnimData(1110, fightTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 75, 4, false));
            _complexAnimation.AddAnimationData("punch2",
                new ComplexAnimData(1111, fightTexture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 75, 4, false));
           // ComplexAnim.AddAnimationData("death",
                //new ComplexAnimData(int.MaxValue, edenTexture, new Rectangle(6, 7, 12, 66), 280, 24, 40, 125, 4, true));

            // Sounds
            Sounds.AddSoundRef("hurt", "Player/hurtSound");
            Sounds.AddSoundRef("jump", "Player/jumpSound");
            Sounds.AddSoundRef("stomp", "Player/jumpSound");
            Sounds.AddSoundRef("fail", "Sounds/Menu/level_fail");

            _complexAnimation.AddToQueue("idle");

            InitializeInput();
            Initialize(0, 0);

            PlayerAttacked += OnPlayerAttack;
            HasFinishedDying += OnPlayerDeath;
            HasTakenDamage += OnDamageTaken;
            HasRevived += OnPlayerRevive;
        }


        private void OnPlayerRevive()
        {
        }

        protected override Rectangle DrawRectangle => new Rectangle(CollRectangle.X - 8, CollRectangle.Y - 16, 48, 80);
        //Player stats
        public int Score;

        public override int MaxHealth => 100;
        //Animation Variables
        public int CurrentAnimationFrame { get; private set; }
        public bool IsPoisoned { get; internal set; }

        public event EventHandler PlayerRespawned;
        public event PlayerHandler AnimationEnded;
        public event PlayerHandler AnimationFrameChanged;
        public event DamageHandler PlayerAttacked;
        public event DamageHandler PlayerDamaged;

        private void OnPlayerAttack(Rectangle damageArea, int damage)
        {
            Sounds.GetSoundRef("punch").Play();
        }

        private void ComplexAnim_FrameChanged(FrameArgs e)
        {
            CurrentAnimationFrame = e.CurrentFrame;
            if (AnimationFrameChanged != null)
                AnimationFrameChanged(this);
        }

        private void ComplexAnim_AnimationStateChanged()
        {
        }

        private void ComplexAnim_AnimationEnded()
        {
            AnimationEnded?.Invoke(this);
        }

        /// <summary>
        ///     This method will set the player's positions to those specified. It should be used when the map is changed.
        /// </summary>
        /// <param name="setX"> The x-Coordinate</param>
        /// <param name="setY"> The y-Coordinate</param>
        public void Initialize(int setX, int setY)
        {
            if (_spawnPointNextLevel != null)
            {
                int spawnIndex;
                if (int.TryParse(_spawnPointNextLevel, out spawnIndex))
                {
                    int x = (spawnIndex % GameWorld.WorldData.LevelWidth) * AdamGame.Tilesize;
                    int y = (spawnIndex / GameWorld.WorldData.LevelWidth) * AdamGame.Tilesize;
                    Position = new Vector2(x, y);
                    RespawnPos = new Vector2(x, y);
                    _spawnPointNextLevel = null;
                    goto NoError;
                }

            }
            //Set the player position according to where in the map his default spawn point is.
            Position = new Vector2(setX, setY);
            RespawnPos = new Vector2(setX, setY);

            NoError:

            //Animation information
            CollRectangle = new Rectangle(0, 0, 32, 64);
            SourceRectangle = new Rectangle(0, 0, 24, 40);

            InitializeInput();
        }

        /// <summary>
        ///     Update player information, checks for collision and input, and many other things.
        /// </summary>
        public override void Update()
        {

            ContainInGameWorld();

            CheckInput();
            Burn();

            rewindTracker.Update(this);
            rewindTimer.Increment();

            if (!IsOnVines)
            {
                IsClimbing = false;
            }
            if (IsClimbing)
            {
                ObeysGravity = false;
            }
            else ObeysGravity = true;

            base.Update();
        }

        private void ContainInGameWorld()
        {
            if (CollRectangle.X < 0)
                SetX(0);
            if (CollRectangle.X > (GameWorld.WorldData.LevelWidth * AdamGame.Tilesize - CollRectangle.Width))
                SetX(GameWorld.WorldData.LevelWidth * AdamGame.Tilesize - CollRectangle.Width);
            if (CollRectangle.Y < 0)
                SetY(0);
            if (CollRectangle.Y > (GameWorld.WorldData.LevelHeight * AdamGame.Tilesize - CollRectangle.Width) + 100)
            {
                // Player dies when he falls out of the world in play mode.
                if (AdamGame.CurrentGameMode == GameMode.Edit)
                    SetY(GameWorld.WorldData.LevelHeight * AdamGame.Tilesize - CollRectangle.Height);
                else
                {
                    TakeDamage(null, MaxHealth);
                }
            }
        }

        public void CreateMovingParticles()
        {
            if (!IsJumping)
            {
                if (Math.Abs(Velocity.X) < 150f)
                    return;
                if (_movementParticlesTimer.TimeElapsedInMilliSeconds > 500 / (Math.Abs(Velocity.X) / 60))
                {
                    _movementParticlesTimer.Reset();

                    GameWorld.ParticleSystem.Add(ParticleType.Smoke, new Vector2(CollRectangle.Center.X, CollRectangle.Bottom),
                        new Vector2(0, (float)(AdamGame.Random.Next(-5, 5) / 10f)), Color.White);
                }
            }
        }

        private void Burn()
        {
            // Checks to see if player is on fire and deals damage accordingly.
            if (IsOnFire)
            {
                if (_onFireTimer.TimeElapsedInSeconds < 4)
                {
                    if (_fireTickTimer.TimeElapsedInMilliSeconds > 500)
                    {
                        TakeDps(EnemyDb.FlameSpitterDps);
                        _fireTickTimer.Reset();
                    }
                    if (_fireSpawnTimer.TimeElapsedInMilliSeconds > 100)
                    {
                        _fireSpawnTimer.Reset();
                    }
                }
                else
                {
                    IsOnFire = false;
                    _onFireTimer.Reset();
                    _fireTickTimer.Reset();
                }
            }
        }

        /// <summary>
        ///     Player takes damage without becoming invincible and without spilling blood.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDps(int damage)
        {
            Health -= damage;
        }

        public void Heal(int amount)
        {
            //TODO add sounds.
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        /// <summary>
        ///     Fires an event to all subscribers saying the player is dealing damage to that area.
        /// </summary>
        /// <param name="damageArea"></param>
        /// <param name="damage"></param>
        public void DealDamage(Rectangle damageArea, int damage)
        {
            PlayerAttacked?.Invoke(damageArea, damage);
        }

        public void MoveTo(Vector2 position)
        {
            Position = position;
        }

        private void OnPlayerDeath(Entity entity)
        {
            _respawnTimer.ResetAndWaitFor(4000);
            _respawnTimer.SetTimeReached += Revive;
        }

        private void OnDamageTaken()
        {
            Sounds.GetSoundRef("hurt").Play();
        }

        public void SetRespawnPoint(int x, int y)
        {
            RespawnPos = new Vector2(x, y);
        }

        /// <summary>
        /// Sets the spawn point for the player when the next level loads.
        /// </summary>
        /// <param name="spawnPoint"></param>
        public void SetSpawnPointForNextLevel(string spawnPoint)
        {
            _spawnPointNextLevel = spawnPoint;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Do not draw player in main menu mode.
            if (AdamGame.CurrentGameMode == GameMode.None) return;

            if (AdamGame.CurrentGameMode == GameMode.Play)
                rewindTracker.Draw(this, spriteBatch);
            if (IsVisible)
                base.Draw(spriteBatch);
        }
    }
}