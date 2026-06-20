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
    private IDictionary<TeamType, IList<IUnit>> _unitsByTeam = new Dictionary<TeamType, IList<IUnit>>();
    public TeamType Turn { get; private set; }
    
    public BattleService() : this(
        Locator.Get<MapService>(), 
        Locator.Get<UnitService>())
    {
    }

    public BattleService(MapService mapService, UnitService unitService)
    {
        _mapService = mapService;
        _unitService = unitService;
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
        StartTurn(TeamType.Player);
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

    private void StartTurn(TeamType team)
    {
        Turn = team;
        var firstUnit = _unitsByTeam[team].FirstOrDefault();
        _unitService.SetActiveUnit(team, firstUnit);
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
    }
}
