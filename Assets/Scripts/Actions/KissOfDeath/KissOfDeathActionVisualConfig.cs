using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Units.KissOfDeath
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Kiss Of Death")]
    public class KissOfDeathActionVisualConfig : ActionVisualConfig
    {
        [SerializeField] private PooledParticleSystem explosionVfx;
        public PooledParticleSystem ExplosionVfx => explosionVfx;
        
        public override IActionVisualHandler Handler => new KissOfDeathActionVisualHandler(this);
    }
}