using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Units.ToxicLovePotion
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Toxic Love Potion")]
    public class ToxicLovePotionVisualConfig : ActionVisualConfig
    {
        [SerializeField] private PooledParticleSystem explosionVfx;
        public PooledParticleSystem ExplosionVfx => explosionVfx;
        
        public override IActionVisualHandler Handler => new ToxicLovePotionActionVisualHandler(this);
    }
}