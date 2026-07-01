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

    private bool _isMotherOfAllLoveBombsDetonated;
    private bool _isObsessiveStrikeTurn = true;
    
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
            return intention;
        }

        var actionChoice = Random.Range(ToxicLovePotion, CreeperCupid + 1);
        switch (actionChoice)
        {
            case ToxicLovePotion:
                SetToxicLovePotionAs(intention);
                return intention;
            case CreeperCupid:
                SetCreeperCupidAs(intention);
                break;
        }

        _isObsessiveStrikeTurn = true;
        return intention;
    }

    private void SetCreeperCupidAs(UnitTurnIntention intention)
    {
        var action = _unit.Actions[CreeperCupid];
        intention.Description = $"{_unit.Config.Name} is going to summon a {action.Config.Name}";
        intention.OnExecute = () =>
        {
            _unitService.Perform(_unit, action);
            var edgeSpaces = _mapService.GetAllEdgeSpaces();
            var availableEdgeSpaces = edgeSpaces.Where(s => s.Occupant == null);
            var space = availableEdgeSpaces.ElementAt(Random.Range(0, edgeSpaces.Count));
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
        var action = _unit.Actions[ObsessiveStrike];
        intention.Description = $"{_unit.Config.Name} is going to hit someone with an {action.Config.Name}";
        intention.OnExecute = () =>
        {
            // find nearest player unit that has an available space next to them
            var units = _battleService.GetTeamUnits(TeamType.Player);
            var sortedUnits = units.OrderBy(u => _mapService.GetSpace(_unit).GetDistanceTo(_mapService.GetSpace(u))).ToList();
            var targetMoveSpace = _mapService.GetSpace(sortedUnits[0]);
            var targetUnitSpace = _mapService.GetSpace(sortedUnits[0]);
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
            
            _mapService.Move(_unit, targetMoveSpace.Q, targetMoveSpace.R);
            _unitService.Perform(_unit, action);
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
                _mapService.Move(_unit, targetSpace.Q, targetSpace.R);
            }

            _unitService.Perform(_unit, action);
            _isMotherOfAllLoveBombsDetonated = true;
        };
    }
}
