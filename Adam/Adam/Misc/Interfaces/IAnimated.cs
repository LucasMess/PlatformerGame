using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Interfaces
{
    public enum AnimationState
    {
        Still,
        Walking,
        Jumping,
        Charging,
        Talking,
        Sleeping,
        Flying,
    }

    public interface IAnimated
    {
        Animation Animation { get; }
        AnimationData[] AnimationData { get; }
        AnimationState CurrentAnimationState { get; set; }

        void Animate();
    }
}
