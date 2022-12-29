using System;
using Godot;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Vision;

public class Area2dVision : Area2D, IDebuggable<Node2D>, IVision
{
    private bool LineOfSight { get; set; }


    [Export] public bool IsDebugging { get; set; }
    public Action<Node2D> OnTargetSeen { get; set; }
    public Action<Node2D> OnTargetOutOfSight { get; set; }

    public Node2D OldTarget { get; set; }
    public Node2D NewTarget { get; set; }

    public bool CanSeeTarget()
    {
        var bodies = GetOverlappingBodies();
        if (bodies == null || bodies.Count == 0) return false;
        for (var i = 0; i < bodies.Count; i++)
        {
            var body = (Node)bodies[i];
            if (body.Name.ToLower().Contains("player")) return true;
        }

        return false;
    }

    public void UpdateFacingDirection(Vector2 newVelocity)
    {
        Rotation = Position.AngleToPoint(newVelocity);
    }

    public void LookAtPoint(Vector2 point)
    {
        LookAt(point);
    }

    public override void _Ready()
    {
        this.TryConnectSignal(Signals.Area2D.BodyEntered, this, nameof(OnVisionRadiusBodyEntered));
        this.TryConnectSignal(Signals.Area2D.BodyExited, this, nameof(OnVisionRadiusBodyExit));
    }

    private void OnVisionRadiusBodyEntered(Node body)
    {
        if (!body.Name.ToLower().Contains("player")) return;

        this.PrintCaller();
        NewTarget = (Node2D)body;
        LineOfSight = HasLineOfSight(NewTarget.Position);
        if (!LineOfSight) return;
        OnTargetSeen?.Invoke(NewTarget);
        OldTarget = NewTarget;
        NewTarget = null;
    }

    private void OnVisionRadiusBodyExit(Node body)
    {
        if (!body.Name.ToLower().Contains("player")) return;
        this.PrintCaller();
        if (OldTarget == null) return;
        LineOfSight = HasLineOfSight(OldTarget.GlobalPosition);
        if (!LineOfSight) return;
        OnTargetOutOfSight?.Invoke(OldTarget);
    }

    public bool HasLineOfSight(Vector2 point)
    {
        var spaceState = GetWorld2d().DirectSpaceState;
        var result = spaceState.IntersectRay(GlobalTransform.origin, point, null, CollisionMask);
        LineOfSight = result?.Count > 0;
        return LineOfSight;
    }
}