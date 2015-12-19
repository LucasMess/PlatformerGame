using Adam;
using Adam.Characters;
using Adam.Characters.Enemies;
using Adam.Interactables;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.Obstacles;
using Adam.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam
{
    public partial class Player : Character
    {
        public delegate void Eventhandler();
        public event EventHandler PlayerRespawned;
        protected PlayerScript script = new PlayerScript();

        public SoundFx AttackSound;

        #region Variables
        Main _game1;
        public Rectangle AttackBox;

        Jetpack _jetpack = new Jetpack();

        float _deltaTime;

        public Vector2 RespawnPos;

        //Debug
        public bool CanFly;
        public bool IsInvulnerable;
        public bool IsGhost;

        //Player stats
        public int Score
        {
            get
            {
                try
                {
                    return _game1.GameData.CurrentSave.PlayerStats.Score;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _game1.GameData.CurrentSave.PlayerStats.Score = value;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return 100;
            }
        }

        //Animation Variables
        public int CurrentAnimationFrame
        {
            get; private set;
        }
        Vector2 _frameCount;
        #endregion

        public delegate void PlayerHandler(Player player);
        public event PlayerHandler AnimationEnded;
        public event PlayerHandler AnimationFrameChanged;

        public delegate void DamageHandler(Rectangle damageArea, int damage);
        public event DamageHandler PlayerAttacked;
        public event DamageHandler PlayerDamaged;

        public Player(Main game1)
        {
            
            this._game1 = game1;

            script.Initialize(this);

            Texture2D edenTexture = ContentHelper.LoadTexture("Characters/adam_eden_darker");
            Texture2D idlePoop = ContentHelper.LoadTexture("Characters/adam_poop");
            Texture2D ninjaDash = ContentHelper.LoadTexture("Characters/adam_ninja");
            Texture2D standupTexture = ContentHelper.LoadTexture("Characters/adam_standup");
            Texture2D fightTexture = ContentHelper.LoadTexture("Characters/adam_punch");

            AttackSound = new SoundFx("Player/attackSound");

            ComplexAnim.AnimationEnded += ComplexAnim_AnimationEnded;
            ComplexAnim.AnimationStateChanged += ComplexAnim_AnimationStateChanged;
            ComplexAnim.FrameChanged += ComplexAnim_FrameChanged;

            // Animation textures.
            ComplexAnim.AddAnimationData("idle", new ComplexAnimData(0, edenTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 400, 4, true));
            ComplexAnim.AddAnimationData("smellPoop", new ComplexAnimData(1, idlePoop, new Rectangle(6, 7, 12, 66), 0, 24, 40, 125, 21, false));
            ComplexAnim.AddAnimationData("sleep", new ComplexAnimData(1, edenTexture, new Rectangle(6, 7, 12, 66), 200, 24, 40, 125, 4, true));
            ComplexAnim.AddAnimationData("fightIdle", new ComplexAnimData(50, fightTexture, new Rectangle(6, 7, 12, 66), 40, 24, 40, 125, 4, true));
            ComplexAnim.AddAnimationData("walk", new ComplexAnimData(100, edenTexture, new Rectangle(6, 7, 12, 66), 40, 24, 40, 125, 4, true));
            ComplexAnim.AddAnimationData("run", new ComplexAnimData(150, edenTexture, new Rectangle(6, 7, 12, 66), 240, 24, 40, 125, 4, true));
            ComplexAnim.AddAnimationData("standup", new ComplexAnimData(155, standupTexture, new Rectangle(15, 11, 12, 66), 0, 45, 40, 125, 3, false));
            ComplexAnim.AddAnimationData("jump", new ComplexAnimData(200, edenTexture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 125, 4, false));
            ComplexAnim.AddAnimationData("climb", new ComplexAnimData(900, edenTexture, new Rectangle(6, 7, 12, 66), 160, 24, 40, 125, 4, true));
            ComplexAnim.AddAnimationData("fall", new ComplexAnimData(1000, edenTexture, new Rectangle(6, 7, 12, 66), 120, 24, 40, 125, 4, true));
            ComplexAnim.AddAnimationData("ninjaDash", new ComplexAnimData(1100, ninjaDash, new Rectangle(19, 8, 12, 66), 0, 48, 40, 200, 1, false));
            ComplexAnim.AddAnimationData("punch", new ComplexAnimData(1110, fightTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 75, 4, false));
            ComplexAnim.AddAnimationData("punch2", new ComplexAnimData(1111, fightTexture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 75, 4, false));

            // Sounds
            Sounds.AddSoundRef("hurt", "Player/hurtSound");
            Sounds.AddSoundRef("jump", "Player/jumpSound");
            Sounds.AddSoundRef("stomp", "Player/jumpSound");
            Sounds.AddSoundRef("punch", "Sounds/punch");

            ComplexAnim.AddToQueue("idle");

            InitializeInput();
            Initialize(0, 0);

            PlayerAttacked += OnPlayerAttack;
        }

        private void OnPlayerAttack(Rectangle damageArea, int damage)
        {
            Sounds.Get("punch").Play();
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
            if (AnimationEnded != null)
                AnimationEnded(this);
        }

        /// <summary>
        /// This method will set the player's positions to those specified. It should be used when the map is changed.
        /// </summary>
        /// <param name="setX"> The x-Coordinate</param>
        /// <param name="setY"> The y-Coordinate</param>
        public void Initialize(int setX, int setY)
        {
            //Set the player position according to where in the map he is supposed to be
            CollRectangle.X = setX;
            CollRectangle.Y = setY;
            RespawnPos = new Vector2(setX, setY);

            //Animation information
            _frameCount = new Vector2(4, 0);
            CollRectangle.Width = 32;
            CollRectangle.Height = 64;
            SourceRectangle = new Rectangle(0, 0, 24, 40);

            InitializeInput();
        }

        /// <summary>
        /// Update player information, checks for collision and input, and many other things.
        /// </summary>
        /// <param name="gameTime"></param>
        /// 
        public void Update(GameTime gameTime)
        {
            script.Run();

            if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
            {
                ContainInGameWorld();
                return;
            }

            _deltaTime = (float)(60 * gameTime.ElapsedGameTime.TotalSeconds);

            if (IsDead())
            {
                Burn();
                return;
            }

            CheckInput();
            Burn();
            UpdatePlayerPosition();
            base.Update();

            _jetpack.Update(this, gameTime);
        }


        /// <summary>
        /// This method updates all of the rectangles and applies velocity.
        /// </summary>
        private void UpdatePlayerPosition()
        {
            ContainInGameWorld();
        }

        private void ContainInGameWorld()
        {
            GameWorld gameWorld = GameWorld.Instance;
            if (CollRectangle.X < 0)
                CollRectangle.X = 0;
            if (CollRectangle.X > (gameWorld.WorldData.LevelWidth * Main.Tilesize - CollRectangle.Width))
                CollRectangle.X = (gameWorld.WorldData.LevelWidth * Main.Tilesize - CollRectangle.Width);
            if (CollRectangle.Y < 0)
                CollRectangle.Y = 0;
            if (CollRectangle.Y > (gameWorld.WorldData.LevelHeight * Main.Tilesize - CollRectangle.Width) + 100)
            {
                // Player dies when he falls out of the world in play mode.
                if (gameWorld.CurrentGameMode == GameMode.Edit)
                    CollRectangle.Y = gameWorld.WorldData.LevelHeight * Main.Tilesize - CollRectangle.Height;
                else
                {
                    KillAndRespawn();
                }
            }
        }

        public void CreateMovingParticles()
        {
            if (!IsJumping)
            {
                _movementParticlesTimer.Increment();
                if (Velocity.X == 0)
                    return;
                if (_movementParticlesTimer.TimeElapsedInMilliSeconds > 10000 / Math.Abs(Velocity.X))
                {
                    _movementParticlesTimer.Reset();
                    SmokeParticle par = new SmokeParticle(CollRectangle.Center.X, CollRectangle.Bottom);
                    GameWorld.ParticleSystem.Add(par);
                }
            }
        }

        private void Burn()
        {
            // Checks to see if player is on fire and deals damage accordingly.
            if (IsOnFire)
            {
                _onFireTimer.Increment();
                if (_onFireTimer.TimeElapsedInSeconds < 4)
                {
                    _fireTickTimer.Increment();
                    _fireSpawnTimer.Increment();
                    if (_fireTickTimer.TimeElapsedInMilliSeconds > 500)
                    {
                        TakeDps(EnemyDb.FlameSpitterDps);
                        _fireTickTimer.Reset();
                    }
                    if (_fireSpawnTimer.TimeElapsedInMilliSeconds > 100)
                    {
                        EntityFlameParticle flame = new EntityFlameParticle(this, Color.Yellow);
                        EntityFlameParticle flame2 = new EntityFlameParticle(this, Color.Red);
                        GameWorld.Instance.Particles.Add(flame);
                        GameWorld.Instance.Particles.Add(flame2);
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                return;
            ComplexAnim.Draw(spriteBatch, IsFacingRight, Color);
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Player takes damage without becoming invincible and without spilling blood.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDps(int damage)
        {
            Health -= damage;
        }

        public void TakeDamageAndKnockBack(int damage) { }

        public override void Revive()
        {
        }

        private void CreateJumpParticles()
        {
            for (int i = 0; i < 20; i++)
            {
                GameWorld.Instance.Particles.Add(new JumpSmokeParticle(this));
            }
        }

        private void CreateStompParticles()
        {
            for (int i = 0; i < 20; i++)
            {
                GameWorld.Instance.Particles.Add(new StompSmokeParticle(this));
            }
        }

        public void KillAndRespawn()
        {
            TakeDamage(null, Health);

            for (int i = 0; i < 10; i++)
            {
                Particle par = new Particle();
                par.CreateDeathSmoke(this);
                GameWorld.Instance.Particles.Add(par);
            }

            int rand = GameWorld.RandGen.Next(20, 30);
            SpillBlood(rand);
            TakeDamageAndKnockBack(Health);
        }

        public void SpillBlood(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Particle par = new Particle();
                par.CreateBloodEffect(this, GameWorld.Instance);
                GameWorld.Instance.Particles.Add(par);
            }
        }

        public void Heal(int amount)
        {
            //TODO add sounds.
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        /// <summary>
        /// Fires an event to all subscribers saying the player is dealing damage to that area.
        /// </summary>
        /// <param name="damageArea"></param>
        /// <param name="damage"></param>
        public void DealDamage(Rectangle damageArea, int damage)
        {
            if (PlayerAttacked != null)
                PlayerAttacked(damageArea, damage);
        }

        public void Teleport(Vector2 position)
        {
            CollRectangle.X = (int)position.X;
            CollRectangle.Y = (int)position.Y;
            Overlay.Instance.FadeIn();
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
