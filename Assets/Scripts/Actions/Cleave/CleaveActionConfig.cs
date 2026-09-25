using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Cleave
{
    [CreateAssetMenu(menuName = "Game Config/Action/Cleave")]
    public class CleaveActionConfig : ActionConfig, ICleaveActionConfigProvider
    {
        [FormerlySerializedAs("damagePercentIncreasePerHit")] [SerializeField] private List<float> percentDamagePerHit;
        public IList<float> PercentDamagePerHit => percentDamagePerHit;
        
        [SerializeField] private int perfectHitThreshold;
        public int PerfectHitThreshold => perfectHitThreshold;
        
        [SerializeField] private float perfectHitDamagePercentIncrease;
        public float PerfectHitDamagePercentIncrease => perfectHitDamagePercentIncrease;       
        
        public override IAction Action => new CleaveAction(this);
    }
}