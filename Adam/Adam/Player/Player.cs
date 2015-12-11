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

namespace Adam
{
    public partial class Player : Character
    {
        public delegate void Eventhandler();
        public event EventHandler PlayerRespawned;
        protected PlayerScript script = new PlayerScript();

        public SoundFx AttackSound;

        #region Variables
        Main game1;
        public Rectangle attackBox;

        Jetpack jetpack = new Jetpack();

        float deltaTime;

        public Vector2 respawnPos;

        //Debug
        public bool canFly;
        public bool isInvulnerable;
        public bool isGhost;

        //Player stats
        public int Score
        {
            get
            {
                try
                {
                    return game1.GameData.CurrentSave.PlayerStats.Score;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                game1.GameData.CurrentSave.PlayerStats.Score = value;
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
        Vector2 frameCount;
        #endregion

        public delegate void PlayerHandler(Player player);
        public event PlayerHandler AnimationEnded;
        public event PlayerHandler AnimationFrameChanged;

        public delegate void DamageHandler(Rectangle damageArea, int damage);
        public event DamageHandler PlayerAttacked;
        public event DamageHandler PlayerDamaged;

        public Player(Main game1)
        {
            
            this.game1 = game1;

            script.Initialize(this);

            Texture2D edenTexture = ContentHelper.LoadTexture("Characters/adam_eden_darker");
            Texture2D idlePoop = ContentHelper.LoadTexture("Characters/adam_poop");
            Texture2D ninjaDash = ContentHelper.LoadTexture("Characters/adam_ninja");
            Texture2D standupTexture = ContentHelper.LoadTexture("Characters/adam_standup");
            Texture2D fightTexture = ContentHelper.LoadTexture("Characters/adam_punch");

            AttackSound = new SoundFx("Player/attackSound");

            complexAnim.AnimationEnded += ComplexAnim_AnimationEnded;
            complexAnim.AnimationStateChanged += ComplexAnim_AnimationStateChanged;
            complexAnim.FrameChanged += ComplexAnim_FrameChanged;

            // Animation textures.
            complexAnim.AddAnimationData("idle", new ComplexAnimData(0, edenTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 400, 4, true));
            complexAnim.AddAnimationData("smellPoop", new ComplexAnimData(1, idlePoop, new Rectangle(6, 7, 12, 66), 0, 24, 40, 125, 21, false));
            complexAnim.AddAnimationData("sleep", new ComplexAnimData(1, edenTexture, new Rectangle(6, 7, 12, 66), 200, 24, 40, 125, 4, true));
            complexAnim.AddAnimationData("fightIdle", new ComplexAnimData(50, fightTexture, new Rectangle(6, 7, 12, 66), 40, 24, 40, 125, 4, true));
            complexAnim.AddAnimationData("walk", new ComplexAnimData(100, edenTexture, new Rectangle(6, 7, 12, 66), 40, 24, 40, 125, 4, true));
            complexAnim.AddAnimationData("run", new ComplexAnimData(150, edenTexture, new Rectangle(6, 7, 12, 66), 240, 24, 40, 125, 4, true));
            complexAnim.AddAnimationData("standup", new ComplexAnimData(155, standupTexture, new Rectangle(15, 11, 12, 66), 0, 45, 40, 125, 3, false));
            complexAnim.AddAnimationData("jump", new ComplexAnimData(200, edenTexture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 125, 4, false));
            complexAnim.AddAnimationData("climb", new ComplexAnimData(900, edenTexture, new Rectangle(6, 7, 12, 66), 160, 24, 40, 125, 4, true));
            complexAnim.AddAnimationData("fall", new ComplexAnimData(1000, edenTexture, new Rectangle(6, 7, 12, 66), 120, 24, 40, 125, 4, true));
            complexAnim.AddAnimationData("ninjaDash", new ComplexAnimData(1100, ninjaDash, new Rectangle(19, 8, 12, 66), 0, 48, 40, 200, 1, false));
            complexAnim.AddAnimationData("punch", new ComplexAnimData(1110, fightTexture, new Rectangle(6, 7, 12, 66), 0, 24, 40, 75, 4, false));
            complexAnim.AddAnimationData("punch2", new ComplexAnimData(1111, fightTexture, new Rectangle(6, 7, 12, 66), 80, 24, 40, 75, 4, false));

            // Sounds
            Sounds.AddSoundRef("hurt", "Player/hurtSound");
            Sounds.AddSoundRef("jump", "Player/jumpSound");
            Sounds.AddSoundRef("stomp", "Player/jumpSound");
            Sounds.AddSoundRef("punch", "Sounds/punch");

            complexAnim.AddToQueue("idle");

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
            collRectangle.X = setX;
            collRectangle.Y = setY;
            respawnPos = new Vector2(setX, setY);

            //Animation information
            frameCount = new Vector2(4, 0);
            collRectangle.Width = 32;
            collRectangle.Height = 64;
            sourceRectangle = new Rectangle(0, 0, 24, 40);

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

            deltaTime = (float)(60 * gameTime.ElapsedGameTime.TotalSeconds);

            if (IsDead())
            {
                Burn();
                return;
            }

            CheckInput();
            Burn();
            UpdatePlayerPosition();
            base.Update();

            jetpack.Update(this, gameTime);
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
            if (collRectangle.X < 0)
                collRectangle.X = 0;
            if (collRectangle.X > (gameWorld.worldData.LevelWidth * Main.Tilesize - collRectangle.Width))
                collRectangle.X = (gameWorld.worldData.LevelWidth * Main.Tilesize - collRectangle.Width);
            if (collRectangle.Y < 0)
                collRectangle.Y = 0;
            if (collRectangle.Y > (gameWorld.worldData.LevelHeight * Main.Tilesize - collRectangle.Width) + 100)
            {
                // Player dies when he falls out of the world in play mode.
                if (gameWorld.CurrentGameMode == GameMode.Edit)
                    collRectangle.Y = gameWorld.worldData.LevelHeight * Main.Tilesize - collRectangle.Height;
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
                movementParticlesTimer.Increment();
                if (velocity.X == 0)
                    return;
                if (movementParticlesTimer.TimeElapsedInMilliSeconds > 10000 / Math.Abs(velocity.X))
                {
                    movementParticlesTimer.Reset();
                    SmokeParticle par = new SmokeParticle(collRectangle.Center.X, collRectangle.Bottom);
                    GameWorld.ParticleSystem.Add(par);
                }
            }
        }

        private void Burn()
        {
            // Checks to see if player is on fire and deals damage accordingly.
            if (IsOnFire)
            {
                onFireTimer.Increment();
                if (onFireTimer.TimeElapsedInSeconds < 4)
                {
                    fireTickTimer.Increment();
                    fireSpawnTimer.Increment();
                    if (fireTickTimer.TimeElapsedInMilliSeconds > 500)
                    {
                        TakeDPS(EnemyDB.FlameSpitter_DPS);
                        fireTickTimer.Reset();
                    }
                    if (fireSpawnTimer.TimeElapsedInMilliSeconds > 100)
                    {
                        EntityFlameParticle flame = new EntityFlameParticle(this, Color.Yellow);
                        EntityFlameParticle flame2 = new EntityFlameParticle(this, Color.Red);
                        GameWorld.Instance.particles.Add(flame);
                        GameWorld.Instance.particles.Add(flame2);
                        fireSpawnTimer.Reset();
                    }
                }
                else
                {
                    IsOnFire = false;
                    onFireTimer.Reset();
                    fireTickTimer.Reset();
                }

            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.CurrentGameMode == GameMode.Edit)
                return;
            complexAnim.Draw(spriteBatch, IsFacingRight, Color);
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Player takes damage without becoming invincible and without spilling blood.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDPS(int damage)
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
                GameWorld.Instance.particles.Add(new JumpSmokeParticle(this));
            }
        }

        private void CreateStompParticles()
        {
            for (int i = 0; i < 20; i++)
            {
                GameWorld.Instance.particles.Add(new StompSmokeParticle(this));
            }
        }

        public void KillAndRespawn()
        {
            TakeDamage(null, Health);

            for (int i = 0; i < 10; i++)
            {
                Particle par = new Particle();
                par.CreateDeathSmoke(this);
                GameWorld.Instance.particles.Add(par);
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
                GameWorld.Instance.particles.Add(par);
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

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(collRectangle.X - 8, collRectangle.Y - 16, 48, 80);
            }
        }
    }

}
