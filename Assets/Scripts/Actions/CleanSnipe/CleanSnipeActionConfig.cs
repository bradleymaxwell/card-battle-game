using UnityEngine;

namespace Units.CleanSnipe
{
    [CreateAssetMenu(menuName = "Game Config/Action/Clean Snipe")]
    public class CleanSnipeActionConfig : ActionConfig
    {
        [SerializeField] private int perfectHitTravelThreshold;
        public int PerfectHitTravelThreshold => perfectHitTravelThreshold;

        [SerializeField] private float baseDamagePercent;
        public float BaseDamagePercent => baseDamagePercent;

        [SerializeField] private float percentIncreasePerSpace;
        public float PercentIncreasePerSpace => percentIncreasePerSpace;       
        
        public override IAction Action => new CleanSnipeAction(this);
    }
}