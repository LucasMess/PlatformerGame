using Adam.Network;

namespace Adam.Characters
{
    public abstract class Character : Entity
    {
        protected Behavior Behavior;

        public override void Update()
        {
            if (Session.IsHost)
                Behavior?.Update(this);
            base.Update();
        }

    }
}
