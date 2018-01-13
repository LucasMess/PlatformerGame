﻿using ThereMustBeAnotherWay.Levels;
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
    public partial class ParticleSystem
    {
        int _nextInt = 0;
        Particle par;

        IEnumerable<Particle> _emptyParticles;
        public static Stopwatch updateTimer = new Stopwatch();
        public static Stopwatch drawTimer = new Stopwatch();
        Thread updateThread;
        Thread updateTimeConstThread;
        public static AutoResetEvent UpdateStartEvent = new AutoResetEvent(true);
        public static AutoResetEvent UpdateStartEvent_TimeConstant = new AutoResetEvent(true);
        /// <summary>
        /// The next available index for creating a particle. Returns -1 if there are no more particles.
        /// </summary>
        int GetNextIndex()
        {

            _nextInt++;     
            if (_nextInt >= _emptyParticles.Count())
                _nextInt = -1;
            return _nextInt;

        }

        public int GetIteration() => _nextInt;

        readonly Particle[] _particles;
        const int MaxParticles = 10000;

        public ParticleSystem()
        {
            _particles = new Particle[MaxParticles];

            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new Particle();
            }

            GetAvailableParticles();


            VisibleParticles = from particle in _particles
                               where !particle.IsDead()
                               select particle;

            updateThread = new Thread(new ThreadStart(Update))
            {
                IsBackground = true,

            };

            updateTimeConstThread = new Thread(new ThreadStart(UpdateTimeConstant))
            {
                IsBackground = true,
            };

            updateThread.Start();
            updateTimeConstThread.Start();
        }

        public void Update()
        {
            while (true)
            {
                UpdateStartEvent.WaitOne();

                updateTimer.Restart();
                for (int i = 0; i < _particles.Length; i++)
                {
                    _particles[i].Update();
                }

                VisibleParticles = from particle in _particles
                                   where !particle.IsDead()
                                   select particle;

                foreach (var emitter in _emitters)
                {
                    emitter.Update();
                }

                updateTimer.Stop();
            }
        }

        public void DrawNormalParticles(SpriteBatch spriteBatch)
        {

            drawTimer.Restart();
            var query = from particle in VisibleParticles
                        where !particle.IsRippleEffect
                        select particle;

            foreach (Particle par in query)
            {
                par.Draw(spriteBatch);
            }
            drawTimer.Stop();
        }

        /// <summary>
        /// Used to update particles that do not stop with time freeze.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void UpdateTimeConstant()
        {
            while (true)
            {
                UpdateStartEvent_TimeConstant.WaitOne();
                var query = from particle in VisibleParticles
                            where particle.IsImmuneToTimeEffects
                            select particle;

                foreach (var item in query)
                {
                    item.Update();
                }
            }
        }

        IEnumerable<Particle> VisibleParticles;

        public void DrawEffectParticles(SpriteBatch spriteBatch)
        {
            var query = from particle in VisibleParticles
                        where particle.IsRippleEffect
                        select particle;


            foreach (Particle par in query)
            {
                par.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// Gets how many particles there are left before there are too many particles for the system to handle.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfAvailableParticles() => _emptyParticles.Count();

        /// <summary>
        /// Gets all of the available particles.
        /// </summary>
        private void GetAvailableParticles()
        {
            _emptyParticles = (from particle in _particles
                               where particle.IsDead()
                               select particle).ToList();
        }

        /// <summary>
        /// Used when there are no more available particles.
        /// </summary>
        private void GetAllParticles()
        {
            _emptyParticles = _particles.ToList();
        }

        string textOfNextPar;
        public void Add(string text, Vector2 position, Vector2 velocityArg, Color color)
        {
            textOfNextPar = text;
            Add(ParticleType.SplashText, position, velocityArg, color);
        }

        public void Add(ParticleType type, Vector2 position, Vector2? velocityArg, Color color)
        {
            int next = GetNextIndex();
            if (next == -1)
            {
                GetAvailableParticles();
                next = GetNextIndex();
                if (next == -1)
                {
                    GetAllParticles();
                    next = GetNextIndex();
                }
            }

            par = _emptyParticles.ElementAt(next);

            par.Reset();
            par.CurrentParticleType = type;

            Vector2 velocity;
            if (velocityArg == null)
            {
                velocity = CalcHelper.GetRandXAndY(new Rectangle(-3, -3, 6, 6)) / 10;
            }
            else
            {
                velocity = (Vector2)velocityArg;
            }

            switch (par.CurrentParticleType)
            {

                case ParticleType.Smoke:
                    par.Position = new Vector2(position.X - 4, position.Y - TMBAW_Game.Random.Next(0, 80) / 10f);
                    par.SourceRectangle = new Rectangle(256, 104, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(5, 20) / 10f;
                    par.Position = new Vector2(par.Position.X - (par.Scale * par.Width) / 2, par.Position.Y - (par.Scale * par.Height) / 2);
                    par._frameChange = TMBAW_Game.Random.Next(200, 300);
                    par._frames = 4;
                    par.IsAnimated = true;
                    break;
                case ParticleType.Flame:
                    par.Position = new Vector2(position.X - 4, position.Y - TMBAW_Game.Random.Next(0, 80) / 10f);
                    par.SourceRectangle = new Rectangle(288, 96, 8, 8);
                    par.Velocity = velocity;
                    par.Scale = TMBAW_Game.Random.Next(5, 30) / 10f;
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par._frameChange = TMBAW_Game.Random.Next(100, 200);
                    par._frames = 4;
                    par.IsAnimated = true;
                    break;
                case ParticleType.Round_Common:
                    par.Position = new Vector2(position.X - 4, position.Y - 4);
                    par.SourceRectangle = new Rectangle(288, 104, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(1, 10) / 10f;
                    par._frameChange = TMBAW_Game.Random.Next(100, 200);
                    par._frames = 4;
                    par.IsAnimated = true;
                    break;
                case ParticleType.Snow:
                    par.Position = new Vector2(position.X - 4, position.Y - 4);
                    par.SourceRectangle = new Rectangle(288, 104, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(1, 10) / 10f;
                    break;
                case ParticleType.Rain:
                    par.Position = new Vector2(position.X - 4, position.Y - 4);
                    par.SourceRectangle = new Rectangle(288, 104, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(1, 10) / 10f;
                    break;
                case ParticleType.HeatEffect:
                    par.IsRippleEffect = true;
                    par.SourceRectangle = new Rectangle(256, 144, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(1, 5);
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    break;
                case ParticleType.RewindFire:
                    par.SourceRectangle = new Rectangle(256, 160, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(5, 10) / 10f;
                    par._frameChange = TMBAW_Game.Random.Next(100, 200);
                    par._frames = 4;
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par.IsAnimated = true;
                    par.IsImmuneToTimeEffects = true;
                    break;
                case ParticleType.RewindFireRibbon:
                    par.SourceRectangle = new Rectangle(256, 152, 8, 8);
                    par.Velocity = Vector2.Zero;
                    par.Color = color;
                    par.Scale = 1;
                    par._frameChange = TMBAW_Game.Random.Next(20, 50);
                    par._frames = 4;
                    par.Position = new Vector2(position.X, position.Y);
                    par.IsAnimated = true;
                    par.IsImmuneToTimeEffects = true;
                    break;
                case ParticleType.RewindFireExplosion:
                    par.SourceRectangle = new Rectangle(288, 168, 24, 24);
                    par.Velocity = Vector2.Zero;
                    par.Color = color;
                    par.Scale = 1;
                    par._frameChange = TMBAW_Game.Random.Next(20, 50);
                    par._frames = 1;
                    par.Position = new Vector2(position.X, position.Y);
                    par.IsAnimated = false;
                    par.IsImmuneToTimeEffects = true;
                    break;
                case ParticleType.FireBall:
                    par.SourceRectangle = new Rectangle(256, 168, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(1, 3);
                    par._frameChange = TMBAW_Game.Random.Next(100, 200);
                    par._frames = 4;
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par.IsAnimated = true;
                    par.IsImmuneToTimeEffects = true;
                    break;
                case ParticleType.Tiny:
                    par.SourceRectangle = new Rectangle(256, 112, 4, 4);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = 1;
                    par._frameChange = TMBAW_Game.Random.Next(100, 500);
                    par._frames = 4;
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par.IsAnimated = true;
                    par.IsImmuneToTimeEffects = false;
                    break;
                case ParticleType.ProjectileHeatTrail:
                    par.IsRippleEffect = true;
                    par.SourceRectangle = new Rectangle(256, 144, 8, 8);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(5, 7);
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    break;
                case ParticleType.Explosion:
                    par.SourceRectangle = new Rectangle(400, 80, 48, 48);
                    par.Velocity = velocity;
                    par.Color = color;
                    par.Scale = 1;
                    par._frameChange = 50;
                    par._frames = 4;
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par.IsAnimated = true;
                    par.IsImmuneToTimeEffects = false;
                    break;
                case ParticleType.TilePiece:
                    velocity += CalcHelper.GetRandXAndY(new Rectangle(0, 0, 12, 12));
                    par.SourceRectangle = new Rectangle((int)(velocity.X), (int)(velocity.Y), 4, 4);
                    par.Velocity = CalcHelper.GetRandXAndY(new Rectangle(-100, -100, 200, 200))/ 10f;
                    par.Color = color;
                    par.Scale = TMBAW_Game.Random.Next(5, 10) / 10f;
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par.IsImmuneToTimeEffects = false;
                    break;
                case ParticleType.SplashText:
                    par.Text = textOfNextPar;
                    par.IsText = true;
                    par.Velocity = velocity;
                    par.Color = color;
                    par.SourceRectangle.Height = 64;
                    double parsed;
                    double.TryParse(textOfNextPar, out parsed);
                    par.SourceRectangle.Height *= (int)Math.Ceiling(parsed / 50);
                    par.Position = new Vector2(position.X - (par.Scale * par.Width) / 2, position.Y - (par.Scale * par.Height) / 2);
                    par.IsImmuneToTimeEffects = true;
                    break;
                default:
                    par.SourceRectangle = new Rectangle(0, 0, 0, 0);
                    break;
            }
        }

        public void Add(ParticleData data)
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
        RewindFire,
        RewindFireRibbon,
        RewindFireExplosion,
        FireBall,
        Tiny,
        ProjectileHeatTrail,
        Explosion,
        TilePiece,
        SplashText,
    }

}