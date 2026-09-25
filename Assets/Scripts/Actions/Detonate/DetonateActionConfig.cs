using UnityEngine;

namespace Units.KissOfDeath
{
    [ CreateAssetMenu(menuName = "Game Config/Action/Detonate")]
    public class DetonateActionConfig : ActionConfig, IRangedAction, IActionConfigProvider
    {
        [SerializeField] private int radius;
        public int Radius => radius;
        
        [SerializeField] private int damage;
        public int Damage => damage;
        
        [SerializeField] private int range;
        public int Range => range;
        
        public override IAction Action => new DetonateAction(this);
    }
}