using System;
using System.Collections;
using Map;
using Units;
using Units.Actions.VisualPlayback;
using UnityEngine;

public class SummonCreeperCupidActionVisualHandler : IActionVisualHandler
{
    private readonly UnitViewManager _unitViewManager;
    private readonly MapService _mapService;
    private bool _isSummoned;
    private bool _isFinished;
    
    public SummonCreeperCupidActionVisualHandler()
    {
        _unitViewManager = Locator.Get<UnitViewManager>();
        _mapService = Locator.Get<MapService>();
    }
    
    public IEnumerator PlayCor(ActionPerformResult result)
    {
        var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
        unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isSummoned = true);
        unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnFinish, () => _isFinished = true);
        var space = _mapService.GetPrefab(result.TargetSpace);
        unitPrefab.Face(space);
        unitPrefab.Animator.SetTrigger(AnimationConstants.Summon);
        yield return new WaitUntil(() => _isSummoned);
        foreach (var (spawnedUnit, spawnSpace) in result.SpawnSpaceByUnit)
        {
            _unitViewManager.OnUnitSpawned(spawnedUnit, spawnSpace);
        }
        
        yield return new WaitUntil(() => _isFinished);
    }
}
