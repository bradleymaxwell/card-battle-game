using System.Collections.Generic;
using AI.LoveBomber;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using Battles;
using FakeItEasy;
using Map;
using NUnit.Framework;
using Targeting;
using Units;

[TestFixture]
public class LoveBomberBrainTests
{
    private ILoveBomberBrainConfigProvider _configProvider;
    private IMapService _mapService;
    private IUnitService _unitService;
    private IBattleService _battleService;
    private ISelectService _selectService;
    private LoveBomberBrain _sut;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization()
        {
            ConfigureMembers = true
        });

        _configProvider = fixture.Freeze<ILoveBomberBrainConfigProvider>();
        _mapService = fixture.Freeze<IMapService>();
        _unitService = fixture.Freeze<IUnitService>();
        _battleService = fixture.Freeze<IBattleService>();
        _selectService = fixture.Freeze<ISelectService>();

        fixture.Register(() => new LoveBomberBrain(_configProvider, _mapService, _unitService, _battleService, _selectService));
        _sut = fixture.Create<LoveBomberBrain>();
    }

    [Test]
    public void GetTurnIntentionReturnsMotherOfAllLoveBombsWhenHealthCrossesThreshold()
    {
        // Arrange
        var unit = CreateLoveBomber(currentHealth: 5);
        var motherOfAllLoveBombs = unit.Actions[LoveBomberBrain.MotherOfAllLoveBombs];
        A.CallTo(() => _configProvider.MotherOfAllLoveBombsThreshold).Returns(10);
        _sut.Initialize(unit);

        // Act
        var intention = _sut.GetTurnIntention();

        // Assert
        Assert.That(intention.Description, Does.Contain(motherOfAllLoveBombs.Config.Name));
    }

    [Test]
    public void GetTurnIntentionDoesNotReturnsMotherOfAllLoveBombsWhenBelowThresholdAndAlreadyDetonated()
    {
        // Arrange
        var unit = CreateLoveBomber(currentHealth: 5);
        var motherOfAllLoveBombs = unit.Actions[LoveBomberBrain.MotherOfAllLoveBombs];
        var currentSpace = new MapSpace(0, 0)
        {
            Occupant = unit
        };

        A.CallTo(() => _configProvider.MotherOfAllLoveBombsThreshold).Returns(10);
        A.CallTo(() => _mapService.GetClosestReachableSpace(unit, 0, 0, 100)).Returns(null);
        A.CallTo(() => _mapService.GetSpace(unit)).Returns(currentSpace);
        _sut.Initialize(unit);
        var detonationIntention = _sut.GetTurnIntention();
        detonationIntention.OnExecute?.Invoke();

        // Act
        var nextIntention = _sut.GetTurnIntention();

        // Assert
        Assert.That(nextIntention.Description, Does.Not.Contain(motherOfAllLoveBombs.Config.Name));
    }

    [Test]
    public void GetTurnIntentionReturnsObsessiveStrikeEveryOtherTurn()
    {
        // Arrange
        var unit = CreateLoveBomber(currentHealth: 100);
        var obsessiveStrike = unit.Actions[LoveBomberBrain.ObsessiveStrike];
        A.CallTo(() => _configProvider.MotherOfAllLoveBombsThreshold).Returns(10);
        _sut.Initialize(unit);

        // Act
        var firstIntention = _sut.GetTurnIntention();
        var secondIntention = _sut.GetTurnIntention();
        var thirdIntention = _sut.GetTurnIntention();

        // Assert
        Assert.That(firstIntention.Description, Does.Contain(obsessiveStrike.Config.Name));
        Assert.That(secondIntention.Description, Does.Not.Contain(obsessiveStrike.Config.Name));
        Assert.That(thirdIntention.Description, Does.Contain(obsessiveStrike.Config.Name));
    }

    [Test]
    public void GetTurnIntentionOnExecuteMovesWithinRangeWhenSetAsObsessiveStrike()
    {
        // Arrange
        var loveBomberUnit = CreateLoveBomber(currentHealth: 100);
        var obsessiveStrike = loveBomberUnit.Actions[LoveBomberBrain.ObsessiveStrike];
        var move = loveBomberUnit.Actions[LoveBomberBrain.Move];
        var loveBomberSpace = new MapSpace(0, 0)
        {
            Occupant = loveBomberUnit
        };

        var playerUnit = CreatePlayerUnit("Player Unit");
        var playerSpace = new MapSpace(4, 0)
        {
            Occupant = playerUnit
        };

        var adjacentOpenSpace = new MapSpace(3, 0);

        A.CallTo(() => _configProvider.MotherOfAllLoveBombsThreshold).Returns(10);
        A.CallTo(() => _battleService.GetTeamUnits(TeamType.Player)).Returns(new List<IUnit> { playerUnit });
        A.CallTo(() => _mapService.GetSpace(loveBomberUnit)).Returns(loveBomberSpace);
        A.CallTo(() => _mapService.GetSpace(playerUnit)).Returns(playerSpace);
        A.CallTo(() => _mapService.GetNeighbors(playerSpace)).Returns(new List<MapSpace> { adjacentOpenSpace });
        A.CallTo(() => obsessiveStrike.CanPerform(loveBomberSpace, playerSpace)).Returns(false);
        A.CallTo(() => _mapService.GetSpace(adjacentOpenSpace.Q, adjacentOpenSpace.R)).Returns(adjacentOpenSpace);
        _sut.Initialize(loveBomberUnit);

        // Act
        var intention = _sut.GetTurnIntention();
        intention.OnExecute?.Invoke();

        // Assert
        A.CallTo(() => _unitService.Perform(loveBomberUnit, move, false, null)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _selectService.Select(adjacentOpenSpace, TeamType.Enemy)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitService.Perform(loveBomberUnit, obsessiveStrike, false, null)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _selectService.Select(playerSpace, TeamType.Enemy)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void GetTurnIntentionOnExecuteSetsUpGaslightExplosivesWhenMotherOfAllLoveBombsNotDetonated()
    {
        // Arrange
        var loveBomberUnit = CreateLoveBomber(currentHealth: 100);
        var obsessiveStrike = loveBomberUnit.Actions[LoveBomberBrain.ObsessiveStrike];
        var setupGaslightExplosives = loveBomberUnit.Actions[LoveBomberBrain.SetupGaslightExplosives];
        var currentSpace = new MapSpace(0, 0)
        {
            Occupant = loveBomberUnit
        };

        var playerUnit = CreatePlayerUnit("Player Unit");
        var playerSpace = new MapSpace(2, 0)
        {
            Occupant = playerUnit
        };

        var availableSpace = new MapSpace(1, 0);
        const int gaslightExplosivesPerTurn = 2;
        A.CallTo(() => _configProvider.MotherOfAllLoveBombsThreshold).Returns(10);
        A.CallTo(() => _configProvider.GaslightExplosivesPerTurn).Returns(gaslightExplosivesPerTurn);
        A.CallTo(() => _configProvider.GaslightExplosiveSearchRadius).Returns(3);

        A.CallTo(() => _battleService.GetTeamUnits(TeamType.Player)).Returns(new List<IUnit> { playerUnit });
        A.CallTo(() => _mapService.GetSpace(loveBomberUnit)).Returns(currentSpace);
        A.CallTo(() => _mapService.GetSpace(playerUnit)).Returns(playerSpace);
        A.CallTo(() => obsessiveStrike.CanPerform(currentSpace, playerSpace)).Returns(true);

        A.CallTo(() => _mapService.GetAreaSpaces(currentSpace, 3, false)).Returns(new List<MapSpace> { availableSpace });
        A.CallTo(() => setupGaslightExplosives.CanPerform(currentSpace, availableSpace)).Returns(true);

        _sut.Initialize(loveBomberUnit);

        // Act
        var intention = _sut.GetTurnIntention();
        intention.OnExecute?.Invoke();

        // Assert
        A.CallTo(() => _unitService.Perform(loveBomberUnit, setupGaslightExplosives, false, null)).MustHaveHappened(gaslightExplosivesPerTurn, Times.Exactly);
        A.CallTo(() => _selectService.Select(availableSpace, TeamType.Enemy)).MustHaveHappened(gaslightExplosivesPerTurn, Times.Exactly);
    }

    [Test]
    public void GetTurnIntentionOnExecuteMovesLoveBomberToNearestAdjacentSpaceToTargetSpaceWhenSettingUpGaslightExplosives()
    {
        // Arrange
        var loveBomberUnit = CreateLoveBomber(currentHealth: 100);
        var obsessiveStrike = loveBomberUnit.Actions[LoveBomberBrain.ObsessiveStrike];
        var setupGaslightExplosives = loveBomberUnit.Actions[LoveBomberBrain.SetupGaslightExplosives];
        var move = loveBomberUnit.Actions[LoveBomberBrain.Move];
        var currentSpace = new MapSpace(0, 0)
        {
            Occupant = loveBomberUnit
        };

        var playerUnit = CreatePlayerUnit("Player Unit");
        var playerSpace = new MapSpace(2, 0)
        {
            Occupant = playerUnit
        };

        var targetSpace = new MapSpace(4, 0);
        var closestReachableNeighbor = new MapSpace(3, 0);
        
        A.CallTo(() => _configProvider.MotherOfAllLoveBombsThreshold).Returns(10);
        A.CallTo(() => _configProvider.GaslightExplosivesPerTurn).Returns(1);
        A.CallTo(() => _configProvider.GaslightExplosiveSearchRadius).Returns(3);

        A.CallTo(() => _battleService.GetTeamUnits(TeamType.Player)).Returns(new List<IUnit> { playerUnit });
        A.CallTo(() => _mapService.GetSpace(loveBomberUnit)).Returns(currentSpace);
        A.CallTo(() => _mapService.GetSpace(playerUnit)).Returns(playerSpace);
        A.CallTo(() => obsessiveStrike.CanPerform(currentSpace, playerSpace)).Returns(true);

        A.CallTo(() => _mapService.GetAreaSpaces(currentSpace, 3, false)).Returns(new List<MapSpace> { targetSpace });
        A.CallTo(() => setupGaslightExplosives.CanPerform(currentSpace, targetSpace)).Returns(false);
        A.CallTo(() => _mapService.GetClosestReachableNeighborSpace(loveBomberUnit, targetSpace.Q, targetSpace.R, 4)).Returns(closestReachableNeighbor);
        A.CallTo(() => _mapService.GetSpace(closestReachableNeighbor.Q, closestReachableNeighbor.R)).Returns(closestReachableNeighbor);

        _sut.Initialize(loveBomberUnit);

        // Act
        var intention = _sut.GetTurnIntention();
        intention.OnExecute?.Invoke();

        // Assert
        A.CallTo(() => _unitService.Perform(loveBomberUnit, move, false, null)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _selectService.Select(closestReachableNeighbor, TeamType.Enemy)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitService.Perform(loveBomberUnit, setupGaslightExplosives, false, null)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _selectService.Select(targetSpace, TeamType.Enemy)).MustHaveHappenedOnceExactly();
    }

    private static IUnit CreateLoveBomber(int currentHealth)
    {
        var unit = A.Fake<IUnit>();
        var config = A.Fake<IUnitConfigProvider>();
        A.CallTo(() => unit.Config).Returns(config);
        A.CallTo(() => unit.Team).Returns(TeamType.Enemy);
        A.CallTo(() => unit.CurrentHealth).Returns(currentHealth);
        A.CallTo(() => unit.Actions).Returns(new List<IAction>
        {
            CreateAction("Obsessive Strike"),
            CreateAction("Toxic Love Potion"),
            CreateAction("Creeper Cupid"),
            CreateAction("Mother Of All Love Bombs"),
            CreateAction("Move"),
            CreateAction("Setup Gaslight Explosives")
        });

        return unit;
    }

    private static IUnit CreatePlayerUnit(string name)
    {
        var unit = A.Fake<IUnit>();
        var config = A.Fake<IUnitConfigProvider>();

        A.CallTo(() => config.Name).Returns(name);
        A.CallTo(() => unit.Config).Returns(config);
        A.CallTo(() => unit.Team).Returns(TeamType.Player);
        
        return unit;
    }

    private static IAction CreateAction(string name)
    {
        var action = A.Fake<IAction>();
        var config = A.Fake<IActionConfigProvider>();
        
        A.CallTo(() => config.Name).Returns(name);
        A.CallTo(() => action.Config).Returns(config);
        
        return action;
    }
}
