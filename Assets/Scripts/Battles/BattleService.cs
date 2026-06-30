using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Battles;
using Map;
using Units;

public class BattleService : IDisposable
{
    private readonly MapService _mapService;
    private readonly UnitService _unitService;
    private readonly CardService _cardService;
    private readonly IDictionary<TeamType, IList<IUnit>> _unitsByTeam = new Dictionary<TeamType, IList<IUnit>>();
    private TeamType Turn { get; set; }
    public event Action<TeamType> OnTurnChanged;
    private Dictionary<NpcUnit, UnitTurnIntention> _nextTurnIntentionsByUnit = new();
    private bool _isEnded;
    
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
        _isEnded = false;
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

    public IList<IUnit> GetTeamUnits(TeamType team)
    {
        return _unitsByTeam.TryGetValue(team, out var units) ? units : new List<IUnit>();
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
        if (team == TeamType.Enemy)
        {
            PlayEnemyTurn();
        }
    }

    private void PlayEnemyTurn()
    {
        var context = GetBattleContext();
        var enemyUnits = _unitsByTeam[TeamType.Enemy];
        foreach (var unit in enemyUnits)
        {
            var npc = (NpcUnit)unit;
            if (_nextTurnIntentionsByUnit.TryGetValue(npc, out var intention))
            {
                intention.OnExecute?.Invoke();
                _nextTurnIntentionsByUnit.Remove(npc);
            }
            else
            {
                intention = npc.Brain.GetTurnIntention();
                intention?.OnExecute?.Invoke();
            }
             
            if (_isEnded)
            {
                break;
            }
            
            var nextIntention = npc.Brain.GetTurnIntention();
            if (nextIntention != null)
            {
                _nextTurnIntentionsByUnit[npc] = nextIntention;
            }
        }
        
        if (!_isEnded)
        {
            EndTurn(TeamType.Enemy);
        }
    }
    
    private void End(TeamType wonTeam)
    {
        _isEnded = true;
    }

    private BattleContext GetBattleContext()
    {
        var context = new BattleContext();
        return context;
    }
    
    public void Dispose()
    {
        if (_unitService != null)
        {
            _unitService.OnUnitDefeated -= OnUnitDefeated;
        }
    }
}
