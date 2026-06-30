using Battles;
using Map;
using Targeting;
using Units;
using UnityEngine;

namespace AI.CreeperCupid
{
    public class CreeperCupidBrain : IUnitBrain
    {
        private readonly BattleService _battleService;
        private readonly MapService _mapService;
        private readonly UnitService _unitService;
        private readonly SelectService _selectService;
        private IUnit _unit;
        private IUnit _target;

        private const int Move = 0;
        private const int KissOfDeath = 1;
        
        private IAction _move;
        private IAction _kissOfDeath;
        
        public CreeperCupidBrain()
        {
            _battleService = Locator.Get<BattleService>();
            _mapService = Locator.Get<MapService>();
            _unitService = Locator.Get<UnitService>();
            _selectService = Locator.Get<SelectService>();
        }
        
        public void Initialize(IUnit unit)
        {
            _unit = unit;
            var playerUnits = _battleService.GetTeamUnits(TeamType.Player);
            _target = playerUnits[Random.Range(0, playerUnits.Count)];
            _move = _unit.Actions[Move];
            _kissOfDeath = _unit.Actions[KissOfDeath];
        }

        public UnitTurnIntention GetTurnIntention()
        {
            var intention = new UnitTurnIntention
            {
                Description = $"{_unit.Config.Name} wants to give {_target.Config.Name} a {_kissOfDeath.Config.Name}",
                OnExecute = () =>
                {
                    var unitSpace = _mapService.GetSpace(_unit);
                    var targetSpace = _mapService.GetSpace(_target);
                    if (!_kissOfDeath.CanPerform(unitSpace, targetSpace))
                    {
                        var maxDistance = _move.Config.EnergyCost > 0 ? _unit.CurrentEnergy / _move.Config.EnergyCost : 100;
                        var closestSpace = _mapService.GetClosestReachableSpace(_unit, targetSpace.Q, targetSpace.R, maxDistance);
                        if (closestSpace == null)
                        {
                            return;
                        }
                        
                        _unitService.Perform(_unit, _move);
                        _selectService.Select(closestSpace, _unit.Team);
                        if (!_kissOfDeath.CanPerform(closestSpace, targetSpace))
                        {
                            return;
                        }
                    }
                    
                    _unitService.Perform(_unit, _kissOfDeath);
                    _selectService.Select(targetSpace, _unit.Team);
                }
            };
            
            return intention;
        }
    }
}