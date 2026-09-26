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
    private readonly IMapService _mapService;
    private readonly IUnitService _unitService;
    private IDictionary<TeamType, IList<IUnit>> UnitsByTeam { get; } = new Dictionary<TeamType, IList<IUnit>>();
    private readonly Logger _logger = new(nameof(BattleService));
    public event Action<IUnit, IUnit> OnTurnChanged;
    public event Action<TeamType> OnBattleEnded;
    public Dictionary<NpcUnit, UnitTurnIntention> NextTurnIntentionsByUnit { get; } = new();
    private bool _isEnded;
    public IList<IUnit> TurnQueue { get; private set; }
    public IList<IUnit> TurnOrder { get; private set; }
    public IUnit ActiveUnit { get; private set; }
    
    public BattleService() : this(
        Locator.Get<MapService>(), 
        Locator.Get<UnitService>())
    {
    }

    public BattleService(IMapService mapService, IUnitService unitService)
    {
        _mapService = mapService;
        _unitService = unitService;
    }

    public void Initialize(IBattleConfigProvider battleConfig, MapSpaceContainer mapSpaceContainer)
    {
        IsInitialized = false;
        _mapService.Initialize(mapSpaceContainer);
        UnitsByTeam.Clear();
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

        UnitsByTeam[TeamType.Enemy] = enemyTeam;
        UnitsByTeam[TeamType.Player] = playerTeam;
        _unitService.OnUnitDefeated += OnUnitDefeated;
        _unitService.OnUnitSpawned += OnUnitSpawned;
        _unitService.ToggleActions(true);
        
        TurnOrder = GenerateTurnOrder();
        TurnQueue = new List<IUnit>(TurnOrder);
        
        _isEnded = false;
        IsInitialized = true;
    }

    public IList<IUnit> GetTeamUnits(TeamType team)
    {
        return UnitsByTeam.TryGetValue(team, out var units) ? units : new List<IUnit>();
    }
    
    public bool IsTurn(TeamType team)
    {
        return ActiveUnit?.Team == team;
    }

    public void StartNextTurn()
    {
        if (_isEnded)
        {
            return;
        }
        
        if (ActiveUnit != null)
        {
            _unitService.DeactivateUnit(ActiveUnit.Team);
        }

        if (TurnQueue is not { Count: > 0 })
        {
            TurnQueue = new List<IUnit>(TurnOrder);
        }
        
        var unit = TurnQueue.FirstOrDefault();
        if (unit == null)
        {
            return;
        }
        
        var previousUnit = ActiveUnit;
        ActiveUnit = unit;
        TurnQueue.RemoveAt(0);
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

        var unit = ActiveUnit;
        _logger.Log($"playing enemy turn for {unit.Config.Name} ({unit.Team})");
        var npc = (NpcUnit)unit;
        if (NextTurnIntentionsByUnit.TryGetValue(npc, out var intention))
        {
            intention.OnExecute?.Invoke();
            _logger.Log($"now executing: {intention.Description}");
            NextTurnIntentionsByUnit.Remove(npc);
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
        if (UnitsByTeam[unit.Team].Contains(unit))
        {
            var nextIntention = npc.Brain.GetTurnIntention();
            if (nextIntention != null)
            {
                _logger.Log($"Next: {nextIntention.Description}");
                NextTurnIntentionsByUnit[npc] = nextIntention;
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
        NextTurnIntentionsByUnit.Clear();
        TurnQueue.Clear();
        _unitService.ToggleActions(false);
        OnBattleEnded?.Invoke(wonTeam);
        _logger.Log($"Battle ended. {wonTeam} team won!");
    }

    public void OnUnitSpawned(IUnit unit)
    {
        var team = UnitsByTeam[unit.Team];
        team.Add(unit);
        TurnOrder.Add(unit);
        TurnQueue.Add(unit);
        if (unit is NpcUnit npc)
        {
            var intention = npc.Brain.GetTurnIntention();
            if (intention != null)
            {
                NextTurnIntentionsByUnit[npc] = intention;
                _logger.Log($"Next: {intention.Description}");
            }
        }
    }
    
    private IList<IUnit> GenerateTurnOrder()
    {
        var turnOrder = new List<IUnit>();
        foreach (var team in UnitsByTeam)
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
    
    public void OnUnitDefeated(IUnit unit)
    {
        var teamUnits = UnitsByTeam[unit.Team];
        teamUnits.Remove(unit);
        TurnOrder.Remove(unit);
        TurnQueue.Remove(unit);
        if (unit is NpcUnit npc)
        {
            NextTurnIntentionsByUnit.Remove(npc);
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
