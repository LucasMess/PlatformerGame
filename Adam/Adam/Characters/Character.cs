using Adam.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters
{
    public abstract class Character : Entity
    {
        protected Script Script;

        public override void Update()
        {
            if (Session.IsActive && Session.IsHost)
                Script?.Run();
            base.Update();
        }

    }
}
