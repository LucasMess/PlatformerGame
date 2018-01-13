using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using ThereMustBeAnotherWay.Misc.Helpers;
using System;
using System.Diagnostics;
using System.Threading;

namespace ThereMustBeAnotherWay.Particles
{
    /// <summary>
    /// This particle system allocates memory once for all particles.
    /// </summary>
    public static partial class ParticleSystem
    {
        /// <summary>
        /// The maximum allowed number of particles.
        /// </summary>
        private const int MAX_PARTICLES = 10000;

        private static List<Particle> _aliveParticles = new List<Particle>(MAX_PARTICLES);
        private static Stack<Particle> _deadParticles = new Stack<Particle>(MAX_PARTICLES);

        private static List<Particle> _rippleAliveParticles = new List<Particle>(MAX_PARTICLES);
        private static Stack<Particle> _rippleDeadParticles = new Stack<Particle>(MAX_PARTICLES);

        private static Stopwatch updateTimer = new Stopwatch();
        private static Stopwatch drawTimer = new Stopwatch();
        private static List<long> pastUpdateTimes = new List<long>();
        private static List<long> pastDrawTimes = new List<long>();

        /// <summary>
        /// Returns the average update time in ticks.
        /// </summary>
        public static long UpdateTime
        {
            get
            {
                long average = 0;
                for (int i = 0; i < pastUpdateTimes.Count; i++)
                {
                    average += pastUpdateTimes[i];
                }
                return average / pastUpdateTimes.Count;
            }
        }

        /// <summary>
        /// Returns the average draw time in ticks.
        /// </summary>
        public static long DrawTime
        {
            get
            {
                long average = 0;
                for (int i = 0; i < pastDrawTimes.Count; i++)
                {
                    average += pastDrawTimes[i];
                }
                return average / pastDrawTimes.Count;
            }
        }

        //static Thread updateThread;
        //public static AutoResetEvent UpdateStartEvent = new AutoResetEvent(true);

        private static Particle _tempParticle;

        /// <summary>
        /// Revives a dead particle and returns it.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Particle GetNextParticle(ParticleType type)
        {
            switch (type)
            {
                // All Ripple type particles.
                case ParticleType.HeatEffect:
                case ParticleType.ProjectileHeatTrail:
                    if (_rippleDeadParticles.Count == 0)
                    {
                        return _rippleAliveParticles[0];
                    }
                    _tempParticle = _rippleDeadParticles.Pop();
                    _rippleAliveParticles.Add(_tempParticle);
                    break;
                // Normal particles.
                default:
                    if (_deadParticles.Count == 0)
                    {
                        return _aliveParticles[0];
                    }
                    _tempParticle = _deadParticles.Pop();
                    _aliveParticles.Add(_tempParticle);
                    break;
            }
            return _tempParticle;
        }


        public static void Initialize()
        {
            // Create all in the dead particle stack to allocate memory before-hand.
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                _deadParticles.Push(new Particle());
                _rippleDeadParticles.Push(new Particle());
            }

            // Start the update thread for the normal particles.
            //updateThread = new Thread(new ThreadStart(Update))
            //{
            //    IsBackground = true,
            //};

            //updateThread.Start();
        }

        public static void Update()
        {
            //while (true)
            //{
                // Wait for gameworld update to start and then start the timer for profiling.
                //UpdateStartEvent.WaitOne();
                updateTimer.Restart();

                // Update only the particles that are alive.
                for (int i = _aliveParticles.Count - 1; i >= 0; i--)
                {
                    _aliveParticles[i].Update();
                    if (_aliveParticles[i].IsDead())
                    {
                        _deadParticles.Push(_aliveParticles[i]);
                        _aliveParticles.RemoveAt(i);
                        continue;
                    }
                }
                for (int i = _rippleAliveParticles.Count - 1; i >= 0; i--)
                {
                    _rippleAliveParticles[i].Update();
                    if (_rippleAliveParticles[i].IsDead())
                    {
                        _rippleDeadParticles.Push(_rippleAliveParticles[i]);
                        _rippleAliveParticles.RemoveAt(i);
                    continue;
                    }
                }

                // Update the particle emitters.
                foreach (var emitter in _emitters)
                {
                    emitter.Update();
                }

                // Stop the update timer and add the value to the average list.
                updateTimer.Stop();
                pastUpdateTimes.Add(updateTimer.ElapsedTicks);
                if (pastUpdateTimes.Count > 100)
                    pastUpdateTimes.RemoveAt(0);
            //}
        }

