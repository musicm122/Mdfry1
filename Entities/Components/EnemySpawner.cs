using Godot;
using System;
using System.Collections.Generic;
using Mdfry1.Entities;
using Mdfry1.Entities.Values;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.Components
{
    public class EnemySpawner : Node2D
    {

        public static Color[] AvailableColors =
        {
            new Color("#27e217"),
            new Color("#a300fe"),
            new Color("#00ffc3"),
            new Color("#ff282b"),
            new Color("#ffffff")
        };

        private Color GetRandomColor()=>
            AvailableColors[new Random().RandomInt(0, AvailableColors.Length - 1)];
        
        
        [Export]
        public bool EnableSpawning = true;

        [Export] 
        public int MaxSpawnCount { get; set; } = 10;
        
        public int EnemiesSpawnedCount = 0;
        
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
            
        Action<float> OnSpawnRateChange { get; set; }
        Action<EnemyV4> OnSpawn { get; set; }

        public EnemyBehaviorStates EnemySpawnState { get; set; } = EnemyBehaviorStates.Wander;
        
        public float AccumulatedTime = 0f;
        
        private float _spawnRate = 10f;

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

        [Export(PropertyHint.File, "*.tscn")] 
        public string EnemyToSpawnPath { get; set; } = "res://Entities/Enemy1.tscn";

        public Area2D SpawnPosition { get; set; }

        public override void _Ready()
        {
            SpawnPosition = GetNode<Area2D>("./SpawnArea");
            AccumulatedTime = 0f;
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
            var enemy = (EnemyV4)GD.Load<PackedScene>(EnemyToSpawnPath).Instance();
            enemy.DefaultState = EnemySpawnState;
            enemy.GlobalPosition = SpawnPosition.Position;
            Owner.AddChild(enemy);
            OnSpawn?.Invoke(enemy);
        }
        
        public void SpawnTougher()
        {
            //var enemy = (EnemyV4)ResourceLoader.Load(EnemyToSpawnPath);
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
            if(GetTree().GetEnemyCount()>=MaxSpawnCount || !EnableSpawning ) return;
            
            AccumulatedTime += delta;
            if (AccumulatedTime<=SpawnRate) return;
            EnemiesSpawnedCount++;
            if (GetTree().GetDayCount() >= 1)
            {
                Spawn();
            }
            else
            {
                SpawnTougher();
            }

            _logger.Debug($"{Name} Spawning enemy # {EnemiesSpawnedCount.ToString()}");
            AccumulatedTime = 0f;
        }
    }

}