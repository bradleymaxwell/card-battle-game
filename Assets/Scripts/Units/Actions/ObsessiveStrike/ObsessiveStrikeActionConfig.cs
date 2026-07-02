using System.Collections.Generic;
using UnityEngine;

namespace Units.ObsessiveStrike
{
    [CreateAssetMenu(menuName = "Game Config/Action/Obsessive Strike", fileName =  "ObsessiveStrikeActionConfig")]
    public class ObsessiveStrikeActionConfig : ActionConfig, IRangedAction
    {
        [SerializeField] private List<float> percentIncreasePerStack;
        public List<float> PercentIncreasePerStack => percentIncreasePerStack;

        [SerializeField] private int baseDamage;
        public int BaseDamage => baseDamage;

        [SerializeField] private int range;
        public int Range => range;       
        
        public override IAction Action => new ObsessiveStrikeAction(this);
    }
}