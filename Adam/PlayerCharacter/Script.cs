namespace Adam
{
    public class Behavior
    {
        protected Entity Entity;

        public virtual void Initialize(Entity entity)
        {
            this.Entity = entity;
        }

        public void Run()
        {
            OnGameTick();
        }

        protected virtual void OnGameTick()
        {
            Entity = Entity.Get();
        }

    }
}