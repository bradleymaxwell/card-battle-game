using System;
using System.Collections;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units
{
    public class BasicAttackActionVisualHandler : IActionVisualHandler
    {
        private readonly UnitSpawner _unitSpawner;
        private bool _isHitLanded;
        
        public BasicAttackActionVisualHandler()
        {
            _unitSpawner = Locator.Get<UnitSpawner>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            var (unitPrefab, _) = _unitSpawner.GetUnitViews(result.Performer);
            var (targetPrefab, targetResourceBar) = _unitSpawner.GetUnitViews(result.Target);
            
            unitPrefab.Face(targetPrefab);
            unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isHitLanded = true);
            unitPrefab.Animator.SetTrigger(AnimationConstants.Attack);
            yield return new WaitUntil(() => _isHitLanded);
            if (result.Target.CurrentHealth <= 0)
            {
                _unitSpawner.OnUnitDefeated(result.Target);
            }
            else
            {
                // play damaged animation on target prefab
                targetResourceBar.Refresh(0);
            }
        }
    }
}