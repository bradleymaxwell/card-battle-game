using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Units.MotherOfAllLoveBombs
{
    [CreateAssetMenu(fileName = "MotherOfAllLoveBombsActionVisualConfig", menuName = "Game Config/Action/Visual/Mother of all Love Bombs")]
    public class MotherOfAllLoveBombsActionVisualConfig : ActionVisualConfig
    {
        [SerializeField] private PooledParticleSystem explosionVfx;
        public PooledParticleSystem ExplosionVfx => explosionVfx;
        
        [SerializeField] private Material detonateMaterial;
        public Material DetonateMaterial => detonateMaterial;       
        
        public override IActionVisualHandler Handler => new MotherOfAllLoveBombsVisualHandler(this);
    }
}