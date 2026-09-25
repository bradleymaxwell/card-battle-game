using System;
using System.Collections;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.KissOfDeath
{
    public class KissOfDeathActionVisualHandler : IActionVisualHandler
    {
        private readonly KissOfDeathActionVisualConfig _config;
        private readonly UnitViewManager _unitViewManager;
        private readonly PoolService _poolService;
        private bool _isKissed;
        
        public KissOfDeathActionVisualHandler(KissOfDeathActionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _poolService = Locator.Get<PoolService>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            // face the target and kiss them
            var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
            var (targetPrefab, _) = _unitViewManager.GetUnitViews(result.Target);
            unitPrefab.Face(targetPrefab);
            
            unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isKissed = true);
            unitPrefab.Animator.SetTrigger(AnimationConstants.Attack);
            yield return new WaitUntil(() => _isKissed);
            
            // when the kiss lands, remove the unit prefab and emit particles
            var explosionVfx = _poolService.Get(_config.ExplosionVfx);
            
            // hardcoded fix to make sure the explosion occurs vertically roughly where the user stands
            explosionVfx.transform.position = new Vector3(unitPrefab.transform.position.x, 0.5f, unitPrefab.transform.position.z);
            _unitViewManager.OnUnitDefeated(result.Performer);
            explosionVfx.ParticleSystem.Play(true);
            
            foreach (var (unit, adjustment) in result.HealthAdjustmentByUnit)
            {
                _unitViewManager.OnHealthAdjusted(unit, adjustment);
            }

            yield return new WaitWhile(() => explosionVfx.ParticleSystem.IsAlive(true));
        }
    }
}