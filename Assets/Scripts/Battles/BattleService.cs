using System;
using System.Collections.Generic;
using System.Linq;
using Battles;
using Map;
using Units;

public class BattleService : IDisposable
{
    private readonly MapService _mapService;
    private readonly UnitService _unitService;
    private readonly CardService _cardService;
    private readonly IDictionary<TeamType, IList<IUnit>> _unitsByTeam = new Dictionary<TeamType, IList<IUnit>>();
    public TeamType Turn { get; private set; }
    public event Action<TeamType> OnTurnChanged;
    
    public BattleService() : this(
        Locator.Get<MapService>(), 
        Locator.Get<UnitService>(),
        Locator.Get<CardService>())
    {
    }

    public BattleService(MapService mapService, UnitService unitService, CardService cardService)
    {
        _mapService = mapService;
        _unitService = unitService;
        _cardService = cardService;
    }

    public void Initialize(BattleConfig battleConfig, MapSpaceContainer mapSpaceContainer)
    {
        _mapService.Initialize(mapSpaceContainer);
        _cardService.Initialize(battleConfig.PlayerDeck);
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

    public void EndTurn(TeamType team)
    {
        if (team != Turn)
        {
            return;
        }
        
        StartTurn(1 - team);
    }

    private void OnUnitDefeated(IUnit unit)
    {
        var teamUnits = _unitsByTeam[unit.Team];
        teamUnits.Remove(unit);
        if (teamUnits.Count <= 0)
        {
            var wonTeam = 1 - unit.Team;
            _unitService.DeactivateUnit(unit.Team);
            End(wonTeam);
        }
        else
        {
            _unitService.SetActiveUnit(unit.Team, teamUnits.First());
        }
    }

    private void StartTurn(TeamType team)
    {
        Turn = team;
        var teamUnits = _unitsByTeam[team];
        var firstUnit = teamUnits.FirstOrDefault();
        _unitService.SetActiveUnit(team, firstUnit);
        foreach (var unit in teamUnits)
        {
            _unitService.AdjustEnergy(unit, 2);
        }

        if (team == TeamType.Player)
        {
            _cardService.Draw(1);
        }
        
        OnTurnChanged?.Invoke(team);
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
