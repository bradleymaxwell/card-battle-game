using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions.PiercingProjectile;
using Map;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.PiercingProjectile
{
    public class PiercingProjectileActionVisualHandler : IActionVisualHandler
    {
        private readonly PiercingProjectileActionVisualConfig _config;
        private readonly UnitViewManager _unitViewManager;
        private readonly MapService _mapService;
        private readonly PoolService _poolService;
        private bool _isHit;
        private bool _isProjectileStopped;
        
        public PiercingProjectileActionVisualHandler(PiercingProjectileActionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _mapService = Locator.Get<MapService>();
            _poolService = Locator.Get<PoolService>();
        }

        public IEnumerator PlayCor(ActionPerformResult result)
        {
            _isHit = false;
            _isProjectileStopped = false;
            var (userModel, _) = _unitViewManager.GetUnitViews(result.Performer);
            var targetSpacePrefab = _mapService.GetPrefab(result.ProjectileTravelPath.Last());
            userModel.Face(targetSpacePrefab);
            userModel.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isHit = true);
            userModel.Animator.SetTrigger(AnimationConstants.Attack);
            yield return new WaitUntil(() => _isHit);
            var hitUnits = new HashSet<UnitPrefab>();
            var projectile = _poolService.Get(_config.ProjectilePrefab);
            projectile.Fire(
                userModel.ProjectileSource.position, 
                targetSpacePrefab,
                _config.Speed,
                hitUnit =>
                {
                    if (!hitUnits.Contains(hitUnit) && result.HealthAdjustmentByUnit.TryGetValue(hitUnit.Unit, out var adjustment))
                    {
                        hitUnits.Add(hitUnit);
                        _unitViewManager.OnHealthAdjusted(hitUnit.Unit, adjustment);
                    }
                },
                () =>
                {
                    _poolService.Return(projectile);
                    _isProjectileStopped = true;
                });
            
            yield return new WaitUntil(() => _isProjectileStopped);
        }
    }
}