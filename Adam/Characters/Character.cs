using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Network;

namespace ThereMustBeAnotherWay.Characters
{
    public abstract class Character : Entity
    {
        protected Behavior Behavior;

        public override void Update()
        {
            if (Session.IsHost && !StoryTracker.InCutscene)
                Behavior?.Update(this);
            base.Update();
        }

    }
}
