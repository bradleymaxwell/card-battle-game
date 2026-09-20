using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Units.Detonate
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Detonate")]
    public class DetonateActionVisualConfig : ActionVisualConfig
    {
        [SerializeField] private PooledParticleSystem explosionVfx;
        public PooledParticleSystem ExplosionVfx => explosionVfx;

        [SerializeField] private Material detonateMaterial;
        public Material DetonateMaterial => detonateMaterial;       
        
        public override IActionVisualHandler Handler => new DetonateActionVisualHandler(this);
    }
}