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
            Weight = 3;
            script = new FrogScript();
            script.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/frog");
            collRectangle = new Rectangle(x, y, 32, 32);            
            sourceRectangle = new Rectangle(0, 0, 24, 32);

            complexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(4, 16, 24, 32), 0, 24, 32, 125, 4, true));
            complexAnim.AddAnimationData("jump", new ComplexAnimData(100, Texture, new Rectangle(4, 16, 24, 32), 32, 24, 32, 125, 4, false));

            Sounds.AddSoundRef("jump", "Sounds/Frog/frog_jump");
        }

        private Rectangle _respawnRect;
        public override Rectangle RespawnLocation
        {
            get
            {
                if (_respawnRect == new Rectangle(0, 0, 0, 0))
                {
                    _respawnRect = collRectangle;
                }
                return _respawnRect;
            }
        }


        public override void Update()
        {
            if (IsDead()) return;

            base.Update();
        }

        public override byte ID
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
                return EnemyDB.Frog_MaxHealth;
            }
        }

        SoundFx meanSound;
        protected override SoundFx MeanSound
        {
            get
            {
                if (meanSound == null)
                    meanSound = new SoundFx("Sounds/Frog/frog_croak");
                return meanSound;
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
                return new Rectangle(collRectangle.X - 8, collRectangle.Y - 32, 48, 64);
            }
        }
    }
}
