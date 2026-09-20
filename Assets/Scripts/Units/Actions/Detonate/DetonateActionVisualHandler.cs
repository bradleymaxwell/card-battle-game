using System;
using System.Collections;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.Detonate
{
    public class DetonateActionVisualHandler : IActionVisualHandler
    {
        private readonly DetonateActionVisualConfig _config;
        private readonly UnitViewManager _unitViewManager;
        private readonly PoolService _poolService;
        
        public DetonateActionVisualHandler(DetonateActionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _poolService = Locator.Get<PoolService>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
            unitPrefab.SetMaterial(_config.DetonateMaterial);
            yield return new WaitForSeconds(1f);
            var explosionVfx = _poolService.Get(_config.ExplosionVfx);
            explosionVfx.transform.position = new Vector3(unitPrefab.transform.position.x, 0.1f, unitPrefab.transform.position.z);
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