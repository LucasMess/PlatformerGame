using Adam.Network;

namespace Adam.Characters
{
    public abstract class Character : Entity
    {
        protected Behavior Script;

        public override void Update()
        {
            if (Session.IsActive && Session.IsHost && AdamGame.CurrentGameMode == GameMode.Play)
                Script?.Run();
            base.Update();
        }

    }
}
