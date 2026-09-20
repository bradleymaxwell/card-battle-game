using System.Linq;
using AI;
using AI.LoveBomber;
using Battles;
using Map;
using Targeting;
using Units;
using UnityEngine;

public class LoveBomberBrain : IUnitBrain
{
    private readonly LoveBomberBrainConfig _config;
    private IUnit _unit;
    private readonly MapService _mapService;
    private readonly UnitService _unitService;
    private readonly BattleService _battleService;
    private readonly SelectService _selectService;
    private const int ObsessiveStrike = 0;
    private const int ToxicLovePotion = 1;
    private const int CreeperCupid = 2;
    private const int MotherOfAllLoveBombs = 3;
    private const int Move = 4;
    private const int SetupGaslightExplosives = 5;

    private bool _isMotherOfAllLoveBombsDetonated;
    private bool _isObsessiveStrikeTurn;
    
    public LoveBomberBrain(LoveBomberBrainConfig config)
    {
        _config = config;
        _mapService = Locator.Get<MapService>();
        _unitService = Locator.Get<UnitService>();
        _battleService = Locator.Get<BattleService>();
        _selectService = Locator.Get<SelectService>();
    }

    public void Initialize(IUnit unit)
    {
        _unit = unit;
        _isMotherOfAllLoveBombsDetonated = false;
        _isObsessiveStrikeTurn = true;
    }

    public UnitTurnIntention GetTurnIntention()
    {
        var intention = new UnitTurnIntention();
        if (_unit.CurrentHealth <= _config.MotherOfAllLoveBombsThreshold && !_isMotherOfAllLoveBombsDetonated)
        {
            SetMotherOfAllLoveBombsAs(intention);
            return intention;
        }
        
        if (_isObsessiveStrikeTurn)
        {
            SetObsessiveStrikeAs(intention);
            _isObsessiveStrikeTurn = false;
        }
        else
        {
            var actionChoice = Random.Range(ToxicLovePotion, CreeperCupid + 1);
            switch (actionChoice)
            {
                case ToxicLovePotion:
                    SetToxicLovePotionAs(intention);
                    break;
                case CreeperCupid:
                    SetCreeperCupidAs(intention);
                    break;
            }
            
            _isObsessiveStrikeTurn = true;
        }

        if (!_isMotherOfAllLoveBombsDetonated)
        {
            AddSetupExplosivesTo(intention);
        }
        
        return intention;
    }

    private void AddSetupExplosivesTo(UnitTurnIntention intention)
    {
        var onExecute = intention.OnExecute;
        intention.OnExecute = () =>
        {
            onExecute?.Invoke();
            var setupGaslightExplosives = _unit.Actions[SetupGaslightExplosives];
            for (var i = 0; i < _config.GaslightExplosivesPerTurn; i++)
            {
                var currentSpace = _mapService.GetSpace(_unit);
                var spaces = _mapService.GetAreaSpaces(currentSpace, _config.GaslightExplosiveSearchRadius, false);
                var availableSpaces = spaces
                    .Where(s => s.Occupant == null)
                    .OrderBy(_ => Random.value);

                foreach (var potentialSpace in availableSpaces)
                {
                    if (setupGaslightExplosives.CanPerform(currentSpace, potentialSpace))
                    {
                        _unitService.Perform(_unit, setupGaslightExplosives);
                        _selectService.Select(potentialSpace, TeamType.Enemy);
                        break;
                    }

                    var closestReachableNeighbor = _mapService.GetClosestReachableNeighborSpace(_unit, potentialSpace.Q, potentialSpace.R, _config.GaslightExplosiveSearchRadius + 1);
                    if (closestReachableNeighbor == null)
                    {
                        continue;
                    }

                    ExecuteMove(closestReachableNeighbor.Q, closestReachableNeighbor.R);
                    _unitService.Perform(_unit, setupGaslightExplosives);
                    _selectService.Select(potentialSpace, TeamType.Enemy);
                    break;
                }
            }
        };
    }

    private void SetCreeperCupidAs(UnitTurnIntention intention)
    {
        var action = _unit.Actions[CreeperCupid];
        intention.Description = $"{_unit.Config.Name} is going to summon a {action.Config.Name}";
        intention.OnExecute = () =>
        {
            _unitService.Perform(_unit, action);
            var edgeSpaces = _mapService.GetAllEdgeSpaces();
            var availableEdgeSpaces = edgeSpaces.Where(s => s.Occupant == null).ToList();
            var space = availableEdgeSpaces.ElementAt(Random.Range(0, availableEdgeSpaces.Count));
            _selectService.Select(space, TeamType.Enemy);
        };
    }
    
    private void SetToxicLovePotionAs(UnitTurnIntention intention)
    {
        var action = _unit.Actions[ToxicLovePotion];
        var units = _battleService.GetTeamUnits(TeamType.Player);
        var unit = units[Random.Range(0, units.Count)];
        intention.Description = $"{_unit.Config.Name} is going to throw a {action.Config.Name} at {unit.Config.Name}";
        intention.OnExecute = () =>
        {
            _unitService.Perform(_unit, action);
            var space = _mapService.GetSpace(unit);
            _selectService.Select(space, TeamType.Enemy);
        };
    }

    private void SetObsessiveStrikeAs(UnitTurnIntention intention)
    {
        var obsessiveStrike = _unit.Actions[ObsessiveStrike];
        intention.Description = $"{_unit.Config.Name} is going to hit someone with an {obsessiveStrike.Config.Name}";
        intention.OnExecute = () =>
        {
            // find nearest player unit that has an available space next to them
            var units = _battleService.GetTeamUnits(TeamType.Player);
            var sortedUnits = units.OrderBy(u => _mapService.GetSpace(_unit).GetDistanceTo(_mapService.GetSpace(u))).ToList();
            var startSpace = _mapService.GetSpace(_unit);
            var targetUnitSpace = _mapService.GetSpace(sortedUnits[0]);
            if (!obsessiveStrike.CanPerform(startSpace, targetUnitSpace))
            {
                var targetMoveSpace = _mapService.GetSpace(sortedUnits[0]);
                foreach (var unit in sortedUnits)
                {
                    var unitSpace = _mapService.GetSpace(unit);
                    var space = _mapService.GetNeighbors(unitSpace).FirstOrDefault(n => n.Occupant == null);
                    if (space != null)
                    {
                        targetMoveSpace = space;
                        targetUnitSpace = unitSpace;
                        break;
                    }
                }
                
                ExecuteMove(targetMoveSpace.Q, targetMoveSpace.R);
            }
            
            _unitService.Perform(_unit, obsessiveStrike);
            _selectService.Select(targetUnitSpace, TeamType.Enemy);
            _isObsessiveStrikeTurn = false;
        };
    }
    
    private void SetMotherOfAllLoveBombsAs(UnitTurnIntention intention)
    {
        var action = _unit.Actions[MotherOfAllLoveBombs];
        intention.Description = $"{_unit.Config.Name} is going to detonate the {action.Config.Name}!";
        intention.OnExecute = () =>
        {
            var targetSpace = _mapService.GetClosestReachableSpace(_unit, 0, 0, 100);
            if (targetSpace != null)
            {
                ExecuteMove(targetSpace.Q, targetSpace.R);
            }

            _unitService.Perform(_unit, action);
            _isMotherOfAllLoveBombsDetonated = true;
        };
    }

    private void ExecuteMove(int q, int r)
    {
        var moveAction = _unit.Actions[Move];
        _unitService.Perform(_unit, moveAction);
        var space = _mapService.GetSpace(q, r);
        _selectService.Select(space, TeamType.Enemy);
    }
}
