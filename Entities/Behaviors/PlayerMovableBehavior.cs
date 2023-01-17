using System;
using Common.Uti;
using Core.Input;
using Godot;

namespace Mdfry1.Entities.Behaviors;

public class PlayerMovableBehavior : BaseMovableBehavior
{
    [Export] public float MaxRollCooldown { get; set; } = 2f;

    public float CurrentRollCooldown { get; set; }

    public Action<Vector2> OnRoll { get; set; }
    public Action<Vector2> OnPhysicsProcessMovement { get; set; }

    [Export] public float RollSpeed { get; set; } = 120f;

    private Vector2 RollVector { get; set; } = Vector2.Down;


    public override void _Ready()
    {
        base._Ready();
    }

    private Vector2 Roll()
    {
        _logger.Debug("Rolling");
        CurrentRollCooldown = MaxRollCooldown;
        var newVelocity = RollVector * RollSpeed;
        OnRoll?.Invoke(newVelocity);
        return newVelocity;
    }

    public override void Move(float currentVelocity)
    {
        MoveAndSlide(Velocity);
    }

    private Vector2 MoveCheck(Vector2 movementVector, Vector2 currentVelocity, float delta)
    {
        if (movementVector != Vector2.Zero)
        {
            RollVector = movementVector;
            OnPhysicsProcessMovement?.Invoke(movementVector);
            OnMove?.Invoke(movementVector, delta);
            currentVelocity = IsRunning
                ? currentVelocity.MoveToward(movementVector * (MaxSpeed * MoveMultiplier), Acceleration * delta)
                : currentVelocity.MoveToward(movementVector * MaxSpeed, Acceleration * delta);
        }
        else
        {
            currentVelocity = currentVelocity.MoveToward(Vector2.Zero, Friction * delta);
            OnIdle?.Invoke(currentVelocity, delta);
        }

        return currentVelocity;
    }

    private bool CanRoll()
    {
        var retval = CanMove && CurrentRollCooldown <= 0f;
        return retval;
    }


    public override void _PhysicsProcess(float delta)
    {
        try
        {
            if (!CanMove) return;
            IsRunning = Input.IsActionPressed(InputConstants.Run);

            var movementVector = InputUtil.GetTopDownWithDiagMovementInputStrengthVector();
            Velocity = MoveCheck(movementVector, Velocity, delta);
            if (CanRoll() && Input.IsActionPressed(InputConstants.Roll)) Velocity = Roll();

            if (CurrentRollCooldown > 0f)
            {
                _logger.Debug("Roll Cooling Down with Remaining Time: " + CurrentRollCooldown);
                CurrentRollCooldown -= delta;
            }

            Move(delta);
            if (GetSlideCount() > 0) HandleMovableObstacleCollision(Velocity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}