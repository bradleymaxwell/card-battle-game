using System;
using System.Collections;
using Units.Actions;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.KissOfDeath
{
    public class KissOfDeathActionVisualHandler : IActionVisualHandler
    {
        private readonly KissOfDeathActionVisualConfig _config;
        private readonly UnitViewManager _unitViewManager;
        private readonly PoolService _poolService;
        private readonly Logger _logger = new(nameof(KissOfDeathActionVisualHandler));
        private bool _isKissed;
        
        public KissOfDeathActionVisualHandler(KissOfDeathActionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _poolService = Locator.Get<PoolService>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            if (result is not AoEActionPerformResult kissResult)
            {
                _logger.LogError($"in order to play the kiss of death action visuals, the result must be of type: {typeof(AoEActionPerformResult)}");
                yield break;
            }

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
            
            // refresh the health bar of all the hit units
            foreach (var unit in kissResult.UnitsHit)
            {
                var (_, hitUnitResourceBar) = _unitViewManager.GetUnitViews(unit);
                if (unit.CurrentHealth <= 0)
                {
                    _unitViewManager.OnUnitDefeated(unit);
                }
                else
                {
                    hitUnitResourceBar.RefreshHealth();
                }
            }

            yield return new WaitWhile(() => explosionVfx.ParticleSystem.IsAlive(true));
        }
    }
}