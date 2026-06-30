using UnityEngine;

namespace Units.ToxicLovePotion
{
    [CreateAssetMenu(menuName = "Game Config/Action/Toxic Love Potion")]
    public class ToxicLovePotionActionConfig : ActionConfig
    {
        [SerializeField] private int radius;
        public int Radius => radius;

        [SerializeField] private int damage;
        public int Damage => damage;
        
        public override IAction Action => new ToxicLovePotionAction(this);
    }
}