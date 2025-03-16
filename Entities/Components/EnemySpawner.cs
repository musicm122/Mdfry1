using System;
using Godot;
using Mdfry1.Logic.Constants;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities.Components;

public class EnemySpawner : Node2D
{
    public static Color[] AvailableColors =
    {
        new("#27e217"),
        new("#a300fe"),
        new("#00ffc3"),
        new("#ff282b"),
        new("#ffffff")
    };

    private float _spawnRate = 10f;

    public float AccumulatedTime;


    [Export] public bool EnableSpawning = true;

    public int EnemiesSpawnedCount;

    [Export] public int MaxSpawnCount { get; set; } = 10;

    protected ILogger _logger { get; set; } = new GDLogger(LogLevelOutput.Debug);
    // [Export]
    // public float HighLightSpawnRate { get; set; } 
    //
    // [Export]
    // public float MediumLightSpawnRate { get; set; }
    //
    // [Export]
    // public float LowLightSpawnRate { get; set; }
    //
    // [Export]
    // public float NoLightSpawnRate { get; set; }

    private Action<float> OnSpawnRateChange { get; set; }
    private Action<EnemyV4> OnSpawn { get; set; }

    public EnemyBehaviorStates EnemySpawnState { get; set; } = EnemyBehaviorStates.Wander;

    public float SpawnRate
    {
        get => _spawnRate;
        set
        {
            _spawnRate = value;
            AccumulatedTime = 0f;
            OnSpawnRateChange?.Invoke(_spawnRate);
        }
    }

    [Export(PropertyHint.File, "*.tscn")] public string EnemyToSpawnPath { get; set; } = "res://Entities/Enemy1.tscn";

    public Area2D SpawnPosition { get; set; }

    private Color GetRandomColor()
    {
        return AvailableColors[new Random().RandomInt(0, AvailableColors.Length - 1)];
    }

    public override void _Ready()
    {
        SpawnPosition = GetNode<Area2D>("./SpawnArea");
        AccumulatedTime = 0f;
        AddToGroup(Groups.Spawner);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup(Groups.Spawner);
    }

    public void SpawnAtPosition(Position2D pos, Node2D parent)
    {
        //var enemy = (EnemyV4)ResourceLoader.Load(EnemyToSpawnPath);
        var enemy = (EnemyV4)GD.Load<PackedScene>(EnemyToSpawnPath).Instance();
        enemy.DefaultState = EnemySpawnState;
        enemy.GlobalPosition = pos.GlobalPosition;
        parent.AddChild(enemy);
        OnSpawn?.Invoke(enemy);
    }

    public void Spawn()
    {
        //var enemy = (EnemyV4)ResourceLoader.Load(EnemyToSpawnPath);
        if (string.IsNullOrWhiteSpace(EnemyToSpawnPath))
        {
            return;
        }

        var enemy = (EnemyV4)GD.Load<PackedScene>(EnemyToSpawnPath).Instance();
        enemy.DefaultState = EnemySpawnState;
        enemy.GlobalPosition = SpawnPosition.Position;
        Owner.AddChild(enemy);
        OnSpawn?.Invoke(enemy);
    }

    public void SpawnTougher()
    {
        //var enemy = (EnemyV4)ResourceLoader.Load(EnemyToSpawnPath);
        if (string.IsNullOrWhiteSpace(EnemyToSpawnPath))
        {
            return;
        }

        var enemy = (EnemyV4)GD.Load<PackedScene>(EnemyToSpawnPath).Instance();
        enemy.EnemyDataStore.MaxHealth = new Random().RandomInt(4, 6);
        enemy.EnemyDataStore.CurrentHealth = enemy.EnemyDataStore.MaxHealth;
        enemy.Modulate = GetRandomColor();
        enemy.DefaultState = EnemySpawnState;
        enemy.GlobalPosition = SpawnPosition.Position;
        Owner.AddChild(enemy);
        OnSpawn?.Invoke(enemy);
    }

    public override void _Process(float delta)
    {
        try
        {
            var enemyCount = GetTree().GetEnemyCount();
            if (enemyCount >= MaxSpawnCount || !EnableSpawning) return;

            AccumulatedTime += delta;
            if (AccumulatedTime <= SpawnRate) return;
            EnemiesSpawnedCount++;
            if (GetTree().GetDayCount() >= 1)
                Spawn();
            else
                SpawnTougher();

            _logger.Debug($"{Name} Spawning enemy # {EnemiesSpawnedCount.ToString()}");
            AccumulatedTime = 0f;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}