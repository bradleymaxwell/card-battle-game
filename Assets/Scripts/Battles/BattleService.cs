using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Battles;
using Map;
using Units;
using Random = UnityEngine.Random;

public class BattleService : IBattleService, IDisposable
{
    public bool IsInitialized { get; private set; }
    private readonly MapService _mapService;
    private readonly UnitService _unitService;
    private readonly IDictionary<TeamType, IList<IUnit>> _unitsByTeam = new Dictionary<TeamType, IList<IUnit>>();
    private readonly Logger _logger = new(nameof(BattleService));
    public event Action<IUnit, IUnit> OnTurnChanged;
    public event Action<TeamType> OnBattleEnded;
    private readonly Dictionary<NpcUnit, UnitTurnIntention> _nextTurnIntentionsByUnit = new();
    private bool _isEnded;
    private IList<IUnit> _turnQueue;
    private IList<IUnit> _turnOrder;
    private IUnit _activeUnit;
    
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
        IsInitialized = false;
        _mapService.Initialize(mapSpaceContainer);
        _unitsByTeam.Clear();
        var enemyTeam = new List<IUnit>();
        foreach (var unitConfig in battleConfig.EnemyUnits)
        {
            var config = new SpawnConfig(unitConfig.Unit, unitConfig.StartQ, unitConfig.StartR, TeamType.Enemy)
            {
                BrainConfig = unitConfig.Brain
            };
            
            var unit = _unitService.Spawn(config);
            enemyTeam.Add(unit);
        }
        
        var playerTeam = new List<IUnit>();
        foreach (var unitConfig in battleConfig.PlayerUnits)
        {
            var config = new SpawnConfig(unitConfig.Unit, unitConfig.StartQ, unitConfig.StartR, TeamType.Player);
            var unit = _unitService.Spawn(config);
            playerTeam.Add(unit);
        }

        _unitsByTeam[TeamType.Enemy] = enemyTeam;
        _unitsByTeam[TeamType.Player] = playerTeam;
        _unitService.OnUnitDefeated += OnUnitDefeated;
        _unitService.OnUnitSpawned += OnUnitSpawned;
        _unitService.ToggleActions(true);
        
        _turnOrder = GenerateTurnOrder();
        _turnQueue = new List<IUnit>(_turnOrder);
        
        _isEnded = false;
        IsInitialized = true;
    }

    public IList<IUnit> GetTeamUnits(TeamType team)
    {
        return _unitsByTeam.TryGetValue(team, out var units) ? units : new List<IUnit>();
    }
    
    public bool IsTurn(TeamType team)
    {
        return _activeUnit?.Team == team;
    }

    public void StartNextTurn()
    {
        if (_isEnded)
        {
            return;
        }
        
        if (_activeUnit != null)
        {
            _unitService.DeactivateUnit(_activeUnit.Team);
        }

        if (_turnQueue is not { Count: > 0 })
        {
            _turnQueue = new List<IUnit>(_turnOrder);
        }
        
        var unit = _turnQueue.FirstOrDefault();
        if (unit == null)
        {
            return;
        }
        
        var previousUnit = _activeUnit;
        _activeUnit = unit;
        _turnQueue.RemoveAt(0);
        _logger.Log($"Starting turn for {unit.Config.Name} ({unit.Team})");
        _unitService.SetActiveUnit(unit.Team, unit);
        _unitService.AdjustEnergy(unit, 2);
        OnTurnChanged?.Invoke(previousUnit, unit);
        if (unit.Team == TeamType.Enemy)
        {
            PlayEnemyTurn();
        }
    }
    
    private void PlayEnemyTurn()
    {
        if (_isEnded)
        {
            return;
        }

        var unit = _activeUnit;
        _logger.Log($"playing enemy turn for {unit.Config.Name} ({unit.Team})");
        var npc = (NpcUnit)unit;
        if (_nextTurnIntentionsByUnit.TryGetValue(npc, out var intention))
        {
            intention.OnExecute?.Invoke();
            _logger.Log($"now executing: {intention.Description}");
            _nextTurnIntentionsByUnit.Remove(npc);
        }
        else
        {
            intention = npc.Brain.GetTurnIntention();
            _logger.Log($"now executing: {intention.Description}");
            intention?.OnExecute?.Invoke();
        }
         
        if (_isEnded)
        {
            return;
        }
        
        // if the unit is removed from the battle, then skip getting their next intention
        if (_unitsByTeam[unit.Team].Contains(unit))
        {
            var nextIntention = npc.Brain.GetTurnIntention();
            if (nextIntention != null)
            {
                _logger.Log($"Next: {nextIntention.Description}");
                _nextTurnIntentionsByUnit[npc] = nextIntention;
            }
        }
        
        if (!_isEnded)
        {
            StartNextTurn();
        }
    }
    
    private void End(TeamType wonTeam)
    {
        _isEnded = true;
        _nextTurnIntentionsByUnit.Clear();
        _turnQueue.Clear();
        _unitService.ToggleActions(false);
        OnBattleEnded?.Invoke(wonTeam);
        _logger.Log($"Battle ended. {wonTeam} team won!");
    }

    private void OnUnitSpawned(IUnit unit)
    {
        var team = _unitsByTeam[unit.Team];
        team.Add(unit);
        _turnOrder.Add(unit);
        _turnQueue.Add(unit);
        if (unit is NpcUnit npc)
        {
            var intention = npc.Brain.GetTurnIntention();
            if (intention != null)
            {
                _nextTurnIntentionsByUnit[npc] = intention;
                _logger.Log($"Next: {intention.Description}");
            }
        }
    }
    
    private IList<IUnit> GenerateTurnOrder()
    {
        var turnOrder = new List<IUnit>();
        foreach (var team in _unitsByTeam)
        {
            foreach (var unit in team.Value)
            {
                turnOrder.Add(unit);
            }
        }

        for (var i = 0; i < turnOrder.Count; i++)
        {
            var random = Random.Range(0, turnOrder.Count);
            (turnOrder[i], turnOrder[random]) = (turnOrder[random], turnOrder[i]);
        }
        
        _logger.Log($"Unit turn order: {string.Join(", ", turnOrder.Select(u => u.Config.Name))}");
        return turnOrder;
    }
    
    public void Dispose()
    {
        if (_unitService != null)
        {
            _unitService.OnUnitDefeated -= OnUnitDefeated;
        }
    }
    
    private void OnUnitDefeated(IUnit unit)
    {
        var teamUnits = _unitsByTeam[unit.Team];
        teamUnits.Remove(unit);
        _turnOrder.Remove(unit);
        _turnQueue.Remove(unit);
        if (unit is NpcUnit npc)
        {
            _nextTurnIntentionsByUnit.Remove(npc);
        }
        
        _logger.Log($"{unit.Config.Name} ({unit.Team}) defeated!");
        if (teamUnits.Count <= 0)
        {
            var wonTeam = 1 - unit.Team;
            _unitService.DeactivateUnit(unit.Team);
            End(wonTeam);
        }
    }
}
