using Adam.Levels;
using Adam.Network;

namespace Adam.Characters
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
