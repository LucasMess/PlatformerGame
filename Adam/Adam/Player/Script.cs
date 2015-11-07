namespace Adam
{
    public class Script
    {
        protected Entity entity;

        public virtual void Initialize(Entity entity)
        {
            this.entity = entity;
        }

        public void Run()
        {
            OnGameTick();
        }

        protected virtual void OnGameTick()
        {
            entity = entity.Get();
        }

    }
}