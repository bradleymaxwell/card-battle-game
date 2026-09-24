using System.Collections.Generic;
using UnityEngine;

namespace Units.Cleave
{
    [CreateAssetMenu(menuName = "Game Config/Action/Cleave")]
    public class CleaveActionConfig : ActionConfig, ICleaveActionConfigProvider
    {
        [SerializeField] private int baseDamage;
        public int BaseDamage => baseDamage;
        
        [SerializeField] private List<float> damagePercentIncreasePerHit;
        public IList<float> DamagePercentIncreasePerHit => damagePercentIncreasePerHit;
        
        [SerializeField] private int perfectHitThreshold;
        public int PerfectHitThreshold => perfectHitThreshold;
        
        [SerializeField] private float perfectHitDamagePercentIncrease;
        public float PerfectHitDamagePercentIncrease => perfectHitDamagePercentIncrease;       
        
        public override IAction Action => new CleaveAction(this);
    }
}