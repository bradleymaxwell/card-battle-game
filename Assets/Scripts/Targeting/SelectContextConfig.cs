using Battles;

namespace Targeting
{
    public class SelectContextConfig
    {
        public int RequiredAmount { get; set; } = 1;
        public TeamType Team { get; set; }
        public bool AutoConfirm { get; set; } = true;
        public bool IsOverriding { get; set; } = true;
        public bool AllowDuplicates { get; set; } = false;
    }
}