using System;

namespace AI
{
    public class UnitTurnIntention
    {
        public Action OnExecute { get; set; }
        public string Description { get; set; }
    }
}