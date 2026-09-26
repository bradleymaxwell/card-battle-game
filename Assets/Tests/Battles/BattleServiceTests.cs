using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using Battles;
using FakeItEasy;
using Map;
using NUnit.Framework;
using Units;

[TestFixture]
public class BattleServiceTests
{
    private IMapService _mapService;
    private IUnitService _unitService;
    private BattleService _sut;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization()
        {
            ConfigureMembers = true
        });

        _mapService = fixture.Freeze<IMapService>();
        _unitService = fixture.Freeze<IUnitService>();

        fixture.Register(() => new BattleService(_mapService, _unitService));
        _sut = fixture.Create<BattleService>();
    }

    [TearDown]
    public void TearDown()
    {
        _sut?.Dispose();
    }

    [Test]
    public void InitializePutsAllUnitsIntoTheirExpectedTeams()
    {
        // Arrange
        var enemyOne = CreateUnit(TeamType.Enemy, "Enemy One");
        var enemyTwo = CreateUnit(TeamType.Enemy, "Enemy Two");
        var playerOne = CreateUnit(TeamType.Player, "Player One");
        var playerTwo = CreateUnit(TeamType.Player, "Player Two");

        var battleConfig = CreateBattleConfig(enemyCount: 2, playerCount: 2);
        StubSpawnedUnits(enemyOne, enemyTwo, playerOne, playerTwo);

        // Act
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        // Assert
        Assert.That(_sut.GetTeamUnits(TeamType.Enemy), Is.EquivalentTo(new[] { enemyOne, enemyTwo }));
        Assert.That(_sut.GetTeamUnits(TeamType.Player), Is.EquivalentTo(new[] { playerOne, playerTwo }));
        Assert.That(_sut.IsInitialized, Is.True);
    }

    [Test]
    public void InitializeGeneratesTurnOrderAndQueueWithAllUnitsHavingOneTurn()
    {
        // Arrange
        var enemyOne = CreateUnit(TeamType.Enemy, "Enemy One");
        var enemyTwo = CreateUnit(TeamType.Enemy, "Enemy Two");
        var playerOne = CreateUnit(TeamType.Player, "Player One");
        var playerTwo = CreateUnit(TeamType.Player, "Player Two");
        var allUnits = new[] { enemyOne, enemyTwo, playerOne, playerTwo };

        var battleConfig = CreateBattleConfig(enemyCount: 2, playerCount: 2);
        StubSpawnedUnits(allUnits);

        // Act
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        // Assert
        Assert.That(_sut.TurnOrder, Is.EquivalentTo(allUnits));
        Assert.That(_sut.TurnQueue, Is.EquivalentTo(allUnits));
        Assert.That(_sut.TurnOrder, Has.Count.EqualTo(allUnits.Length));
        Assert.That(_sut.TurnQueue, Has.Count.EqualTo(allUnits.Length));
    }

    [Test]
    public void StartNextTurnRefreshesTurnQueueWhenQueueIsEmpty()
    {
        // Arrange
        var playerOne = CreateUnit(TeamType.Player, "Player One");
        var playerTwo = CreateUnit(TeamType.Player, "Player Two");

        var battleConfig = CreateBattleConfig(enemyCount: 0, playerCount: 2);
        StubSpawnedUnits(playerOne, playerTwo);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());
        _sut.TurnQueue.Clear();

        // Act
        _sut.StartNextTurn();

        // Assert
        Assert.That(_sut.ActiveUnit, Is.Not.Null);
        Assert.That(_sut.TurnQueue, Is.EquivalentTo(_sut.TurnOrder.Where(unit => unit != _sut.ActiveUnit)));
    }

    [Test]
    public void StartNextTurnGivesActiveUnitMoreEnergy()
    {
        // Arrange
        var player = CreateUnit(TeamType.Player, "Player");
        var battleConfig = CreateBattleConfig(enemyCount: 0, playerCount: 1);
        StubSpawnedUnits(player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        // Act
        _sut.StartNextTurn();

        // Assert
        A.CallTo(() => _unitService.AdjustEnergy(player, A<int>._)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void OnUnitSpawnedGeneratesNextTurnIntentionWhenUnitIsNpc()
    {
        // Arrange
        var player = CreateUnit(TeamType.Player, "Player");
        var spawnedNpc = CreateNpcUnit(TeamType.Enemy, "Spawned Enemy");
        var intention = new UnitTurnIntention { Description = "Spawned NPC intention" };

        A.CallTo(() => spawnedNpc.Brain.GetTurnIntention()).Returns(intention);

        var battleConfig = CreateBattleConfig(enemyCount: 0, playerCount: 1);
        StubSpawnedUnits(player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        // Act
        _sut.OnUnitSpawned(spawnedNpc);

        // Assert
        Assert.That(_sut.GetTeamUnits(TeamType.Enemy), Does.Contain(spawnedNpc));
        Assert.That(_sut.NextTurnIntentionsByUnit[spawnedNpc], Is.SameAs(intention));
        A.CallTo(() => spawnedNpc.Brain.GetTurnIntention()).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void StartNextTurnExecutesNextTurnIntentionWhenActiveUnitIsNpc()
    {
        // Arrange
        var enemy = CreateNpcUnit(TeamType.Enemy, "Enemy");
        var player = CreateUnit(TeamType.Player, "Player");
        var executed = false;
        var cachedIntention = new UnitTurnIntention
        {
            Description = "Cached intention",
            OnExecute = () => executed = true
        };

        A.CallTo(() => enemy.Brain.GetTurnIntention())
            .Returns(new UnitTurnIntention { Description = "Next intention" });

        var battleConfig = CreateBattleConfig(enemyCount: 1, playerCount: 1);
        StubSpawnedUnits(enemy, player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        ForceTurnOrder(enemy, player);
        _sut.NextTurnIntentionsByUnit[enemy] = cachedIntention;

        // Act
        _sut.StartNextTurn();

        // Assert
        Assert.That(executed, Is.True);
    }

    [Test]
    public void StartNextTurnGeneratesNextTurnIntentionWhenActiveUnitIsNpcAndNpcStillAlive()
    {
        // Arrange
        var enemy = CreateNpcUnit(TeamType.Enemy, "Enemy");
        var player = CreateUnit(TeamType.Player, "Player");

        var currentIntention = new UnitTurnIntention { Description = "Current intention" };
        var nextIntention = new UnitTurnIntention { Description = "Next intention" };

        A.CallTo(() => enemy.Brain.GetTurnIntention()).ReturnsNextFromSequence(currentIntention, nextIntention);

        var battleConfig = CreateBattleConfig(enemyCount: 1, playerCount: 1);
        StubSpawnedUnits(enemy, player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        ForceTurnOrder(enemy, player);

        // Act
        _sut.StartNextTurn();

        // Assert
        Assert.That(_sut.NextTurnIntentionsByUnit[enemy], Is.SameAs(nextIntention));
        A.CallTo(() => enemy.Brain.GetTurnIntention()).MustHaveHappenedTwiceExactly();
    }

    [Test]
    public void StartNextTurnPlaysTurnThenStartsNextTurnWhenActiveUnitIsNpcAndBattleNotOver()
    {
        // Arrange
        var enemy = CreateNpcUnit(TeamType.Enemy, "Enemy");
        var player = CreateUnit(TeamType.Player, "Player");

        A.CallTo(() => enemy.Brain.GetTurnIntention())
            .Returns(new UnitTurnIntention { Description = "Enemy intention" });

        var battleConfig = CreateBattleConfig(enemyCount: 1, playerCount: 1);
        StubSpawnedUnits(enemy, player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        ForceTurnOrder(enemy, player);

        // Act
        _sut.StartNextTurn();

        // Assert
        Assert.That(_sut.ActiveUnit, Is.SameAs(player));
        A.CallTo(() => _unitService.SetActiveUnit(TeamType.Enemy, enemy)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitService.SetActiveUnit(TeamType.Player, player)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void OnUnitDefeatedEndsBattleWhenAllUnitsForTeamAreDefeated()
    {
        // Arrange
        var enemy = CreateUnit(TeamType.Enemy, "Enemy");
        var player = CreateUnit(TeamType.Player, "Player");
        TeamType? winningTeam = null;

        var battleConfig = CreateBattleConfig(enemyCount: 1, playerCount: 1);
        StubSpawnedUnits(enemy, player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());
        _sut.OnBattleEnded += team => winningTeam = team;

        // Act
        _sut.OnUnitDefeated(enemy);

        // Assert
        Assert.That(winningTeam, Is.EqualTo(TeamType.Player));
        Assert.That(_sut.GetTeamUnits(TeamType.Enemy), Is.Empty);
    }

    [Test]
    public void OnUnitSpawnedAddsUnitToCorrectTeam()
    {
        // Arrange
        var player = CreateUnit(TeamType.Player, "Player");
        var spawnedEnemy = CreateUnit(TeamType.Enemy, "Spawned Enemy");

        var battleConfig = CreateBattleConfig(enemyCount: 0, playerCount: 1);
        StubSpawnedUnits(player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        // Act
        _sut.OnUnitSpawned(spawnedEnemy);

        // Assert
        Assert.That(_sut.GetTeamUnits(TeamType.Enemy), Does.Contain(spawnedEnemy));
    }

    [Test]
    public void OnUnitSpawnedAddsUnitToTurnOrderAndQueueWhenSpawned()
    {
        // Arrange
        var player = CreateUnit(TeamType.Player, "Player");
        var spawnedEnemy = CreateUnit(TeamType.Enemy, "Spawned Enemy");

        var battleConfig = CreateBattleConfig(enemyCount: 0, playerCount: 1);
        StubSpawnedUnits(player);
        _sut.Initialize(battleConfig, A.Fake<MapSpaceContainer>());

        // Act
        _sut.OnUnitSpawned(spawnedEnemy);

        // Assert
        Assert.That(_sut.TurnOrder, Does.Contain(spawnedEnemy));
        Assert.That(_sut.TurnQueue, Does.Contain(spawnedEnemy));
    }

    private static IUnit CreateUnit(TeamType team, string name)
    {
        var config = A.Fake<IUnitConfigProvider>();
        A.CallTo(() => config.Name).Returns(name);

        return new Unit
        {
            Team = team,
            Config = config,
            CurrentHealth = 10,
            CurrentEnergy = 0,
            Energy = 10,
            Actions = new List<IAction>()
        };
    }

    private static NpcUnit CreateNpcUnit(TeamType team, string name)
    {
        var config = A.Fake<IUnitConfigProvider>();
        A.CallTo(() => config.Name).Returns(name);

        return new NpcUnit
        {
            Team = team,
            Config = config,
            CurrentHealth = 10,
            CurrentEnergy = 0,
            Energy = 10,
            Actions = new List<IAction>(),
            Brain = A.Fake<IUnitBrain>()
        };
    }

    private void StubSpawnedUnits(params IUnit[] units)
    {
        var queue = new Queue<IUnit>(units);
        A.CallTo(() => _unitService.Spawn(A<SpawnConfig>._)).ReturnsLazily(() => queue.Dequeue());
    }

    private static IBattleConfigProvider CreateBattleConfig(int enemyCount, int playerCount)
    {
        var battleConfig = A.Fake<IBattleConfigProvider>();
        A.CallTo(() => battleConfig.EnemyUnits).Returns(Enumerable.Range(0, enemyCount).Select(_ => new BattleUnitConfig()).ToList());
        A.CallTo(() => battleConfig.PlayerUnits).Returns(Enumerable.Range(0, playerCount).Select(_ => new BattleUnitConfig()).ToList());
        return battleConfig;
    }

    private void ForceTurnOrder(params IUnit[] units)
    {
        _sut.TurnOrder.Clear();
        _sut.TurnQueue.Clear();
        foreach (var unit in units)
        {
            _sut.TurnOrder.Add(unit);
            _sut.TurnQueue.Add(unit);
        }
    }
}
