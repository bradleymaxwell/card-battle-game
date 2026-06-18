using Map;

namespace Units
{
    public interface IAction
    {
        bool CanPerform(MapSpace userSpace, MapSpace targetSpace);
        void OnPerform(MapSpace userSpace, MapSpace targetSpace);
    }
}