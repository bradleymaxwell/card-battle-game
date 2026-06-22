using Battles;

namespace Targeting
{
    public class SelectContextConfig
    {
        public int RequiredAmount { get; }
        public TeamType Team { get; }
        public bool AutoConfirm { get; }
        public bool IsOverriding { get; }
        public bool AllowDuplicates { get; }
        public SelectContextType Type { get; }
        
        public SelectContextConfig(
            TeamType team, 
            SelectContextType type,
            int requiredAmount = 1, 
            bool autoConfirm = true, 
            bool isOverriding = true, 
            bool allowDuplicates = false)
        {
            Team = team;
            Type = type;
            RequiredAmount = requiredAmount;
            AutoConfirm = autoConfirm;
            IsOverriding = isOverriding;
            AllowDuplicates = allowDuplicates;
        }
    }
}