        /// <summary>
        /// Draw all the alive particles without effects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawNormalParticles(SpriteBatch spriteBatch)
        {
            drawTimer.Restart();
            for (int i = 0; i < _aliveParticles.Count; i++)
            {
                _aliveParticles[i].Draw(spriteBatch);
            }

            // Stop draw timer and add its value to the average list.
            drawTimer.Stop();
            pastDrawTimes.Add(drawTimer.ElapsedTicks);
            if (pastDrawTimes.Count > 100)
                pastDrawTimes.RemoveAt(0);
        }

        /// <summary>
        /// Draw all the alive particles with effects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawEffectParticles(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _rippleAliveParticles.Count; i++)
            {
                _rippleAliveParticles[i].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Gets how many particles are currently in use by the system.
        /// </summary>
        /// <returns></returns>
        public static int GetInUseParticleCount() => _aliveParticles.Count + _rippleAliveParticles.Count;

        private static string textOfNextPar;
        public static void Add(string text, Vector2 position, Vector2 velocityArg, Color color)
        {
            textOfNextPar = text;
            Add(ParticleType.SplashText, position, velocityArg, color);
        }

        public static void Add(ParticleType type, Vector2 position, Vector2? velocityArg, Color color)
        {
            _tempParticle = GetNextParticle(type);
            _tempParticle.Reset();
            _tempParticle.CurrentParticleType = type;

            Vector2 velocity;
            if (velocityArg == null)
            {
                velocity = CalcHelper.GetRandXAndY(new Rectangle(-3, -3, 6, 6)) / 10;
            }
            else
            {
                velocity = (Vector2)velocityArg;
            }

            switch (_tempParticle.CurrentParticleType)
            {

                case ParticleType.Smoke:
                    _tempParticle.Position = new Vector2(position.X - 4, position.Y - TMBAW_Game.Random.Next(0, 80) / 10f);
                    _tempParticle.SourceRectangle = new Rectangle(256, 104, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(5, 20) / 10f;
                    _tempParticle.Position = new Vector2(_tempParticle.Position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, _tempParticle.Position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(200, 300);
                    _tempParticle._frames = 4;
                    _tempParticle.IsAnimated = true;
                    break;
                case ParticleType.Flame:
                    _tempParticle.Position = new Vector2(position.X - 4, position.Y - TMBAW_Game.Random.Next(0, 80) / 10f);
                    _tempParticle.SourceRectangle = new Rectangle(288, 96, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(5, 30) / 10f;
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(100, 200);
                    _tempParticle._frames = 4;
                    _tempParticle.IsAnimated = true;
                    break;
                case ParticleType.Round_Common:
                    _tempParticle.Position = new Vector2(position.X - 4, position.Y - 4);
                    _tempParticle.SourceRectangle = new Rectangle(288, 104, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(1, 10) / 10f;
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(100, 200);
                    _tempParticle._frames = 4;
                    _tempParticle.IsAnimated = true;
                    break;
                case ParticleType.Snow:
                    _tempParticle.Position = new Vector2(position.X - 4, position.Y - 4);
                    _tempParticle.SourceRectangle = new Rectangle(288, 104, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(1, 10) / 10f;
                    break;
                case ParticleType.Rain:
                    _tempParticle.Position = new Vector2(position.X - 4, position.Y - 4);
                    _tempParticle.SourceRectangle = new Rectangle(288, 104, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(1, 10) / 10f;
                    break;
                case ParticleType.HeatEffect:
                    _tempParticle.IsRippleEffect = true;
                    _tempParticle.SourceRectangle = new Rectangle(256, 144, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(1, 5);
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    break;
                case ParticleType.BlueFire:
                    _tempParticle.SourceRectangle = new Rectangle(256, 160, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(5, 10) / 10f;
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(100, 200);
                    _tempParticle._frames = 4;
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle.IsAnimated = true;
                    break;
                case ParticleType.BlueFireRibbon:
                    _tempParticle.SourceRectangle = new Rectangle(256, 152, 8, 8);
                    _tempParticle.Velocity = Vector2.Zero;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = 1;
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(20, 50);
                    _tempParticle._frames = 4;
                    _tempParticle.Position = new Vector2(position.X, position.Y);
                    _tempParticle.IsAnimated = true;
                    break;
                case ParticleType.BlueFireExplosion:
                    _tempParticle.SourceRectangle = new Rectangle(288, 168, 24, 24);
                    _tempParticle.Velocity = Vector2.Zero;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = 1;
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(20, 50);
                    _tempParticle._frames = 1;
                    _tempParticle.Position = new Vector2(position.X, position.Y);
                    _tempParticle.IsAnimated = false;
                    break;
                case ParticleType.FireBall:
                    _tempParticle.SourceRectangle = new Rectangle(256, 168, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(1, 3);
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(100, 200);
                    _tempParticle._frames = 4;
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle.IsAnimated = true;
                    _tempParticle.IsImmuneToTimeEffects = true;
                    break;
                case ParticleType.Tiny:
                    _tempParticle.SourceRectangle = new Rectangle(256, 112, 4, 4);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = 1;
                    _tempParticle._frameChange = TMBAW_Game.Random.Next(100, 500);
                    _tempParticle._frames = 4;
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle.IsAnimated = true;
                    _tempParticle.IsImmuneToTimeEffects = false;
                    break;
                case ParticleType.ProjectileHeatTrail:
                    _tempParticle.IsRippleEffect = true;
                    _tempParticle.SourceRectangle = new Rectangle(256, 144, 8, 8);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(5, 7);
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    break;
                case ParticleType.Explosion:
                    _tempParticle.SourceRectangle = new Rectangle(400, 80, 48, 48);
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = 1;
                    _tempParticle._frameChange = 50;
                    _tempParticle._frames = 4;
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle.IsAnimated = true;
                    _tempParticle.IsImmuneToTimeEffects = false;
                    break;
                case ParticleType.TilePiece:
                    velocity += CalcHelper.GetRandXAndY(new Rectangle(0, 0, 12, 12));
                    _tempParticle.SourceRectangle = new Rectangle((int)(velocity.X), (int)(velocity.Y), 4, 4);
                    _tempParticle.Velocity = CalcHelper.GetRandXAndY(new Rectangle(-100, -100, 200, 200)) / 10f;
                    _tempParticle.Color = color;
                    _tempParticle.Scale = TMBAW_Game.Random.Next(5, 10) / 10f;
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle.IsImmuneToTimeEffects = false;
                    break;
                case ParticleType.SplashText:
                    _tempParticle.Text = textOfNextPar;
                    _tempParticle.IsText = true;
                    _tempParticle.Velocity = velocity;
                    _tempParticle.Color = color;
                    _tempParticle.SourceRectangle.Height = 64;
                    double parsed;
                    double.TryParse(textOfNextPar, out parsed);
                    _tempParticle.SourceRectangle.Height *= (int)Math.Ceiling(parsed / 50);
                    _tempParticle.Position = new Vector2(position.X - (_tempParticle.Scale * _tempParticle.Width) / 2, position.Y - (_tempParticle.Scale * _tempParticle.Height) / 2);
                    _tempParticle.IsImmuneToTimeEffects = true;
                    break;
                default:
                    _tempParticle.SourceRectangle = new Rectangle(0, 0, 0, 0);
                    break;
            }
        }

        public static void Add(ParticleData data)
        {
            if (data.Text == null)
            {
                Add(data.Type, data.Position, data.Velocity, data.Color);
            }
            else
            {
                Add(data.Text, data.Position, data.Velocity, data.Color);
            }
        }

    }

    public enum ParticleType
    {
        Smoke,
        Flame,
        Speed,
        Round_Common,
        Snow,
        Rain,
        HeatEffect,
        BlueFire,
        BlueFireRibbon,
        BlueFireExplosion,
        FireBall,
        Tiny,
        ProjectileHeatTrail,
        Explosion,
        TilePiece,
        SplashText,
    }

}
