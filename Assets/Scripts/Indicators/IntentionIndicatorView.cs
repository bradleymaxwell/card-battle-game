using AI;

namespace DefaultNamespace.Indicators
{
    public class IntentionIndicatorView : IndicatorView
    {
        private UnitTurnIntention _intention;
        
        public void Bind(UnitTurnIntention intention)
        {
            _intention = intention;
        }
        
        public override string GetTooltip()
        {
            return _intention.Description;
        }
    }
}