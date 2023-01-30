using System.Globalization;
using Common.Constants;
using Godot;
using Mdfry1.Entities.Interfaces;
//using Mdfry1.Entities.Vision;
using Mdfry1.Logic.Constants;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities;

public class SecurityCamera : Node2D, IDebuggable<Node2D>
{
    public float CurrentCoolDownCounter { get; set; }

    public float MaxCoolDownTime { get; set; } = 10f;

    private bool IsStartMovement { get; set; } = true;

    private bool IsPausing { get; set; }

    private float Elapsed { get; set; }

    private Label DebugLabel { get; set; }

    public States.CameraState CurrentState { get; set; } = States.CameraState.Idle;

    public Mdfry1.Logic.Sight.IVision VisionManager { get; set; }

    public Node2D PlayerRef { get; set; }

    [Export] public float MaxRotationMovementTime { get; set; } = 2f;

    [Export] public float PauseRotationTime { get; set; } = 2f;

    [Export] public float RotationSpeed { get; set; } = 80f;

    public Node2D Target { get; set; }

    public Polygon2D CameraSprite { get; set; }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public override void _Ready()
    {
        DebugLabel = GetNode<Label>("DebugLabel");
        VisionManager = GetNode<Mdfry1.Logic.Sight.RaycastVision>("Pivot/RayCast2D");
        CameraSprite = GetNode<Polygon2D>("Polygon2D");
        if (VisionManager == null) return;
        VisionManager.OnTargetSeen += OnTargetDetection;
        VisionManager.OnTargetOutOfSight += OnTargetLost;
    }

    private void OnTargetLost(Node2D player)
    {
        this.Print($"Player lost. Player last known position at {player.GlobalPosition.ToString()}");
        CameraSprite.Color = CommonColors.AggroColor;
        Target = null;
    }

    public void OnTargetDetection(Node2D player)
    {
        PlayerRef = player;
        CurrentState = States.CameraState.Aggro;
    }

    private void OnIdle(float delta)
    {
        CameraSprite.Color = CommonColors.IdleColor;
        if (Elapsed > MaxRotationMovementTime && !IsPausing)
        {
            IsStartMovement = !IsStartMovement;
            IsPausing = true;
            Elapsed = 0f;
        }

        if (Elapsed > MaxRotationMovementTime && IsPausing)
        {
            IsPausing = false;
            Elapsed = 0f;
        }

        if (!IsPausing)
        {
            if (IsStartMovement)
                Rotation += RotationSpeed * delta;
            else
                Rotation -= RotationSpeed * delta;
        }

        Elapsed += delta;
    }

    private void OnWarning(float delta)
    {
        CameraSprite.Color = CommonColors.WarningColor;
    }

    private void OnAggro(float delta)
    {
        this.PrintCaller();
        if (PlayerRef == null) return;
        var targetPoint = PlayerRef.GlobalPosition;
        CameraSprite.Color = CommonColors.AggroColor;
        LookAt(targetPoint);
        GetTree().AlertAllEnemies();
        CurrentCoolDownCounter = MaxCoolDownTime;
    }

    private void OnDamaged(float delta)
    {
        this.PrintCaller();
    }

    private void OnStun(float delta)
    {
        this.PrintCaller();
    }

    public void Alert()
    {
        (_, PlayerRef) = GetTree().GetPlayerNode();
        CurrentState = States.CameraState.Aggro;
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (CurrentState)
        {
            case States.CameraState.Warning:
                OnWarning(delta);
                break;
            case States.CameraState.Aggro:
                OnAggro(delta);
                break;
            case States.CameraState.Damaged:
                OnDamaged(delta);
                break;
            case States.CameraState.Stun:
                OnStun(delta);
                break;
            case States.CameraState.Idle:
            default:
                OnIdle(delta);
                break;
        }

        if (CurrentCoolDownCounter > 0)
            CurrentCoolDownCounter -= delta;
        else
            CurrentState = States.CameraState.Idle;

        if (IsDebugging)
            DebugLabel.Text =
                @$"
------Angle--------------
Rotation : {Mathf.Rad2Deg(Rotation).ToString(CultureInfo.InvariantCulture)}
Global Rotation: {Mathf.Rad2Deg(GlobalRotation).ToString(CultureInfo.InvariantCulture)}
GlobalTransform.Rotation: {Mathf.Rad2Deg(GlobalTransform.Rotation).ToString(CultureInfo.InvariantCulture)}

------Degree-------------
Rotation: {Rotation.ToString(CultureInfo.InvariantCulture)}
Global Rotation: {GlobalRotation.ToString(CultureInfo.InvariantCulture)}
GlobalTransform.Rotation: {GlobalTransform.Rotation.ToString(CultureInfo.InvariantCulture)}";
    }
}