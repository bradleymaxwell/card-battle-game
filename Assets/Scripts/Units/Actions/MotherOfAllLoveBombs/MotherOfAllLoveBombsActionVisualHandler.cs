using System;
using System.Collections;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.MotherOfAllLoveBombs
{
    public class MotherOfAllLoveBombsVisualHandler : IActionVisualHandler
    {
        private readonly MotherOfAllLoveBombsActionVisualConfig _config;
        private readonly Logger _logger = new(nameof(MotherOfAllLoveBombsVisualHandler));
        private readonly UnitViewManager _unitViewManager;
        private readonly PoolService _poolService;
        private bool _isCastFinished;
        private VisualPlaybackManager _visualPlaybackManager;
        
        public MotherOfAllLoveBombsVisualHandler(MotherOfAllLoveBombsActionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _poolService = Locator.Get<PoolService>();
            _visualPlaybackManager = Locator.Get<VisualPlaybackManager>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            if (result is not MotherOfAllLoveBombsActionResult moalbResult)
            {
                _logger.LogError($"cannot play visual playback because type of result needs to be: {nameof(MotherOfAllLoveBombsActionResult)}");
                yield break;
            }
            
            // play the cast animation
            _isCastFinished = false;
            var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
            unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isCastFinished = true);
            unitPrefab.Animator.SetTrigger(AnimationConstants.Cast);
            yield return new WaitUntil(() => _isCastFinished);
            
            // on cast complete, play the explosion animation
            var explosionVfx = _poolService.Get(_config.ExplosionVfx);
            explosionVfx.transform.position = new Vector3(0, 0.1f, 0);
            explosionVfx.ParticleSystem.Play(true);
            foreach (var (unit, adjustment) in moalbResult.HealthAdjustmentByUnit)
            {
                _unitViewManager.OnHealthAdjusted(unit, adjustment);
            }
            
            foreach (var detonationResult in moalbResult.DetonationResults)
            {
                var (detonatedUnitPrefab, _) = _unitViewManager.GetUnitViews(detonationResult.Performer);
                detonatedUnitPrefab.SetMaterial(_config.DetonateMaterial);
            }
            
            yield return new WaitUntil(() => !explosionVfx.ParticleSystem.IsAlive(true));
            
            foreach (var detonationResult in moalbResult.DetonationResults)
            {
                yield return _visualPlaybackManager.PlayActionCor(detonationResult);
            }
        }
    }
}