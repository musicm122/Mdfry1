using System;
using Godot;

namespace Mdfry1.Entities.Interfaces;

public interface IVision
{
    Action<Node2D> OnTargetSeen { get; set; }
    Action<Node2D> OnTargetOutOfSight { get; set; }
    bool IsDebugging { get; set; }
    public Node2D OldTarget { get; set; }
    public Node2D NewTarget { get; set; }

    public bool CanCheckFrame(int interval = 2) =>
        new Random().Next() % interval == 0;

    void UpdateFacingDirection(Vector2 newVelocity);

    bool CanSeeTarget();
}