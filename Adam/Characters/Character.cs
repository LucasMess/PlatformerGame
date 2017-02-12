using Adam.Network;

namespace Adam.Characters
{
    public abstract class Character : Entity
    {
        protected Behavior Script;

        public override void Update()
        {
            Script?.Update(this);
            base.Update();
        }

    }
}
