using System;
using System.Collections;
using System.Collections.Generic;
using Battles;
using Units;
using UnityEngine;

public class VisualPlaybackManager : MonoBehaviour
{
    [SerializeField] private EndTurnView endTurnView;
    [SerializeField] private ActionViewContainer actionViewContainer;
    [SerializeField] private UnitViewManager unitViewManager;
    private UnitService _unitService;
    private BattleService _battleService;
    private readonly Logger _logger = new(nameof(VisualPlaybackManager));
    private readonly Queue<Func<IEnumerator>> _playbackQueue = new();
    private bool _isPlaying;
    
    private void Awake()
    {
        _unitService = Locator.Get<UnitService>();
        _battleService = Locator.Get<BattleService>();
    }

    public void Initialize()
    {
        _unitService.OnActionPerformed += OnActionPerformed;
        _battleService.OnTurnChanged += OnTurnChanged;
        _battleService.OnBattleEnded += OnBattleEnded;
    }

    private void OnDisable()
    {
        if (_unitService != null)
        {
            _unitService.OnActionPerformed -= OnActionPerformed;
        }
        
        if (_battleService != null)
        {
            _battleService.OnTurnChanged -= OnTurnChanged;
            _battleService.OnBattleEnded -= OnBattleEnded;
        }
    }

    private void OnBattleEnded(TeamType team)
    {
        AddToQueue(() => EndBattleCor(team));
    }

    private void OnActionPerformed(ActionPerformResult result)
    {
        AddToQueue(() => PlayActionCor(result));
    }

    private IEnumerator PlayActionCor(ActionPerformResult result)
    {
        if (result.EnergyConsumed > 0)
        {
            var (_, resourceBar) = unitViewManager.GetUnitViews(result.Performer);
            if (resourceBar != null)
            {
                resourceBar.RefreshEnergy();
            }
        }
        
        var handler = result.Action.Config.Visual?.Handler;
        if (handler == null)
        {
            _logger.LogWarning($"No handler for action: {result.Action.Config.Name}, so skipping playback");
            yield break;
        }
            
        yield return handler.PlayCor(result);
    }
    
    private void OnTurnChanged(IUnit previousUnit, IUnit newUnit)
    {
        AddToQueue(() => PlayChangeTurnCor(previousUnit, newUnit));
    }

    private IEnumerator PlayChangeTurnCor(IUnit previousUnit, IUnit newUnit)
    {
        if (previousUnit != null)
        {
            var (_, previousUnitResourceBar) = unitViewManager.GetUnitViews(previousUnit);
            if (previousUnitResourceBar != null)
            {
                previousUnitResourceBar.OnActiveUnitChanged(previousUnit.Team, null);
                previousUnitResourceBar.RefreshEnergy();
            }
        }
        
        if (newUnit != null)
        {
            var (_, newUnitResourceBar) = unitViewManager.GetUnitViews(newUnit);
            newUnitResourceBar.OnActiveUnitChanged(newUnit.Team, newUnit);
            actionViewContainer.OnActiveUnitChanged(newUnit.Team, newUnit);
            newUnitResourceBar.RefreshEnergy();
        }
        
        endTurnView.OnTurnChanged(newUnit);
        yield break;
    }

    private IEnumerator PlayCor()
    {
        if (_playbackQueue.Count == 0)
        {
            yield break;
        }

        _isPlaying = true;
        while (_playbackQueue.Count > 0)
        {
            var playback = _playbackQueue.Dequeue();
            yield return playback?.Invoke();
        }
        
        _isPlaying = false;
    }

    private IEnumerator EndBattleCor(TeamType team)
    {
        actionViewContainer.OnActiveUnitChanged(TeamType.Enemy, null);
        actionViewContainer.OnActiveUnitChanged(TeamType.Player, null);
        endTurnView.gameObject.SetActive(false);
        yield break;
    }

    private void AddToQueue(Func<IEnumerator> factory)
    {
        if (factory == null)
        {
            return;
        }

        _playbackQueue.Enqueue(factory);
        if (!_isPlaying)
        {
            StartCoroutine(PlayCor());
        }
    }
}
