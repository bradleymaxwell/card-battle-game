using Projectiles;
using Units.Actions.VisualPlayback;
using Units.PiercingProjectile;
using Units.VisualPlayback;
using UnityEngine;

namespace Actions.PiercingProjectile
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Piercing Projectile")]
    public class PiercingProjectileActionVisualConfig : ActionVisualConfig
    {
        [SerializeField] private Projectile projectilePrefab;
        public Projectile ProjectilePrefab => projectilePrefab;
        
        [SerializeField] private float speed = 10f;
        public float Speed => speed;
        
        public override IActionVisualHandler Handler => new PiercingProjectileActionVisualHandler(this);
    }
}