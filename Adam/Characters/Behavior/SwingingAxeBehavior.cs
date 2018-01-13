using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Misc;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    class SwingingAxeBehavior : Behavior
    {
        const float MaxHeight = (float)(Math.PI - Math.PI/8)/2;
        const float Period = 2000;
        GameTimer timer = new GameTimer();

        public SwingingAxeBehavior()
        {
        }

        public override void Update(Entity entity)
        {
            timer.Increment();
            SwingingAxe axe = entity as SwingingAxe;
            axe.Rotation = MaxHeight * (float)Math.Cos(timer.TimeElapsedInMilliSeconds/Period * 2 * Math.PI);

            base.Update(entity);
        }
    }
}
