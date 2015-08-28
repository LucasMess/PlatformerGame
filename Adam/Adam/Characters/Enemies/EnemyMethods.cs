using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;

namespace Adam.Characters.Enemies
{
    public abstract partial class NewEnemy : Entity
    {
        public override void Update()
        {
            hitByPlayerTimer.Increment();
            wasMeanTimer.Increment();
            PlayMeanSound();

            base.Update();
        }

        /// <summary>
        /// Deals a certain amount of damage to the player.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            Health -= damage;
            hitByPlayerTimer.Reset();
        }

        /// <summary>
        /// The color that the enemy will be drawn in.
        /// </summary>
        public override Color Color
        {
            get
            {
                //If the enemy was recently hit, its color is changed to red.
                if (HasTakenDamageRecently())
                    Color = Color.Red;
                else Color = Color.White;

                return base.Color;
            }

            set
            {
                base.Color = value;
            }
        }

        /// <summary>
        /// Checks to see if it is time to be mean again.
        /// </summary>
        private void PlayMeanSound()
        {
            if (IsTimeToBeMean())
                MeanSound.PlayIfStopped();
        }



    }
}
