using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Characters.Scripts
{
    class SnakeScript : Behavior
    {

        public override void Initialize(Entity entity)
        {
            entity.AddAnimationToQueue("still");


            base.Initialize(entity);
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
        }

    }
}
