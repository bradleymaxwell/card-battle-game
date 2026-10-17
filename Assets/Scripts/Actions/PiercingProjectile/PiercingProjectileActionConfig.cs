using UnityEngine;

namespace Units.PiercingProjectile
{
    [CreateAssetMenu(menuName = "Game Config/Action/Piercing Projectile")]
    public class PiercingProjectileActionConfig : ActionConfig
    {
        [SerializeField] private float damagePercent;
        public float DamagePercent => damagePercent;

        [SerializeField] private int travelDistance;
        public int TravelDistance => travelDistance;

        [SerializeField] private int perfectHitThreshold;
        public int PerfectHitThreshold => perfectHitThreshold;      
        
        public override IAction Action => new PiercingProjectileAction(this);
    }
}