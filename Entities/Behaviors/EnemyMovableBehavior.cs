using Godot;

namespace Mdfry1.Entities.Behaviors;

public class EnemyMovableBehavior : BaseMovableBehavior
{
    [Export] public int AvoidForce { get; set; } = 1000;

    [Export] public int ArrivalZoneRadius { get; set; }

    [Export] public Rect2 EnclosureZone { get; set; } = new(16, 16, 100, 100);

    [Export] public float MaxSteering { get; set; } = 2.5f;
}