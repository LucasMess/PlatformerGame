using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Interfaces
{
    interface IAnimated
    {
        Animation Animation { get; set; }
        AnimationData[] AnimationData { get; set; }

        void Animate();
    }
}
