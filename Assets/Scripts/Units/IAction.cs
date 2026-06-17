namespace Units
{
    public interface IAction
    {
        bool CanPerform();
        void OnPerform();
    }
}