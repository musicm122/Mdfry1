using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Entities.Components;

namespace Mdfry1.Entities.Interfaces;

public interface IEnemy
{
    Label Cooldown { get; set; }
    IDamagableBehavior Damagable { get; }
    Node2D ObstacleAvoidance { get; set; }
    NodePath PatrolPath { get; set; }
    EnemyDataStore EnemyDataStore { get; set; }

    void Init();
    void _PhysicsProcess(float delta);
    void _Ready();
}