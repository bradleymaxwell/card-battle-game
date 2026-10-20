using Actions.EnergySap;
using UnityEngine;

namespace Units.EnergySap
{
    [CreateAssetMenu(menuName = "Game Config/Action/Energy Sap")]
    public class EnergySapActionConfig : ActionConfig, IRangedAction
    {
        [SerializeField] private int sapAmount;
        public int SapAmount => sapAmount;

        [SerializeField] private float perfectHitPercentEnergyThreshold;
        public float PerfectHitEnergyThreshold => perfectHitPercentEnergyThreshold;

        [SerializeField] private int perfectBonusSnapAmount;
        public int PerfectBonusSnapAmount => perfectBonusSnapAmount;      
        
        [SerializeField] private int range;
        public int Range => range;
        
        public override IAction Action => new EnergySapAction(this);
    }
}