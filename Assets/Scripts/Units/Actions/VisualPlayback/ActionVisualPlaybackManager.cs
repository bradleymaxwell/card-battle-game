using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class ActionVisualPlaybackManager : MonoBehaviour
{
    private UnitService _unitService;
    private BattleService _battleService;
    private readonly Logger _logger = new(nameof(ActionVisualPlaybackManager));
    private readonly Queue<ActionPerformResult> _actionResultsToPlay = new();
    private bool _isPlaying;
    
    private void Awake()
    {
        Locator.Register(this);
        _unitService = Locator.Get<UnitService>();
        _battleService = Locator.Get<BattleService>();
    }

    public void Initialize()
    {
        _unitService.OnActionPerformed += OnActionPerformed;
    }

    private void OnDisable()
    {
        if (_unitService != null)
        {
            _unitService.OnActionPerformed -= OnActionPerformed;
        }
    }

    private void OnActionPerformed(ActionPerformResult result)
    {
        _actionResultsToPlay.Enqueue(result);
        if (!_isPlaying)
        {
            StartCoroutine(PlayCor());
        }
    }

    private IEnumerator PlayCor()
    {
        if (_actionResultsToPlay.Count == 0)
        {
            yield break;
        }

        _isPlaying = true;
        while (_actionResultsToPlay.Count > 0)
        {
            var result = _actionResultsToPlay.Dequeue();
            var handler = result.Action.Config.Visual?.Handler;
            if (handler == null)
            {
                _logger.LogWarning($"No handler for action: {result.Action.Config.Name}, so skipping playback");
                continue;
            }
            
            yield return handler.PlayCor(result);
        }
        
        _isPlaying = false;
    }
}
