using System;
using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities.Behaviors;

public abstract class BaseMovableBehavior : KinematicBody2D, IDebuggable<Node>, Mdfry1.Entities.Behaviors.Interfaces.IMovableBehavior
{
    protected ILogger _logger { get; set; } = new GDLogger(LogLevelOutput.Warning);

    public Action<Vector2, float> OnMove { get; set; }

    public Action<Vector2, float> OnIdle { get; set; }

    [Export] public float Acceleration { get; set; } = 500f;

    [Export] public float Friction { get; set; } = 500f;

    public bool CanMove { get; set; } = true;

    public bool IsRunning { get; set; }

    [Export] public bool IsDebugging { get; set; }


    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    [Export] public float PushSpeed { get; set; } = 20f;

    [Export] public float MoveMultiplier { get; set; } = 1.5f;

    public Vector2 Velocity { get; set; }

    public virtual Vector2 GetMovementVelocity(Vector2 movementVector, Vector2 currentVelocity, float delta)
    {
        return movementVector != Vector2.Zero
            ? GetAcceleration(movementVector, currentVelocity, delta)
            : GetFriction(currentVelocity, delta);
    }

    public virtual Vector2 GetMovementVelocity(Vector2 currentVelocity, float delta)
    {
        return currentVelocity != Vector2.Zero
            ? GetAcceleration(currentVelocity, currentVelocity, delta)
            : GetFriction(currentVelocity, delta);
    }


    [Export] public float MaxSpeed { get; set; } = 10f;

    public void HandleMovableObstacleCollision(Vector2 motion)
    {
        this.PrintCaller();
        motion = motion.Normalized();

        if (GetSlideCollision(0).Collider is PushBlock { CanBePushed: true } box) box.Push(PushSpeed * motion);
    }

    public virtual Vector2 GetAcceleration(Vector2 movementVector, Vector2 currentVelocity, float delta)
    {
        return IsRunning
            ? currentVelocity.MoveToward(movementVector * (MaxSpeed * MoveMultiplier), Acceleration * delta)
            : currentVelocity.MoveToward(movementVector * MaxSpeed, Acceleration * delta);
    }

    public virtual Vector2 GetFriction(Vector2 currentVelocity, float delta)
    {
        return currentVelocity.MoveToward(Vector2.Zero, Friction * delta);
    }

    public virtual void Move(float delta)
    {
        if (!CanMove) return;
        if (Velocity != Vector2.Zero)
        {
            Velocity = GetMovementVelocity(Velocity, delta);
            OnMove?.Invoke(Velocity, delta);
        }
        else
        {
            Velocity = GetFriction(Velocity, delta);
            OnIdle?.Invoke(Velocity, delta);
        }

        MoveAndSlide(Velocity);
        if (GetSlideCount() <= 0) return;
        HandleMovableObstacleCollision(Velocity);
    }

    public void MoveToward(Vector2 newPoint, float delta)
    {
        if (!CanMove) return;
        OnMove?.Invoke(Velocity, delta);
        Position.MoveToward(newPoint, MaxSpeed * delta);
        if (GetSlideCount() <= 0) return;
        HandleMovableObstacleCollision(Velocity);
    }


    // ReSharper disable once RedundantOverriddenMember
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(float delta)
    {
        Move(delta);
    }
}