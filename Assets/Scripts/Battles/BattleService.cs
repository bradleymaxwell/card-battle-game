using System;
using System.Collections.Generic;
using System.Linq;
using Battles;
using DefaultNamespace;
using Map;
using Targeting;
using Units;
using UnityEngine.InputSystem;

public class BattleService : IDisposable
{
    private readonly MapService _mapService;
    private readonly UnitService _unitService;
    private readonly InputAction _clickAction;
    private readonly TargetService _targetService;
    private IDictionary<TeamType, IList<IUnit>> _unitsByTeam = new Dictionary<TeamType, IList<IUnit>>();
    public TeamType Turn { get; private set; }
    
    public BattleService() : this(
        Locator.Get<MapService>(), 
        Locator.Get<UnitService>(),
        Locator.Get<InputService>(),
        Locator.Get<TargetService>())
    {
    }
    
    public BattleService(MapService mapService, UnitService unitService, InputService inputService, TargetService targetService)
    {
        _mapService = mapService;
        _unitService = unitService;
        _clickAction = inputService.GetAction(PlayerInputConstants.UI, PlayerInputConstants.Click);
        _targetService = targetService;
    }
    
    public void Initialize(BattleConfig battleConfig, MapSpaceContainer mapSpaceContainer)
    {
        _mapService.Initialize(mapSpaceContainer);
        _unitsByTeam.Clear();
        var enemyTeam = new List<IUnit>();
        foreach (var unitConfig in battleConfig.EnemyUnits)
        {
            var unit = _unitService.Spawn(unitConfig, TeamType.Enemy);
            enemyTeam.Add(unit);
        }
        
        var playerTeam = new List<IUnit>();
        foreach (var unitConfig in battleConfig.PlayerUnits)
        {
            var unit = _unitService.Spawn(unitConfig, TeamType.Player);
            playerTeam.Add(unit);
        }

        _unitsByTeam[TeamType.Enemy] = enemyTeam;
        _unitsByTeam[TeamType.Player] = playerTeam;
        _unitService.OnUnitDefeated += OnUnitDefeated;
        _clickAction.performed += OnPlayerClick;
        Turn = TeamType.Player;
    }

    private void OnUnitDefeated(IUnit unit)
    {
        var teamUnits = _unitsByTeam[unit.Team];
        teamUnits.Remove(unit);
        if (teamUnits.Count <= 0)
        {
            var wonTeam = 1 - unit.Team;
            End(wonTeam);
        }
    }

    private void OnPlayerClick(InputAction.CallbackContext context)
    {
        if (Turn != TeamType.Player)
        {
            return;
        }
        
        var playerTarget = _targetService.GetTarget(TeamType.Player);
        if (playerTarget == null || playerTarget is not MapSpacePrefab mapSpacePrefab)
        {
            return;
        }
        
        // todo: temporary for testing out the movement
        var unit = _unitsByTeam[TeamType.Player].FirstOrDefault();
        var action = unit?.Actions.FirstOrDefault();
        if (action != null)
        {
            _unitService.Perform(unit, action, mapSpacePrefab.Space);
        }
    }
    
    private void End(TeamType wonTeam)
    {
    }
    
    public void Dispose()
    {
        if (_unitService != null)
        {
            _unitService.OnUnitDefeated -= OnUnitDefeated;
        }

        if (_clickAction != null)
        {
            _clickAction.performed -= OnPlayerClick;
        }
    }
}
