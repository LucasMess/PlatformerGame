using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters
{
    public abstract class Character : Entity
    {
        protected Script script;

        public override void Update()
        {
            script?.Run();
            base.Update();
        }

    }
}
