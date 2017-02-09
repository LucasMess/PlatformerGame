namespace Adam
{
    public class Behavior
    {
        protected Entity Entity;

        public virtual void Initialize(Entity entity)
        {
            this.Entity = entity;
        }

        public virtual void Update(Entity entity)
        {

        }

        public void Run()
        {
            Entity = Entity.Get();
        }

    }
}