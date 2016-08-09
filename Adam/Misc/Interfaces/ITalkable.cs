namespace Adam.Misc.Interfaces
{
    public interface ITalkable
    {
        int StartingConversation { get; set; }
        int CurrentConversation { get; set; }
        bool EndConversation { get; set; }

        void OnNextDialog();
    }

}
