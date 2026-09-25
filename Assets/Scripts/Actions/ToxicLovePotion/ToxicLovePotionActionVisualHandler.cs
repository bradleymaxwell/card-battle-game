using System;
using System.Collections;
using Map;
using Units.Actions;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.ToxicLovePotion
{
    public class ToxicLovePotionActionVisualHandler : IActionVisualHandler
    {
        private readonly ToxicLovePotionVisualConfig _config;
        private readonly UnitViewManager _unitViewManager;
        private readonly MapService _mapService;
        private readonly PoolService _poolService;
        private readonly Logger _logger = new(nameof(ToxicLovePotionActionVisualHandler));
        private bool _isFinished;
        
        public ToxicLovePotionActionVisualHandler(ToxicLovePotionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _mapService = Locator.Get<MapService>();
            _poolService = Locator.Get<PoolService>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            var space = _mapService.GetPrefab(result.TargetSpace);
            var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
            unitPrefab.Face(space);
            
            unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnFinish, () => _isFinished = true);
            unitPrefab.Animator.SetTrigger(AnimationConstants.Throw);
            yield return new WaitUntil(() => _isFinished);
            
            var explosionVfx = _poolService.Get(_config.ExplosionVfx);
            explosionVfx.transform.position = new Vector3(space.transform.position.x, 0.1f, space.transform.position.z);
            explosionVfx.ParticleSystem.Play(true);
            foreach (var (unit, adjustment) in result.HealthAdjustmentByUnit)
            {
                _unitViewManager.OnHealthAdjusted(unit, adjustment);
            }
            
            yield return new WaitWhile(() => explosionVfx.ParticleSystem.IsAlive(true));
        }
    }
}