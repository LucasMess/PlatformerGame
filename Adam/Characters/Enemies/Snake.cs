using Adam.Characters.Scripts;
using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using System;

namespace Adam.Characters.Enemies
{
    class Snake : Enemy
    {

        public Snake(int x, int y)
        {
            Weight = 10;
            Script = new SnakeScript();
            Script.Initialize(this);

            Texture = ContentHelper.LoadTexture("Enemies/snake");
            SetPosition(new Vector2(x, y));
            CollRectangle = new Rectangle(0, 0, 32 * 2, 48 * 2);
            SourceRectangle = new Rectangle(0, 0, 32, 48);

            ComplexAnim.AddAnimationData("still", new ComplexAnimData(1, Texture, new Rectangle(0, 0, 32, 48), 0, 32, 48, 125, 8, true));

            Sounds.AddSoundRef("idle", "Sounds/Snake/snake_idle");
            Sounds.AddSoundRef("spit", "Sounds/Snake/snake_spit");
            Sounds.AddSoundRef("death", "Sounds/Snake/snake_death");
        }

        public override byte Id
        {
            get
            {
                return 201;
            }
        }

        public override int MaxHealth
        {
            get
            {
                return EnemyDb.SnakeMaxHealth;
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
                return Sounds.GetSoundRef("death");
            }
        }

        protected override Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle(CollRectangle.X, CollRectangle.Y, 32 * 2, 48 * 2);
            }
        }

        protected override SoundFx MeanSound
        {
            get
            {
                return Sounds.GetSoundRef("idle");
            }
        }
    }
}
