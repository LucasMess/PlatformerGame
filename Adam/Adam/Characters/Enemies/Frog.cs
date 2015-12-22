using Adam.Characters.Scripts;
using Adam.Misc;
using Adam.Misc.Interfaces;
using Adam.Misc.Sound;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    public class Frog : Enemy
    {
        public Frog(int x, int y)
        {
            Weight = 10;
            Script = new FrogScript();
            Script.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/frog");
            CollRectangle = new Rectangle(x, y, 32, 32);            
            SourceRectangle = new Rectangle(0, 0, 24, 32);

            ComplexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(4, 16, 24, 32), 0, 24, 32, 125, 4, true));
            ComplexAnim.AddAnimationData("jump", new ComplexAnimData(100, Texture, new Rectangle(4, 16, 24, 32), 32, 24, 32, 50, 4, false));

            Sounds.AddSoundRef("jump", "Sounds/Frog/frog_jump");
        }

        private Rectangle _respawnRect;
        public override Rectangle RespawnLocation
        {
            get
            {
                if (_respawnRect == new Rectangle(0, 0, 0, 0))
                {
                    _respawnRect = CollRectangle;
                }
                return _respawnRect;
            }
        }

        public override byte Id
        {
            get
            {
                return 202;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDb.FrogMaxHealth;
            }
        }

        SoundFx _meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                if (_meanSound == null)
                    _meanSound = new SoundFx("Sounds/Frog/frog_croak");
                return _meanSound;
            }
        }

        protected override SoundFx AttackSound
        {
            get
            {
                return null;
            }
        }

        protected override SoundFx DeathSound
        {
            get
            {
                return null;
            }
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X - 8, CollRectangle.Y - 32, 48, 64);
            }
        }
    }
}
